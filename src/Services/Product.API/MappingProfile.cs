using AutoMapper;
using Product.API.Entities;
using Shared.Dtos.Products;
using Infrastructure.Extensions;
namespace Product.API
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<CatalogProduct, ProductDto>();
            CreateMap<CreateProductDto, CatalogProduct>();
            CreateMap<UpdateProductDto, CatalogProduct>().IgnoreAllNonExisting();
        }
    }
}
