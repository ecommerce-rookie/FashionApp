using Domain.Aggregates.FeedbackAggregate.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repository
{
    public class FeedbackRepository : SqlRepository<Feedback>, IFeedbackRepository
    {
        public FeedbackRepository(EcommerceContext context) : base(context)
        {
        }

        public async Task<bool> CheckExistUserInProduct(Guid userId, Guid productId)
        {
            return await _dbSet.AnyAsync(x => x.UserId.Equals(userId) && x.ProductId.Equals(productId));
        }

        public async Task<Feedback?> GetMyFeedback(Guid userId, Guid productId)
        {
            return await _dbSet
                .Include(f => f.CreatedByNavigation)
                .FirstOrDefaultAsync(x => x.UserId.Equals(userId) && x.ProductId.Equals(productId));
        }

    }
}
