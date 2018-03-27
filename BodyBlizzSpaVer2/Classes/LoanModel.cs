using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyBlizzSpaVer2.Classes
{
    public class LoanModel
    {

        public string ID { get; set; }

        public string LoanDate { get; set; }

        public string DatePaid { get; set; }

        public string TherapistID { get; set; }

        public string Therapist { get; set; }

        public string LoanAmount { get; set; }

        public string TotalLoans { get; set; }

        public string RecordID { get; set; }

        public string LoanBalance { get; set; }

        public bool ifPaid { get; set; }

    }
}
