package com.example.directdebit;

public class Main {
  public static void main(String[] args) {
    // verifyIPN();
    tokenRegistration();
  }

  private static void tokenRegistration() {
    String merchantId = Config.MERCHANT_ID;
    Api.tokenRegistration(merchantId);
  }

  private static void retrieveRegisteredTokenInformation() {
    String merchantId = Config.MERCHANT_ID;
    String merchantTokenRef = "DUONGTTTOKEN_1728451454043";
    Api.retrieveRegisteredTokenInformation(merchantId, merchantTokenRef);
  }

  private static void paymentWithRegisteredToken() {
    String merchantId = Config.MERCHANT_ID;
    String merchantToken = "TKN-ignWFHpdQ9-UKO-URy_8fQ";
    Api.paymentWithRegisteredToken(merchantId, merchantToken);
  }

  private static void retrievePaymentInformation() {
    String merchantId = Config.MERCHANT_ID;
    String merchantTxnRef = "PAYMENTDDT_1707297010437";
    Api.retrievePaymentInformation(merchantId, merchantTxnRef);

  }

  private static void deleteToken() {
    String merchantId = Config.MERCHANT_ID;
    String merchantToken = "TKN-DdQSuQEYRtSkcYtkHJ1Tlg";
    Api.deleteToken(merchantId, merchantToken);
  }

  private static void retrieveTokenDeletionInfo() {
    String merchantId = Config.MERCHANT_ID;
    String merchantToken = "TKN-kbGluJFlRqWfnGcRzcb1Fg";
    String merchantDelRef = "DUONGTTTOKEN_1707986593103";
    Api.retrieveTokenDeletionInfo(merchantId, merchantToken, merchantDelRef);
  }

  private static void verifyIPN() {
    Api.verifyIPN();
  }
}
