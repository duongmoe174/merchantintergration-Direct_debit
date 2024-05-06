<?php

class Auth
{
    const PRIVATEKEY = "yq6zIGeQK99dfX/pm8nLeq1e1tKmdfVTUmvjdtJKQ/VhVOzoGix3EwvAiqzPl7eCTb0vEyJQlzorLSaLOF4oDg==";

    function makeSign($stringToSign)
    {
        $key = base64_decode(Auth::PRIVATEKEY);

        // Tạo chữ ký
        $signature = sodium_crypto_sign_detached($stringToSign, $key);

        // Chuyển đổi chữ ký thành dạng base64
        $signature_base64 = base64_encode($signature);

        // In ra chữ ký
        echo "Chữ ký: $signature_base64\n";

        return $signature_base64;
    }
}