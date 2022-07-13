using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Security.Cryptography;
using System.IO;
using licensing_demo;

namespace licensing_demo
{
    class MainDriver
    {
        private static Dictionary<string, IPlugin> plugins = new Dictionary<string, IPlugin>();



        private static string PluginPath = "";

        private static String getMotherBoardID()
        {
            String serial = "";
            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard");
                ManagementObjectCollection moc = mos.Get();

                foreach (ManagementObject mo in moc)
                {
                    serial = mo["SerialNumber"].ToString();
                }
                return serial;
            }
            catch (Exception)
            {
                return serial;
            }
        }


        private static void WriteString(string filePath, string content)
        {
            File.WriteAllText(filePath, content);
        }
        

        static void Main(string[] args)
        {
 
            Console.WriteLine("Start!");
            //Print current directory path
            Console.WriteLine("The current directory is: " + System.AppDomain.CurrentDomain.BaseDirectory);
            //Print motherboard ID
            Console.WriteLine(getMotherBoardID());
            DateTime now = DateTime.Now;
            //Print current DateTime
            Console.WriteLine(now);
            
            KeyHandler keyHandler = new KeyHandler("");

            Console.WriteLine("Private key: "+ keyHandler.GetPriKeyStr());
            Console.WriteLine("Public key: "+ keyHandler.GetPubKeyStr());


            keyHandler.WritePubKeyToFile();
            keyHandler.WritePriKeyToFile();

            LicenseHandler newLicense = new LicenseHandler();

            Console.WriteLine("License string representation:");
            Console.WriteLine(newLicense);

            //Signing id and date to prevent manipulation with date in the file.
            string idAndDate = newLicense.getIdAndDate();
            string signature = keyHandler.SignData(idAndDate);
            newLicense.addSign(signature);
            //Save the license to a file
            newLicense.WriteLicenseToFile();

            /*
             * Using an other key handler (only with public key) and a 
             * new license class to make sure the verification of the 
             * signature works as intended.
             */
            LicenseHandler newLicense2 = LicenseHandler.ReadLicenseFromFile();

            string idDateRead = newLicense2.getIdAndDate();
            string signRead = newLicense2.getSignature();

            KeyHandler keyHandlerPublic = new KeyHandler("", false);

            bool verificationResult = keyHandlerPublic.VerifyData(idDateRead, signRead);
            Console.WriteLine(String.Format(">>> Verification success: {0} <<<", verificationResult));



            Controller ct = new Controller();
            Console.WriteLine("Control check:");
            Console.WriteLine(ct.Controll(""));

        }
    }

}
