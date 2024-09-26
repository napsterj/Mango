using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models
{
    public class ProductDto
    {    
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(1, 1000)]
        public double? ProductPrice { get; set; }

        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string? ImageUrl { get; set; }
    }
}
