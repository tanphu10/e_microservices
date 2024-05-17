using AutoMapper;
using Product.API.Entities;
using Shared.Dtos.Products;
using Infrastructure.Mapping;
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
