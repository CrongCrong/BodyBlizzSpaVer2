using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyBlizzSpaVer2.Classes
{
    public class LoyaltyCardModel
    {
        public string ID { get; set; }

        public string SerialNumber { get; set; }

        public string ClientID { get; set; }

        public int ServiceCount { get; set; }

        public string FirstFree { get; set; }

        public string SecondFree { get; set; }

        public string ClientName { get; set; }
    }
}
