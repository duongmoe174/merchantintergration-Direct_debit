const util = require("./Util");
const Config = require("./Config");
const IConstants = require("./IContants");
const Auth = require("./Auth");

function tokenRegistration(merchantId) {
  let method = IConstants.PUT;
  // Lấy thời gian hiện tại
  let currentTime = new Date();
  let fiveMinutesLater = new Date(currentTime.getTime());
  fiveMinutesLater.setMinutes(fiveMinutesLater.getMinutes() + 5);

  // Chuyển đổi thời gian hiện tại thành Unix timestamp (milliseconds since the Unix Epoch)
  let createtime = currentTime.getTime();
  let expireTime = fiveMinutesLater.getTime();
  let reference = "DUONGTTTOKEN_" + createtime;

  let path =
    Config.URL_PREFIX +
    IConstants.MERCHANTS +
    "/" +
    merchantId +
    "/" +
    IConstants.DD_TOKENS +
    "/" +
    reference;

  let body_content = {
    browserPayment: {
      callbackUrl: "https://mtf.onepay.vn/paygate/api/rest/v1/ipn",
      returnUrl: "https://mtf.onepay.vn/ldp/direct-debit/result",
    },
    apiOperation: IConstants.TOKENIZE_DIRECT_DEBIT,
    sourceOfFunds: { types: ["DD_SGTTVNVX", "DD_BIDVVNVX"] },
    locale: "vi",
    device: {
      browser: "User Agent",
      mobilePhoneModel: "iphone",
      ipAddress: "192.168.1.999",
    },
    customer: {
      phone: "0367573933",
      name: "TRAN THAI DUONG",
      account: { id: "000000001" },
      email: "duongtt@onepay.vn",
    },
  };
  let jsonContent = JSON.stringify(body_content);
  let textEncoder = new TextEncoder();
  let byteArray = textEncoder.encode(jsonContent);
  let hashPayload = util.sha256Hash(byteArray);
  let contentDigest =
    "sha-256=:" + Buffer.from(hashPayload).toString("base64") + ":";
  let contentType = IConstants.APPLICATION_JSON;
  let contentLength = jsonContent.length + "";
  let stringToSign = util.generateStringToSign(
    method,
    path,
    contentType,
    contentLength,
    contentDigest,
    createtime,
    expireTime
  );
  let signature = Auth.makeSign(stringToSign);
  let signatureInput = util.createSignatureInput(
    method,
    createtime,
    expireTime
  );

  let headerRequest = util.createHeaderRequest(
    signatureInput,
    signature,
    contentType,
    contentLength,
    contentDigest
  );

  let urlRequest = Config.BASE_URL + path;

  console.log("urlRequest: " + urlRequest);
  console.log("bodyContent: " + jsonContent);
  console.log("HeaderRequest: " + headerRequest);
  console.log("StringToSign: " + stringToSign);
  console.log("signatureInput: " + signatureInput);
  console.log("signature: " + signature);

  util.makePutRequest(urlRequest, jsonContent, headerRequest);
}

function retrieveRegisteredTokenInformation(merchantId, merchantTokenRef) {
  let method = IConstants.GET;
  // Lấy thời gian hiện tại
  let currentTime = new Date();
  let fiveMinutesLater = new Date(currentTime.getTime());
  fiveMinutesLater.setMinutes(fiveMinutesLater.getMinutes() + 5);

  // Chuyển đổi thời gian hiện tại thành Unix timestamp (milliseconds since the Unix Epoch)
  let createtime = currentTime.getTime();
  let expireTime = fiveMinutesLater.getTime();

  let path =
    Config.URL_PREFIX +
    IConstants.MERCHANTS +
    "/" +
    merchantId +
    "/" +
    IConstants.DD_TOKENS +
    "/" +
    merchantTokenRef;

  let stringToSign = util.generateStringToSign(
    method,
    path,
    "",
    "",
    "",
    createtime,
    expireTime
  );

  let signature = Auth.makeSign(stringToSign);

  let signatureInput = util.createSignatureInput(
    method,
    createtime,
    expireTime
  );

  let urlRequest = Config.BASE_URL + path;

  let headerRequest = util.createHeaderRequest(
    signatureInput,
    signature,
    "",
    "",
    ""
  );

  console.log("urlRequest: " + urlRequest);
  console.log("HeaderRequest: " + headerRequest);
  console.log("StringToSign: " + stringToSign);
  console.log("signatureInput: " + signatureInput);
  console.log("signature: " + signature);

  util.makeGetRequest(urlRequest, headerRequest);
}

