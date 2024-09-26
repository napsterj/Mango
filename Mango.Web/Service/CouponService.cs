using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using apiType = Mango.Web.Utilities.Enums.ApiType;

namespace Mango.Web.Service
{
    public class CouponService : ICouponService
    {
        private readonly IBaseService _baseService;
        private readonly ITokenProvider _tokenProvider;
        private string BaseUrl = "https://localhost:7001";
        public CouponService(IBaseService baseService, 
                             ITokenProvider tokenProvider)
        {
            _baseService = baseService;
            _tokenProvider = tokenProvider;
        }

        public async Task<ResponseDto> AddCouponAsync(CouponDto couponDto)
        {
            var requestDto = new RequestDto
            {
                ApiType = apiType.POST,
                Url = $"{Enums.CouponAPIBase}/api/coupon/AddCoupon",
                Data = couponDto,
                AccessToken = _tokenProvider.GetToken()
            };

            return await _baseService.SendAsync(requestDto);

        }

        public async Task<ResponseDto> DeleteCouponAsync(int couponId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = apiType.DELETE,
                Url = $"{Enums.CouponAPIBase}/api/coupon/DeleteCoupon/{couponId}",
                AccessToken = _tokenProvider.GetToken()
            });
        }

        public async Task<ResponseDto> GetCouponByCodeAsync(string couponCode)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType= apiType.GET,
                Url = $"{Enums.CouponAPIBase}/api/coupon/GetCouponByCode/{couponCode}",
                AccessToken = _tokenProvider.GetToken()
            });
        }

        public async Task<ResponseDto> GetCouponByIdAsync(int couponId)
        {
           return await _baseService.SendAsync(new RequestDto
            {
                ApiType = apiType.GET,
                Url = $"{Enums.CouponAPIBase}/api/coupon/GetCoupon/{couponId}",
                AccessToken = _tokenProvider.GetToken()
           });
        }

        public async Task<ResponseDto> GetCouponsAsync()
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Url = $"{Enums.CouponAPIBase}/api/coupon/get/coupons",
                ApiType = apiType.GET,
                AccessToken = _tokenProvider.GetToken()
            });
        }

        public async Task<ResponseDto> UpdateCouponAsync(CouponDto couponDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = apiType.PUT,
                Url = $"{Enums.CouponAPIBase}/api/coupon/UpdateCoupon",
                Data = couponDto,
                AccessToken = _tokenProvider.GetToken()
            });
        }
    }
}
