using Domain.Aggregates.FeedbackAggregate.Entities;
using Domain.Repositories.BaseRepositories;

namespace Domain.Repositories
{
    public interface IFeedbackRepository : ISqlRepository<Feedback>
    {
        Task<bool> CheckExistUserInProduct(Guid userId, Guid productId);
        Task<Feedback?> GetMyFeedback(Guid userId, Guid productId);
    }
}