function paymentWithRegisteredToken(merchantId, merchantToken) {
  let method = IConstants.POST;
  // Lấy thời gian hiện tại
  let currentTime = new Date();
  let fiveMinutesLater = new Date(currentTime.getTime());
  fiveMinutesLater.setMinutes(fiveMinutesLater.getMinutes() + 5);

  // Chuyển đổi thời gian hiện tại thành Unix timestamp (milliseconds since the Unix Epoch)
  let createtime = currentTime.getTime();
  let expireTime = fiveMinutesLater.getTime();
  let reference = "PAYMENTDTT_" + createtime;

  let path =
    Config.URL_PREFIX +
    IConstants.MERCHANTS +
    "/" +
    merchantId +
    "/" +
    IConstants.PAYMENTS_DDT +
    "/" +
    reference;

  let body_content = {
    apiOperation: IConstants.PURCHASE_DIRECT_DEBIT,
    invoice: {
      amount: 10000000,
      description: "DUONGTT THANH TOAN",
      currency: "VND",
    },
    device: {
      browser: "IE",
      mobilePhoneModel: "NOKIA 1280",
      ipAddress: "127.0.01",
    },
    transaction: {
      sourceOfFunds: {
        type: IConstants.DIRECT_DEBIT_TOKEN,
        token: merchantToken,
      },
    },
    customer: {
      phone: "0367573933",
      name: "TRAN THAI DUONG",
      account: { id: "000000001" },
      email: "duongtt@onepay.vn",
    },
  };

  let jsonContent = JSON.stringify(body_content);
  let textEncoder = new TextEncoder();
  let byteArray = textEncoder.encode(jsonContent);
  let hashPayload = util.sha256Hash(byteArray);
  let contentDigest =
    "sha-256=:" + Buffer.from(hashPayload).toString("base64") + ":";
  let contentType = IConstants.APPLICATION_JSON;
  let contentLength = jsonContent.length + "";
  let stringToSign = util.generateStringToSign(
    method,
    path,
    contentType,
    contentLength,
    contentDigest,
    createtime,
    expireTime
  );
  let signature = Auth.makeSign(stringToSign);
  let signatureInput = util.createSignatureInput(
    method,
    createtime,
    expireTime
  );

  let headerRequest = util.createHeaderRequest(
    signatureInput,
    signature,
    contentType,
    contentLength,
    contentDigest
  );

  let urlRequest = Config.BASE_URL + path;

  console.log("urlRequest: " + urlRequest);
  console.log("bodyContent: " + jsonContent);
  console.log("HeaderRequest: " + headerRequest);
  console.log("StringToSign: " + stringToSign);
  console.log("signatureInput: " + signatureInput);
  console.log("signature: " + signature);

  util.makePostRequest(urlRequest, jsonContent, headerRequest);
}

function retrievePaymentInformation(merchantId, merchantTxnRef) {
  let method = IConstants.GET;
  // Lấy thời gian hiện tại
  let currentTime = new Date();
  let fiveMinutesLater = new Date(currentTime.getTime());
  fiveMinutesLater.setMinutes(fiveMinutesLater.getMinutes() + 5);

  // Chuyển đổi thời gian hiện tại thành Unix timestamp (milliseconds since the Unix Epoch)
  let createtime = currentTime.getTime();
  let expireTime = fiveMinutesLater.getTime();

  let path =
    Config.URL_PREFIX +
    IConstants.MERCHANTS +
    "/" +
    merchantId +
    "/" +
    IConstants.PAYMENTS_DDT +
    "/" +
    merchantTxnRef;

  let stringToSign = util.generateStringToSign(
    method,
    path,
    "",
    "",
    "",
    createtime,
    expireTime
  );

  let signature = Auth.makeSign(stringToSign);

  let signatureInput = util.createSignatureInput(
    method,
    createtime,
    expireTime
  );

  let urlRequest = Config.BASE_URL + path;

  let headerRequest = util.createHeaderRequest(
    signatureInput,
    signature,
    "",
    "",
    ""
  );

  console.log("urlRequest: " + urlRequest);
  console.log("HeaderRequest: " + headerRequest);
  console.log("StringToSign: " + stringToSign);
  console.log("signatureInput: " + signatureInput);
  console.log("signature: " + signature);

  util.makeGetRequest(urlRequest, headerRequest);
}

