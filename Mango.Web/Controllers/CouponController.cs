using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
               var handleForList = JsonConvert.DeserializeObject<DeserializeHandleForList>(Convert.ToString(responseDto.Result));
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
                    TempData["Error"] = "Coupon added successfully.";
                    return RedirectToAction("Index");
                }
                else if (!response.IsSuccess)
                {

                    TempData["Error"] = response.Message;
                }
            }
            return View(couponDto);

        }
    }
}
