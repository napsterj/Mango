using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var responseDto = await _couponService.GetCouponsAsync();
            var coupons = new List<CouponDto>();

            
            if(responseDto != null && responseDto.IsSuccess) 
            {
               var handleForList = JsonConvert.DeserializeObject<DeserializeHandleForCoupons>(Convert.ToString(responseDto.Result));
               coupons = handleForList.Result; 
            }

            return View(coupons);
        }

        [HttpGet]
        public IActionResult CreateCoupon()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoupon(CouponDto couponDto)
        {
            if (ModelState.IsValid)
            {
                var response = await _couponService.AddCouponAsync(couponDto);

                if (response != null && response.IsSuccess)
                {
                    
                    TempData["Success"] = "Coupon added successfully.";                    
                    return RedirectToAction("Index");
                }
                else if (!response.IsSuccess)
                {

                    TempData["Error"] = response.Message;
                }
            }
            return View(couponDto);

        }

        [HttpGet]
        public async Task<IActionResult> UpdateCoupon(int couponId)
        {
            var response = await _couponService.GetCouponByIdAsync(couponId);
            CouponDto? couponDto;
            ResponseDto responseDto = new ResponseDto();
                        
            if (response != null && response.IsSuccess && response.Result != null) 
            {
                couponDto = new();
                //couponDto.

            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCoupon(CouponDto couponDto)
        {
            if (ModelState.IsValid)
            {
                var response = await _couponService.UpdateCouponAsync(couponDto);

            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCoupon(int couponId)
        {
            var response =  await _couponService.DeleteCouponAsync(couponId);
            
            if(response != null && response.IsSuccess) 
            {
                TempData["Success"] = "Coupon deleted successfully";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = response?.Message;
                return RedirectToAction("Index");
            }

        }
    }
}
