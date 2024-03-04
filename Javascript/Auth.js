const crypto = require("crypto");

// Đọc khóa từ chuỗi PEM
const privateKeyPEM = `-----BEGIN PRIVATE KEY-----
MC4CAQAwBQYDK2VwBCIEIJ+DYvh6SEqVTm50DFtMDoQikTmiCqirVv9mWG9qfSnF
-----END PRIVATE KEY-----`;

function makeSign(stringToSign) {
  // Chuyển đổi khóa PEM thành buffer
  const privateKeyBuffer = Buffer.from(privateKeyPEM, "utf-8");

  // Tạo đối tượng khóa từ khóa PEM
  const privateKeyObject = crypto.createPrivateKey({
    key: privateKeyBuffer,
    format: "pem",
    type: "pkcs8",
  });

  // Ký dữ liệu
  const signature = crypto.sign(null, Buffer.from(stringToSign), {
    key: privateKeyObject,
    padding: crypto.constants.RSA_PKCS1_PSS_PADDING,
    saltLength: 32,
  });

  return signature.toString("base64");
}

// Dữ liệu cần ký
const dataToSign =
  '"@method": PUT\n' +
  '"@path": /paygate/api/rest/v1/merchants/OPTEST/dd_tokens/DUONGTTTOKEN_1707196828629206900\n' +
  '"content-digest": sha-256=:YUd7b09NPdDjyrNveLs3KHbzxlzGRlU0+R69rpl7c9Y=:\n' +
  '"content-type": application/json\n' +
  '"content-length": 450\n' +
  '"@signature-params": ("@method" "@path" "content-digest" "content-type" "content-length");created=1707196828;expires=1707197128;keyid="OPTEST";alg="ed25519"';

module.exports = {
  makeSign,
};
