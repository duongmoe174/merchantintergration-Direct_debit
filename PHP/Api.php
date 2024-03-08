<?php
include 'Auth.php';
include 'Util.php';
include 'Config.php';
include 'IConstants.php';

class Api
{
  function tokenRegistration($merchantId)
  {
    $auth = new Auth();
    $util = new Util();

    $method = GET;

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
    $hashPayload = $util->sha256Hash($jsonContent);
    $contentType = APPLICATION_JSON;
    $contentDigest = "sha-256=:" . base64_encode($hashPayload) . ":";
    $contentLength = strlen($jsonContent);
    $stringToSign = $util->generateStringToSign($method, $path, $contentType, $contentLength, $contentDigest, $createTime, $expireTime);
    $signature = $auth->makeSign($stringToSign);
    $signatureInput = $util->createSignatureInput($method, $createTime, $expireTime);

    $headerRequest = $util->createHeaderRequest($signatureInput, $signature, $contentType, $contentLength, $contentDigest);

    $urlRequest = BASE_URL . $path;

    echo "urlRequest: " . $urlRequest . "\n";
    echo "bodyContent: " . $jsonContent . "\n";
    echo "HeaderRequest: " . $headerRequest . "\n";
    echo "StringToSign: " . $stringToSign . "\n";
    echo "signatureInput: " . $signatureInput . "\n";
    echo "signature: " . $signature . "\n";

    $util->callPutRequest($urlRequest, $jsonContent, $headerRequest);
  }
}