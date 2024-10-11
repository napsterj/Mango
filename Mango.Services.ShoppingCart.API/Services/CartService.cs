using AutoMapper;
using Mango.MessageBus;
using Mango.Services.ShoppingCart.API.Data;
using Mango.Services.ShoppingCart.API.Models;
using Mango.Services.ShoppingCart.API.Models.Dto;
using Mango.Services.ShoppingCart.API.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCart.API.Services
{
	public class CartService : ICartService
	{
		private readonly CartDbContext _cartContext;
		private readonly IMapper _mapper;
		private readonly IProductService _productService;
		private readonly ICouponService _couponService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        private ResponseDto _responseDto;

		public CartService(CartDbContext cartContext,
						   IMapper mapper,
						   IProductService productService,
						   ICouponService couponService, 
						   IMessageBus messageBus, 
						   IConfiguration configuration)
		{
			_cartContext = cartContext;
			_mapper = mapper;
			_productService = productService;
			_couponService = couponService;
            _messageBus = messageBus;
            _configuration = configuration;
            _responseDto = new ResponseDto();
		}

		public async Task<ResponseDto> ApplyCoupon(CartDto cartDto)
		{
			try
			{
				var cartHeader = await _cartContext.CartHeaders.FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeaderDto.UserId);
			
				if (cartHeader != null)
				{
					cartHeader.CouponCode = cartDto.CartHeaderDto.CouponCode;
					_cartContext.CartHeaders.Update(cartHeader);
					await _cartContext.SaveChangesAsync();
					_responseDto.IsSuccess = true;					
				}								
			}
			catch (Exception ex)
			{
				_responseDto.IsSuccess = false;
				_responseDto.Result = ex.Message;
			}

			return _responseDto;
		}

        public async Task<ResponseDto> EmailCart(CartDto cartDto)
        {
			try
			{
				var queueName = _configuration.GetValue<string>("ServiceBus:MessageQueue");
				
				// Commenting below code to save Azure free credit. Uncomment when using Service bus functionality.
				//await _messageBus.PostMessageToBus(cartDto, queueName);
				_responseDto.IsSuccess = true;
			}
			catch(Exception ex) 
			{
				_responseDto.IsSuccess = false;
				_responseDto.Message = ex.ToString();
			}

			return _responseDto;
        }

        public async Task<ResponseDto> LoadCart(string userId)
		{			
			var cardDto = new CartDto();
		
			try
			{
				// get cart header 
				cardDto.CartHeaderDto = _mapper.Map<CartHeaderDto>(await _cartContext.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId));

				//get cart details
				cardDto.CartDetailsDto = _mapper.Map<IEnumerable<CartDetailsDto>>(_cartContext.CartDetails.Where(u => u.CartHeaderId == cardDto.CartHeaderDto.CartHeaderId));

				// get each product added from card details and extract name of product, price from product table
				foreach (var item in cardDto.CartDetailsDto)
				{
					item.ProductDto = _mapper.Map<ProductDto>(await _productService.GetProduct(item.ProductId));
					item.CartHeaderDto = cardDto.CartHeaderDto;
					item.CartHeaderDto.CartTotal += item.Count * item.ProductDto.ProductPrice.Value;
				}

				if (!string.IsNullOrWhiteSpace(cardDto.CartHeaderDto.CouponCode))
				{
					var couponDto = await _couponService.GetCouponByCode(cardDto.CartHeaderDto.CouponCode);
					if (cardDto.CartHeaderDto.CartTotal > couponDto.MinAmount)
					{
						cardDto.CartHeaderDto.CartTotal -= couponDto.DiscountAmount;
						cardDto.CartHeaderDto.Discount = couponDto.DiscountAmount;
					}
				}

				_responseDto.IsSuccess = true;
				_responseDto.Result = cardDto;
				
				return _responseDto;								
			}
			catch(Exception ex)
			{
				_responseDto.IsSuccess = false;
				_responseDto.Message = ex.Message;
			}

			return _responseDto;
		}

		public async Task<ResponseDto> RemoveCoupon(CartDto cartDto)
		{
			try
			{
				var cartHeader = await _cartContext.CartHeaders.FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeaderDto.UserId);
				if (cartHeader != null)
				{
					cartHeader.CouponCode = "";
					_cartContext.CartHeaders.Update(cartHeader);
					await _cartContext.SaveChangesAsync();
					_responseDto.IsSuccess = true;
				}
			}
			catch(Exception ex) 
			{
				_responseDto.IsSuccess = false;
				_responseDto.Result = ex.Message;
			}

			return _responseDto;
		}

		public async Task<ResponseDto> UpsertCart(CartDto cartDto)
		{
			try
			{
				//retrieve cart header
				var cartHeader = await _cartContext.CartHeaders.AsNoTracking()
												   .FirstOrDefaultAsync(ch => ch.UserId == cartDto.CartHeaderDto.UserId);
				if (cartHeader == null)
				{
					var cardHeader = _mapper.Map<CartHeader>(cartDto.CartHeaderDto);

					await _cartContext.CartHeaders.AddAsync(cardHeader);
					await _cartContext.SaveChangesAsync();

					var cartDetails = _mapper.Map<IEnumerable<CartDetails>>(cartDto.CartDetailsDto);
					foreach (var cartDetail in cartDetails)
					{
						cartDetail.Product = cartDto.CartDetailsDto.First().ProductDto;
						//cartDto.CartDetailsDto.First().ProductDto = await _productService.GetProduct(cartDto.CartDetailsDto.First().ProductId);
						cartDto.CartDetailsDto.First().CartHeaderId = cardHeader.CartHeaderId;
						cartDetail.ProductId = cartDetail.Product.ProductId;
						cartDetail.CartHeaderId = cardHeader.CartHeaderId;

						await _cartContext.CartDetails.AddAsync(cartDetail);
						await _cartContext.SaveChangesAsync();
                        cartDto.CartDetailsDto.First().CartDetailsId = cartDetail.CartDetailsId;
                    }
				}
				else
				{
					var existingUserCartDetails = await _cartContext.CartDetails.AsNoTracking()
														.FirstOrDefaultAsync(cd => cd.ProductId == cartDto.CartDetailsDto.First().ProductId &&
																			 cd.CartHeader.CartHeaderId == cartHeader.CartHeaderId);

					if (existingUserCartDetails == null)
					{
						cartDto.CartDetailsDto.First().CartHeaderId = cartHeader.CartHeaderId;
						var cartDetails = _mapper.Map<IEnumerable<CartDetails>>(cartDto.CartDetailsDto);
						await _cartContext.CartDetails.AddAsync(cartDetails.First());
						await _cartContext.SaveChangesAsync();

					}
					else
					{
						cartDto.CartDetailsDto.First().Count += existingUserCartDetails.Count;
						cartDto.CartDetailsDto.First().CartHeaderId = cartHeader.CartHeaderId;
					    cartDto.CartDetailsDto.First().CartDetailsId =	existingUserCartDetails.CartDetailsId;
						
						_cartContext.CartDetails.Update((_mapper.Map<IEnumerable<CartDetails>>(cartDto.CartDetailsDto)).First());
						await _cartContext.SaveChangesAsync();
					}
				}

				_responseDto.IsSuccess = true;
				_responseDto.Result = cartDto;
				return _responseDto;
			}
			catch(Exception ex)
			{
				_responseDto.IsSuccess=false;
				_responseDto.Message = ex.Message;
			}

			return _responseDto;
		}
	}
}
