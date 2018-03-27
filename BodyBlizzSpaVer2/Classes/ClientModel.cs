using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyBlizzSpaVer2.Classes
{
    public class ClientModel
    {
        private string ID;

        public string ID1
        {
            get { return ID; }
            set { ID = value; }
        }
        private string dateServiced;

        public string DateServiced
        {
            get { return dateServiced; }
            set { dateServiced = value; }
        }
        private string serialNumber;

        public string SerialNumber
        {
            get { return serialNumber; }
            set { serialNumber = value; }
        }
        private string cod;

        public string Cod
        {
            get { return cod; }
            set { cod = value; }
        }
        private string firstName;

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }
        private string lastName;

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }
        private string address;

        public string Address
        {
            get { return address; }
            set { address = value; }
        }
        private string serviceMode;

        public string ServiceMode
        {
            get { return serviceMode; }
            set { serviceMode = value; }
        }
        private List<ServiceTypeModel> lstServiceTypeModel = new List<ServiceTypeModel>();

        internal List<ServiceTypeModel> LstServiceTypeModel
        {
            get { return lstServiceTypeModel; }
            set { lstServiceTypeModel = value; }
        }
        private string totalAmt;

        public string TotalAmt
        {
            get { return totalAmt; }
            set { totalAmt = value; }
        }
        private string timeIn;

        public string TimeIn
        {
            get { return timeIn; }
            set { timeIn = value; }
        }
        private string timeOut;

        public string TimeOut
        {
            get { return timeOut; }
            set { timeOut = value; }
        }

        private List<TherapistModel> lstTherapistModel = new List<TherapistModel>();

        internal List<TherapistModel> LstTherapistModel
        {
            get { return lstTherapistModel; }
            set { lstTherapistModel = value; }
        }

        private bool ifViewDetails;

        public bool IfViewDetails
        {
            get { return ifViewDetails; }
            set { ifViewDetails = value; }
        }

        private bool ifEditDetails;

        public bool IfEditDetails
        {
            get { return ifEditDetails; }
            set { ifEditDetails = value; }
        }

        private bool ifPrintWaiver;

        public bool IfPrintWaiver
        {
            get { return ifPrintWaiver; }
            set { ifPrintWaiver = value; }
        }

        public List<ProductBoughtModel> lstProductsBought { get; set; }

        public string isLoyal { get; set; }

        public string LoyaltyID { get; set; }

        public string ServiceCount { get; set; }

        public string FirstFree { get; set; }

        public string SecondFree { get; set; }

        public string LoyaltyPrimaryKeyID { get; set; }

        public string PhoneNumber { get; set; }

    }
}
