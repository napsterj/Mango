using AutoMapper;
using Mango.Services.ShoppingCart.API.Models;
using Mango.Services.ShoppingCart.API.Models.Dto;

namespace Mango.Services.ShoppingCart.API
{
	public class MapperConfig
	{
		
		public static MapperConfiguration TranslateDtoEntities()
		{
			var configuration = new MapperConfiguration(config =>
			{
				config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
				config.CreateMap<CartDetailsDto, CartDetails>().ReverseMap();						
			});

			return configuration;


		}
	}
}
