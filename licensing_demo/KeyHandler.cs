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
    class KeyHandler
    {
        readonly string path;// =;//change as needed
        //readonly string priKeyPath;// = "private.key";//change as needed

        //private string pubKeyStr;
        //private string priKeyStr;

        //private RSAParameters pubKey;
        //private RSAParameters priKey;

        RSACryptoServiceProvider csp;

        public KeyHandler(string path = "")
        {
            //"public.key", string priPath = "private.key"
            this.path = path;
            //this.priKeyPath = priPath;

            //MakeKey();
            this.csp = new RSACryptoServiceProvider(2048);

        }

        public KeyHandler(string path = "", bool readPrivate = false)
        {
            //"public.key", string priPath = "private.key"
            this.path = path;
            //this.priKeyPath = priPath;

            //MakeKey();
            if (readPrivate)
            {
                this.csp = ReadParameters(path + "private.key");
            }
            else
            {
                this.csp = ReadParameters(path + "public.key");
            }
            
            //csp.ImportParameters(ReadParameters(path));

        }



        private void MakeKey()
        {
            //lets take a new CSP with a new 2048 bit rsa key pair
            this.csp = new RSACryptoServiceProvider(2048);

            //how to get the private key
            //RSAParameters privKey = csp.ExportParameters(true);

            //and the public key ...
            //RSAParameters pubKey = csp.ExportParameters(false);
            


            
        }

        public string GetPubKeyStr()
        {
            //converting the public key into a string representation
            //string pubKeyString;

            //we need some buffer
            var sw = new StringWriter();
            //we need a serializer
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            //serialize the key into the stream
            xs.Serialize(sw, csp.ExportParameters(false));
            //get the string from the stream
            string pubKeyStr = sw.ToString();
            //File.WriteAllText(pubKeyPath, pubKeyStr);

            return pubKeyStr;
        }
        public string GetPriKeyStr()
        {
            //we need some buffer
            var sw = new StringWriter();
            //we need a serializer
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            //serialize the key into the stream
            xs.Serialize(sw, csp.ExportParameters(true));
            //get the string from the stream
            string priKeyStr = sw.ToString();
            //File.WriteAllText(priKeyPath, priKeyStr);

            return priKeyStr;
        }

        public void WritePubKeyToFile()
        {
           
            File.WriteAllText(path + "public.key", GetPubKeyStr());
            //File.WriteAllText(priKeyPath, GetPriKeyStr());
        }

        public void WritePriKeyToFile()
        {
            File.WriteAllText(path + "private.key", GetPriKeyStr());
            //File.WriteAllText(priKeyPath, GetPriKeyStr());
        }
        public static RSACryptoServiceProvider ReadParameters(string path)
        {

            string keyStr = File.ReadAllText(path);
            //string priKeyStr = File.ReadAllText(priKeyPath);

            var sr = new StringReader(keyStr);

            //we need a deserializer
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));

            //get the object back from the stream
            RSACryptoServiceProvider localCsp = new RSACryptoServiceProvider(2048);
            localCsp.ImportParameters((RSAParameters)xs.Deserialize(sr));

            return localCsp;
            //RSAParameters params = (RSAParameters)xs.Deserialize(sr);
            //RSACryptoServiceProvider csp = localCsp;

            /*
            localCsp.ImportParameters (params);

            //this.pubKey = csp.ExportParameters(false);
            this.pubKey = pubKey;


            //get a stream from the string
            var sr2 = new StringReader(priKeyStr);
            //we need a deserializer
            var xs2 = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            //get the object back from the stream
            RSAParameters privKey = (RSAParameters)xs2.Deserialize(sr2);
            //csp.ImportParameters(privKey);

            //this.priKey = csp.ExportParameters(true);
            this.priKey = privKey;
            */



        }

        //https://stackoverflow.com/questions/8437288/signing-and-verifying-signatures-with-rsa-c-sharp
        public string SignData(string message)
        {
            //// The array to store the signed message in bytes
            byte[] signedBytes;
            //using (var rsa = new RSACryptoServiceProvider())
            //{
                //// Write the message to a byte array using UTF8 as the encoding.
                var encoder = new UTF8Encoding();
                byte[] originalData = encoder.GetBytes(message);
                try
                {
                    //// Import the private key used for signing the message
                    //rsa.ImportParameters(this.priKey);
                    //rsa.ImportParameters(this.pubKey);

                    //// Sign the data, using SHA512 as the hashing algorithm 
                    signedBytes = this.csp.SignData(originalData, CryptoConfig.MapNameToOID("SHA512"));
                    //signedBytes = rsa.SignData(originalData, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);

                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.Message);
                    return null;

                }
                finally
                {
                    //// Set the keycontainer to be cleared when rsa is garbage collected.
                    //rsa.PersistKeyInCsp = false;
                }
            //}
            //// Convert the a base64 string before returning
            return Convert.ToBase64String(signedBytes);
        }

        public bool VerifyData(string originalMessage, string signedMessage)
        {
            bool success = false;
            //using (var rsa = new RSACryptoServiceProvider())
            //{
                var encoder = new UTF8Encoding();
                byte[] bytesToVerify = encoder.GetBytes(originalMessage);

                byte[] signedBytes = Convert.FromBase64String(signedMessage);
                try
                {
                    //rsa.ImportParameters(publicKey);

                    SHA512Managed Hash = new SHA512Managed();

                    byte[] hashedData = Hash.ComputeHash(signedBytes);

                    success = this.csp.VerifyData(bytesToVerify, CryptoConfig.MapNameToOID("SHA512"), signedBytes);
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.Message);
                }
                //finally
                //{
                //    rsa.PersistKeyInCsp = false;
                //}
            //}
            return success;
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
            //var sr = new StringReader(this.pubKeyStr);

            //we need a deserializer
            //var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));

            //get the object back from the stream
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider(2048);
            //csp.ImportParameters((RSAParameters)xs.Deserialize(sr));
            //csp.ImportParameters(this.pubKey);
            byte[] bytesPlainTextData = File.ReadAllBytes(filePath);

            //apply pkcs#1.5 padding and encrypt our data 
            var bytesCipherText = this.csp.Encrypt(bytesPlainTextData, false);
            //we might want a string representation of our cypher text... base64 will do
            string encryptedText = Convert.ToBase64String(bytesCipherText);
            File.WriteAllText(filePath, encryptedText);
        }
        public void DecryptFile(string filePath)
        {
            //we want to decrypt, therefore we need a csp and load our private key
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider(2048);

            /*
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
            */
            //csp.ImportParameters(this.priKey);
            string encryptedText;
            using (StreamReader reader = new StreamReader(filePath)) { encryptedText = reader.ReadToEnd(); }
            byte[] bytesCipherText = Convert.FromBase64String(encryptedText);

            //decrypt and strip pkcs#1.5 padding
            byte[] bytesPlainTextData = this.csp.Decrypt(bytesCipherText, false);

            //get our original plainText back...
            File.WriteAllBytes(filePath, bytesPlainTextData);
        }

    }
}
