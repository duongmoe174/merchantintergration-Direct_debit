<?php
include 'Api.php';
include 'Config.php';


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