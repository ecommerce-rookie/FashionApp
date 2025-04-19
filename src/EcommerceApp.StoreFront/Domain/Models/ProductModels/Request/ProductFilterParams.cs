using static StoreFront.Domain.Enums.ProductEnums;

namespace StoreFront.Domain.Models.ProductModels.Request
{
    public class ProductFilterParams
    {
        public int Page { get; set; }
        public int EachPage { get; set; }
        public ProductSortBy? SortBy { get; set; }
        public bool? IsAscending { get; set; }
        public IEnumerable<int>? Categories { get; set; }
        public string? Search { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? IsNew { get; set; }
        public bool? IsSale { get; set; }
        public IEnumerable<string>? Sizes { get; set; }
    }
}
