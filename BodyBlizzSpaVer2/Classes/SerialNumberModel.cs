using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyBlizzSpaVer2.Classes
{
    public class SerialNumberModel
    {

        private string ID;

        public string ID1
        {
            get { return ID; }
            set { ID = value; }
        }

        private string serialNumber;

        public string SerialNumber
        {
            get { return serialNumber; }
            set { serialNumber = value; }
        }

        private DateTime dateServiced;

        public DateTime DateServiced
        {
            get { return dateServiced; }
            set { dateServiced = value; }
        }
    }
}
