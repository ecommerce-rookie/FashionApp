using Application.Features.UserFeatures.Models;
using Refit;
using StoreFront.Domain.Models.Common;

namespace StoreFront.Application.Services
{
    public interface IUserService
    {
        [Post("/users")]
        [Multipart]
        Task<ApiResponse<APIResponse>> CreateUserInfo(
                [AliasAs("firstName")] string firstName,
                [AliasAs("lastName")] string lastName,
                [AliasAs("phone")] string phoneNumber,
                [AliasAs("address")] string address,
                [AliasAs("avatar")] StreamPart avatar
            );

        [Get("/users/author")]
        Task<APIResponse<AuthorResponse>> GetAuthorInfo();

    }
}
