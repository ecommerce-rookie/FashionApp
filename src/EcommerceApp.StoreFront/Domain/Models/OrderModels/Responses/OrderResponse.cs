
using StoreFront.Domain.Models.UserModels.Responses;
using static StoreFront.Domain.Enums.OrderEnum;

namespace StoreFront.Domain.Models.OrderModels.Responses
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public decimal TotalPrice { get; set; }

        public string? Address { get; set; }

        public OrderStatus? OrderStatus { get; set; }

        public PaymentMethod? PaymentMethod { get; set; }

        public string? NameReceiver { get; set; }

        public int TotalItems { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public AuthorResponse? Customer { get; set; }

        public IEnumerable<OrderDetailResponse>? Details { get; set; }
    }
}
