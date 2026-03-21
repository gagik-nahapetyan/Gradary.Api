using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Services;

/// <summary>
/// Represents an <see cref="ReviewService"/>.
/// </summary>
public class ReviewService(IReviewRepository reviewRepository) : IReviewService
{
    public async Task<ReviewModel> CreateAsync(ReviewModel reviewModel)
    {
        var review = reviewModel.ToEntity();
        review = await reviewRepository.InsertAsync(review);

        await reviewRepository.SaveChangesAsync();

        return review.ToModel();
    }

    public async Task UpdateAsync(ReviewModel reviewModel)
    {
        var existingReview = await reviewRepository.GetByIdAsync(reviewModel.Id);
        if (existingReview is null)
            throw new KeyNotFoundException($"Review with id {reviewModel.Id} not found");

        var review = reviewModel.ToEntity();
        reviewRepository.Update(review);

        await reviewRepository.SaveChangesAsync();
    }

    public async Task<ReviewModel> GetByIdAsync(int id)
    {
        var review = await reviewRepository.GetByIdAsync(id);
        if (review is null)
            throw new KeyNotFoundException($"Review with id {id} not found");

        var reviewModel = review.ToModel();

        return reviewModel;
    }

    public async Task<List<ReviewModel>> GetByBookIdAsync(int bookId)
    {
        var reviews = await reviewRepository.GetByBookIdAsync(bookId);
        var reviewModels = reviews.Select(r => r.ToModel()).ToList();

        return reviewModels;
    }
}
