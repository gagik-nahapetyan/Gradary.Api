using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Services;

/// <summary>
/// Represents an <see cref="ReviewService"/>.
/// </summary>
public class ReviewService(IReviewRepository reviewRepository) : IReviewService
{
    public async Task<ReviewModel> CreateAsync(ReviewModel model)
    {
        var entity = model.ToEntity();
        entity = await reviewRepository.InsertAsync(entity);

        await reviewRepository.SaveChangesAsync();

        return entity.ToModel();
    }

    public async Task UpdateAsync(ReviewModel model)
    {
        var entity = model.ToEntity();
        reviewRepository.Update(entity);

        await reviewRepository.SaveChangesAsync();
    }

    public async Task<ReviewModel> GetByIdAsync(int id)
    {
        var entity = await reviewRepository.GetByIdAsync(id);
        if (entity is null)
            throw new KeyNotFoundException($"Review with id {id} not found");

        var model = entity.ToModel();

        return model;
    }

    public async Task<List<ReviewModel>> GetByBookIdAsync(int bookId)
    {
        var entities = await reviewRepository.GetByBookIdAsync(bookId);
        var models = entities.Select(e => e.ToModel()).ToList();

        return models;
    }
}
