using Application.Features.UserFeatures.Models;
using Domain.Aggregates.OrderAggregate.Enums;
using Infrastructure.Authentication.Settings;

namespace Application.Features.OrderFeatures.Models
{
    public class OrderResponseModel
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

        public AuthorResponseModel? Customer { get; set; }
    }
}
