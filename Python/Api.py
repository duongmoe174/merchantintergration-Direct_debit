from IConstants import IConstants
from Config import Config
import json
import Util
import base64
import Auth
import time
import hashlib


def token_registration(merchant_id: str):
    method = IConstants.PUT

    current_unix_time = int(time.time())
    five_minutes_later = current_unix_time + 5 * 600

    create_time = str(current_unix_time)
    expire_time = str(five_minutes_later)

    reference = "DUONGTTTOKEN_" + create_time

    path = (
        Config.URL_PREFIX
        + IConstants.MERCHANTS
        + "/"
        + merchant_id
        + "/"
        + IConstants.DD_TOKENS
        + "/"
        + reference
    )

    body_content = {
        "browserPayment": {
            "callbackUrl": "https://dev.onepay.vn/paygate/api/rest/v1/ipn",
            "returnUrl": "https://dev.onepay.vn/ldp/direct-debit/result",
        },
        "apiOperation": IConstants.TOKENIZE_DIRECT_DEBIT,
        "sourceOfFunds": {"types": ["DD_SGTTVNVX", "DD_BIDVVNVX"]},
        "locale": "vi",
        "device": {
            "browser": "User Agent",
            "mobilePhoneModel": "iphone",
            "ipAddress": "192.168.1.999",
        },
        "customer": {
            "phone": "0367573933",
            "name": "TRAN THAI DUONG",
            "account": {"id": "000000001"},
            "email": "duongtt@onepay.vn",
        },
    }

    json_content = json.dumps(body_content)
    hash_object = hashlib.sha256(json_content.encode())
    hash_pay_load = hash_object.digest()
    content_digest = "sha-256=:" + base64.b64encode(hash_pay_load).decode() + ":"
    content_type = IConstants.APPLICATION_JSON
    content_length = str(len(json_content))
    string_to_sign = Util.generate_string_to_sign(
        method,
        path,
        content_type,
        content_length,
        content_digest,
        create_time,
        expire_time,
    )
    signature = Auth.make_sign(string_to_sign)
    signature_input = Util.create_signature_input(method, create_time, expire_time)

    header_request = Util.create_header_request(
        signature_input, signature, content_type, content_length, content_digest
    )

    url_request = Config.BASE_URL + path

    print("contentDigest: " + content_digest)
    print("urlRequest: " + url_request)
    print("bodyContent: " + json_content)
    print("HeaderRequest: " + str(header_request))
    print("StringToSign: " + string_to_sign)
    print("signatureInput: " + signature_input)
    print("signature: " + signature)
    Util.call_put_request(url_request, body_content, header_request)


def retrieve_registered_token_information(merchant_id: str, merchant_token_ref: str):
    method = IConstants.GET

    current_unix_time = int(time.time())
    five_minutes_later = current_unix_time + 5 * 600

    create_time = str(current_unix_time)
    expire_time = str(five_minutes_later)

    path = (
        Config.URL_PREFIX
        + IConstants.MERCHANTS
        + "/"
        + merchant_id
        + "/"
        + IConstants.DD_TOKENS
        + "/"
        + merchant_token_ref
    )

    string_to_sign = Util.generate_string_to_sign(
        method, path, "", "", "", create_time, expire_time
    )

    signature = Auth.make_sign(string_to_sign)

    signature_input = Util.create_signature_input(method, create_time, expire_time)

    url_request = Config.BASE_URL + path

    header_request = Util.create_header_request(signature_input, signature, "", "", "")

    print("urlRequest: " + url_request)
    print("HeaderRequest: " + str(header_request))
    print("StringToSign: " + string_to_sign)
    print("signatureInput: " + signature_input)
    print("signature: " + signature)
    Util.call_get_request(url_request, header_request)


def payment_with_registered_token(merchant_id: str, merchant_token: str):
    method = IConstants.POST

    current_unix_time = int(time.time())
    five_minutes_later = current_unix_time + 5 * 600

    create_time = str(current_unix_time)
    expire_time = str(five_minutes_later)

    reference = "PAYMENTDTT_" + create_time

    path = (
        Config.URL_PREFIX
        + IConstants.MERCHANTS
        + "/"
        + merchant_id
        + "/"
        + IConstants.PAYMENTS_DDT
        + "/"
        + reference
    )

    body_content = {
        "apiOperation": IConstants.PURCHASE_DIRECT_DEBIT,
        "invoice": {
            "amount": 10000000,
            "description": "DUONGTT THANH TOAN",
            "currency": "VND",
        },
        "device": {
            "browser": "IE",
            "mobilePhoneModel": "NOKIA 1280",
            "ipAddress": "127.0.01",
        },
        "transaction": {
            "sourceOfFunds": {
                "type": IConstants.DIRECT_DEBIT_TOKEN,
                "token": merchant_token,
            }
        },
        "customer": {
            "phone": "0367573933",
            "name": "TRAN THAI DUONG",
            "account": {"id": "000000001"},
            "email": "duongtt@onepay.vn",
        },
    }
    json_content = json.dumps(body_content)
    hash_object = hashlib.sha256(json_content.encode())
    hash_pay_load = hash_object.digest()
    content_digest = "sha-256=:" + base64.b64encode(hash_pay_load).decode() + ":"
    content_type = IConstants.APPLICATION_JSON
    content_length = str(len(json_content))
    string_to_sign = Util.generate_string_to_sign(
        method,
        path,
        content_type,
        content_length,
        content_digest,
        create_time,
        expire_time,
    )
    signature = Auth.make_sign(string_to_sign)
    signature_input = Util.create_signature_input(method, create_time, expire_time)

    header_request = Util.create_header_request(
        signature_input, signature, content_type, content_length, content_digest
    )

    url_request = Config.BASE_URL + path

    print("contentDigest: " + content_digest)
    print("urlRequest: " + url_request)
    print("bodyContent: " + json_content)
    print("HeaderRequest: " + str(header_request))
    print("StringToSign: " + string_to_sign)
    print("signatureInput: " + signature_input)
    print("signature: " + signature)
    Util.call_post_request(url_request, body_content, header_request)


