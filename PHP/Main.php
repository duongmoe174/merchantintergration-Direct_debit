<?php
include_once 'Api.php';
include_once 'Config.php';


function main()
{
    tokenRegistration();
}

function tokenRegistration()
{
    $api = new Api();
    $merchantId = MERCHANT_ID;
    $api->tokenRegistration($merchantId);
}

main();