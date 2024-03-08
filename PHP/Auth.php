<?php
class Auth
{
    const PIKBASE64 = "MC4CAQAwBQYDK2VwBCIEIJ+DYvh6SEqVTm50DFtMDoQikTmiCqirVv9mWG9qfSnF";
    const PUKBASE64 = "MCowBQYDK2VwAyEAJrQLj5P/89iXES9+vFgrIy29clF9CC/oPPsw3c5D0bs=";

    function makeSign($stringToSign)
    {
        //decode to binary
        $pikBinary = base64_decode(Auth::PIKBASE64);
        $pukBinary = base64_decode(Auth::PUKBASE64);

        //get last32byte -> true key
        $pikLast32Bytes = substr($pikBinary, -32);
        $pukLast32Bytes = substr($pukBinary, -32);

        //make seedkey
        $seedkey = $pikLast32Bytes . $pukLast32Bytes;

        if (strlen($seedkey) !== SODIUM_CRYPTO_SIGN_SECRETKEYBYTES) {
            throw new Exception("Invalid private key length");
        } else {
            print_r("SEED OK!");
        }

        $signature = sodium_crypto_sign_detached($stringToSign, $seedkey);

        return base64_encode($signature);
    }

}

$data2 = "\"@method\": PUT\n" .
    "\"@path\": /paygate/api/rest/v1/merchants/OPTEST/dd_tokens/DUONGTTTOKEN_1707196828629206900\n" .
    "\"content-digest\": sha-256=:YUd7b09NPdDjyrNveLs3KHbzxlzGRlU0+R69rpl7c9Y=:\n" .
    "\"content-type\": application/json\n" .
    "\"content-length\": 450\n" .
    "\"@signature-params\": (\"@method\" \"@path\" \"content-digest\" \"content-type\" \"content-length\");created=1707196828;expires=1707197128;keyid=\"OPTEST\";alg=\"ed25519\"";

