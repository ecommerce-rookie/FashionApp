using Application.Features.FeedbackFeatures.Enums;
using Application.Features.FeedbackFeatures.Models;
using Application.Features.UserFeatures.Models;
using Domain.Aggregates.FeedbackAggregate.Entities;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using Infrastructure.Cache.Attributes;
using System.Text.Json.Serialization;

namespace Application.Features.FeedbackFeatures.Queries
{
    [Cache(nameof(Feedback), 60 * 5)]
    public class GetListFeedbackQuery : IQuery<PagedList<FeedbackResponseModel>>
    {
        [JsonIgnore]
        public Guid ProductId { get; set; }
        public int Page { get; set; }
        public int EachPage { get; set; }
    }

    public class GetListFeedbackQueryHandler : IQueryHandler<GetListFeedbackQuery, PagedList<FeedbackResponseModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetListFeedbackQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedList<FeedbackResponseModel>> Handle(GetListFeedbackQuery request, CancellationToken cancellationToken)
        {
            var feedbacks = await _unitOfWork.FeedbackRepository.GetAll(p => 
                p.ProductId.Equals(request.ProductId),
                p => new FeedbackResponseModel()
                {
                    Author = new AuthorResponseModel()
                    {
                        Avatar = p.CreatedByNavigation!.Avatar!.Url,
                        FirstName = p.CreatedByNavigation!.FirstName,
                        LastName = p.CreatedByNavigation.LastName,
                        Id = p.CreatedByNavigation.Id
                    },
                    CreatedAt = p.CreatedAt,
                    Content = p.Content,
                    Rating = p.Rating,
                    UpdatedAt = p.UpdatedAt
                },
                request.Page, request.EachPage,
                FeedbackSortBy.CreatedAt.ToString(), true
            );

            return feedbacks;
        }
    }

}
