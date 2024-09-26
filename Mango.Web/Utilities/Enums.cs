namespace Mango.Web.Utilities
{
    public class Enums
    {
        public static string CouponAPIBase { get; set; }
        public static string AuthAPIBase { get; set; }
        public static string ProductApi {  get; set; }
        public const string JwtCookie = "JwtCookie";

        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE,
        }
    }
}
