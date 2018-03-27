using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyBlizzSpaVer2.Classes
{
    public class AttendanceModel
    {
        public string ID { get; set; }

        public string Date { get; set; }

        public string TimeIn { get; set; }

        public string TimeOut { get; set; }

        public string MinutesLate { get; set; }

        public string Deduction { get; set; }

        public string Late { get; set; }

        public string HalfDay { get; set; }

        public string Undertime { get; set; }


    }
}
