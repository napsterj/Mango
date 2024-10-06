using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models
{
    public class ProductDetailsDto
    {
        public int ProductId { get; set; }
        
        public string Name { get; set; }
        
        public decimal? ProductPrice { get; set; }

        [Range(1, 200,ErrorMessage ="Product quantity can only be minimum 1 to maximum 200.")]
        public int Count { get; set; }

        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string? ImageUrl { get; set; }
    }
}
