package com.example.directdebit;

import java.util.ArrayList;
import java.util.Base64;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.logging.Logger;

import org.bouncycastle.util.Exceptions;

import com.google.gson.Gson;

public class Api {
  private static final Logger logger = Logger.getLogger(Api.class.getName());

  public static void tokenRegistration(String merchantId) {
    try {
      String method = IConstants.PUT;

      String reference = "DUONGTTTOKEN_" + System.currentTimeMillis();

      String path = Config.URL_PREFIX + IConstants.MERCHANTS + "/" + merchantId + "/" + IConstants.DD_TOKEN + "/"
          + reference;

      long unixCurrentTime = System.currentTimeMillis() / 1000L;
      long fiveMinutesLater = unixCurrentTime + (5 * 60);

      String createTime = String.valueOf(unixCurrentTime);
      String expireTime = String.valueOf(fiveMinutesLater);

      /* ========== Start create bodyContent ========== */
      Map<String, Object> bodyContent = new HashMap<>();

      Map<String, Object> browserPayment = new HashMap<>();
      browserPayment.put("returnUrl", "https://mtf.onepay.vn/ldp/direct-debit/result");
      browserPayment.put("callbackUrl", "https://webhook.site/02fb1eda-7122-4db8-ab03-b4dc13d6b157");

      Map<String, Object> customer = new HashMap<>();
      Map<String, Object> customerAccount = new HashMap<>();
      customerAccount.put("id", "000020031");
      customer.put("account", customerAccount);
      // customer.put("email", "duongtt@onepay.vn");
      customer.put("name", "TRAN THAI DUONG");
      customer.put("phone", "0367573933");

      Map<String, Object> sourceOfFunds = new HashMap<>();
      List<String> types = new ArrayList<>();
      types.add("DD_SGTTVNVX");
      types.add("DD_BIDVVNVX");
      types.add("DD_MSCBVNVX");
      types.add("DD_MCOBVNVX");
      types.add("DD_ICBVVNVX");
      sourceOfFunds.put("types", types);

      Map<String, Object> device = new HashMap<>();
      device.put("ipAddress", "192.168.1.999");
      device.put("browser", "User Agent");
      device.put("mobilePhoneModel", "iphone");

      bodyContent.put("apiOperation", IConstants.TOKENIZE_DIRECT_DEBIT);
      bodyContent.put("browserPayment", browserPayment);
      bodyContent.put("customer", customer);
      bodyContent.put("sourceOfFunds", sourceOfFunds);
      // bodyContent.put("device", device);
      bodyContent.put("locale", "vi");
      /* ========== End create bodyContent ========== */

      Gson gson = new Gson();

      String jsonContent = gson.toJson(bodyContent);

      byte[] hashPayload = Util.sha256Hash(jsonContent);
      String contentDigest = "sha-256=:" + Base64.getEncoder().encodeToString(hashPayload) + ":";
      System.out.println("contentDigest: " + contentDigest);

      String contentType = IConstants.APPLICATION_JSON;

      String contentLength = String.valueOf(jsonContent.length());

      String stringToSign = Util.generateStringToSign(method, path, contentType, contentLength, contentDigest,
          createTime, expireTime);

      String signature = Auth.makeSign(stringToSign);

      String signatureInput = Util.createSignatureInput(method, createTime, expireTime);

      Map<String, Object> headerRequest = Util.createHeaderRequest(signatureInput, signature, contentType,
          contentLength, contentDigest);

      String urlRequest = Config.BASE_URL + path;

      logger.info("urlRequest: " + urlRequest);
      logger.info("bodyContent: " + jsonContent);
      logger.info("HeaderRequest: " + headerRequest);
      logger.info("StringToSign: " + stringToSign);
      logger.info("signatureInput: " + signatureInput);
      logger.info("signature: " + signature);

      logger.info("============== HTTP RESPOSNE DATA ==============");
      Map<String, Object> responseData = Util.executePut2(urlRequest, headerRequest, jsonContent);

      for (Map.Entry<String, Object> entry : responseData.entrySet()) {
        String key = entry.getKey();
        Object value = entry.getValue();
        logger.info(key + " : " + value);
      }

    } catch (Exception e) {
      // TODO: handle exception
      logger.info(e.getMessage());
      e.printStackTrace();
    }
  }

