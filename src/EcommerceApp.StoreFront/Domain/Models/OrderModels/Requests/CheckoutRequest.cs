using static StoreFront.Domain.Enums.OrderEnum;

namespace StoreFront.Domain.Models.OrderModels.Requests
{
    public class CheckoutRequest
    {
        public IEnumerable<CartRequest>? Carts { get; set; }
        public string? Address { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public string? NameReceiver { get; set; }
    }
}
