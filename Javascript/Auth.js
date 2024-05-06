const nacl = require("tweetnacl");

const FINAL_KEY =
  "yq6zIGeQK99dfX/pm8nLeq1e1tKmdfVTUmvjdtJKQ/VhVOzoGix3EwvAiqzPl7eCTb0vEyJQlzorLSaLOF4oDg==";

function makeSign(stringToSign) {
  const privateKeyBuffer = Buffer.from(FINAL_KEY, "base64");
  const signature = nacl.sign.detached(
    Buffer.from(stringToSign),
    privateKeyBuffer
  );
  const signatureBase64 = Buffer.from(signature).toString("base64");
  return signatureBase64;
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
