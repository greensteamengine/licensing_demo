using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.IO;
using System.Threading;

namespace licensing_demo
{
    /*
     * This class implements the needed functionailty in the plugin.
     * Some files (license, open key) are used here and in the driver too, since
     * in the real scenario they will have to be sent manually (or in an automatized way) between the user and the license provider.
     */
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
            Console.WriteLine("Read from: " + System.AppDomain.CurrentDomain.BaseDirectory);
            string licensePath = path + "license.txt";
            bool exists = File.Exists(licensePath);
            if (!exists)
            {
                Console.WriteLine(String.Format("License file not found on path: {0}", path));
                return false;
            }

            try
            {
                string licenseString = File.ReadAllText(licensePath);
                LicenseHandler lh = new LicenseHandler(licenseString);
                if (!lh.hasSignature())
                {
                    Console.WriteLine("License file missing signature!");
                    return false;
                }

                this.licenseHandler = lh;
                Console.WriteLine("License file read!");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading license file! Exception: " + e);
                return false;
            }

            //return false;
        }


        
        private bool CheckLicense(KeyHandler keyHandler)
        {
            string idAndDate = this.licenseHandler.getIdAndDate();
            string signature = this.licenseHandler.getSignature();
            bool verificationResult = keyHandler.VerifyData(idAndDate, signature);
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

            DateTime? prevTime = GetDateFromRegistry();
            DateTime prevLaunch;

            int timeComparsion = -1;
            DateTime now = DateTime.Now;
            if (prevTime.HasValue)
            {
                prevLaunch = prevTime.Value;
                
                timeComparsion = DateTime.Compare(prevLaunch, dt);
                if (timeComparsion >= 0)
                {
                    Console.WriteLine("Wrong date time in license!");
                    return false;
                }
            }
            else
            {
                
                Console.WriteLine("Time of previous lunch not in the register, First launch!");
            }

            //>0 − If date1 is later than date2
            //https://www.tutorialspoint.com/datetime-compare-method-in-chash
            
            

            //Check if current lounch time is larger than the license time!
            timeComparsion = DateTime.Compare(dt, now);
            if (timeComparsion >= 0)
            {
                Console.WriteLine("Error! License date time larger than system date time!");
                return false;
            }

            //License expiration ?
            //Add new date or this comparsion is enough?
            return true;
        }

        public DateTime? GetDateFromRegistry()
        {
            string dateString = "";

            //var hklm = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64);
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("License Date");


            
            Object regDateObj = key.GetValue("Date");
            if (regDateObj != null)
            {
                dateString = (string)regDateObj;// new Version(o as String);  //"as" because it's REG_SZ...otherwise ToString() might be safe(r)
                                                //do what you like with version
            }
            else
            {
                Console.WriteLine("No date in registry, getting sytem time!");
                return null;
            }


            DateTime regDate = DateTime.Parse(dateString);
            return regDate;
        }

        //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-create-a-key-in-the-registry
        public void SaveDateToRegistry(DateTime dt)
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

            //Uncomment the date saving if you want to 
            SaveDateToRegistry(now);
            //Waiting to get later time
            Thread.Sleep(2000);

            //Check if the given conditions are satisfied
            bool ok = ReadLicense(path) && CheckLicense(keyHandler) && CheckDate(now);

            if (ok)
            {
                Console.WriteLine("License ok, ready to run!");
                SaveDateToRegistry(now);
                return true;
            }
            else
            {
                LicenseHandler newLicense = new LicenseHandler();
                newLicense.WriteLicenseToFile("user_license\\");
                Console.WriteLine("License fail! Please send the newly generated license file for verification!");
                return false;
            }

        }


    }
}
