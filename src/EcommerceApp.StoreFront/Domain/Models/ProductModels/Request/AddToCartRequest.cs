namespace StoreFront.Domain.Models.ProductModels.Request
{
    public class AddToCartRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
