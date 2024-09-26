using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/Coupon")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private ResponseDto _responseDto;
        public CouponController(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _responseDto = new ResponseDto();
        }

        [HttpGet]
        [Route("get/coupons")]
        public ResponseDto Get()
        {
            try
            {
                var coupons = _appDbContext.Coupons.ToList();
                _responseDto.Result = _mapper.Map<IEnumerable<CouponDto>>(coupons);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpGet]
        [Route("GetCoupon/{couponId:int}")]
        
        public ResponseDto Get(int couponId)
        {
            try
            {
                var coupon = _appDbContext.Coupons.First(c => c.CouponId == couponId);
                _responseDto.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpGet]
        [Route("GetCouponByCode/{couponCode}")]
        public ResponseDto GetCouponByCode(string couponCode)
        {
            try
            {
                var coupon = _appDbContext.Coupons
                                          .FirstOrDefault(cc => cc.CouponCode.ToLower() == couponCode.ToLower());
                if(coupon == null)
                {
                    _responseDto.IsSuccess=false;
                    _responseDto.Message = "Invalid coupon code.";
                    return _responseDto;
                }

                _responseDto.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch(Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;                
            }
            return _responseDto;
        }

        [HttpPost]
        [Authorize(Roles = "SUPERADMIN")]
        [Route("AddCoupon")]
        public ResponseDto AddCoupon([FromBody]CouponDto couponDto)
        {
            try
            {
                var coupon = _mapper.Map<Coupon>(couponDto);
                _appDbContext.Coupons.Add(coupon);
                _appDbContext.SaveChanges();

                _responseDto.Result = _mapper.Map<CouponDto>(coupon);

            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }

            return _responseDto;
        }


        [HttpPut]
        [Authorize(Roles= "SUPERADMIN")]
        public ResponseDto UpdateCoupon([FromBody] CouponDto couponDto)
        {
            try
            {                
                var coupon = _mapper.Map<Coupon>(couponDto);
                _appDbContext.Coupons.Update(coupon);
                _appDbContext.SaveChanges();

                _responseDto.Result = _mapper.Map<CouponDto>(coupon);

            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }

            return _responseDto;
        }

        [HttpDelete]
        [Authorize(Roles= "SUPERADMIN")]
        [Route("DeleteCoupon/{couponId:int}")]        
        public ResponseDto DeleteCoupon(int couponId)
        {
            try
            {
                var coupon = _appDbContext.Coupons.First(x =>x.CouponId == couponId);
                _appDbContext.Coupons.Remove(coupon);
                _appDbContext.SaveChanges();

                _responseDto.IsSuccess = true;                      

            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }

            return _responseDto;
        }
    }
}
