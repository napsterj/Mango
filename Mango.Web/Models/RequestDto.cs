using Mango.Web.Utilities;
using static Mango.Web.Utilities.Enums;

namespace Mango.Web.Models
{
    public class RequestDto
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; } = "";

        //Using object because data can be of any type i.e string, int, object, collections etc 
        public object Data { get; set; } 
        public string AccessToken { get; set; }
    }
}
