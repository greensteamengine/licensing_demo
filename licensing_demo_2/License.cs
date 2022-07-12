using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace licensing_demo
{
    

    internal class License
    {
        private DateTime creationDate;
        private String boardId;
        private String publicKey;

        public License(DateTime creationDate, string boardId, string publicKey)
        {
            this.creationDate = creationDate;
            this.boardId = boardId;
            this.publicKey = publicKey;
        }

        public License(string licenseRepr)
        {
            //init class from split
            string[] separator = { "\n\n" };
            string[] licenseParts = licenseRepr.Split(separator, 1, StringSplitOptions.RemoveEmptyEntries);

            this.creationDate = DateTime.Parse(licenseParts[0]);
        }

        public override string ToString()
        {
            return this.creationDate.ToString()
                + "\n\n"
                + this.boardId
                + "\n\n"
                + this.publicKey;
        }

    }
}
