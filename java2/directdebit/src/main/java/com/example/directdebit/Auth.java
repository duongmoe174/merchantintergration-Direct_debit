package com.example.directdebit;

import java.security.KeyFactory;
import java.security.PrivateKey;
import java.security.Security;
import java.security.Signature;
import java.security.spec.PKCS8EncodedKeySpec;
import java.util.logging.Logger;
import org.bouncycastle.jce.provider.BouncyCastleProvider;
import org.bouncycastle.util.encoders.Base64;

public class Auth {
  public static final Logger logger = Logger.getLogger(Auth.class.getName());

  public static final String testKeyEd25519 = "-----BEGIN PRIVATE KEY-----\n" +
      "MC4CAQAwBQYDK2VwBCIEIJ+DYvh6SEqVTm50DFtMDoQikTmiCqirVv9mWG9qfSnF\n" +
      "-----END PRIVATE KEY-----";
  public static final String testKeyEd25519Pub = "-----BEGIN PRIVATE KEY-----\n" +
      "MCowBQYDK2VwAyEAJrQLj5P/89iXES9+vFgrIy29clF9CC/oPPsw3c5D0bs=\n" +
      "-----END PRIVATE KEY-----";

  public static String makeSign(String stringToSign) {
    try {
      Security.addProvider(new BouncyCastleProvider());
      byte[] privateKeyBytes = Base64.decode(testKeyEd25519.replace("-----BEGIN PRIVATE KEY-----", "")
          .replace("-----END PRIVATE KEY-----", "")
          .replace("\n", ""));

      PKCS8EncodedKeySpec privateKeySpec = new PKCS8EncodedKeySpec(privateKeyBytes);
      KeyFactory keyFactory = KeyFactory.getInstance("Ed25519", "BC");
      PrivateKey privateKey = keyFactory.generatePrivate(privateKeySpec);

      Signature signature = Signature.getInstance("Ed25519", "BC");
      signature.initSign(privateKey);
      signature.update(stringToSign.getBytes());

      byte[] signatureBytes = signature.sign();
      return Base64.toBase64String(signatureBytes);

    } catch (Exception e) {
      logger.info(e.getMessage());
      return null;
    }
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
