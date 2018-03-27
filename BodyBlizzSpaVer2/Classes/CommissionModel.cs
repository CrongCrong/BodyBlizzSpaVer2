using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyBlizzSpaVer2.Classes
{
    public class CommissionModel
    {
        private string ID;

        public string ID1
        {
            get { return ID; }
            set { ID = value; }
        }

        private ServiceTypeModel serviceTypeModel;

        public ServiceTypeModel ServiceTypeModel
        {
            get { return serviceTypeModel; }
            set { serviceTypeModel = value; }
        }

        private string serviceTypeID;

        public string ServiceTypeID
        {
            get { return serviceTypeID; }
            set { serviceTypeID = value; }
        }

        private string commission;

        public string Commission
        {
            get { return commission; }
            set { commission = value; }
        }

        private bool ifEditDetails;

        public bool IfEditDetails
        {
            get { return ifEditDetails; }
            set { ifEditDetails = value; }
        }
    }
}
