using Newtonsoft.Json;
using Refit;
using StoreFront.Domain.Models.Common;

namespace StoreFront.Application.Helpers
{
    public static class PaginationHelper
    {
        public static PagedList<T> ToPagedList<T>(this ApiResponse<IEnumerable<T>> response) where T : class
        {
            if (response.IsSuccessStatusCode)
            {
                var pagedProducts = response.Content;
                Metadata? metadata = null;

                if (response.Headers.TryGetValues("X-Pagination", out var totalPagesValues))
                {
                    metadata = JsonConvert.DeserializeObject<Metadata>(totalPagesValues.FirstOrDefault()!);
                }

                if (metadata != null)
                {
                    return new PagedList<T>(
                        pagedProducts!,
                        metadata.TotalCount,
                        metadata.CurrentPage,
                        metadata.PageSize
                    );
                }

                return new PagedList<T>(pagedProducts!); 
            }

            var errorMessage = response.Error;
            Console.WriteLine($"Error: {errorMessage.Message}");

            return new PagedList<T>(new List<T>(), 0, 0, 0);
        }
    }
}
