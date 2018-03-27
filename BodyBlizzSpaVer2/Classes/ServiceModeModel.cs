using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyBlizzSpaVer2.Classes
{
    public class ServiceModeModel
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

        private bool ifEditDetails;

        public bool IfEditDetails
        {
            get { return ifEditDetails; }
            set { ifEditDetails = value; }
        }
    }
}
