using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Entities;
using OnlineLibrary.Domain.Enums;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Services;

/// <summary>
/// Represents an <see cref="ReviewService"/>.
/// </summary>
public class ReviewService(
    IReviewRepository reviewRepository,
    IBookRepository bookRepository,
    IUserRepository userRepository) : IReviewService
{
    public async Task<ReviewModel> CreateAsync(ReviewModel reviewModel, CancellationToken cancellationToken = default)
    {
        if (!await bookRepository.ExistAsync(b => b.Id == reviewModel.BookId, cancellationToken))
            throw new KeyNotFoundException($"Book with id {reviewModel.BookId} not found");

        if (!await userRepository.ExistAsync(u => u.Id == reviewModel.UserId, cancellationToken))
            throw new KeyNotFoundException($"User with id {reviewModel.UserId} not found");

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

        if (!await bookRepository.ExistAsync(b => b.Id == reviewModel.BookId, cancellationToken))
            throw new KeyNotFoundException($"Book with id {reviewModel.BookId} not found");

        if (!await userRepository.ExistAsync(u => u.Id == reviewModel.UserId, cancellationToken))
            throw new KeyNotFoundException($"User with id {reviewModel.UserId} not found");

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

    public async Task<PagedList<ReviewModel>> GetByBookIdAsync(int bookId, int page, int pageSize, string? orderBy = null, OrderType orderType = OrderType.Desc, CancellationToken cancellationToken = default)
    {
        var paged = await reviewRepository.GetByBookIdPagedAsync(bookId, page, pageSize, BuildOrderBy(orderBy, orderType), cancellationToken);

        return new PagedList<ReviewModel>
        {
            Items = paged.Items.Select(r => r.ToModel()).ToList(),
            TotalCount = paged.TotalCount,
            CurrentPage = paged.CurrentPage,
            PageSize = paged.PageSize
        };
    }

    private static Func<IQueryable<Review>, IOrderedQueryable<Review>> BuildOrderBy(string? orderBy, OrderType orderType) =>
        orderBy?.ToLower() switch
        {
            "rating" => orderType == OrderType.Asc
                ? q => q.OrderBy(r => r.Rating)
                : q => q.OrderByDescending(r => r.Rating),
            _ => orderType == OrderType.Asc
                ? q => q.OrderBy(r => r.CreatedAt)
                : q => q.OrderByDescending(r => r.CreatedAt)
        };

    public async Task DeleteAsync(int id, int callerId, CancellationToken cancellationToken = default)
    {
        var review = await reviewRepository.GetByIdAsync(id, cancellationToken);
        if (review is null)
            throw new KeyNotFoundException($"Review with id {id} not found");

        if (review.UserId != callerId)
            throw new UnauthorizedAccessException("You do not own this review.");

        reviewRepository.Delete(review);
        await reviewRepository.SaveChangesAsync(cancellationToken);
    }
}
