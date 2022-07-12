using System;
using licensing_demo;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace licensing_plugin

{
    internal class Plugin_A : IPlugin
    {
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
        private void CreateLicense()
        {
            Console.WriteLine(System.AppDomain.CurrentDomain.BaseDirectory);
            //motherboeard id?
            Console.WriteLine(getMotherBoardID());
            //date?
            DateTime now = DateTime.Now;
            Console.WriteLine(now);
        }

        private bool LicensePresent(string path)
        {
            return false;
        }
        private bool ReadLicense(string path)
        {
            return false;
        }   

        private bool DecryptLicense(string license)
        {
            return false;
        }

        /*
        private bool CheckLicense(string license)
        {
            return false;
        }
        */
        public void SaveDate()
        {

        }
        public bool CheckLicense(string path)
        {
            //is present?
            if (!ReadLicense(path)) 
            {
                return false;
            }
            if (!DecryptLicense("sss"))
            {
                return false;
            }
            if (!CheckLicense("sss"))
            {
                return false;
            }
            SaveDate();

            //read & decrypt

            //is ok? (board id, time)

            //date to registry

            return true;
        }
        public void DoProcess()
        {
            throw new NotImplementedException();
        }
    }
}
