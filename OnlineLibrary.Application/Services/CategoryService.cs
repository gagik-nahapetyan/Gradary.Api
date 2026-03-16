using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Services;

/// <summary>
/// Represents a <see cref="CategoryService"/>.
/// </summary>
public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public async Task<CategoryModel> CreateAsync(CategoryModel model)
    {
        await ValidateParentAsync(model);

        var entity = model.ToEntity();
        entity = await categoryRepository.InsertAsync(entity);

        await categoryRepository.SaveChangesAsync();

        return entity.ToModel();
    }

    public async Task UpdateAsync(CategoryModel model)
    {
        await ValidateParentAsync(model);

        var entity = model.ToEntity();
        categoryRepository.Update(entity);

        await categoryRepository.SaveChangesAsync();
    }

    public async Task<List<CategoryModel>> GetAsync()
    {
        var entities = await categoryRepository.GetAllAsync();
        var models = entities.Select(e => e.ToModel()).ToList();

        return models;
    }

    public async Task<CategoryModel> GetByIdAsync(int id)
    {
        var entity = await categoryRepository.GetByIdAsync(id);
        if (entity is null)
            throw new KeyNotFoundException($"Category with id {id} not found");

        var model = entity.ToModel();

        return model;
    }

    private async Task ValidateParentAsync(CategoryModel model)
    {
        if (!model.ParentId.HasValue)
            return;

        if (model.ParentId == model.Id)
            throw new ArgumentException("Category cannot be its own parent.", nameof(model.ParentId));

        var parent = await categoryRepository.GetByIdAsync(model.ParentId.Value);
        if (parent is null)
            throw new KeyNotFoundException($"Parent category with id {model.ParentId.Value} not found.");

        var currentParentId = parent.ParentId;
        while (currentParentId.HasValue)
        {
            var currentParent = await categoryRepository.GetByIdAsync(currentParentId.Value);
            if (currentParent is null)
                break;

            if (currentParent.Id == model.Id)
                throw new InvalidOperationException("Category hierarchy cannot contain cycles.");

            currentParentId = currentParent.ParentId;
        }
    }
}