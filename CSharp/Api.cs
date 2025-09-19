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
            browserPayment.Add("returnUrl", "https://mtf.onepay.vn/ldp/direct-debit/result");
            browserPayment.Add("callbackUrl", "https://webhook.site/1ccd5e66-8a50-4ced-87f5-b16e2b837220");

            JsonObject customer = new JsonObject();
            JsonObject customerAccount = new JsonObject();
            customerAccount.Add("id", "000002111");
            customer.Add("account", customerAccount);
            customer.Add("email", "duongtt@onepay.vn");
            customer.Add("name", "TRAN THAI DUONG");
            customer.Add("phone", "0367573933");

            JsonObject sourceOfFunds = new JsonObject();
            List<string> types = new List<string>();
            types.Add("DD_SGTTVNVX");
            types.Add("DD_BIDVVNVX");
            types.Add("DD_MSCBVNVX");
            types.Add("DD_MCOBVNVX");
            types.Add("DD_ICBVVNVX");
            JsonArray jArrayTypes = new JsonArray();
            foreach (string item in types)
            {
                jArrayTypes.Add(item);
            }
            sourceOfFunds.Add("types", jArrayTypes);

            JsonObject device = new JsonObject();
            device.Add("id", "DV213124124");
            device.Add("ipAddress", "192.168.1.999");
            device.Add("browser", "Chrome");
            device.Add("mobilePhoneModel", "nokia 1280");
            device.Add("fingerprint", "");

            bodyContent.Add("apiOperation", IConstants.TOKENIZE_DIRECT_DEBIT);
            bodyContent.Add("browserPayment", browserPayment);
            bodyContent.Add("customer", customer);
            bodyContent.Add("sourceOfFunds", sourceOfFunds);
            bodyContent.Add("device", device);
            bodyContent.Add("locale", "vi");
            /* ========== End create bodyContent ========== */

            string jsonContent = bodyContent.ToString();
            byte[] hashPayload = Util.Sha256Hash(jsonContent);
            string contentDigest = "sha-256=:" + Convert.ToBase64String(hashPayload) + ":";
            string contentType = IConstants.APPLICATION_JSON;
            string contentLength = jsonContent.Length.ToString();

            string stringToSign = Util.GenerateStringToSign(method, path, contentType, contentLength, contentDigest, createTime, expireTime);
            string signature = Auth.MakeSign(stringToSign);
            string signatureInput = Util.CreateSignatureInput(method, createTime, expireTime);

            Dictionary<string, string> headerRequest = Util.CreateHeaderRequest(signatureInput, signature, contentType, contentLength, contentDigest);

            string urlRequest = Config.BASE_URL + path;

            Console.WriteLine("urlRequest: " + urlRequest);
            Console.WriteLine("bodyContent: " + jsonContent);
            Console.WriteLine("HeaderRequest: " + headerRequest.ToString());
            foreach (var entry in headerRequest)
            {
                Console.WriteLine($"Key: {entry.Key}, Value: {entry.Value}");
            }
            Console.WriteLine("StringToSign: " + stringToSign);
            Console.WriteLine("signatureInput: " + signatureInput);
            Console.WriteLine("signature: " + signature);

            _ = Util.HttpPutRequest(urlRequest, jsonContent, headerRequest);
        }

        public static void RetrieveRegisteredTokenInformation(string merchantId, string merchantTokenRef)
        {
            string method = IConstants.GET;
            string path = Config.URL_PREFIX + IConstants.MERCHANTS + "/" + merchantId + "/" + IConstants.DD_TOKEN + "/" + merchantTokenRef;

            long unixCurrentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long fiveMinutesLater = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds();

            string createTime = unixCurrentTime.ToString();
            string expireTime = fiveMinutesLater.ToString();

            string stringToSign = Util.GenerateStringToSign(method, path, createTime, expireTime);
            string signature = Auth.MakeSign(stringToSign);
            string signatureInput = Util.CreateSignatureInput(method, createTime, expireTime);

            string urlRequest = Config.BASE_URL + path;

            Dictionary<string, string> headerRequest = Util.CreateHeaderRequest(signatureInput, signature);

            Console.WriteLine("urlRequest: " + urlRequest);
            Console.WriteLine("HeaderRequest: " + headerRequest.ToString());
            foreach (var entry in headerRequest)
            {
                Console.WriteLine($"Key: {entry.Key}, Value: {entry.Value}");
            }
            Console.WriteLine("StringToSign: " + stringToSign);
            Console.WriteLine("signatureInput: " + signatureInput);
            Console.WriteLine("signature: " + signature);
            _ = Util.HttpGetRequest(urlRequest, headerRequest);

        }

        public static void PaymentWithRegisteredToken(string merchantId, string merchantToken)
        {
            string method = IConstants.POST;

            string reference = "PAYMENTDDT_" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            string path = Config.URL_PREFIX + IConstants.MERCHANTS + "/" + merchantId + "/" + IConstants.PAYMENTS_DDT + "/" + reference;

            long unixCurrentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long fiveMinutesLater = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeMilliseconds();

            string createTime = unixCurrentTime.ToString();
            string expireTime = fiveMinutesLater.ToString();

            /* ========== START CREATE BODY CONTENT ========== */
            JsonObject bodyContent = new JsonObject();

            JsonObject invoice = new JsonObject();
            invoice.Add("amount", 1000000);
            invoice.Add("currency", "VND");
            invoice.Add("description", "DUONGTT THANH TOAN");

            JsonObject customer = new JsonObject();
            JsonObject customerAccount = new JsonObject();
            customerAccount.Add("id", "000000001");
            customer.Add("account", customerAccount);
            customer.Add("email", "duongtt@onepay.vn");
            customer.Add("name", "TRAN THAI DUONG");
            customer.Add("phone", "0367573933");

            JsonObject transaction = new JsonObject();
            JsonObject sourceOfFunds = new JsonObject();
            sourceOfFunds.Add("type", IConstants.DIRECT_DEBIT_TOKEN);
            sourceOfFunds.Add("token", merchantToken);
            transaction.Add("sourceOfFunds", sourceOfFunds);
            transaction.Add("source", "MIT");

            JsonObject device = new JsonObject();
            device.Add("id", "DV213124124");
            device.Add("ipAddress", "127.0.01");
            device.Add("browser", "IE");
            device.Add("mobilePhoneModel", "NOKIA 1280");
            device.Add("fingerprint", "");

            bodyContent.Add("apiOperation", IConstants.PURCHASE_DIRECT_DEBIT);
            bodyContent.Add("invoice", invoice);
            bodyContent.Add("customer", customer);
            bodyContent.Add("transaction", transaction);
            bodyContent.Add("device", device);
            /* ========== End create bodyContent ========== */

            string jsonContent = bodyContent.ToString();

            byte[] hashPayload = Util.Sha256Hash(jsonContent);
            string contentDigest = "sha-256=:" + Convert.ToBase64String(hashPayload) + ":";
            string contentType = IConstants.APPLICATION_JSON;
            string contentLength = jsonContent.Length.ToString();

            string stringToSign = Util.GenerateStringToSign(method, path, contentType, contentLength, contentDigest, createTime, expireTime);

            string signature = Auth.MakeSign(stringToSign);

            string signatureInput = Util.CreateSignatureInput(method, createTime, expireTime);

            Dictionary<string, string> headerRequest = Util.CreateHeaderRequest(signatureInput, signature, contentType, contentLength, contentDigest);

            string urlRequest = Config.BASE_URL + path;

            Console.WriteLine("urlRequest: " + urlRequest);
            Console.WriteLine("bodyContent: " + jsonContent);
            Console.WriteLine("HeaderRequest: " + headerRequest.ToString());
            foreach (var entry in headerRequest)
            {
                Console.WriteLine($"Key: {entry.Key}, Value: {entry.Value}");
            }
            Console.WriteLine("StringToSign: " + stringToSign);
            Console.WriteLine("signatureInput: " + signatureInput);
            Console.WriteLine("signature: " + signature);

            _ = Util.HttpPostRequest(urlRequest, jsonContent, headerRequest);

        }

        public static void RetrievePaymentInformation(string merchantId, string merchantTxnRef)
        {
            string method = IConstants.GET;

            string path = Config.URL_PREFIX + IConstants.MERCHANTS + "/" + merchantId + "/" + IConstants.PAYMENTS_DDT + "/" + merchantTxnRef;

            long unixCurrentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long fiveMinutesLater = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeMilliseconds();

            string createTime = unixCurrentTime.ToString();
            string expireTime = fiveMinutesLater.ToString();

            string stringToSign = Util.GenerateStringToSign(method, path, createTime, expireTime);

            string signature = Auth.MakeSign(stringToSign);

            string signatureInput = Util.CreateSignatureInput(method, createTime, expireTime);

            string urlRequest = Config.BASE_URL + path;

            Dictionary<string, string> headerRequest = Util.CreateHeaderRequest(signatureInput, signature);

            Console.WriteLine("urlRequest: " + urlRequest);
            Console.WriteLine("HeaderRequest: " + headerRequest.ToString());
            foreach (var entry in headerRequest)
            {
                Console.WriteLine($"Key: {entry.Key}, Value: {entry.Value}");
            }
            Console.WriteLine("StringToSign: " + stringToSign);
            Console.WriteLine("signatureInput: " + signatureInput);
            Console.WriteLine("signature: " + signature);

            _ = Util.HttpGetRequest(urlRequest, headerRequest);

        }

        public static void DeleteToken(string merchantId, string merchantToken)
        {
            string method = IConstants.PUT;
            string reference = "DELETETOKEN_" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            string path = Config.URL_PREFIX + IConstants.MERCHANTS + "/" + merchantId + "/" + IConstants.DD_TOKEN + "/" + merchantToken + "/" + IConstants.DELETIONS + "/" + reference;

            long unixCurrentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long fiveMinutesLater = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeMilliseconds();

            string createTime = unixCurrentTime.ToString();
            string expireTime = fiveMinutesLater.ToString();

            /* ========== START CREATE BODY CONTENT ========== */
            JsonObject bodyContent = new JsonObject();

            JsonObject browserPayment = new JsonObject();
            browserPayment.Add("returnUrl", "https://mtf.onepay.vn/ldp/direct-debit/result");
            browserPayment.Add("callbackUrl", "https://mtf.onepay.vn/paygate/api/rest/v1/ipn");

            JsonObject customer = new JsonObject();
            JsonObject customerAccount = new JsonObject();
            customerAccount.Add("id", "000002111");
            customer.Add("account", customerAccount);
            customer.Add("email", "duongtt@onepay.vn");
            customer.Add("name", "TRAN THAI DUONG");
            customer.Add("phone", "0367573933");

            JsonObject device = new JsonObject();
            device.Add("ipAddress", "192.168.166.146");
            device.Add("browser", "Chrome/120.0.0.0");
            device.Add("mobilePhoneModel", "nokia 1280");

            bodyContent.Add("apiOperation", IConstants.DELETE_TOKEN_DIRECT_DEBIT);
            bodyContent.Add("browserPayment", browserPayment);
            bodyContent.Add("customer", customer);
            bodyContent.Add("device", device);
            bodyContent.Add("locale", "vi");
            /* ========== END CREATE BODY CONTENT ========== */

            string jsonContent = bodyContent.ToString();

            byte[] hashPayload = Util.Sha256Hash(jsonContent);
            string contentDigest = "sha-256=:" + Convert.ToBase64String(hashPayload) + ":";
            string contentType = IConstants.APPLICATION_JSON;
            string contentLength = jsonContent.Length.ToString();

            string stringToSign = Util.GenerateStringToSign(method, path, contentType, contentLength, contentDigest, createTime, expireTime);

            string signature = Auth.MakeSign(stringToSign);

            string signatureInput = Util.CreateSignatureInput(method, createTime, expireTime);

            Dictionary<string, string> headerRequest = Util.CreateHeaderRequest(signatureInput, signature, contentType, contentLength, contentDigest);

            string urlRequest = Config.BASE_URL + path;

            Console.WriteLine("urlRequest: " + urlRequest);
            Console.WriteLine("bodyContent: " + jsonContent);
            Console.WriteLine("HeaderRequest: " + headerRequest.ToString());
            foreach (var entry in headerRequest)
            {
                Console.WriteLine($"Key: {entry.Key}, Value: {entry.Value}");
            }
            Console.WriteLine("StringToSign: " + stringToSign);
            Console.WriteLine("signatureInput: " + signatureInput);
            Console.WriteLine("signature: " + signature);

            _ = Util.HttpPutRequest(urlRequest, jsonContent, headerRequest);
        }

        public static void RetrieveTokenDeletionInfo(string merchantId, string merchantToken, string merchantDelRef)
        {
            string method = IConstants.GET;

            string path = Config.URL_PREFIX + IConstants.MERCHANTS + "/" + merchantId + "/" + IConstants.DD_TOKEN + "/" + merchantToken + "/" + IConstants.DELETIONS + "/" + merchantDelRef;

            long unixCurrentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long fiveMinutesLater = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeMilliseconds();

            string createTime = unixCurrentTime.ToString();
            string expireTime = fiveMinutesLater.ToString();

            string stringToSign = Util.GenerateStringToSign(method, path, createTime, expireTime);

            string signature = Auth.MakeSign(stringToSign);

            string signatureInput = Util.CreateSignatureInput(method, createTime, expireTime);

            string urlRequest = Config.BASE_URL + path;

            Dictionary<string, string> headerRequest = Util.CreateHeaderRequest(signatureInput, signature);

            Console.WriteLine("urlRequest: " + urlRequest);
            Console.WriteLine("HeaderRequest: " + headerRequest.ToString());
            foreach (var entry in headerRequest)
            {
                Console.WriteLine($"Key: {entry.Key}, Value: {entry.Value}");
            }
            Console.WriteLine("StringToSign: " + stringToSign);
            Console.WriteLine("signatureInput: " + signatureInput);
            Console.WriteLine("signature: " + signature);

            _ = Util.HttpGetRequest(urlRequest, headerRequest);
        }

        public static void VerifyIPN()
        {
            try
            {
                string signatureInputReq = "sig=(\"content-type\" \"content-length\" \"content-digest\");created=1726545759;expires=1726546059;keyid=\"TESTONEPAY50\";alg=\"ed25519\"";
                string contentDigestReq = "sha-256=:4QPXwxUM1GGvO5VIOPlXOAM1N+7eztDIOIkvQuh5QH8=:";
                string signatureReq = "sig=:vn8XYqappH1/SFmm4hFyzUBBbMSvcgPZf1uHQXODOh8+GDUi4ntzLkSnzi8KCaZfTEtmM3P1kNpRGUX0YDKsAQ==:";
                string contentReq = "{\"merchantId\":\"TESTONEPAY50\",\"merchTokenRef\":\"DUONGTTTOKEN_1726545744078\",\"token\":\"TKN-4-L8Wbv7SsWK-L1Orf2Q2w\",\"state\":\"approved\",\"customer\":{\"account\":{\"id\":\"000000001\"},\"email\":\"duongtt@onepay.vn\",\"name\":\"TRAN THAI DUONG\",\"phone\":\"0367573933\"},\"sourceOfFunds\":{\"type\":\"DD_BIDVVNVX\",\"provided\":{\"type\":\"account\",\"accountNumber\":\"xxx1241\",\"accountHolder\":\"TRAN THAI DUONG\"}}}";

                string contentType = IConstants.APPLICATION_JSON;
                string contentLength = contentReq.Length.ToString();

                //Check content-digest
                byte[] hashPayload = Util.Sha256Hash(contentReq);
                string contentDigestVerify = "sha-256=:" + Convert.ToBase64String(hashPayload) + ":";
                if (!contentDigestReq.Equals(contentDigestVerify))
                {
                    throw new Exception("Content-Digest is invalid");
                }
                else
                {
                    //check Signature
                    Console.WriteLine("Content-Digest is Valid");
                    Console.WriteLine("======Start verify signature======");
                    string stringToVerify = Util.GenerateStringToVerify(contentType, contentLength, contentDigestReq, signatureInputReq);
                    string trueSignature = Util.GetSignatureReq(signatureReq);
                    bool verifySignature = Auth.VerifySign(stringToVerify, trueSignature);
                    Console.WriteLine("SignatureReq: " + trueSignature);
                    Console.WriteLine("String To Verify: " + stringToVerify);
                    Console.WriteLine("Verify Signature: " + verifySignature);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
