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
        var entity = model.ToEntity();
        entity = await categoryRepository.InsertAsync(entity);

        await categoryRepository.SaveChangesAsync();

        return entity.ToModel();
    }

    public async Task UpdateAsync(CategoryModel model)
    {
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
}

