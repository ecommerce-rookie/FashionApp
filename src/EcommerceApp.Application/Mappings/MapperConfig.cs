using Application.Features.FeedbackFeatures.Models;
using Application.Features.OrderFeatures.Models;
using Application.Features.ProductFeatures.Models;
using Application.Features.UserFeatures.Models;
using Application.Utilities;
using AutoMapper;
using Domain.Aggregates.FeedbackAggregate.Entities;
using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.Models.Common;

namespace Application.Mappings
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<User, AuthorResponseModel>()
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar!.Url));

            CreateMap<Product, ProductResponseModel>()
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Price!.UnitPrice.Amount))
                .ForMember(dest => dest.PurchasePrice, opt => opt.MapFrom(src => src.Price!.PurchasePrice.Amount))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category!.Name))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.CreatedByNavigation))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ImageProducts!.Select(x => x.Image.Url)))
                .ForMember(dest => dest.IsNew, opt => opt.MapFrom(src => src.CreatedAt.IsNewProduct()))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Feedbacks!.Count()))
                .ForMember(dest => dest.Star, opt => opt.MapFrom(src => src.Feedbacks!.Count() == 0 ? 0 : src.Feedbacks!.Sum(x => x.Rating) / src.Feedbacks!.Count()))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Product, ProductPreviewResponseModel>()
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Price!.UnitPrice.Amount))
                .ForMember(dest => dest.PurchasePrice, opt => opt.MapFrom(src => src.Price!.PurchasePrice.Amount))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ImageProducts!
                    .Where(i => i.OrderNumber == 1)
                    .Select(i => i.Image.Url)
                    .FirstOrDefault()))
                .ForMember(dest => dest.IsNew, opt => opt.MapFrom(src => src.CreatedAt.IsNewProduct()))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<User, UserPreviewResponseModel>()
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar!.Url))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value));

            CreateMap<PagedList<User>, PagedList<UserPreviewResponseModel>>();

            CreateMap<Order, OrderResponseModel>()
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice!.Amount))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
                .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => src.OrderDetails!.Count()));


            CreateMap<PagedList<Order>, PagedList<OrderResponseModel>>();

            CreateMap<Feedback, FeedbackResponseModel>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.CreatedByNavigation));
        }
    }
}
