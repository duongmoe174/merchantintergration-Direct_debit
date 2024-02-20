namespace AppConfig
{
    public interface Config
    {
        public static readonly string MERCHANT_ID = "OPTEST";
        public static readonly string BASE_URL = "https://dev.onepay.vn";
        public static readonly string URL_PREFIX = "/paygate/api/rest/v1/";
        public static readonly string ALG = "ed25519";
    }
}