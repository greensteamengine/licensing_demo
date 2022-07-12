using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace licensing_demo
{
    //https://stackoverflow.com/questions/17128038/c-sharp-rsa-encryption-decryption-with-transmission
    class RSAFileHelper
    {
        readonly string pubKeyPath;// =;//change as needed
        readonly string priKeyPath;// = "private.key";//change as needed

        private string pubKeyStr;
        private string priKeyStr;

        public RSAFileHelper(string pubPath = "public.key", string priPath = "private.key", bool generate=true)
        {
            this.pubKeyPath = pubPath;
            this.priKeyPath = priPath;
            if (generate)
            {
                MakeKey();
            }
            else
            {
                ReadKeysFromFiles();
            }
        }



        private void MakeKey()
        {
            //lets take a new CSP with a new 2048 bit rsa key pair
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider(2048);

            //how to get the private key
            RSAParameters privKey = csp.ExportParameters(true);

            //and the public key ...
            RSAParameters pubKey = csp.ExportParameters(false);
            //converting the public key into a string representation
            //string pubKeyString;
            {
                //we need some buffer
                var sw = new StringWriter();
                //we need a serializer
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                //serialize the key into the stream
                xs.Serialize(sw, pubKey);
                //get the string from the stream
                pubKeyStr = sw.ToString();
                //File.WriteAllText(pubKeyPath, pubKeyStr);
            }


            //string privKeyString;
            {
                //we need some buffer
                var sw = new StringWriter();
                //we need a serializer
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                //serialize the key into the stream
                xs.Serialize(sw, privKey);
                //get the string from the stream
                priKeyStr = sw.ToString();
                //File.WriteAllText(priKeyPath, priKeyStr);
            }
        }

        public string GetPubKeyStr()
        {
            return pubKeyStr;
        }
        public string GetPriKeyStr()
        {
            return priKeyStr;
        }

        public void WriteKeysToFiles()
        {
            File.WriteAllText(pubKeyPath, pubKeyStr);
            File.WriteAllText(priKeyPath, priKeyStr);
        }
        public void ReadKeysFromFiles()
        {
            this.pubKeyStr = File.ReadAllText(pubKeyPath);
            this.priKeyStr = File.ReadAllText(priKeyPath);
        }

        public void EncryptFile(string filePath)
        {
            //converting the public key into a string representation
            /*string pubKeyString;
            {
                using (StreamReader reader = new StreamReader(pubKeyPath)) { pubKeyString = reader.ReadToEnd(); }
            }*/
            //get a stream from the string
            //var sr = new StringReader(pubKeyString);
            var sr = new StringReader(this.pubKeyStr);

            //we need a deserializer
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));

            //get the object back from the stream
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
            csp.ImportParameters((RSAParameters)xs.Deserialize(sr));
            byte[] bytesPlainTextData = File.ReadAllBytes(filePath);

            //apply pkcs#1.5 padding and encrypt our data 
            var bytesCipherText = csp.Encrypt(bytesPlainTextData, false);
            //we might want a string representation of our cypher text... base64 will do
            string encryptedText = Convert.ToBase64String(bytesCipherText);
            File.WriteAllText(filePath, encryptedText);
        }
        public void DecryptFile(string filePath)
        {
            //we want to decrypt, therefore we need a csp and load our private key
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();

            string privKeyString;
            {
                privKeyString = File.ReadAllText(priKeyPath);
                //get a stream from the string
                var sr = new StringReader(privKeyString);
                //we need a deserializer
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                //get the object back from the stream
                RSAParameters privKey = (RSAParameters)xs.Deserialize(sr);
                csp.ImportParameters(privKey);
            }
            string encryptedText;
            using (StreamReader reader = new StreamReader(filePath)) { encryptedText = reader.ReadToEnd(); }
            byte[] bytesCipherText = Convert.FromBase64String(encryptedText);

            //decrypt and strip pkcs#1.5 padding
            byte[] bytesPlainTextData = csp.Decrypt(bytesCipherText, false);

            //get our original plainText back...
            File.WriteAllBytes(filePath, bytesPlainTextData);
        }

    }
}
