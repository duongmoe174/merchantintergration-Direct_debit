<?php
include_once 'Auth.php';
include_once 'Util.php';
include_once 'Config.php';
include_once 'IConstants.php';

class Api
{
  function tokenRegistration($merchantId)
  {
    $auth = new Auth();
    $util = new Util();

    $method = IConstants::PUT;

    $createTime = time() . "";
    $expireTime = $createTime . (5 * 60) . "";

    $reference = "DUONGTTTOKEN_" . $createTime;

    $path = Config::URL_PREFIX . IConstants::MERCHANTS . "/" . $merchantId . "/" . IConstants::DD_TOKEN . "/" . $reference;

    $bodyContent = [
      "browserPayment" => [
        "callbackUrl" => "https://webhook.site/ff9e3e57-3021-4922-954d-32c94ef3c7e2",
        "returnUrl" => "https://mtf.onepay.vn/ldp/direct-debit/result"
      ],
      "apiOperation" => IConstants::TOKENIZE_DIRECT_DEBIT,
      "sourceOfFunds" => [
        "types" => [
          "DD_SGTTVNVX",
          "DD_BIDVVNVX"
        ]
      ],
      "locale" => "vi",
      "device" => [
        "browser" => "User Agent",
        "mobilePhoneModel" => "iphone",
        "ipAddress" => "192.168.1.999"
      ],
      "customer" => [
        "phone" => "0367573933",
        "name" => "TRAN THAI DUONG",
        "account" => [
          "id" => "000000001"
        ],
        "email" => "duongtt@onepay.vn"
      ]
    ];
    $jsonContent = json_encode($bodyContent, JSON_UNESCAPED_SLASHES);
    $hashPayload = hash('sha256', $jsonContent, true);
    $contentDigest = "sha-256=:" . base64_encode($hashPayload) . ":";
    $contentType = IConstants::APPLICATION_JSON;
    $contentLength = strlen($jsonContent);
    $stringToSign = $util->generateStringToSign($method, $path, $contentType, $contentLength, $contentDigest, $createTime, $expireTime);
    $signature = $auth->makeSign($stringToSign);
    $signatureInput = $util->createSignatureInput($method, $createTime, $expireTime);

    $headerRequest = $util->createHeaderRequest($signatureInput, $signature, $contentType, $contentLength, $contentDigest);
    $headerLog = implode(", ", $headerRequest);
    $urlRequest = Config::BASE_URL . $path;

    print_r("urlRequest: " . $urlRequest . "\n");
    print_r("bodyContent: " . $jsonContent . "\n");
    print_r("HeaderRequest: " . $headerLog . "\n");
    print_r("StringToSign: " . $stringToSign . "\n");
    print_r("signatureInput: " . $signatureInput . "\n");
    print_r("signature: " . $signature . "\n");

    $util->callPutRequest($urlRequest, $jsonContent, $headerRequest);
  }

  function retrieveRegisteredTokenInformation($merchantId, $merchantTokenRef)
  {
    $auth = new Auth();
    $util = new Util();

    $method = IConstants::GET;

    $createTime = time() . "";
    $expireTime = $createTime . (5 * 60) . "";

    $path = Config::URL_PREFIX . IConstants::MERCHANTS . "/" . $merchantId . "/" . IConstants::DD_TOKEN . "/" . $merchantTokenRef;

    $stringToSign = $util->generateStringToSign($method, $path, "", "", "", $createTime, $expireTime);
    $signature = $auth->makeSign($stringToSign);
    $signatureInput = $util->createSignatureInput($method, $createTime, $expireTime);

    $urlRequest = Config::BASE_URL . $path;
    $headerRequest = $util->createHeaderRequest($signatureInput, $signature, "", "", "");
    $headerLog = implode(", ", $headerRequest);

    print_r("urlRequest: " . $urlRequest . "\n");
    print_r("HeaderRequest: " . $headerLog . "\n");
    print_r("StringToSign: " . $stringToSign . "\n");
    print_r("signatureInput: " . $signatureInput . "\n");
    print_r("signature: " . $signature . "\n");

    $util->callGetRequest($urlRequest, $headerRequest);
  }

