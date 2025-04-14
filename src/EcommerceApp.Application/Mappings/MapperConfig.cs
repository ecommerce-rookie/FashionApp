using Application.Features.ProductFeatures.Models;
using Application.Features.UserFeatures.Models;
using AutoMapper;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.UserAggregate.Entities;

namespace Application.Mappings
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<User, AuthorResponseModel>();

            CreateMap<Product, ProductResponseModel>()
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Price!.UnitPrice.Amount))
                .ForMember(dest => dest.PurchasePrice, opt => opt.MapFrom(src => src.Price!.PurchasePrice.Amount))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category!.Name))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ImageProducts.Select(x => x.Image.Url)));

        }
    }
}
