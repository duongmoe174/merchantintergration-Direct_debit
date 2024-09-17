const Api = require("./Api");
const Config = require("./Config");

function main() {
  retrieveRegisteredTokenInformation();
}

function tokenRegistration() {
  const merchantId = Config.MERCHANT_ID;
  Api.tokenRegistration(merchantId);
}

function retrieveRegisteredTokenInformation() {
  const merchantId = Config.MERCHANT_ID;
  const merchantTokenRef = "DUONGTTTOKEN_1709604237625";
  Api.retrieveRegisteredTokenInformation(merchantId, merchantTokenRef);
}

function paymentWithRegisteredToken() {
  const merchantId = Config.MERCHANT_ID;
  const merchantToken = "TKN-kbGluJFlRqWfnGcRzcb1Fg";
  Api.paymentWithRegisteredToken(merchantId, merchantToken);
}

function retrievePaymentInformation() {
  const merchantId = Config.MERCHANT_ID;
  const merchantTxnRef = "PAYMENTDTT_1709605601825";
  Api.retrievePaymentInformation(merchantId, merchantTxnRef);
}

function deleteToken() {
  const merchantId = Config.MERCHANT_ID;
  const merchantToken = "TKN-kbGluJFlRqWfnGcRzcb1Fg";
  Api.deleteToken(merchantId, merchantToken);
}

function retrieveTokenDeletionInfo() {
  const merchantId = Config.MERCHANT_ID;
  const merchantToken = "TKN-kbGluJFlRqWfnGcRzcb1Fg";
  const merchantDelRef = "DELETION_1709606464466";
  Api.retrieveTokenDeletionInfo(merchantId, merchantToken, merchantDelRef);
}

main();
