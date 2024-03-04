const util = require("./Util");
const Config = require("./Config");
const IConstants = require("./IContants");
const Auth = require("./Auth");

function tokenRegistration(merchantId) {
  const method = IConstants.PUT;
  // Lấy thời gian hiện tại
  const currentTime = new Date();
  const fiveMinutesLater = new Date(currentTime.getTime());
  fiveMinutesLater.setMinutes(fiveMinutesLater.getMinutes() + 5);

  // Chuyển đổi thời gian hiện tại thành Unix timestamp (milliseconds since the Unix Epoch)
  const createtime = currentTime.getTime();
  const expireTime = fiveMinutesLater.getTime();
  const reference = "DUONGTTTOKEN_" + createtime;

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
      callbackUrl: "https://dev.onepay.vn/paygate/api/rest/v1/ipn",
      returnUrl: "https://dev.onepay.vn/ldp/direct-debit/result",
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
  const textEncoder = new TextEncoder();
  const byteArray = textEncoder.encode(jsonContent);
  const hashPayload = util.sha256Hash(byteArray);
  const contentDigest =
    "sha-256=:" + Buffer.from(hashPayload).toString("base64") + ":";
  const contentType = IConstants.APPLICATION_JSON;
  const contentLength = jsonContent.length + "";
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

tokenRegistration(Config.MERCHANT_ID);
