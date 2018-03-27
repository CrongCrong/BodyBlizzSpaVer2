using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyBlizzSpaVer2.Classes
{
    class Deductions
    {
        private int lateFrom = Properties.Settings.Default.lateFrom;

        public int LateFrom
        {
            get { return lateFrom; }
            set { lateFrom = value; }
        }

        private int lateTo = Properties.Settings.Default.lateTo;

        public int LateTo
        {
            get { return lateTo; }
            set { lateTo = value; }
        }

        private int lateFrom2 = Properties.Settings.Default.lateFrom2;

        public int LateFrom2
        {
            get { return lateFrom2; }
            set { lateFrom2 = value; }
        }

        private int lateTo2 = Properties.Settings.Default.lateTo2;

        public int LateTo2
        {
            get { return lateTo2; }
            set { lateTo2 = value; }
        }

        private int lateFrom3 = Properties.Settings.Default.lateFrom3;

        public int LateFrom3
        {
            get { return lateFrom3; }
            set { lateFrom3 = value; }
        }

        private int lateTo3 = Properties.Settings.Default.lateTo3;

        public int LateTo3
        {
            get { return lateTo3; }
            set { lateTo3 = value; }
        }

        private int lateFrom4 = Properties.Settings.Default.lateFrom4;

        public int LateFrom4
        {
            get { return lateFrom4; }
            set { lateFrom4 = value; }
        }

        private int lateTo4 = Properties.Settings.Default.lateTo4;

        public int LateTo4
        {
            get { return lateTo4; }
            set { lateTo4 = value; }
        }

        private int deduction = Properties.Settings.Default.deduction;

        public int Deduction
        {
            get { return deduction; }
            set { deduction = value; }
        }

        private int deduction2 = Properties.Settings.Default.deduction2;

        public int Deduction2
        {
            get { return deduction2; }
            set { deduction2 = value; }
        }

        private int deduction3 = Properties.Settings.Default.deduction3;

        public int Deduction3
        {
            get { return deduction3; }
            set { deduction3 = value; }
        }

        private int deduction4 = Properties.Settings.Default.deduction4;

        public int Deduction4
        {
            get { return deduction4; }
            set { deduction4 = value; }
        }

    }
}
