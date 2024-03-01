import requests
import hashlib
from IConstants import IConstants
from Config import Config


def get_bytes_utf_8(data):
    return data.encode("utf-8")


def call_put_request(url, content, header):
    response = requests.put(url, json=content, headers=header)
    print(response.status_code)
    print(response.text)


def call_post_request(url, content, header):
    response = requests.post(url, json=content, headers=header)
    print(response.status_code)
    print(response.text)


def call_get_request(url, header):
    response = requests.get(url, headers=header)
    print(response.status_code)
    print(response.text)


def sha256_hash(data):
    sha256_hash_code = hashlib.sha256()
    sha256_hash_code.update(data)
    return sha256_hash_code.digest()


def create_signature_input(method: str, create_time: str, expire_time: str):
    result = ""
    if method == IConstants.GET:
        result = (
            """sig=("@method" "@path")"""
            + ";"
            + "created="
            + create_time
            + ";"
            + "expires="
            + expire_time
            + ";"
            + "keyid="
            + '"'
            + Config.MERCHANT_ID
            + '"'
            + ";"
            + "alg="
            + '"'
            + Config.ALG
            + '"'
        )
    elif method == IConstants.PUT or method == IConstants.POST:
        result = (
            """sig=("@method" "@path" "content-digest" "content-type" "content-length")"""
            + ";"
            + "created="
            + create_time
            + ";"
            + "expires="
            + expire_time
            + ";"
            + "keyid="
            + '"'
            + Config.MERCHANT_ID
            + '"'
            + ";"
            + "alg="
            + '"'
            + Config.ALG
            + '"'
        )
    return result


def generate_string_to_sign(
    method: str,
    path: str,
    content_type: str,
    content_length: str,
    content_digest: str,
    create_time: str,
    expire_time: str,
):
    result = ""
    if content_type != "" and content_length != "" and content_digest != "":
        sign_method = '"@method": ' + method
        sign_path = '"@path": ' + path
        sign_content_digest = '"content-digest": ' + content_digest
        sign_content_type = '"content-type": ' + content_type
        sign_content_length = '"content-length": ' + content_length
        sign_param = (
            '"@signature-params": ("@method" "@path" "content-digest" "content-type" "content-length")'
            + ";"
            + "created="
            + create_time
            + ";"
            + "expires="
            + expire_time
            + ";"
            + "keyid="
            + '"'
            + Config.MERCHANT_ID
            + '"'
            + ";"
            + "alg="
            + '"'
            + Config.ALG
            + '"'
        )
        result = (
            sign_method
            + "\n"
            + sign_path
            + "\n"
            + sign_content_digest
            + "\n"
            + sign_content_type
            + "\n"
            + sign_content_length
            + "\n"
            + sign_param
        )
    else:
        sign_method = '"@method": ' + method
        sign_path = '"@path": ' + path
        sign_param = (
            '"@signature-params": ("@method" "@path")'
            + ";"
            + "created="
            + create_time
            + ";"
            + "expires="
            + expire_time
            + ";"
            + "keyid="
            + '"'
            + Config.MERCHANT_ID
            + '"'
            + ";"
            + "alg="
            + '"'
            + Config.ALG
            + '"'
        )
        result = sign_method + "\n" + sign_path + "\n" + sign_param
    return result


def create_header_request(
    signature_input: str,
    signature: str,
    content_type: str,
    content_length: str,
    content_digest: str,
):
    header: dict
    if content_type != "" and content_length != "" and content_digest != "":
        header = {
            IConstants.SIGNATURE_INPUT: signature_input,
            IConstants.SIGNATURE: "sig=:" + signature + ":",
            IConstants.CONTENT_TYPE: content_type,
            IConstants.CONTENT_LENGTH: content_length,
            IConstants.CONTENT_DIGEST: content_digest,
        }
    else:
        header = {
            IConstants.SIGNATURE_INPUT: signature_input,
            IConstants.SIGNATURE: signature,
        }
    return header
