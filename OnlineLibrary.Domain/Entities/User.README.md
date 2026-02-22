# User Entity — API & Service Specification

This document describes the **User** flow from API to repository, mirroring the pattern used for **Book** (Controller → DTOs ↔ Service ↔ Model ↔ Repository ↔ Entity).

---

## 1. Entity Reference

**File:** `OnlineLibrary.Domain/Entities/User.cs`

| Property       | Type              | Notes                    |
|----------------|-------------------|--------------------------|
| Id             | int               | From `EntityBase`        |
| FullName       | string (required) |                          |
| Email          | string (required) |                          |
| PasswordHash   | string (required) | Never expose in DTOs     |
| Role           | UserRole          | Enum: Admin, User        |
| Reviews        | ICollection\<Review\> | Navigation          |
| CreatedAt, CreatedBy, UpdatedAt, UpdatedBy | Audit | From `AuditEntity` |

---

## 2. API Layer

### 2.1 UserController

**File:** `OnlineLibrary.Api/Controllers/UserController.cs`

**Base route:** `api/[controller]` → `api/user`

| Method | Route | Action | Request | Response |
|--------|--------|--------|---------|----------|
| POST   | `create` | Create user | `UserRequest` | `UserDto` |
| POST   | `update/{id}` | Update user | `UserRequest`, `id` (route, int, min 1) | `UserDto` |
| GET    | `{id}` | Get by id | — | `UserDto` |
| GET    | (default) | List all users | — | `List<UserDto>` |

**Pattern (align with BookController):**

- Constructor inject `IUserService userService`.
- **Create:** `UserRequest` → `ToModel()` → `userService.CreateAsync(model)` → `model.ToDto()` → `Ok(dto)`.
- **Update:** `UserRequest` + `id` → `ToModel(id)` → `userService.UpdateAsync(model)` → `model.ToDto()` → `Ok(dto)`.
- **Get by id:** `userService.GetByIdAsync(id)` → `ToDto()` → `Ok(dto)`.
- **Get list:** `userService.GetAsync()` → map each to `UserDto` → `Ok(dtos)`.

### 2.2 DTOs

**Location:** `OnlineLibrary.Api/Dtos/`

**UserDto** (response; do **not** include `PasswordHash`):

- `Id` (int)
- `FullName` (string)
- `Email` (string)
- `Role` (UserRole)
- Optional: audit fields if needed by clients (`CreatedAt`, `UpdatedAt`, etc.)

**UserRequest** (create/update body):

- `FullName` (string, required)
- `Email` (string, required)
- `Password` (string): required for **create**, optional for **update** (only sent when changing password).
- `Role` (UserRole)

---

## 3. Application Layer

### 3.1 IUserService (abstraction)

**File:** `OnlineLibrary.Application/Abstractions/Services/IUserService.cs`

```csharp
Task<List<UserModel>> GetAsync();
Task<UserModel> GetByIdAsync(int id);
Task<UserModel> CreateAsync(UserModel model);
Task UpdateAsync(UserModel model);
```

- Same shape as `IBookService`, using `UserModel` instead of `BookModel`.

### 3.2 UserService (implementation)

**File:** `OnlineLibrary.Application/Services/UserService.cs`

- Constructor: inject `IUserRepository userRepository`.
- **CreateAsync:** `model.ToEntity()` → `userRepository.InsertAsync(entity)` → `SaveChangesAsync()` → return `entity.ToModel()`.  
  **Important:** In Create (and Update when password is set), hash the plain `Password` from the request into `PasswordHash` before building the entity (e.g. in a mapper or in the service). Do not store plain passwords.
- **UpdateAsync:** `model.ToEntity()` → `userRepository.Update(entity)` → `SaveChangesAsync()`.  
  When updating, if password is not being changed, keep existing `PasswordHash` (do not overwrite with empty or re-hash of empty).
- **GetAsync:** `userRepository.GetAllAsync()` → map each entity with `ToModel()` → return list.
- **GetByIdAsync:** `userRepository.GetByIdAsync(id)` → if null throw `KeyNotFoundException` → return `entity.ToModel()`.

