using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using static Mango.Web.Utilities.Enums;

namespace Mango.Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BaseService(IHttpClientFactory httpClientFactory) 
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<ResponseDto> SendAsync(RequestDto requestDto)
        {
            
            try
            {
                HttpRequestMessage message = new();

                var client = _httpClientFactory.CreateClient();
                
                message.Headers.Add("Accept", "application/json");
                message.Headers.Add("Authorization", $"Bearer {requestDto.AccessToken}");
                message.RequestUri = new Uri(requestDto.Url);
                
                if (requestDto.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data),
                                                        Encoding.UTF8, "application/json");                    
                }

                HttpResponseMessage responseMessage = new();

                message.Method = requestDto.ApiType switch
                {
                    ApiType.POST => HttpMethod.Post,
                    ApiType.PUT => HttpMethod.Put,
                    ApiType.DELETE => HttpMethod.Delete,
                    _ => HttpMethod.Get,
                };
                                                
                responseMessage = await client.SendAsync(message);
                var responseDto = new ResponseDto();
                
                if (responseMessage != null)
                {
                    switch (responseMessage.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                            responseDto.IsSuccess = false;
                            responseDto.Message = "Not Found"; break;

                        case HttpStatusCode.Unauthorized:
                            responseDto.IsSuccess = false;
                            responseDto.Message = "Access Denied"; break;

                        case HttpStatusCode.Forbidden:
                            responseDto.IsSuccess = false;
                            responseDto.Message = "Unauthorized"; break;


                        case HttpStatusCode.InternalServerError:
                            responseDto.IsSuccess = false;
                            responseDto.Message = "Internal Server Error"; break;

                        case HttpStatusCode.BadRequest:
                            responseDto.IsSuccess = false;
							var content = await responseMessage.Content.ReadAsStringAsync();
							responseDto.Message = $"Bad Request {JsonConvert.DeserializeObject<DeserializeHandlerForProduct>(content).Result}"; break;		

                        default:
                            var apiContent = await responseMessage.Content.ReadAsStringAsync();
                            responseDto.Result = JsonConvert.DeserializeObject<object>(apiContent); break;
                            
                    }
                    return responseDto;
                }
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = ex.Message.ToString(),
                };
            }
            
            return new ResponseDto { };
        }
    }
}
