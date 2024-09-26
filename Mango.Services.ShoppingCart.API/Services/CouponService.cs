using AutoMapper;
using Mango.Services.ShoppingCart.API.Data;
using Mango.Services.ShoppingCart.API.Models.Dto;
using Mango.Services.ShoppingCart.API.Services.IServices;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCart.API.Services
{
	public class CouponService : ICouponService
	{		
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IMapper _mapper;
		ResponseDto _responseDto;
		public CouponService(IHttpClientFactory httpClientFactory, IMapper mapper)
		{	
			_httpClientFactory = httpClientFactory;
			_mapper = mapper;
			_responseDto = new ResponseDto();
		}
		
		public async Task<CouponDto> GetCouponByCode(string couponCode)
		{
			var client = _httpClientFactory.CreateClient("Coupon");
			
			var httpResponse = await client.GetAsync(new Uri($"/api/GetCouponByCode/{couponCode}"));
			
			if(httpResponse != null)
			{
			   var response =  await httpResponse.Content.ReadAsStringAsync();
			   
			   _responseDto =  JsonConvert.DeserializeObject<ResponseDto>(response);
			   
			   if(_responseDto != null && _responseDto.IsSuccess)
			   {
					return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(_responseDto.Result));
			   }			   			   
			}

			return new CouponDto();
		}
	}
}
