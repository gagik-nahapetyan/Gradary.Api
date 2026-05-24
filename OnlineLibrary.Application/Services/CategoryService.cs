using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Services;

/// <summary>
/// Represents a <see cref="CategoryService"/>.
/// </summary>
public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public async Task<CategoryModel> CreateAsync(CategoryModel categoryModel, CancellationToken cancellationToken = default)
    {
        await ValidateParentAsync(categoryModel, cancellationToken);

        var category = categoryModel.ToEntity();
        category = await categoryRepository.InsertAsync(category, cancellationToken);

        await categoryRepository.SaveChangesAsync(cancellationToken);

        return category.ToModel();
    }

    public async Task UpdateAsync(CategoryModel categoryModel, CancellationToken cancellationToken = default)
    {
        var existingCategory = await categoryRepository.GetByIdAsync(categoryModel.Id, cancellationToken, tracked: true);
        if (existingCategory is null)
            throw new KeyNotFoundException($"Category with id {categoryModel.Id} not found");

        await ValidateParentAsync(categoryModel, cancellationToken);

        existingCategory.Name = categoryModel.Name;
        existingCategory.Description = categoryModel.Description;
        existingCategory.ParentId = categoryModel.ParentId;

        await categoryRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<CategoryModel>> GetAsync(CancellationToken cancellationToken = default)
    {
        var categories = await categoryRepository.GetAllAsync(cancellationToken);
        var categoryModels = categories.Select(c => c.ToModel()).ToList();

        return categoryModels;
    }

    public async Task<CategoryModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category is null)
            throw new KeyNotFoundException($"Category with id {id} not found");

        var categoryModel = category.ToModel();

        return categoryModel;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category is null)
            throw new KeyNotFoundException($"Category with id {id} not found");

        categoryRepository.Delete(category);
        await categoryRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateParentAsync(CategoryModel categoryModel, CancellationToken cancellationToken = default)
    {
        if (!categoryModel.ParentId.HasValue)
            return;

        if (categoryModel.ParentId == categoryModel.Id)
            throw new ArgumentException("Category cannot be its own parent.", nameof(categoryModel.ParentId));

        var parent = await categoryRepository.GetByIdAsync(categoryModel.ParentId.Value, cancellationToken);
        if (parent is null)
            throw new KeyNotFoundException($"Parent category with id {categoryModel.ParentId.Value} not found.");

        var currentParentId = parent.ParentId;
        while (currentParentId.HasValue)
        {
            var currentParent = await categoryRepository.GetByIdAsync(currentParentId.Value, cancellationToken);
            if (currentParent is null)
                break;

            if (currentParent.Id == categoryModel.Id)
                throw new InvalidOperationException("Category hierarchy cannot contain cycles.");

            currentParentId = currentParent.ParentId;
        }
    }
}
