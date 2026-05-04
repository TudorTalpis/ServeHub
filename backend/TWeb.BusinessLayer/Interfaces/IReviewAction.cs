using TWeb.Domain.Models;

namespace TWeb.BusinessLayer.Interfaces;

public interface IReviewAction
{
    List<ReviewDto> GetAllReviewAction();
    List<ReviewDto> GetByProviderIdReviewAction(string providerId);
    ReviewDto CreateReviewAction(CreateReviewDto dto);
}
