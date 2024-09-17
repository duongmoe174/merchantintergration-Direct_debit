package com.example.directdebit;

import java.util.logging.Logger;

import org.bouncycastle.crypto.params.Ed25519PrivateKeyParameters;
import org.bouncycastle.crypto.params.Ed25519PublicKeyParameters;
import org.bouncycastle.crypto.signers.Ed25519Signer;
import org.bouncycastle.util.encoders.Base64;

public class Auth {
  public static final Logger logger = Logger.getLogger(Auth.class.getName());

  public static final String MERCHANT_SIGNING_KEY = "yq6zIGeQK99dfX/pm8nLeq1e1tKmdfVTUmvjdtJKQ/VhVOzoGix3EwvAiqzPl7eCTb0vEyJQlzorLSaLOF4oDg==";

  public static final String MERCHANT_VERIFY_KEY = "eXsQS+RSrL97uMSKWS2BXEtZEeFAVMehteU/A6KyAIM=";

  public static String makeSign(String stringToSign) {
    byte[] privateKeyBytes = Base64.decode(MERCHANT_SIGNING_KEY);
    Ed25519PrivateKeyParameters privateKeyParameters = new Ed25519PrivateKeyParameters(privateKeyBytes, 0);
    Ed25519Signer signer = new Ed25519Signer();
    signer.init(true, privateKeyParameters);
    byte[] stringToSignBytes = stringToSign.getBytes();
    signer.update(stringToSignBytes, 0, stringToSignBytes.length);
    byte[] signatureBytes = signer.generateSignature();
    return Base64.toBase64String(signatureBytes);
  }

  public static boolean verifySign(String stringToSign, String signatureIPNHeader) {
    byte[] publicKeyBytes = Base64.decode(MERCHANT_VERIFY_KEY);
    Ed25519PublicKeyParameters publicKeyParameters = new Ed25519PublicKeyParameters(publicKeyBytes, 0);
    Ed25519Signer verifier = new Ed25519Signer();
    verifier.init(false, publicKeyParameters);
    verifier.update(stringToSign.getBytes(), 0, stringToSign.length());
    String trueSignature = Util.getSignatureIPNHeaders(signatureIPNHeader);
    return verifier.verifySignature(Base64.decode(trueSignature));
  }

  public static void main(String[] args) {
    String stringToSign = "\"@method\": PUT\n" +
        "\"@path\": /paygate/api/rest/v1/merchants/OPTEST/dd_tokens/DUONGTTTOKEN_1707196828629206900\n"
        +
        "\"content-digest\": sha-256=:YUd7b09NPdDjyrNveLs3KHbzxlzGRlU0+R69rpl7c9Y=:\n" +
        "\"content-type\": application/json\n" +
        "\"content-length\": 450\n" +
        "\"@signature-params\": (\"@method\" \"@path\" \"content-digest\" \"content-type\" \"content-length\");created=1707196828;expires=1707197128;keyid=\"OPTEST\";alg=\"ed25519\"";

    String sign = makeSign(stringToSign);
    logger.info(stringToSign);
    logger.info(sign);
  }
}
