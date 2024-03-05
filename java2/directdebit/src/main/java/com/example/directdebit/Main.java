package com.example.directdebit;

public class Main {
  public static void main(String[] args) {
    paymentWithRegisteredToken();
  }

  private static void tokenRegistration() {
    String merchantId = Config.MERCHANT_ID;
    Api.tokenRegistration(merchantId);
  }

  private static void retrieveRegisteredTokenInformation() {
    String merchantId = Config.MERCHANT_ID;
    String merchantTokenRef = "DUONGTTTOKEN_1708486760333";
    Api.retrieveRegisteredTokenInformation(merchantId, merchantTokenRef);
  }

  private static void paymentWithRegisteredToken() {
    String merchantId = Config.MERCHANT_ID;
    String merchantToken = "TKN-kbGluJFlRqWfnGcRzcb1Fg";
    Api.paymentWithRegisteredToken(merchantId, merchantToken);
  }

  private static void retrievePaymentInformation() {
    String merchantId = Config.MERCHANT_ID;
    String merchantTxnRef = "PAYMENTDDT_1707297010437";
    Api.retrievePaymentInformation(merchantId, merchantTxnRef);

  }

  private static void deleteToken() {
    String merchantId = Config.MERCHANT_ID;
    String merchantToken = "TKN-kbGluJFlRqWfnGcRzcb1Fg";
    Api.deleteToken(merchantId, merchantToken);
  }

  private static void retrieveTokenDeletionInfo() {
    String merchantId = Config.MERCHANT_ID;
    String merchantToken = "TKN-kbGluJFlRqWfnGcRzcb1Fg";
    String merchantDelRef = "DUONGTTTOKEN_1707986593103";
    Api.retrieveTokenDeletionInfo(merchantId, merchantToken, merchantDelRef);
  }
}
