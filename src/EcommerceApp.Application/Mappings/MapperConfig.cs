using Application.Features.ProductFeatures.Models;
using Application.Features.UserFeatures.Models;
using Application.Utilities;
using AutoMapper;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.UserAggregate.Entities;

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
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ImageProducts.Select(x => x.Image.Url)))
                .ForMember(dest => dest.IsNew, opt => opt.MapFrom(src => src.CreatedAt.IsNewProduct()))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Product, ProductPreviewResponseModel>()
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Price!.UnitPrice.Amount))
                .ForMember(dest => dest.PurchasePrice, opt => opt.MapFrom(src => src.Price!.PurchasePrice.Amount))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ImageProducts
                    .Where(i => i.OrderNumber == 1)
                    .Select(i => i.Image.Url)
                    .FirstOrDefault()))
                .ForMember(dest => dest.IsNew, opt => opt.MapFrom(src => src.CreatedAt.IsNewProduct()))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}
