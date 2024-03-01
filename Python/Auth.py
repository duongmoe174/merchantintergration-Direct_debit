from cryptography.hazmat.primitives import serialization
from cryptography.hazmat.primitives.asymmetric import ed25519
import base64

# Chuỗi chứa khóa riêng tư
private_key_str = """
-----BEGIN PRIVATE KEY-----
MC4CAQAwBQYDK2VwBCIEIJ+DYvh6SEqVTm50DFtMDoQikTmiCqirVv9mWG9qfSnF
-----END PRIVATE KEY-----
"""
private_key = serialization.load_pem_private_key(
    private_key_str.encode(), password=None
)

# Chuyển đổi khóa riêng tư thành bytes
private_key_bytes = private_key.private_bytes(
    encoding=serialization.Encoding.Raw,
    format=serialization.PrivateFormat.Raw,
    encryption_algorithm=serialization.NoEncryption(),
)

pk_ed25519 = ed25519.Ed25519PrivateKey.from_private_bytes(private_key_bytes)
print(pk_ed25519)

# Kiểm tra thuật toán ký
if isinstance(pk_ed25519, ed25519.Ed25519PrivateKey):
    print("Đây là khóa dùng thuật toán Ed25519")
else:
    print("Đây không phải là khóa dùng thuật toán Ed25519")


def make_sign(string_to_sign: str):
    private_key = serialization.load_pem_private_key(
        private_key_str.encode(), password=None
    )

    string_to_sign_bytes = string_to_sign.encode("utf-8")
    signature_bytes = private_key.sign(string_to_sign_bytes)
    signature_base64 = base64.b64encode(signature_bytes).decode("utf-8")
    print(signature_base64)
    return signature_base64
