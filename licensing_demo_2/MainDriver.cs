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
            //TODO move to plugin when ready
            Console.WriteLine("Start!");
            //current directory?
            Console.WriteLine(System.AppDomain.CurrentDomain.BaseDirectory);
            //motherboeard id?
            Console.WriteLine(getMotherBoardID());
            //date?
            DateTime now = DateTime.Now;
            Console.WriteLine(now);
            //1.pont
            KeyHandler rsaHelper = new KeyHandler("");

            Console.WriteLine(">>>>>>>>>>>>>>>>>");
            Console.WriteLine(rsaHelper.GetPriKeyStr());
            Console.WriteLine(">>>>>>>>>>>>>>>>>");
            Console.WriteLine(rsaHelper.GetPubKeyStr());
            Console.WriteLine(">>>>>>>>>>>>>>>>>");

            rsaHelper.WritePubKeyToFile();
            rsaHelper.WritePriKeyToFile();

            LicenseHandler newLicense = new LicenseHandler();

            Console.WriteLine(newLicense);

            string id = newLicense.getID();

            string signature = rsaHelper.SignData(id);

            newLicense.addSign(signature);

            newLicense.WriteLicenseToFile();
            Console.WriteLine(">>>>>>>>>>>>>>>>>");

            LicenseHandler newLicense2 = LicenseHandler.ReadLicenseFromFile();

            string idRead = newLicense2.getID();
            string signRead = newLicense2.getSignature();

            KeyHandler rsaHelper2 = new KeyHandler("", false);

            bool verificationResult = rsaHelper2.VerifyData(idRead, signRead);
            Console.WriteLine(String.Format("Verification success: {0}", verificationResult));
            //aHelper.


            Controller ct = new Controller();
            Console.WriteLine("Control check:");
            Console.WriteLine(ct.Controll(""));

        }
    }

}
