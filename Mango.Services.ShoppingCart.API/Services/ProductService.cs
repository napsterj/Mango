using Mango.Services.ShoppingCart.API.Models;
using Mango.Services.ShoppingCart.API.Models.Dto;
using Mango.Services.ShoppingCart.API.Services.IServices;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCart.API.Services
{
	public class ProductService : IProductService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _configuration;

		public ProductService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
		{
			_httpClientFactory = httpClientFactory;
			_configuration = configuration;
		}

		public async Task<ProductDto> GetProduct(int productId)
		{
			HttpResponseMessage response = null;
			
			var client = _httpClientFactory.CreateClient("Producr");
		    
			response = await client.GetAsync("/api/product");			
			
			if(response != null) 
			{
				var result = await response.Content.ReadAsStringAsync();	
				var responseDto = JsonConvert.DeserializeObject<ResponseDto>(result);
				if (responseDto != null && responseDto.IsSuccess)
				{
					var productHandler = JsonConvert.DeserializeObject<DeserializeHandlerForProduct>(Convert.ToString(responseDto.Result));					
					return productHandler.Result;					
				}								
			}
			
			return new ProductDto();
		}
	}
}
