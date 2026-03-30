using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Services;

/// <summary>
/// Represents an <see cref="ReviewService"/>.
/// </summary>
public class ReviewService(IReviewRepository reviewRepository) : IReviewService
{
    public async Task<ReviewModel> CreateAsync(ReviewModel reviewModel, CancellationToken cancellationToken = default)
    {
        var review = reviewModel.ToEntity();
        review = await reviewRepository.InsertAsync(review, cancellationToken);

        await reviewRepository.SaveChangesAsync(cancellationToken);

        return review.ToModel();
    }

    public async Task UpdateAsync(ReviewModel reviewModel, CancellationToken cancellationToken = default)
    {
        var existingReview = await reviewRepository.GetByIdAsync(reviewModel.Id, cancellationToken);
        if (existingReview is null)
            throw new KeyNotFoundException($"Review with id {reviewModel.Id} not found");

        var review = reviewModel.ToEntity();
        reviewRepository.Update(review);

        await reviewRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<ReviewModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var review = await reviewRepository.GetByIdAsync(id, cancellationToken);
        if (review is null)
            throw new KeyNotFoundException($"Review with id {id} not found");

        var reviewModel = review.ToModel();

        return reviewModel;
    }

    public async Task<List<ReviewModel>> GetByBookIdAsync(int bookId, CancellationToken cancellationToken = default)
    {
        var reviews = await reviewRepository.GetByBookIdAsync(bookId, cancellationToken);
        var reviewModels = reviews.Select(r => r.ToModel()).ToList();

        return reviewModels;
    }
}
