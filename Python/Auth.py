import base64
import ed25519

# Chuỗi chứa khóa riêng tư
private_key_base64 = "yq6zIGeQK99dfX/pm8nLeq1e1tKmdfVTUmvjdtJKQ/VhVOzoGix3EwvAiqzPl7eCTb0vEyJQlzorLSaLOF4oDg=="


def make_sign(string_to_sign: str):

    # Khóa private từ chuỗi base64
    private_key = ed25519.SigningKey(base64.b64decode(private_key_base64))

    # Ký chuỗi
    signature = private_key.sign(string_to_sign.encode())

    # Encode chữ ký dưới dạng base64
    signature_base64 = base64.b64encode(signature)

    # Trả về chữ ký dưới dạng base64
    print("Chữ ký (base64):", signature_base64.decode())

    return signature_base64.decode()
