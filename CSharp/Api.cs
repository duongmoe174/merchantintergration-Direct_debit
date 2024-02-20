using System.Data;
using System.Text.Json.Nodes;
using AppConfig;
using AppParam;
using AppUtil;
using AuthDD;

namespace AppApi
{
    public class Api
    {
        public static void TokenRegistration(string merchantId)
        {
            string method = IConstants.PUT;
            string reference = "DUONGTTTOKEN_" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            string path = Config.URL_PREFIX + IConstants.MERCHANTS + "/" + merchantId + "/" + IConstants.DD_TOKEN + "/" + reference;

            long unixCurrentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long fiveMinutesLater = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds();

            string createTime = unixCurrentTime.ToString();
            string expireTime = fiveMinutesLater.ToString();

            /* ========== Start create bodyContent ========== */
            JsonObject bodyContent = new JsonObject();

            JsonObject browserPayment = new JsonObject();
            browserPayment.Add("returnUrl", "https://dev.onepay.vn/ldp/direct-debit/result");
            browserPayment.Add("callbackUrl", "https://dev.onepay.vn/paygate/api/rest/v1/ipn");

            JsonObject customer = new JsonObject();
            JsonObject customerAccount = new JsonObject();
            customerAccount.Add("id", "000000001");
            customer.Add("account", customerAccount);
            customer.Add("email", "duongtt@onepay.vn");
            customer.Add("name", "TRAN THAI DUONG");
            customer.Add("phone", "0367573933");

            JsonObject sourceOfFunds = new JsonObject();
            List<string> types = new List<string>();
            types.Add("DD_SGTTVNVX");
            types.Add("DD_BIDVVNVX");
            JsonArray jArrayTypes = new JsonArray();
            foreach (string item in types)
            {
                jArrayTypes.Add(item);
            }
            sourceOfFunds.Add("types", jArrayTypes);

            JsonObject device = new JsonObject();
            device.Add("ipAddress", "192.168.1.999");
            device.Add("browser", "Chrome");
            device.Add("mobilePhoneModel", "nokia 1280");

            bodyContent.Add("apiOperation", IConstants.TOKENIZE_DIRECT_DEBIT);
            bodyContent.Add("browserPayment", browserPayment);
            bodyContent.Add("customer", customer);
            bodyContent.Add("sourceOfFunds", sourceOfFunds);
            bodyContent.Add("device", device);
            bodyContent.Add("locale", "vi");
            /* ========== End create bodyContent ========== */

            string jsonContent = bodyContent.ToString();
            byte[] hashPayload = Util.Sha256Hash(jsonContent);
            string contentDigest = "sha-256=:" + Convert.ToBase64String(hashPayload);
            string contentType = IConstants.APPLICATION_JSON;
            string contentLength = jsonContent.Length.ToString();

            string stringToSign = Util.GenerateStringToSign(method, path, contentType, contentLength, contentDigest, createTime, expireTime);
            string signature = Auth.MakeSign(stringToSign);
            string signatureInput = Util.CreateSignatureInput(method, createTime, expireTime);

            Dictionary<string, string> headerRequest = Util.CreateHeaderRequest(signatureInput, signature, contentType, contentLength, contentDigest);

            string urlRequest = Config.BASE_URL + path;

            Console.WriteLine("urlRequest: " + urlRequest);
            Console.WriteLine("bodyContent: " + jsonContent);
            Console.WriteLine("HeaderRequest: " + headerRequest);
            Console.WriteLine("StringToSign: " + stringToSign);
            Console.WriteLine("signatureInput: " + signatureInput);
            Console.WriteLine("signature: " + signature);

            _ = Util.HttpPutRequest(urlRequest, jsonContent, headerRequest);

        }
    }
}