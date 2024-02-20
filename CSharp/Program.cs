using System;
using AuthDD;
using AppApi;
using AppConfig;
public class Program
{
        static void Main(string[] agrs)
        {
                TokenRegistration();
        }
        private static void testSign()
        {
                string stringToSign = "\"@method\": PUT\n" +
                "\"@path\": /paygate/api/rest/v1/merchants/OPTEST/dd_tokens/DUONGTTTOKEN_1707196828629206900\n"
                +
                "\"content-digest\": sha-256=:YUd7b09NPdDjyrNveLs3KHbzxlzGRlU0+R69rpl7c9Y=:\n" +
                "\"content-type\": application/json\n" +
                "\"content-length\": 450\n" +
                "\"@signature-params\": (\"@method\" \"@path\" \"content-digest\" \"content-type\" \"content-length\");created=1707196828;expires=1707197128;keyid=\"OPTEST\";alg=\"ed25519\"";
                string signature = Auth.MakeSign(stringToSign);
                Console.WriteLine("Signature: " + signature);
        }

        private static void TokenRegistration()
        {
                string merchantId = Config.MERCHANT_ID;
                Api.TokenRegistration(merchantId);
        }
}

