using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Products()
        {
            var response = await _productService.GetProducts();
            List<ProductDto> products = new();
            if(response != null && response.IsSuccess)
            {                
                var result = JsonConvert.DeserializeObject<DeserializeHandleForProducts>(Convert.ToString(response.Result));
                products = result.Result;
            }
            
            return View(products);
        }        

        [HttpGet]
        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductDto productDto)
        {
            try
            {                
                if (ModelState.IsValid)
                {
                    ResponseDto response = await _productService.CreateNewProduct(productDto);
                    if (response != null && response.IsSuccess)
                    {
                        return RedirectToAction("Products");
                    }
                    
                    if (response != null && !response.IsSuccess)
                    {
                        TempData["Error"] = response.Message;
                    }
                }                                                
            } 
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.ToString());
            }

            return View(productDto);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProduct(int productId)
        {
            var responseDto = await _productService.GetProduct(productId);
            
            ProductDto productDto = new();
            
            if (responseDto != null && responseDto.IsSuccess)
            {
                var result = JsonConvert.DeserializeObject<DeserializeHandlerForProduct>(Convert.ToString(responseDto.Result));
                productDto = result.Result;
                return View(productDto);
            }
            else if (responseDto != null && !responseDto.IsSuccess)
            {
                TempData["Error"] = responseDto.Message;
            }
            
            return View(productDto);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(ProductDto productDto)
        {
            var response = await _productService.UpdateProduct(productDto);

            if(response != null && response.IsSuccess)
            {
                TempData["Success"] = "Product updated successfully";

                return RedirectToAction("Products");
            }
            else if (response != null && !response.IsSuccess)
            {
                TempData["Error"] = response.Message;
            }

            return View(productDto);   
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var response = await _productService.DeleteProduct(productId);
            if(response != null && response.IsSuccess)
            {
                TempData["Success"] = "Product deleted successfully";
                return RedirectToAction("Products");
            }
            else if (response != null && !response.IsSuccess)
            {
                TempData["Error"] = response.Message;
            }

            return RedirectToAction("Products");
        }

    }
}
