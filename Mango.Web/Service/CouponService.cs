using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using apiType = Mango.Web.Utilities.Enums.ApiType;

namespace Mango.Web.Service
{
    public class CouponService : ICouponService
    {
        private readonly IBaseService _baseService;
        private string BaseUrl = "https://localhost:7001";
        public CouponService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto> AddCouponAsync(CouponDto couponDto)
        {
            var requestDto = new RequestDto
            {
                ApiType = apiType.POST,
                Url = $"{BaseUrl}/api/coupon",
                Data = JsonConvert.SerializeObject(couponDto),
            };

            return await _baseService.SendAsync(requestDto);

        }

        public async Task<ResponseDto> DeleteCouponAsync(int couponId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = apiType.DELETE,
                Url = $"{BaseUrl}/api/coupon/{couponId}"
            });
        }

        public async Task<ResponseDto> GetCouponByCodeAsync(string couponCode)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType= apiType.GET,
                Url = $"{BaseUrl}/api/coupon/GetCouponByCode/{couponCode}",
            });
        }

        public async Task<ResponseDto> GetCouponByIdAsync(int couponId)
        {
           return await _baseService.SendAsync(new RequestDto
            {
                ApiType = apiType.GET,
                Url = $"{BaseUrl}/api/coupon/{couponId}",
            });
        }

        public async Task<ResponseDto> GetCouponsAsync()
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Url = $"{BaseUrl}/api/coupon",
                ApiType = apiType.GET
            });
        }

        public async Task<ResponseDto> UpdateCouponAsync(CouponDto couponDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = apiType.PUT,
                Url = $"{BaseUrl}/api/coupon",
                Data = JsonConvert.SerializeObject(couponDto)
            });
        }
    }
}
