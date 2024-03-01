import base64
import hashlib
import json

json_content = '{"browserPayment":{"callbackUrl":"https://dev.onepay.vn/paygate/api/rest/v1/ipn","returnUrl":"https://dev.onepay.vn/ldp/direct-debit/result"},"apiOperation":"TOKENIZE_DIRECT_DEBIT","sourceOfFunds":{"types":["DD_SGTTVNVX","DD_BIDVVNVX"]},"locale":"vi","device":{"browser":"User Agent","mobilePhoneModel":"iphone","ipAddress":"192.168.1.999"},"customer":{"phone":"0367573933","name":"TRAN THAI DUONG","account":{"id":"000000001"},"email":"duongtt@onepay.vn"}}'

json_object = {
    "browserPayment": {
        "callbackUrl": "https://dev.onepay.vn/paygate/api/rest/v1/ipn",
        "returnUrl": "https://dev.onepay.vn/ldp/direct-debit/result",
    },
    "apiOperation": "TOKENIZE_DIRECT_DEBIT",
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

json_str = json.dumps(json_object)
hash_object = hashlib.sha256(json_str.encode())
hash_payload = hash_object.digest()
content_digest = "sha-256=:" + base64.b64encode(hash_payload).decode() + ":"
print(content_digest)
