using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyBlizzSpaVer2.Classes
{
    public class ServiceTypeModel
    {
        private string ID;

        public string ID1
        {
            get { return ID; }
            set { ID = value; }
        }
        private string serviceType;

        public string ServiceType
        {
            get { return serviceType; }
            set { serviceType = value; }
        }
        private string price;

        public string Price
        {
            get { return price; }
            set { price = value; }
        }

        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string CommissionID { get; set; }

        public string Commission { get; set; }

        public bool ifPromo { get; set; }

        public override string ToString()
        {
            return serviceType;
        }
    }
}
