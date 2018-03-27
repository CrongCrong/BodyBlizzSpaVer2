using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyBlizzSpaVer2.Classes
{
    public class ServiceMadeModel
    {

        public string ID { get; set; }

        public string PromoID { get; set; }

        public string PromoServicesClientID { get; set; }

        public string ServiceTypeID { get; set; }

        public string ServiceType { get; set; }

        public string TherapistID { get; set; }

        public string Therapist { get; set; }

        public string Price { get; set; }

        public string Discount { get; set; }

        public string Discounted { get; set; }

        public string isDiscounted { get; set; }

        public string DateServiced { get; set; }

        public string Commission { get; set; }

        public string isSavedToCard { get; set; }

        public string FirstFree { get; set; }

        public string SecondFree { get; set; }

        public bool ifPromoService { get; set; }

        public bool ifPaid { get; set; }

    }
}
