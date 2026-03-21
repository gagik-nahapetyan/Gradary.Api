using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Services;

/// <summary>
/// Represents a <see cref="CategoryService"/>.
/// </summary>
public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public async Task<CategoryModel> CreateAsync(CategoryModel categoryModel)
    {
        await ValidateParentAsync(categoryModel);

        var category = categoryModel.ToEntity();
        category = await categoryRepository.InsertAsync(category);

        await categoryRepository.SaveChangesAsync();

        return category.ToModel();
    }

    public async Task UpdateAsync(CategoryModel categoryModel)
    {
        var existingCategory = await categoryRepository.GetByIdAsync(categoryModel.Id);
        if (existingCategory is null)
            throw new KeyNotFoundException($"Category with id {categoryModel.Id} not found");

        await ValidateParentAsync(categoryModel);

        var category = categoryModel.ToEntity();
        categoryRepository.Update(category);

        await categoryRepository.SaveChangesAsync();
    }

    public async Task<List<CategoryModel>> GetAsync()
    {
        var categories = await categoryRepository.GetAllAsync();
        var categoryModels = categories.Select(c => c.ToModel()).ToList();

        return categoryModels;
    }

    public async Task<CategoryModel> GetByIdAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        if (category is null)
            throw new KeyNotFoundException($"Category with id {id} not found");

        var categoryModel = category.ToModel();

        return categoryModel;
    }

    private async Task ValidateParentAsync(CategoryModel categoryModel)
    {
        if (!categoryModel.ParentId.HasValue)
            return;

        if (categoryModel.ParentId == categoryModel.Id)
            throw new ArgumentException("Category cannot be its own parent.", nameof(categoryModel.ParentId));

        var parent = await categoryRepository.GetByIdAsync(categoryModel.ParentId.Value);
        if (parent is null)
            throw new KeyNotFoundException($"Parent category with id {categoryModel.ParentId.Value} not found.");

        var currentParentId = parent.ParentId;
        while (currentParentId.HasValue)
        {
            var currentParent = await categoryRepository.GetByIdAsync(currentParentId.Value);
            if (currentParent is null)
                break;

            if (currentParent.Id == categoryModel.Id)
                throw new InvalidOperationException("Category hierarchy cannot contain cycles.");

            currentParentId = currentParent.ParentId;
        }
    }
}
