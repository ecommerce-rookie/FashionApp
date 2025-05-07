namespace Application.Features.OrderFeatures.Models
{
    public class CartRequestModel
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