  function paymentWithRegisterToken($merchantId, $merchantToken)
  {
    $auth = new Auth();
    $util = new Util();

    $method = IConstants::POST;

    $createTime = time() . "";
    $expireTime = $createTime . (5 * 60) . "";

    $reference = "PAYMENTDTT_" . $createTime;

    $path = Config::URL_PREFIX . IConstants::MERCHANTS . "/" . $merchantId . "/" . IConstants::PAYMENTS_DDT . "/" . $reference;

    $bodyContent = [
      "apiOperation" => IConstants::PURCHASE_DIRECT_DEBIT,
      "invoice" => [
        "amount" => 10000000,
        "description" => "DUONGTT THANH TOAN",
        "currency" => "VND",
      ],
      "device" => [
        "browser" => "IE",
        "mobilePhoneModel" => "NOKIA 1280",
        "ipAddress" => "127.0.01",
      ],
      "transaction" => [
        "sourceOfFunds" => [
          "type" => IConstants::DIRECT_DEBIT_TOKEN,
          "token" => $merchantToken,
        ]
      ],
      "customer" => [
        "phone" => "0367573933",
        "name" => "TRAN THAI DUONG",
        "account" => ["id" => "000000001"],
        "email" => "duongtt@onepay.vn",
      ]
    ];
    $jsonContent = json_encode($bodyContent, JSON_UNESCAPED_SLASHES);
    $hashPayload = hash('sha256', $jsonContent, true);
    $contentDigest = "sha-256=:" . base64_encode($hashPayload) . ":";
    $contentType = IConstants::APPLICATION_JSON;
    $contentLength = strlen($jsonContent);
    $stringToSign = $util->generateStringToSign($method, $path, $contentType, $contentLength, $contentDigest, $createTime, $expireTime);
    $signature = $auth->makeSign($stringToSign);
    $signatureInput = $util->createSignatureInput($method, $createTime, $expireTime);

    $headerRequest = $util->createHeaderRequest($signatureInput, $signature, $contentType, $contentLength, $contentDigest);
    $headerLog = implode(", ", $headerRequest);
    $urlRequest = Config::BASE_URL . $path;

    print_r("urlRequest: " . $urlRequest . "\n");
    print_r("bodyContent: " . $jsonContent . "\n");
    print_r("HeaderRequest: " . $headerLog . "\n");
    print_r("StringToSign: " . $stringToSign . "\n");
    print_r("signatureInput: " . $signatureInput . "\n");
    print_r("signature: " . $signature . "\n");

    $util->callPostRequest($urlRequest, $jsonContent, $headerRequest);
  }

  function retrievePaymentInformation($merchantId, $merchantTxnRef)
  {
    $auth = new Auth();
    $util = new Util();

    $method = IConstants::GET;

    $createTime = time() . "";
    $expireTime = $createTime . (5 * 60) . "";

    $path = Config::URL_PREFIX . IConstants::MERCHANTS . "/" . $merchantId . "/" . IConstants::PAYMENTS_DDT . "/" . $merchantTxnRef;

    $stringToSign = $util->generateStringToSign($method, $path, "", "", "", $createTime, $expireTime);
    $signature = $auth->makeSign($stringToSign);
    $signatureInput = $util->createSignatureInput($method, $createTime, $expireTime);

    $urlRequest = Config::BASE_URL . $path;
    $headerRequest = $util->createHeaderRequest($signatureInput, $signature, "", "", "");
    $headerLog = implode(", ", $headerRequest);

    print_r("urlRequest: " . $urlRequest . "\n");
    print_r("HeaderRequest: " . $headerLog . "\n");
    print_r("StringToSign: " . $stringToSign . "\n");
    print_r("signatureInput: " . $signatureInput . "\n");
    print_r("signature: " . $signature . "\n");

    $util->callGetRequest($urlRequest, $headerRequest);
  }

