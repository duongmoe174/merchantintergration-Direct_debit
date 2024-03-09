<?php

include_once 'Config.php';
include_once 'IConstants.php';

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

    function createSignatureInput($method, $createTime, $expireTime)
    {
        $result = "";
        switch ($method) {
            case IConstants::GET:
                $result = "sig=(\"@method\" \"@path\")" . ";" . "created=" . $createTime . ";" . "expires="
                    . $expireTime . ";" . "keyid=" . "\"" . Config::MERCHANT_ID . "\"" . ";" . "alg=" . "\"" . Config::ALG
                    . "\"";
                break;
            case IConstants::POST:
            case IConstants::PUT:
                $result = "sig=(\"@method\" \"@path\" \"content-digest\" \"content-type\" \"content-length\")"
                    . ";" . "created=" . $createTime . ";" . "expires=" . $expireTime . ";" . "keyid=" . "\""
                    . Config::MERCHANT_ID . "\"" . ";" . "alg=" . "\"" . Config::ALG . "\"";
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
                . Config::MERCHANT_ID
                . "\"" . ";" . "alg=" . "\"" . Config::ALG . "\"";
            $result = $signMethod . "\n" . $signPath . "\n" . $signContentDigest . "\n" . $signContentType . "\n"
                . $signContentLength . "\n" . $signParam;
        } else {
            $signMethod = "\"@method\": " . $method;
            $signPath = "\"@path\": " . $path;
            $signParam = "\"@signature-params\": (\"@method\" \"@path\")"
                . ";" . "created=" . $createTime . ";" . "expires=" . $expireTime . ";" . "keyid=" . "\""
                . Config::MERCHANT_ID
                . "\"" . ";" . "alg=" . "\"" . Config::ALG . "\"";
            $result = $signMethod . "\n" . $signPath . "\n" . $signParam;
        }
        return $result;
    }

    function createHeaderRequest($signatureInput, $signature, $contentType, $contentLength, $contentDigest)
    {
        $header = array();
        if ($contentType != "" && $contentLength != "" && $contentDigest) {
            $header = array(
                IConstants::CONTENT_TYPE . ":" . $contentType,
                IConstants::CONTENT_LENGTH . ":" . $contentLength,
                IConstants::CONTENT_DIGEST . ":" . $contentDigest,
                IConstants::SIGNATURE_INPUT . ":" . $signatureInput,
                IConstants::SIGNATURE . ":" . "sig=:" . $signature . ":"
            );
        } else {
            $header = array(
                IConstants::SIGNATURE_INPUT . ":" . $signatureInput,
                IConstants::SIGNATURE . ":" . "sig=:" . $signature . ":"
            );
        }
        return $header;
    }
}