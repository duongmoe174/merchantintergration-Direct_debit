/* dotnet add package BouncyCastle.NetCore --version 2.2.1 */
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.OpenSsl;
using System.Text;
namespace AuthDD
{
    public class Auth
    {
        public static readonly string ed25519pkcs8 = @"-----BEGIN PRIVATE KEY-----
                        MC4CAQAwBQYDK2VwBCIEIJ+DYvh6SEqVTm50DFtMDoQikTmiCqirVv9mWG9qfSnF
                        -----END PRIVATE KEY-----";

        public static string MakeSign(string stringToSign)
        {
            PemReader pemReaderPrivate = new PemReader(new StringReader(ed25519pkcs8));
            Ed25519PrivateKeyParameters ed25519pkcs8Parameters = (Ed25519PrivateKeyParameters)pemReaderPrivate.ReadObject();

            // Sign
            byte[] dataToSign = Encoding.UTF8.GetBytes(stringToSign);
            ISigner signer = new Ed25519Signer();
            signer.Init(true, ed25519pkcs8Parameters);
            signer.BlockUpdate(dataToSign, 0, dataToSign.Length);
            byte[] signatureBytes = signer.GenerateSignature();
            string signatureStr = Convert.ToBase64String(signatureBytes);
            return signatureStr;
        }
    }
}