  public static void retrieveRegisteredTokenInformation(String merchantId, String merchantTokenRef) {
    try {
      String method = IConstants.GET;

      long unixCurrentTime = System.currentTimeMillis() / 1000L;
      long fiveMinutesLater = unixCurrentTime + (5 * 60);

      String createTime = String.valueOf(unixCurrentTime);
      String expireTime = String.valueOf(fiveMinutesLater);

      String path = Config.URL_PREFIX + IConstants.MERCHANTS + "/" + merchantId + "/" + IConstants.DD_TOKEN + "/"
          + merchantTokenRef;

      String stringToSign = Util.generateStringToSign(method, path, createTime, expireTime);

      String signature = Auth.makeSign(stringToSign);

      String signatureInput = Util.createSignatureInput(method, createTime, expireTime);

      String urlRequest = Config.BASE_URL + path;

      Map<String, Object> headerRequest = Util.createHeaderRequest(signatureInput, signature);

      logger.info("urlRequest: " + urlRequest);
      logger.info("HeaderRequest: " + headerRequest);
      logger.info("StringToSign: " + stringToSign);
      logger.info("signatureInput: " + signatureInput);
      logger.info("signature: " + signature);

      Util.executeGet(urlRequest, headerRequest);

    } catch (Exception e) {
      // TODO: handle exception
      logger.info(e.getMessage());
      e.printStackTrace();
    }
  }

  public static void paymentWithRegisteredToken(String merchantId, String merchantToken) {
    try {
      String method = IConstants.POST;

      String reference = "PAYMENTDDT_" + System.currentTimeMillis();

      String path = Config.URL_PREFIX + IConstants.MERCHANTS + "/" + merchantId + "/" + IConstants.PAYMENTS_DDT + "/"
          + reference;

      long unixCurrentTime = System.currentTimeMillis() / 1000L;
      long fiveMinutesLater = unixCurrentTime + (5 * 60);

      String createTime = String.valueOf(unixCurrentTime);
      String expireTime = String.valueOf(fiveMinutesLater);

      /* ========== START CREATE BODY CONTENT ========== */
      Map<String, Object> bodyContent = new HashMap<>();

      Map<String, Object> invoice = new HashMap<>();
      invoice.put("amount", 10000000);
      invoice.put("currency", "VND");
      invoice.put("description", "DUONGTT THANH TOAN");

      Map<String, Object> customer = new HashMap<>();
      Map<String, Object> customerAccount = new HashMap<>();
      customerAccount.put("id", "000000001");
      customer.put("account", customerAccount);
      customer.put("email", "duongtt@onepay.vn");
      customer.put("name", "TRAN THAI DUONG");
      customer.put("phone", "0367573933");

      Map<String, Object> transaction = new HashMap<>();
      Map<String, Object> sourceOfFunds = new HashMap<>();
      sourceOfFunds.put("type", IConstants.DIRECT_DEBIT_TOKEN);
      sourceOfFunds.put("token", merchantToken);
      transaction.put("sourceOfFunds", sourceOfFunds);
      transaction.put("source", "MIT");

      Map<String, Object> device = new HashMap<>();
      device.put("ipAddress", "127.0.01");
      device.put("browser", "IE");
      device.put("mobilePhoneModel", "NOKIA 1280");

      bodyContent.put("apiOperation", IConstants.PURCHASE_DIRECT_DEBIT);
      bodyContent.put("invoice", invoice);
      bodyContent.put("customer", customer);
      bodyContent.put("transaction", transaction);
      // bodyContent.put("device", device);
      /* ========== End create bodyContent ========== */

      Gson gson = new Gson();

      String jsonContent = gson.toJson(bodyContent);

      byte[] hashPayload = Util.sha256Hash(jsonContent);
      String contentDigest = "sha-256=:" + Base64.getEncoder().encodeToString(hashPayload) + ":";

      String contentType = IConstants.APPLICATION_JSON;

      String contentLength = String.valueOf(jsonContent.length());

      String stringToSign = Util.generateStringToSign(method, path, contentType, contentLength, contentDigest,
          createTime, expireTime);

      String signature = Auth.makeSign(stringToSign);

      String signatureInput = Util.createSignatureInput(method, createTime, expireTime);

      Map<String, Object> headerRequest = Util.createHeaderRequest(signatureInput, signature, contentType,
          contentLength, contentDigest);

      String urlRequest = Config.BASE_URL + path;

      logger.info("urlRequest: " + urlRequest);
      logger.info("bodyContent: " + jsonContent);
      logger.info("HeaderRequest: " + headerRequest);
      logger.info("StringToSign: " + stringToSign);
      logger.info("signatureInput: " + signatureInput);
      logger.info("signature: " + signature);

      Util.executePost(urlRequest, headerRequest, jsonContent);
    } catch (Exception e) {
      // TODO: handle exception
      logger.info(e.getMessage());
      e.printStackTrace();
    }
  }

