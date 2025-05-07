namespace StoreFront.Domain.Models.OrderModels.Responses
{
    public class OrderDetailResponse
    {
        public int Id { get; set; }

        public int? Quantity { get; set; }

        public decimal? Price { get; set; }

        public string? Size { get; set; }

        public string? ImageProduct { get; set; }

        public string? NameProduct { get; set; }

        public string? Slug { get; set; }
    }
}
