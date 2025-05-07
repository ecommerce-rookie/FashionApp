namespace StoreFront.Domain.Models.CategoryModels.Requests
{
    public class CategoryRequestQuery
    {
        public int Page { get; set; }
        public int EachPage { get; set; }
        public string? Search { get; set; }
    }
}
