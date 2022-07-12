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
    class MainStub
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
            RSAFileHelper rsaHelper = new RSAFileHelper();

            //Console.WriteLine(">>>>>>>>>>>>>>>>>");
            //Console.WriteLine(rsaHelper.GetPriKeyStr());
            //Console.WriteLine(rsaHelper.GetPubKeyStr());
            rsaHelper.WriteKeysToFiles();
            //aHelper.
            
        }
    }

}
