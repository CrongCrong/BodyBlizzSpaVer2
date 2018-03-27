using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyBlizzSpaVer2.Classes
{
    public class CODModel
    {
        private string ID;

        public string ID1
        {
            get { return ID; }
            set { ID = value; }
        }

        private string COD;

        public string COD1
        {
            get { return COD; }
            set { COD = value; }
        }

        private DateTime dateServiced;

        public DateTime DateServiced
        {
            get { return dateServiced; }
            set { dateServiced = value; }
        }
    }
}
