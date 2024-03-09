<?php
include_once 'Api.php';
include_once 'Config.php';


function main()
{
    retrieveTokenDeletionInfo();
}

function tokenRegistration()
{
    $api = new Api();
    $merchantId = Config::MERCHANT_ID;
    $api->tokenRegistration($merchantId);
}

function retrieveRegisteredTokenInformation()
{
    $api = new Api();
    $merchantId = Config::MERCHANT_ID;
    $merchantTokenRef = "DUONGTTTOKEN_1709952883";
    $api->retrieveRegisteredTokenInformation($merchantId, $merchantTokenRef);
}

function paymentWithRegisteredToken()
{
    $api = new Api();
    $merchantId = Config::MERCHANT_ID;
    $merchantToken = "TKN-OEWd62S2TOCJKzhSDMCWmg";
    $api->paymentWithRegisterToken($merchantId, $merchantToken);
}

function retrievePaymentInformation()
{
    $api = new Api();
    $merchantId = Config::MERCHANT_ID;
    $merchantTxnRef = "PAYMENTDTT_1709954392";
    $api->retrievePaymentInformation($merchantId, $merchantTxnRef);
}

function deleteToken()
{
    $api = new Api();
    $merchantId = Config::MERCHANT_ID;
    $merchantToken = "TKN-OEWd62S2TOCJKzhSDMCWmg";
    $api->deleteToken($merchantId, $merchantToken);
}

function retrieveTokenDeletionInfo()
{
    $api = new Api();
    $merchantId = Config::MERCHANT_ID;
    $merchantToken = "TKN-OEWd62S2TOCJKzhSDMCWmg";
    $merchantDelRef = "DELETETOKEN_1709955058";
    $api->retrieveTokenDeletionInfo($merchantId, $merchantToken, $merchantDelRef);
}

main();