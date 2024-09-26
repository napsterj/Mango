using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;

namespace Mango.Services.ProductAPI
{
    public class MapperConfig 
    {
       public static MapperConfiguration ConfigureEntitiesDto()
       {
            var configure = new MapperConfiguration(config =>
            {                
                config.CreateMap<Product, ProductDto>();
                config.CreateMap<ProductDto, Product>();
            });
            return configure;
       }
    }
}
