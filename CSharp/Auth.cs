/* dotnet add package BouncyCastle.NetCore --version 2.2.1 */
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Utilities.Encoders;

namespace AuthDD
{
    public class Auth
    {
        public static readonly string MERCHANT_SIGNING_KEY = "yq6zIGeQK99dfX/pm8nLeq1e1tKmdfVTUmvjdtJKQ/VhVOzoGix3EwvAiqzPl7eCTb0vEyJQlzorLSaLOF4oDg==";
        public static readonly string MERCHANT_VERIFY_KEY = "eXsQS+RSrL97uMSKWS2BXEtZEeFAVMehteU/A6KyAIM=";

        public static string MakeSign(string stringToSign)
        {
            byte[] keyBytes = Convert.FromBase64String(MERCHANT_SIGNING_KEY);
            Ed25519PrivateKeyParameters privateKeyParameters = new Ed25519PrivateKeyParameters(keyBytes, 0);

            ISigner signer = SignerUtilities.GetSigner("Ed25519");
            signer.Init(true, privateKeyParameters);

            byte[] stringToSignBytes = System.Text.Encoding.UTF8.GetBytes(stringToSign);

            signer.BlockUpdate(stringToSignBytes, 0, stringToSignBytes.Length);
            byte[] signatureBytes = signer.GenerateSignature();

            string signatureBase64 = Convert.ToBase64String(signatureBytes);
            return signatureBase64;
        }

        public static bool VerifySign(string stringToSign, string signatureReq)
        {
            byte[] keyBytes = Convert.FromBase64String(MERCHANT_VERIFY_KEY);
            Ed25519PublicKeyParameters publicKeyParameters = new Ed25519PublicKeyParameters(keyBytes, 0);

            // Create the Ed25519 signer and initialize it with the public key
            ISigner verifier = SignerUtilities.GetSigner("Ed25519");
            verifier.Init(false, publicKeyParameters);

            // Update the signer with the message
            byte[] stringToSignBytes = System.Text.Encoding.UTF8.GetBytes(stringToSign);
            verifier.BlockUpdate(stringToSignBytes, 0, stringToSignBytes.Length);

            // Verify the signature
            bool isVerified = verifier.VerifySignature(Convert.FromBase64String(signatureReq));

            return isVerified;
        }
    }
}
