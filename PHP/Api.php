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

    $method = PUT;

    $createTime = time() . "";
    $expireTime = $createTime . (5 * 60) . "";

    $reference = "DUONGTTTOKEN_" . $createTime;

    $path = URL_PREFIX . MERCHANTS . "/" . $merchantId . "/" . DD_TOKEN . "/" . $reference;

    $bodyContent = [
      "browserPayment" => [
        "callbackUrl" => "https://dev.onepay.vn/paygate/api/rest/v1/ipn",
        "returnUrl" => "https://dev.onepay.vn/ldp/direct-debit/result"
      ],
      "apiOperation" => TOKENIZE_DIRECT_DEBIT,
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
    $contentType = APPLICATION_JSON;
    $contentLength = strlen($jsonContent);
    $stringToSign = $util->generateStringToSign($method, $path, $contentType, $contentLength, $contentDigest, $createTime, $expireTime);
    $signature = $auth->makeSign($stringToSign);
    $signatureInput = $util->createSignatureInput($method, $createTime, $expireTime);

    $headerRequest = $util->createHeaderRequest($signatureInput, $signature, $contentType, $contentLength, $contentDigest);
    $headerLog = implode(", ", $headerRequest);
    $urlRequest = BASE_URL . $path;

    print_r("urlRequest: " . $urlRequest . "\n");
    print_r("bodyContent: " . $jsonContent . "\n");
    print_r("HeaderRequest: " . $headerLog . "\n");
    print_r("StringToSign: " . $stringToSign . "\n");
    print_r("signatureInput: " . $signatureInput . "\n");
    print_r("signature: " . $signature . "\n");

    $util->callPutRequest($urlRequest, $jsonContent, $headerRequest);
  }
}