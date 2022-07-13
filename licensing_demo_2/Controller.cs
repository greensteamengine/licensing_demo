using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.IO;

namespace licensing_demo
{
    internal class Controller
    {
        private LicenseHandler licenseHandler;

        //https://social.msdn.microsoft.com/Forums/SqlServer/en-US/f393708f-d7e3-4aa3-a624-7e8c6662f343/how-to-get-the-serial-of-my-motherboard?forum=Vsexpressvb
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
            bool exists = File.Exists(path);
            if (!exists)
            {
                Console.WriteLine(String.Format("License file not found on path: {0}", path));
            }

            try
            {
                string licenseString = File.ReadAllText(path);
                LicenseHandler lh = new LicenseHandler(licenseString);
                if (!lh.hasSignature())
                {
                    Console.WriteLine("License file missing signature!");
                    return false;
                }

                this.licenseHandler = lh;
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Error reading license file!");
                return false;
            }

            //return false;
        }


        
        private bool CheckLicense(KeyHandler keyHandler)
        {
            string pcID = getMotherBoardID();
            string signature = this.licenseHandler.getSignature();
            bool verificationResult = keyHandler.VerifyData(pcID, signature);
            if (verificationResult)
            {
                Console.WriteLine("Signature verification successful!");
                return true;
            }
            Console.WriteLine("Signature verification failed!");
            return false;
        }

        //https://stackoverflow.com/questions/18232972/how-to-read-value-of-a-registry-key-c-sharp
        //Internet time not implemented yet!
        private bool CheckDate(DateTime dt)
        {

            string dateString = dt.ToString();

            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("License Date");
            Object prevDate = key.GetValue("Date");
            if (prevDate != null)
            {
                dateString = (string)prevDate;// new Version(o as String);  //"as" because it's REG_SZ...otherwise ToString() might be safe(r)
                //do what you like with version
            }
            else
            {
                Console.WriteLine("No date in registry, getting sytem time!");
            }

            

            DateTime prevTime = DateTime.Parse(dateString);

            DateTime now = DateTime.Now;

            int timeComparsion = DateTime.Compare(prevTime, now);

            //>0 − If date1 is later than date2
            //https://www.tutorialspoint.com/datetime-compare-method-in-chash
            if (timeComparsion >= 0)
            {
                Console.WriteLine("Wrong date time in license!");
                return false;
            }
            //License expiration ?
            //Add new date or this comparsion is enough?

            return true;
        }

        //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-create-a-key-in-the-registry
        public void SaveDate(DateTime dt)
        {
            Microsoft.Win32.RegistryKey key;
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("License Date");

            key.SetValue("Date", dt.ToString());
            key.Close();
        }
        public bool Controll(string path)
        {
            KeyHandler keyHandler = new KeyHandler("", false);
            DateTime now = DateTime.Now;

            //is present?
            bool ok = ReadLicense(path) && CheckLicense(keyHandler) && CheckDate(now);

            if (ok)
            {
                Console.WriteLine("License ok, ready to run!");
                SaveDate(now);
                return true;
            }
            else
            {
                LicenseHandler newLicense = new LicenseHandler();
                newLicense.WriteLicenseToFile();
                Console.WriteLine("License fail! Please send the newly generated license file for verification!");
                return false;
            }

        }


    }
}
