using licensing_demo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Management;
using System.Threading.Tasks;


namespace application_licensing
{
    internal class Plugin_A : IPlugin
    {
        /*
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
        */
        private void CreateLicense()
        {
            Console.WriteLine(System.AppDomain.CurrentDomain.BaseDirectory);
            //motherboeard id?
            //Console.WriteLine(getMotherBoardID());
            //date?
            DateTime now = DateTime.Now;
            Console.WriteLine(now);
        }
        public void DoProcess()
        {
            throw new NotImplementedException();
        }
    } 
}
