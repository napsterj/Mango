using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
	public class ShoppingCartController : Controller
	{
        private readonly ICartService _cartService;
        private readonly IProductService _productService;

        public ShoppingCartController(ICartService cartService, 
									  IProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }

		[HttpGet]
        public async Task<IActionResult> Index()
		{
			try
			{				
                var cartDto = await LoadUsersShoppingCart();
                
				if (cartDto != null) 
				{
					return View(cartDto);
				}
			} 
			catch (Exception ex) 
			{ 
				Console.WriteLine(ex.Message);
			}

			return View(new CartDto());
		}

		[HttpPost]
		public async Task<IActionResult> EmailCart(CartDto cartDto)
		{
			try
			{
				cartDto = await LoadUsersShoppingCart();

                if (cartDto != null)
				{
					var responseDto = await _cartService.EmailCart(cartDto);
				
					if (responseDto != null && responseDto.IsSuccess)
					{
						TempData["Success"] = "Your will receive an email with details of your products in shopping basket.";
					}
					else if (responseDto != null && !string.IsNullOrWhiteSpace(responseDto.Message))
					{
						TempData["Error"] = "Some error has occured. Please try again later";
					}
				}
				else 
				{ 
					TempData["Error"] = "Some error has occured. Please try again later"; 
				}
            }
			catch(Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}

            return RedirectToAction("Index");
        }


		[HttpPost]
		public async Task<IActionResult> ProductDetails(ProductDto productDto)
		{
			try
			{
				CartHeaderDto cartHeaderDto = new()
				{
					UserId = User.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub)?.Value,
				};

				CartDetailsDto cartDetailsDto = new()
				{
					ProductDto = productDto,
					ProductId = productDto.ProductId,
					Count = productDto.Count.Value,
				};

				CartDto cartDto = new() 
				{ 
					CartHeaderDto = cartHeaderDto, 
					CartDetailsDto = new HashSet<CartDetailsDto>() { cartDetailsDto }
				};

				var responseDto = await _cartService.UpsertCart(cartDto);

				if (responseDto != null && responseDto.IsSuccess)
				{
					TempData["Success"] = "Product added successfully to the cart";
					
				}
				else
				{
					TempData["Error"] = responseDto.Message;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

            return RedirectToAction(nameof(Index));

        }

		private async Task<CartDto> LoadUsersShoppingCart()
		{
			try
			{
                var response = await _cartService.LoadCart(User.Claims
                                                                  .FirstOrDefault(usr =>
                                                                          usr.Type == JwtRegisteredClaimNames.Sub).Value);
				if(response != null && response.IsSuccess) 
				{
					var deserializedCartDto = JsonConvert.DeserializeObject<DeserializeForCartDto>(Convert.ToString(response.Result));
					return deserializedCartDto.Result;
				}
            }
			catch (Exception ex) 
			{ 
				Console.WriteLine(ex.ToString());
			}
			return new CartDto();
		}
	}
}
