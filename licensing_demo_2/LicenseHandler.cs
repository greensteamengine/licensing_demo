using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.IO;

namespace licensing_demo
{
    

    internal class LicenseHandler
    {
        private DateTime creationDate;
        private string boardID;
        private string signature;


        public LicenseHandler(DateTime creationDate, string boardId)
        {
            this.creationDate = creationDate;
            this.boardID = boardId;
            this.signature = null;

    }

        public LicenseHandler(string licenseRepr)
        {
            //init class from split
            string[] separator = { "\n", "\r" };
            string[] licenseParts = licenseRepr.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            //string[] licenseParts = licenseRepr.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);




            //foreach (string p in licenseParts)
            //{
            //    Console.WriteLine("__________________");
            //    Console.WriteLine(p);
            //}


            this.creationDate = DateTime.Parse(licenseParts[0]);
            this.boardID = licenseParts[1];

            if(licenseParts.Length == 3)
            {
                this.signature = licenseParts[2];
            }
        }

        public void addSign(string sign)
        {
            this.signature = sign;
        }

        public bool hasSignature()
        {
            return this.signature != null;
        }

        public LicenseHandler()
        {
            this.creationDate = DateTime.Now;
            this.boardID = extractMotherBoardID();
        }

        private static String extractMotherBoardID()
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

        public DateTime getCreationTime()
        {
            return this.creationDate;
        }

        public string getID()
        {
            return this.boardID;
        }

        public string getSignature()
        {
            if(this.signature == null)
            {
                throw new NullReferenceException("The  license has no signature (it equals null)!");
            }

            return this.signature;
        }

        

        public override string ToString()
        {
            string res  =  this.creationDate.ToString() + "\n" + this.boardID;

            if(this.signature != null)
            {
                res += "\n" + this.signature;
            }

            return res;
        }

        public void WriteLicenseToFile(string path = "")
        {
            File.WriteAllText(path + "license.txt", this.ToString());
            //File.WriteAllText(priKeyPath, GetPriKeyStr());
        }

        public static LicenseHandler ReadLicenseFromFile(string path = "")
        {

            string strRepr = File.ReadAllText(path + "license.txt");

            return new LicenseHandler(strRepr);
            //File.WriteAllText(priKeyPath, GetPriKeyStr());
        }

    }
}