def retrieve_payment_information(merchant_id, merchant_txn_ref):
    method = IConstants.GET

    current_unix_time = int(time.time())
    five_minutes_later = current_unix_time + 5 * 600

    create_time = str(current_unix_time)
    expire_time = str(five_minutes_later)

    path = (
        Config.URL_PREFIX
        + IConstants.MERCHANTS
        + "/"
        + merchant_id
        + "/"
        + IConstants.PAYMENTS_DDT
        + "/"
        + merchant_txn_ref
    )

    string_to_sign = Util.generate_string_to_sign(
        method, path, "", "", "", create_time, expire_time
    )

    signature = Auth.make_sign(string_to_sign)

    signature_input = Util.create_signature_input(method, create_time, expire_time)

    url_request = Config.BASE_URL + path

    header_request = Util.create_header_request(signature_input, signature, "", "", "")

    print("urlRequest: " + url_request)
    print("HeaderRequest: " + str(header_request))
    print("StringToSign: " + string_to_sign)
    print("signatureInput: " + signature_input)
    print("signature: " + signature)
    Util.call_get_request(url_request, header_request)


def delete_token(merchant_id: str, merchant_token: str):
    method = IConstants.PUT

    current_unix_time = int(time.time())
    five_minutes_later = current_unix_time + 5 * 600

    create_time = str(current_unix_time)
    expire_time = str(five_minutes_later)

    reference = "DELETETOKEN_" + create_time

    path = (
        Config.URL_PREFIX
        + IConstants.MERCHANTS
        + "/"
        + merchant_id
        + "/"
        + IConstants.DD_TOKENS
        + "/"
        + merchant_token
        + "/"
        + IConstants.DELETIONS
        + "/"
        + reference
    )

    body_content = {
        "browserPayment": {
            "callbackUrl": "https://dev.onepay.vn/paygate/api/rest/v1/ipn",
            "returnUrl": "https://dev.onepay.vn/ldp/direct-debit/result",
        },
        "apiOperation": IConstants.DELETE_TOKEN_DIRECT_DEBIT,
        "locale": "vi",
        "device": {
            "browser": "Chrome/120.0.0.0",
            "mobilePhoneModel": "nokia 1280",
            "ipAddress": "192.168.166.146",
        },
        "customer": {
            "phone": "0367573933",
            "name": "TRAN THAI DUONG",
            "account": {"id": "000000001"},
            "email": "duongtt@onepay.vn",
        },
    }
    json_content = json.dumps(body_content)
    hash_object = hashlib.sha256(json_content.encode())
    hash_pay_load = hash_object.digest()
    content_digest = "sha-256=:" + base64.b64encode(hash_pay_load).decode() + ":"
    content_type = IConstants.APPLICATION_JSON
    content_length = str(len(json_content))
    string_to_sign = Util.generate_string_to_sign(
        method,
        path,
        content_type,
        content_length,
        content_digest,
        create_time,
        expire_time,
    )
    signature = Auth.make_sign(string_to_sign)
    signature_input = Util.create_signature_input(method, create_time, expire_time)

    header_request = Util.create_header_request(
        signature_input, signature, content_type, content_length, content_digest
    )

    url_request = Config.BASE_URL + path

    print("contentDigest: " + content_digest)
    print("urlRequest: " + url_request)
    print("bodyContent: " + json_content)
    print("HeaderRequest: " + str(header_request))
    print("StringToSign: " + string_to_sign)
    print("signatureInput: " + signature_input)
    print("signature: " + signature)
    Util.call_put_request(url_request, body_content, header_request)


def retrieve_token_deletion_info(
    merchant_id: str, merchant_token: str, merchant_del_ref: str
):
    method = IConstants.GET

    current_unix_time = int(time.time())
    five_minutes_later = current_unix_time + 5 * 600

    create_time = str(current_unix_time)
    expire_time = str(five_minutes_later)

    path = (
        Config.URL_PREFIX
        + IConstants.MERCHANTS
        + "/"
        + merchant_id
        + "/"
        + IConstants.DD_TOKENS
        + "/"
        + merchant_token
        + "/"
        + IConstants.DELETIONS
        + "/"
        + merchant_del_ref
    )

    string_to_sign = Util.generate_string_to_sign(
        method, path, "", "", "", create_time, expire_time
    )

    signature = Auth.make_sign(string_to_sign)

    signature_input = Util.create_signature_input(method, create_time, expire_time)

    url_request = Config.BASE_URL + path

    header_request = Util.create_header_request(signature_input, signature, "", "", "")

    print("urlRequest: " + url_request)
    print("HeaderRequest: " + str(header_request))
    print("StringToSign: " + string_to_sign)
    print("signatureInput: " + signature_input)
    print("signature: " + signature)
    Util.call_get_request(url_request, header_request)
