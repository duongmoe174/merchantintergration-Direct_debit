<?php

include 'Config.php';
include 'IConstants.php';

class Util
{
    function callPostRequest($url, $content, $headers)
    {
        $ch = curl_init($url);
        // Set cURL options
        curl_setopt($ch, CURLOPT_CUSTOMREQUEST, 'POST');
        curl_setopt($ch, CURLOPT_POSTFIELDS, $content);
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_HTTPHEADER, $headers);
        $response = curl_exec($ch);

        // Check for cURL errors
        if (curl_errno($ch)) {
            echo 'cURL error: ' . curl_error($ch);
        }

        // Close cURL session
        curl_close($ch);

        // Handle the response as needed
        print '<br>';
        print_r($response);
    }

    function callPutRequest($url, $content, $headers)
    {
        $ch = curl_init($url);
        // Set cURL options
        curl_setopt($ch, CURLOPT_CUSTOMREQUEST, 'PUT');
        curl_setopt($ch, CURLOPT_POSTFIELDS, $content);
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_HTTPHEADER, $headers);
        $response = curl_exec($ch);

        // Check for cURL errors
        if (curl_errno($ch)) {
            echo 'cURL error: ' . curl_error($ch);
        }

        // Close cURL session
        curl_close($ch);

        // Handle the response as needed
        print '<br>';
        print_r($response);
    }

    function callGetRequest($url, $headers)
    {
        $ch = curl_init($url);
        // Set cURL options
        curl_setopt($ch, CURLOPT_CUSTOMREQUEST, 'GET');
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_HTTPHEADER, $headers);
        $response = curl_exec($ch);

        // Check for cURL errors
        if (curl_errno($ch)) {
            echo 'cURL error: ' . curl_error($ch);
        }

        // Close cURL session
        curl_close($ch);

        // Handle the response as needed
        print '<br>';
        print_r($response);
    }

    function sha256Hash($data)
    {
        return hash('sha256', $data);
    }

    function hmacSha256($key, $data)
    {
        return hash_hmac('sha256', $data, $key);
    }
    function hmacSha256Hex($key, $data)
    {
        return hash_hmac('sha256', $data, pack('H*', $key));
    }

    function createSignatureInput($method, $createTime, $expireTime)
    {
        $result = "";
        switch ($method) {
            case GET:
                $result = "sig=(\"@method\" \"@path\")" . ";" . "created=" . $createTime . ";" . "expires="
                    . $expireTime . ";" . "keyid=" . "\"" . MERCHANT_ID . "\"" . ";" . "alg=" . "\"" . ALG
                    . "\"";
                break;
            case POST:
            case PUT:
                $result = "sig=(\"@method\" \"@path\" \"content-digest\" \"content-type\" \"content-length\")"
                    . ";" . "created=" . $createTime . ";" . "expires=" . $expireTime . ";" . "keyid=" . "\""
                    . MERCHANT_ID . "\"" . ";" . "alg=" . "\"" . ALG . "\"";
                break;
            default:
                $result = "";
        }
        return $result;
    }

    function generateStringToSign($method, $path, $contentType, $contentLength, $contentDigest, $createTime, $expireTime)
    {
        $result = "";
        if ($contentType != "" && $contentLength != "" && $contentDigest != "") {
            $signMethod = "\"@method\": " . $method;
            $signPath = "\"@path\": " . $path;
            $signContentDigest = "\"content-digest\": " . $contentDigest;
            $signContentType = "\"content-type\": " . $contentType;
            $signContentLength = "\"content-length\": " . $contentLength;
            $signParam = "\"@signature-params\": (\"@method\" \"@path\" \"content-digest\" \"content-type\" \"content-length\")"
                . ";" . "created=" . $createTime . ";" . "expires=" . $expireTime . ";" . "keyid=" . "\""
                . MERCHANT_ID
                . "\"" . ";" . "alg=" . "\"" . ALG . "\"";
            $result = $signMethod . "\n" . $signPath . "\n" . $signContentDigest . "\n" . $signContentType . "\n"
                . $signContentLength . "\n" . $signParam;
        } else {
            $signMethod = "\"@method\": " . $method;
            $signPath = "\"@path\": " . $path;
            $signParam = "\"@signature-params\": (\"@method\" \"@path\")"
                . ";" . "created=" . $createTime . ";" . "expires=" . $expireTime . ";" . "keyid=" . "\""
                . MERCHANT_ID
                . "\"" . ";" . "alg=" . "\"" . ALG . "\"";
            $result = $signMethod . "\n" . $signPath . "\n" . $signParam;
        }
        return $result;
    }

    function createHeaderRequest($signatureInput, $signature, $contentType, $contentLength, $contentDigest)
    {
        $header = array();
        if ($contentType != "" && $contentLength != "" && $contentDigest) {
            $header = array(
                CONTENT_TYPE => $contentType,
                CONTENT_LENGTH => $contentLength,
                CONTENT_DIGEST => $contentDigest,
                SIGNATURE_INPUT => $signatureInput,
                SIGNATURE => "sig=:" . $signature . ":"
            );
        } else {
            $header = array(
                SIGNATURE_INPUT => $signatureInput,
                SIGNATURE => "sig=:" . $signature . ":"
            );
        }
        return $header;
    }
}