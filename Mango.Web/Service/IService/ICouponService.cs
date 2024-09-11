using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface ICouponService
    {
        Task<ResponseDto> GetCouponsAsync();
        Task<ResponseDto> GetCouponByIdAsync(int couponId);
        Task<ResponseDto> GetCouponByCodeAsync(string couponCode);
        Task<ResponseDto> AddCouponAsync(CouponDto couponDto);
        Task<ResponseDto> UpdateCouponAsync(CouponDto couponDto);
        Task<ResponseDto> DeleteCouponAsync(int couponId);
    }
}