### 3.3 UserModel

**File:** `OnlineLibrary.Domain/Models/UserModel.cs`

- Inherit `AuditModel` (same as `BookModel`).
- Properties: `Id`, `FullName`, `Email`, `PasswordHash`, `Role`, plus audit from `AuditModel`.

Used only inside the application layer and for mapping; never returned from the API (API returns `UserDto` without `PasswordHash`).

### 3.4 Mappings (Application)

**File:** `OnlineLibrary.Application/CustomMapper.cs` (extend existing)

- `UserModel ToModel(this User entity)` — Entity → UserModel (all fields including PasswordHash).
- `User ToEntity(this UserModel model)` — UserModel → User entity.

---

## 4. API Mappings

**File:** `OnlineLibrary.Api/CustomMapper.cs` (extend existing)

- `UserModel ToModel(this UserRequest dto, int id = 0)` — UserRequest → UserModel.  
  For create, `id = 0` and password comes from request (to be hashed in service). For update, pass existing id and only set password in model when `dto.Password` is not null/empty.
- `UserDto ToDto(this UserModel model)` — UserModel → UserDto.  
  Do **not** map `PasswordHash`; only `Id`, `FullName`, `Email`, `Role` (and any audit fields if present on DTO).

---

## 5. Repository Layer

### 5.1 IUserRepository

**File:** `OnlineLibrary.Application/Abstractions/Repositories/IUserRepository.cs`

```csharp
public interface IUserRepository : IRepository<User>
{
}
```

- Same pattern as `IBookRepository`; base interface provides `InsertAsync`, `Update`, `GetByIdAsync`, `GetAllAsync`, `FindAsync`, `Delete`, `SaveChangesAsync`.

### 5.2 UserRepository

**File:** `OnlineLibrary.Persistence/Repositories/UserRepository.cs`

```csharp
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(OnlineLibraryDbContext dbContext) : base(dbContext)
    {
    }
}
```

- No extra methods unless you add specific queries (e.g. by email) later.

---

## 6. Dependency Registration

- **Application:** `OnlineLibrary.Application/Extensions/ServiceCollectionExtensions.cs`  
  - Register: `services.AddScoped<IUserService, UserService>();`
- **Persistence:** `OnlineLibrary.Persistence/Extensions/ServiceCollectionExtensions.cs`  
  - Register: `services.AddScoped<IUserRepository, UserRepository>();`

---

## 7. Flow Summary

```
HTTP Request
    → UserController (UserRequest / route params)
    → Api CustomMapper: UserRequest → UserModel (ToModel)
    → IUserService / UserService
    → Application CustomMapper: UserModel → User (ToEntity)
    → IUserRepository / UserRepository (IRepository<User>)
    → DbContext (User entity)
    ← Entity
    ← Application CustomMapper: User → UserModel (ToModel)
    ← UserService
    → Api CustomMapper: UserModel → UserDto (ToDto)  [no PasswordHash]
    ← UserController
HTTP Response (UserDto or List<UserDto>)
```

---

## 8. Security Notes

- **Never** return `PasswordHash` (or any password field) in `UserDto` or in any API response.
- **Create:** Always hash the password from `UserRequest.Password` before persisting (e.g. BCrypt, ASP.NET Core Identity, or your existing auth stack).
- **Update:** Only update `PasswordHash` when the client sends a new password; otherwise leave the existing hash unchanged.
- Consider unique constraint and validation on `Email` (e.g. in `UserConfiguration` and/or service).

---

## 9. Optional: Delete Endpoint

If you need to expose delete (repository already supports it):

- **Controller:** `DELETE api/user/{id}` → get entity via `GetByIdAsync` (or repository), then `userRepository.Delete(entity)` and `SaveChangesAsync()`.  
  Alternatively, add `DeleteAsync(int id)` to `IUserService` / `UserService` that loads the entity, calls `Delete`, then `SaveChangesAsync()`.

This completes the specification for User endpoints, UserService (with abstraction), and User repository aligned with the existing Book flow.