function deleteToken(merchantId, merchantToken) {
  let method = IConstants.PUT;
  // Lấy thời gian hiện tại
  let currentTime = new Date();
  let fiveMinutesLater = new Date(currentTime.getTime());
  fiveMinutesLater.setMinutes(fiveMinutesLater.getMinutes() + 5);

  // Chuyển đổi thời gian hiện tại thành Unix timestamp (milliseconds since the Unix Epoch)
  let createtime = currentTime.getTime();
  let expireTime = fiveMinutesLater.getTime();
  let reference = "DELETION_" + createtime;

  let path =
    Config.URL_PREFIX +
    IConstants.MERCHANTS +
    "/" +
    merchantId +
    "/" +
    IConstants.DD_TOKENS +
    "/" +
    merchantToken +
    "/" +
    IConstants.DELETIONS +
    "/" +
    reference;

  let body_content = {
    browserPayment: {
      callbackUrl: "https://mtf.onepay.vn/paygate/api/rest/v1/ipn",
      returnUrl: "https://mtf.onepay.vn/ldp/direct-debit/result",
    },
    apiOperation: IConstants.DELETE_TOKEN_DIRECT_DEBIT,
    locale: "vi",
    device: {
      browser: "Chrome/120.0.0.0",
      mobilePhoneModel: "nokia 1280",
      ipAddress: "192.168.166.146",
    },
    customer: {
      phone: "0367573933",
      name: "TRAN THAI DUONG",
      account: { id: "000000001" },
      email: "duongtt@onepay.vn",
    },
  };

  let jsonContent = JSON.stringify(body_content);
  let textEncoder = new TextEncoder();
  let byteArray = textEncoder.encode(jsonContent);
  let hashPayload = util.sha256Hash(byteArray);
  let contentDigest =
    "sha-256=:" + Buffer.from(hashPayload).toString("base64") + ":";
  let contentType = IConstants.APPLICATION_JSON;
  let contentLength = jsonContent.length + "";
  let stringToSign = util.generateStringToSign(
    method,
    path,
    contentType,
    contentLength,
    contentDigest,
    createtime,
    expireTime
  );
  let signature = Auth.makeSign(stringToSign);
  let signatureInput = util.createSignatureInput(
    method,
    createtime,
    expireTime
  );

  let headerRequest = util.createHeaderRequest(
    signatureInput,
    signature,
    contentType,
    contentLength,
    contentDigest
  );

  let urlRequest = Config.BASE_URL + path;

  console.log("urlRequest: " + urlRequest);
  console.log("bodyContent: " + jsonContent);
  console.log("HeaderRequest: " + headerRequest);
  console.log("StringToSign: " + stringToSign);
  console.log("signatureInput: " + signatureInput);
  console.log("signature: " + signature);

  util.makePutRequest(urlRequest, jsonContent, headerRequest);
}

function retrieveTokenDeletionInfo(merchantId, merchantToken, merchantDelRef) {
  let method = IConstants.GET;
  // Lấy thời gian hiện tại
  let currentTime = new Date();
  let fiveMinutesLater = new Date(currentTime.getTime());
  fiveMinutesLater.setMinutes(fiveMinutesLater.getMinutes() + 5);

  // Chuyển đổi thời gian hiện tại thành Unix timestamp (milliseconds since the Unix Epoch)
  let createtime = currentTime.getTime();
  let expireTime = fiveMinutesLater.getTime();

  let path =
    Config.URL_PREFIX +
    IConstants.MERCHANTS +
    "/" +
    merchantId +
    "/" +
    IConstants.DD_TOKENS +
    "/" +
    merchantToken +
    "/" +
    IConstants.DELETIONS +
    "/" +
    merchantDelRef;

  let stringToSign = util.generateStringToSign(
    method,
    path,
    "",
    "",
    "",
    createtime,
    expireTime
  );

  let signature = Auth.makeSign(stringToSign);

  let signatureInput = util.createSignatureInput(
    method,
    createtime,
    expireTime
  );

  let urlRequest = Config.BASE_URL + path;

  let headerRequest = util.createHeaderRequest(
    signatureInput,
    signature,
    "",
    "",
    ""
  );

  console.log("urlRequest: " + urlRequest);
  console.log("HeaderRequest: " + headerRequest);
  console.log("StringToSign: " + stringToSign);
  console.log("signatureInput: " + signatureInput);
  console.log("signature: " + signature);

  util.makeGetRequest(urlRequest, headerRequest);
}

module.exports = {
  tokenRegistration,
  retrieveRegisteredTokenInformation,
  paymentWithRegisteredToken,
  retrievePaymentInformation,
  deleteToken,
  retrieveTokenDeletionInfo,
};