  function deleteToken($merchantId, $merchantToken)
  {
    $auth = new Auth();
    $util = new Util();

    $method = IConstants::PUT;

    $createTime = time() . "";
    $expireTime = $createTime . (5 * 60) . "";

    $reference = "DELETETOKEN_" . $createTime;

    $path = Config::URL_PREFIX . IConstants::MERCHANTS . "/" . $merchantId . "/" . IConstants::DD_TOKEN . "/" . $merchantToken . "/" . IConstants::DELETIONS . "/" . $reference;

    $bodyContent = [
      "browserPayment" => [
        "callbackUrl" => "https://mtf.onepay.vn/paygate/api/rest/v1/ipn",
        "returnUrl" => "https://mtf.onepay.vn/ldp/direct-debit/result",
      ],
      "apiOperation" => IConstants::DELETE_TOKEN_DIRECT_DEBIT,
      "locale" => "vi",
      "device" => [
        "browser" => "Chrome/120.0.0.0",
        "mobilePhoneModel" => "nokia 1280",
        "ipAddress" => "192.168.166.146",
      ],
      "customer" => [
        "phone" => "0367573933",
        "name" => "TRAN THAI DUONG",
        "account" => ["id" => "000000001"],
        "email" => "duongtt@onepay.vn",
      ],
    ];
    $jsonContent = json_encode($bodyContent, JSON_UNESCAPED_SLASHES);
    $hashPayload = hash('sha256', $jsonContent, true);
    $contentDigest = "sha-256=:" . base64_encode($hashPayload) . ":";
    $contentType = IConstants::APPLICATION_JSON;
    $contentLength = strlen($jsonContent);
    $stringToSign = $util->generateStringToSign($method, $path, $contentType, $contentLength, $contentDigest, $createTime, $expireTime);
    $signature = $auth->makeSign($stringToSign);
    $signatureInput = $util->createSignatureInput($method, $createTime, $expireTime);

    $headerRequest = $util->createHeaderRequest($signatureInput, $signature, $contentType, $contentLength, $contentDigest);
    $headerLog = implode(", ", $headerRequest);
    $urlRequest = Config::BASE_URL . $path;

    print_r("urlRequest: " . $urlRequest . "\n");
    print_r("bodyContent: " . $jsonContent . "\n");
    print_r("HeaderRequest: " . $headerLog . "\n");
    print_r("StringToSign: " . $stringToSign . "\n");
    print_r("signatureInput: " . $signatureInput . "\n");
    print_r("signature: " . $signature . "\n");

    $util->callPutRequest($urlRequest, $jsonContent, $headerRequest);
  }

  function retrieveTokenDeletionInfo($merchantId, $merchantToken, $merchantDelRef)
  {
    $auth = new Auth();
    $util = new Util();

    $method = IConstants::GET;

    $createTime = time() . "";
    $expireTime = $createTime . (5 * 60) . "";

    $path = Config::URL_PREFIX . IConstants::MERCHANTS . "/" . $merchantId . "/" . IConstants::DD_TOKEN . "/" . $merchantToken . "/" . IConstants::DELETIONS . "/" . $merchantDelRef;

    $stringToSign = $util->generateStringToSign($method, $path, "", "", "", $createTime, $expireTime);
    $signature = $auth->makeSign($stringToSign);
    $signatureInput = $util->createSignatureInput($method, $createTime, $expireTime);

    $urlRequest = Config::BASE_URL . $path;
    $headerRequest = $util->createHeaderRequest($signatureInput, $signature, "", "", "");
    $headerLog = implode(", ", $headerRequest);

    print_r("urlRequest: " . $urlRequest . "\n");
    print_r("HeaderRequest: " . $headerLog . "\n");
    print_r("StringToSign: " . $stringToSign . "\n");
    print_r("signatureInput: " . $signatureInput . "\n");
    print_r("signature: " . $signature . "\n");

    $util->callGetRequest($urlRequest, $headerRequest);
  }
}