  public static void retrievePaymentInformation(String merchantId, String merchantTxnRef) {
    try {
      String method = IConstants.GET;

      String path = Config.URL_PREFIX + IConstants.MERCHANTS + "/" + merchantId + "/" + IConstants.PAYMENTS_DDT + "/"
          + merchantTxnRef;

      long unixCurrentTime = System.currentTimeMillis() / 1000L;
      long fiveMinutesLater = unixCurrentTime + (5 * 60);

      String createTime = String.valueOf(unixCurrentTime);
      String expireTime = String.valueOf(fiveMinutesLater);

      String stringToSign = Util.generateStringToSign(method, path, createTime, expireTime);

      String signature = Auth.makeSign(stringToSign);

      String signatureInput = Util.createSignatureInput(method, createTime, expireTime);

      String urlRequest = Config.BASE_URL + path;

      Map<String, Object> headerRequest = Util.createHeaderRequest(signatureInput, signature);

      logger.info("urlRequest: " + urlRequest);
      logger.info("HeaderRequest: " + headerRequest);
      logger.info("StringToSign: " + stringToSign);
      logger.info("signatureInput: " + signatureInput);
      logger.info("signature: " + signature);

      Util.executeGet(urlRequest, headerRequest);

    } catch (Exception e) {
      // TODO: handle exception
      logger.info(e.getMessage());
      e.printStackTrace();
    }
  }

  public static void deleteToken(String merchantId, String token) {
    try {

      String method = IConstants.PUT;

      String reference = "DELETETOKEN_" + System.currentTimeMillis();

      String path = Config.URL_PREFIX + IConstants.MERCHANTS + "/" + merchantId + "/" + IConstants.DD_TOKEN + "/"
          + token + "/" + IConstants.DELETIONS + "/" + reference;

      long unixCurrentTime = System.currentTimeMillis() / 1000L;
      long fiveMinutesLater = unixCurrentTime + (5 * 60);

      String createTime = String.valueOf(unixCurrentTime);
      String expireTime = String.valueOf(fiveMinutesLater);

      /* ========== START CREATE BODY CONTENT ========== */
      Map<String, Object> bodyContent = new HashMap<>();

      Map<String, Object> browserPayment = new HashMap<>();
      browserPayment.put("returnUrl", "https://mtf.onepay.vn/ldp/direct-debit/result");
      browserPayment.put("callbackUrl", "https://webhook.site/6211e954-5f1c-4998-bcb8-4dc62ad37fab");

      Map<String, Object> customer = new HashMap<>();
      Map<String, Object> customerAccount = new HashMap<>();
      customerAccount.put("id", "000000001");
      customer.put("account", customerAccount);
      customer.put("email", "duongtt@onepay.vn");
      customer.put("name", "TRAN THAI DUONG");
      customer.put("phone", "0367573933");

      Map<String, Object> device = new HashMap<>();
      device.put("ipAddress", "192.168.166.146");
      device.put("browser", "Chrome/120.0.0.0");
      device.put("mobilePhoneModel", "nokia 1280");

      bodyContent.put("apiOperation", IConstants.DELETE_TOKEN_DIRECT_DEBIT);
      bodyContent.put("browserPayment", browserPayment);
      bodyContent.put("customer", customer);
      // bodyContent.put("device", device);
      bodyContent.put("locale", "vi");
      /* ========== END CREATE BODY CONTENT ========== */

      Gson gson = new Gson();

      String jsonContent = gson.toJson(bodyContent);

      byte[] hashPayload = Util.sha256Hash(jsonContent);
      String contentDigest = "sha-256=:" + Base64.getEncoder().encodeToString(hashPayload) + ":";

      String contentType = IConstants.APPLICATION_JSON;

      String contentLength = String.valueOf(jsonContent.length());

      String stringToSign = Util.generateStringToSign(method, path, contentType, contentLength, contentDigest,
          createTime, expireTime);

      String signature = Auth.makeSign(stringToSign);

      String signatureInput = Util.createSignatureInput(method, createTime, expireTime);

      Map<String, Object> headerRequest = Util.createHeaderRequest(signatureInput, signature, contentType,
          contentLength, contentDigest);

      String urlRequest = Config.BASE_URL + path;

      logger.info("urlRequest: " + urlRequest);
      logger.info("bodyContent: " + jsonContent);
      logger.info("HeaderRequest: " + headerRequest);
      logger.info("StringToSign: " + stringToSign);
      logger.info("signatureInput: " + signatureInput);
      logger.info("signature: " + signature);

      Util.executePut(urlRequest, headerRequest, jsonContent);
    } catch (Exception e) {
      // TODO: handle exception
      logger.info(e.getMessage());
      e.printStackTrace();
    }
  }

