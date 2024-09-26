using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductDbContext _prodContext;
        private readonly IMapper _mapperConfig;
        ResponseDto _responseDto;

        public ProductService(ProductDbContext prodContext, IMapper mapper)
        {
            _prodContext = prodContext;
            _mapperConfig = mapper;
            _responseDto = new ResponseDto();
        }
         
        public async Task<ResponseDto> CreateNewProduct(ProductDto productDto)
        {
            try
            {
                var product = await _prodContext.Products.FirstOrDefaultAsync(p => p.ProductId == productDto.ProductId);
                if (product == null)
                {
                    var newProduct = _mapperConfig.Map<Product>(productDto);
                    newProduct.ProductPrice = productDto.ProductPrice;
                    
                    await _prodContext.AddAsync(newProduct);
                    await _prodContext.SaveChangesAsync();
                    
                    _responseDto.IsSuccess = true;
                    _responseDto.Result = productDto;
                    
                    return _responseDto;
                }

                _responseDto.Message = "Product your trying to add already exists";
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);                
            }
            return _responseDto;
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            try
            {
                var prodToDelete = await _prodContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
                
                if (prodToDelete != null)
                {
                    _prodContext.Products.Remove(prodToDelete);
                    return true;
                }
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        public async Task<ResponseDto> GetProduct(int productId)
        {
            try
            {
                var product = await _prodContext.Products.FirstOrDefaultAsync(a=>a.ProductId == productId);
                if(product != null)
                {
                    _responseDto.IsSuccess = true;
                    _responseDto.Result = product;
                    return _responseDto;
                }

                _responseDto.Message = "The product does not exists at the moment.";
            }
            catch(Exception ex )
            {
                Console.WriteLine(ex.Message);
            }

            return _responseDto;
        }

        public async Task<ResponseDto> GetProducts()
        {
            try
            {
                var products = await _prodContext.Products.ToListAsync();
                if(products != null && products.Count > 0)
                {
                    _responseDto.IsSuccess= true;
                    _responseDto.Result = products;
                    return _responseDto;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return _responseDto;
        }

        public async Task<ResponseDto> UpdateProduct(ProductDto productDto)
        {
            try
            {
                var productToUpdate = await _prodContext.Products.FirstOrDefaultAsync(p => p.ProductId == productDto.ProductId);
            
                if (productToUpdate != null)
                {
                    productToUpdate = _mapperConfig.Map<Product>(productDto);

                    _prodContext.Products.Update(productToUpdate);

                    _prodContext.SaveChanges();

                    _responseDto.IsSuccess = true;
                
                    _responseDto.Result = _mapperConfig.Map<ProductDto>(productToUpdate);
                    
                    return _responseDto;
                }

                _responseDto.Message = "Some error has occured";
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);                
            }

            return _responseDto;
        }
    }
}
