using Newtonsoft.Json;
using Refit;
using StoreFront.Application.Helpers;
using StoreFront.Domain.Models.Common;
using System.Net;

namespace StoreFront.Application.Services
{
    public class SafeApiCaller
    {
        private readonly ILogger<SafeApiCaller> _logger;

        public SafeApiCaller(ILogger<SafeApiCaller> logger)
        {
            _logger = logger;
        }

        public async Task<PagedList<T>> CallListSafeAsync<T>(
            Func<Task<ApiResponse<IEnumerable<T>>>> apiCall) where T : class
        {
            try
            {
                var response = await apiCall();

                if (response.IsSuccessStatusCode)
                {
                    var result = response.ToPagedList();

                    _logger.LogInformation(
                        "API call successful. Status code: {StatusCode}, Total items: {TotalItems}, Current page: {CurrentPage}",
                        response.StatusCode, result.TotalItems, result.CurrentPage);

                    return result;
                }

                return new PagedList<T>();
            } catch (ApiException ex)
            {
                _logger.LogError(
                    "API call failed. Status code: {StatusCode}, Error message: {ErrorMessage}",
                    ex.StatusCode, ex.Content ?? ex.Message);

                return new PagedList<T>();
            } catch (Exception ex)
            {
                _logger.LogError(
                    "An unexpected error occurred. Error message: {ErrorMessage}",
                    ex.Message);

                return new PagedList<T>();
            }
        }

        public async Task<APIResponse> CallSafeAsync(Func<Task<ApiResponse<APIResponse>>> apiCall)
        {
            try
            {
                var response = await apiCall();

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content;
                    _logger.LogInformation(
                        "API call successful. Status code: {StatusCode}, Message: {Message}",
                        response.StatusCode, result?.Message);

                    return result!;
                }

                _logger.LogError(
                    "API call failed. Status code: {StatusCode}, Error message: {ErrorMessage}",
                    response.StatusCode, response.Error?.Message);

                return new APIResponse
                {
                    Status = response.StatusCode,
                    Message = response.Error?.Message ?? "Unexpected error"
                };
            } catch (ApiException ex)
            {
                _logger.LogError(
                    "API call failed. Status code: {StatusCode}, Error message: {ErrorMessage}",
                    ex.StatusCode, ex.Content ?? ex.Message);

                var content = JsonConvert.DeserializeObject<APIResponse>(ex.Content!);

                return content ?? new APIResponse
                {
                    Status = ex.StatusCode,
                    Message = ex.Message
                };
            } catch (Exception ex)
            {
                _logger.LogError(
                    "An unexpected error occurred. Error message: {ErrorMessage}",
                    ex.Message);

                return new APIResponse
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = ex.Message
                };
            }
        }

        public async Task<APIResponse<T?>> CallSafeAsync<T>(
            Func<Task<ApiResponse<APIResponse<T?>>>> apiCall)
        {
            try
            {
                var response = await apiCall();

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content;
                    _logger.LogInformation(
                        "API call successful. Status code: {StatusCode}, Message: {Message}",
                        response.StatusCode, result?.Message);

                    return result!;
                }

                _logger.LogError(
                    "API call failed. Status code: {StatusCode}, Error message: {ErrorMessage}",
                    response.StatusCode, response.Error?.Message);

                return new APIResponse<T?>
                {
                    Status = response.StatusCode,
                    Message = response.Error?.Message ?? "Unexpected error",
                    Data = default
                };

            } catch (ApiException ex)
            {
                _logger.LogError(
                    "API call failed. Status code: {StatusCode}, Error message: {ErrorMessage}",
                    ex.StatusCode, ex.Content ?? ex.Message);

                var content = JsonConvert.DeserializeObject<APIResponse<T?>>(ex.Content!);

                if(content == null)
                {
                    return new APIResponse<T?>
                    {
                        Status = ex.StatusCode,
                        Message = ex.Message,
                        Data = default
                    };
                }

                return content;
            } catch (Exception ex)
            {
                _logger.LogError(
                    "An unexpected error occurred. Error message: {ErrorMessage}",
                    ex.Message);

                return new APIResponse<T?>
                {
                    Status = System.Net.HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Data = default
                };
            }
        }
    }

}
