namespace StoreFront.Domain.Models.OrderModels.Requests
{
    public class CartRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
