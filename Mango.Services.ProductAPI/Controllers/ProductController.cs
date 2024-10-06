using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }


        [HttpGet]
        [Route("Products")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetProducts()
        {
            var responseDto = await _productService.GetProducts();
            
            if (responseDto.IsSuccess)
            {
                return Ok(responseDto);
            }

            return BadRequest(responseDto);
        }

        [HttpGet]
        [Route("GetProduct/{productId:int}")]
        [Authorize(Roles ="ADMIN")]
        public async Task<IActionResult> GetProduct(int productId)
        {
            var responseDto = await _productService.GetProduct(productId);
            
            if(responseDto.IsSuccess) 
            {
                return Ok(responseDto);
            }
            
            responseDto.Message = "The product is not available";

            return NotFound(responseDto);
        }
        
        [HttpPost]
        [Route("Create")]
        [Authorize(Roles="SUPERADMIN")]
        public async Task<IActionResult> CreateNewProduct([FromBody]ProductDto productDto) 
        {
            var responseDto = await _productService.CreateNewProduct(productDto);

            if (responseDto.IsSuccess) 
            {
                return Ok(responseDto);
            }

            return BadRequest(responseDto);
        }

        [HttpPost]
        [Route("Update/Product")]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> UpdateProduct([FromBody]ProductDto productDto)
        {

            var responseDto = await _productService.UpdateProduct(productDto);

            if(responseDto.IsSuccess) 
            {
                return Ok(responseDto); 
            }

            return BadRequest(responseDto);
        }

        [HttpDelete]
        [Route("Delete/Product/{productId:int}")]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var response = await _productService.DeleteProduct(productId);
            
            var responseDto = new ResponseDto();
            
            if (response)
            {
                responseDto.IsSuccess = true;
                return Ok(responseDto);
            }

            responseDto.Message = "Some error encountered.";

            return BadRequest(responseDto);
        }
    }
}
