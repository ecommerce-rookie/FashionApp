using Application.Features.OrderFeatures.Models;
using Domain.Models.Common;
using Domain.Shared;

namespace Application.Features.OrderFeatures.Queries
{
    public class GetOrderQuery : IQuery<APIResponse<OrderDetailResponseModel>>
    {
    }
}
