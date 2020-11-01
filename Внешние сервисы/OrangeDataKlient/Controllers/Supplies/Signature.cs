using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OrangeDataKlient.Controllers.Supplies
{
    public class Signature
    {
        public string ComputeSignature(string document)
        {
            var data = Encoding.UTF8.GetBytes(document);

            using (var rsa = RSA.Create())
            {
                rsa.FromXmlString(File.ReadAllText("rsa_2048_private_key.xml"));
                return Convert.ToBase64String(rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
            }
        }

    }
}
