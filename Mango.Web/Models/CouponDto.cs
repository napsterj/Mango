using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models
{
    public class CouponDto
    {        
        public int CouponId { get; set; } = 0;

        [Required(ErrorMessage = "* Coupon code is mandatory")]
        public string CouponCode { get; set; }
        
        [Required(ErrorMessage = "* Discount amount is mandatory")]
        public double DiscountAmount { get; set; }

        [Required(ErrorMessage = "* Minimum amount is mandatory")]
        public int MinAmount { get; set; }
    }
}