  public static void retrieveTokenDeletionInfo(String merchantId, String token, String merchantDelRef) {
    try {
      String method = IConstants.GET;

      String path = Config.URL_PREFIX + "merchants" + "/" + merchantId + "/" + IConstants.DD_TOKEN + "/"
          + token + "/" + IConstants.DELETIONS + "/" + merchantDelRef;

      long unixCurrentTime = System.currentTimeMillis() / 1000L;
      long fiveMinutesLater = unixCurrentTime + (5 * 60);

      String createTime = String.valueOf(unixCurrentTime);
      String expireTime = String.valueOf(fiveMinutesLater);

      String stringToSign = Util.generateStringToSign(method, path, createTime, expireTime);

      String signature = Auth.makeSign(stringToSign);

      String signatureInput = Util.createSignatureInput(method, createTime, expireTime);

      String urlRequest = Config.BASE_URL + path;

      Map<String, Object> headerRequest = Util.createHeaderRequest(signatureInput, signature);

      logger.info("urlRequest: " + urlRequest);
      logger.info("HeaderRequest: " + headerRequest);
      logger.info("StringToSign: " + stringToSign);
      logger.info("signatureInput: " + signatureInput);
      logger.info("signature: " + signature);

      Util.executeGet(urlRequest, headerRequest);
    } catch (Exception e) {
      logger.log(null, merchantDelRef, e);
      e.printStackTrace();
    }
  }

  public static void verifyIPN() {
    try {
      String signatureInputReq = "sig=(\"content-type\" \"content-length\" \"content-digest\");created=1726545759;expires=1726546059;keyid=\"TESTONEPAY50\";alg=\"ed25519\"";
      String contentDigestReq = "sha-256=:4QPXwxUM1GGvO5VIOPlXOAM1N+7eztDIOIkvQuh5QH8=:";
      String signatureReq = "sig=:vn8XYqappH1/SFmm4hFyzUBBbMSvcgPZf1uHQXODOh8+GDUi4ntzLkSnzi8KCaZfTEtmM3P1kNpRGUX0YDKsAQ==:";
      String contentReq = "{\"merchantId\":\"TESTONEPAY50\",\"merchTokenRef\":\"DUONGTTTOKEN_1726545744078\",\"token\":\"TKN-4-L8Wbv7SsWK-L1Orf2Q2w\",\"state\":\"approved\",\"customer\":{\"account\":{\"id\":\"000000001\"},\"email\":\"duongtt@onepay.vn\",\"name\":\"TRAN THAI DUONG\",\"phone\":\"0367573933\"},\"sourceOfFunds\":{\"type\":\"DD_BIDVVNVX\",\"provided\":{\"type\":\"account\",\"accountNumber\":\"xxx1241\",\"accountHolder\":\"TRAN THAI DUONG\"}}}";

      String contentType = IConstants.APPLICATION_JSON;
      String contentLength = String.valueOf(contentReq.length());

      // Check content-digest
      byte[] hashPayload = Util.sha256Hash(contentReq);
      String contentDigestVerify = "sha-256=:" + Base64.getEncoder().encodeToString(hashPayload) + ":";
      if (!contentDigestReq.equals(contentDigestVerify)) {
        throw new Exception("Content-Digest is invalid");
      } else {
        // check Signature
        System.out.println("Content_Disgest is valid!");
        String stringToVerify = Util.generateStringToVerify(contentType, contentLength, contentDigestReq,
            signatureInputReq);

        boolean verifySignature = Auth.verifySign(stringToVerify, signatureReq);

        System.out.println("stringToVerify: " + stringToVerify);
        System.out.println("Content Length: " + contentLength);
        System.out.println("Request Signature: " + signatureReq);
        System.out.println("Verify Signature: " + verifySignature);

      }

    } catch (Exception e) {
      logger.log(null, e.getMessage(), e);
      e.printStackTrace();
    }
  }
}
