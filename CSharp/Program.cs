using System;
using AuthDD;
using AppApi;
using AppConfig;
public class Program
{
        static void Main(string[] agrs)
        {
                RetrieveTokenDeletionInfo();
        }
        private static void TokenRegistration()
        {
                string merchantId = Config.MERCHANT_ID;
                Api.TokenRegistration(merchantId);
        }

        private static void RetrieveRegisteredTokenInformation()
        {
                string merchantId = Config.MERCHANT_ID;
                string merchantTokenRef = "DUONGTTTOKEN_1708486760333";
                Api.RetrieveRegisteredTokenInformation(merchantId, merchantTokenRef);
        }

        private static void PaymentWithRegisteredToken()
        {
                string merchantId = Config.MERCHANT_ID;
                string merchantToken = "TKN-bTpVL1w5SYi21LyNsItkwA";
                Api.PaymentWithRegisteredToken(merchantId, merchantToken);
        }

        private static void RetrievePaymentInformation()
        {
                string merchantId = Config.MERCHANT_ID;
                string merchantTxnRef = "PAYMENTDDT_1708492392353";
                Api.RetrievePaymentInformation(merchantId, merchantTxnRef);
        }

        private static void DeleteToken()
        {
                string merchantId = Config.MERCHANT_ID;
                string merchantToken = "TKN-bTpVL1w5SYi21LyNsItkwA";
                Api.DeleteToken(merchantId, merchantToken);
        }

        private static void RetrieveTokenDeletionInfo()
        {
                string merchantId = Config.MERCHANT_ID;
                string merchantToken = "TKN-bTpVL1w5SYi21LyNsItkwA";
                string merchantDelRef = "DELETETOKEN_1708501951420";
                Api.RetrieveTokenDeletionInfo(merchantId, merchantToken, merchantDelRef);
        }
}

