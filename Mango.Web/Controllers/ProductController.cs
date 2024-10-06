using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
		private readonly ICartService _cartService;

		public ProductController(IProductService productService, 
                                 ICartService cartService)
        {
            _productService = productService;
			_cartService = cartService;
		}

        [HttpGet]
        public async Task<IActionResult> Products()
        {
            var response = await _productService.GetProducts();
            List<ProductDto> products = new();
            if (response != null && response.IsSuccess)
            {
                var result = JsonConvert.DeserializeObject<DeserializeHandleForProducts>(Convert.ToString(response.Result));
                products = result.Result;
            }
            else if(response != null && !string.IsNullOrWhiteSpace(response.Message))
            {
                TempData["Error"] = "You are not authorized to view this page";
                return RedirectToAction("Index", "Home");
            }
            
            return View(products);
        }        

        [HttpGet]        
        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            var response = await _productService.GetProduct(productId);
            if(response != null && response.IsSuccess)
            {
                var deserialised = JsonConvert.DeserializeObject<DeserializeHandlerForProduct>(Convert.ToString(response.Result));
                return View(deserialised.Result);
            }
            else if(response != null && !string.IsNullOrWhiteSpace(response.Message))
            {
                TempData["Error"] = "You are not authorized to perform this operation";
                return RedirectToAction("Index", "Home");
            }
           
            var errors = ModelState.SelectMany(ms => ms.Value.Errors);

            if (errors.Any())
            {
                string commaseparatedError = string.Empty;

                foreach (var error in errors)
                {
                    commaseparatedError = string.Join(",", error.ErrorMessage);
                }

                ModelState.AddModelError("Errors", commaseparatedError);
            }

            return View(new ProductDetailsDto());
        }

        [HttpPost]
        public async Task<IActionResult> ProductDetails(ProductDetailsDto productDetailsDto)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    CartHeaderDto cartHeaderDto = new()
                    {
                        UserId = User.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub)?.Value,
                    };

                    ProductDto productDto = new()
                    {
                        CategoryName = productDetailsDto.CategoryName,
                        Count = productDetailsDto.Count,
                        Description = productDetailsDto.Description,
                        ImageUrl = productDetailsDto.ImageUrl,
                        Name = productDetailsDto.Name,
                        ProductId = productDetailsDto.ProductId,
                        ProductPrice =productDetailsDto.ProductPrice.Value,
                    };

                    CartDetailsDto cartDetailsDto = new()
                    {
                        //Count = productDto.Count.Value,
                        ProductDto = productDto,                        
                    };

                    CartDto cartDto = new()
                    {
                        CartHeaderDto = cartHeaderDto,
                        CartDetailsDto = new HashSet<CartDetailsDto> { cartDetailsDto }
                    };
                    
                    var responseDto = await _cartService.UpsertCart(cartDto);
                    
                    if(responseDto != null && responseDto.IsSuccess) 
                    {
                        TempData["Success"] = "Product successfully added to the cart";
                        return RedirectToAction("Index", "ShoppingCart");
                    }
                }
            }
            catch(Exception ex) 
            { 
                Console.WriteLine(ex.Message);
            }

            return View(productDetailsDto);
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
                
                var productDetailsDto = result.Result;

                productDto.ImageUrl = productDetailsDto.ImageUrl;
                productDto.ProductPrice = productDetailsDto.ProductPrice.Value;
                productDto.Description = productDetailsDto.Description;
                productDto.ProductId = productDetailsDto.ProductId;
                productDto.CategoryName = productDetailsDto.CategoryName;
                //productDto.Count = productDetailsDto.Count;

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
