using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyBlizzSpaVer2.Classes
{
    public class TherapistModel
    {
        private string ID;

        public string ID1
        {
            get { return ID; }
            set { ID = value; }
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
        private string wage;

        public string Wage
        {
            get { return wage; }
            set { wage = value; }
        }

        private string description;

        public string Description
        {
            get { return firstName + " " + lastName; }
            set { description = value; }
        }

        private bool ifEditDetails;

        public bool IfEditDetails
        {
            get { return ifEditDetails; }
            set { ifEditDetails = value; }
        }

        private string[] services;

        public string[] Services
        {
            get { return services; }
            set { services = value; }
        }

        private string[] amount;

        public string[] Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        private string[] penalties;

        public string[] Penalties
        {
            get { return penalties; }
            set { penalties = value; }
        }

        private string numberOfDays;

        public string NumberOfDays
        {
            get { return numberOfDays; }
            set { numberOfDays = value; }
        }

        private string totalWage;

        public string TotalWage
        {
            get { return totalWage; }
            set { totalWage = value; }
        }

        private string totalCommission;

        public string TotalCommission
        {
            get { return totalCommission; }
            set { totalCommission = value; }
        }

        private DateTime attendanceDate;

        public DateTime AttendanceDate
        {
            get { return attendanceDate; }
            set { attendanceDate = value; }
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

        private string minutesLate;

        public string MinutesLate
        {
            get { return minutesLate; }
            set { minutesLate = value; }
        }

        private int isLate;

        public int IsLate
        {
            get { return isLate; }
            set { isLate = value; }
        }

        private string deduction;

        public string Deduction
        {
            get { return deduction; }
            set { deduction = value; }
        }

        private int ifhalfday;

        public int Ifhalfday
        {
            get { return ifhalfday; }
            set { ifhalfday = value; }
        }

        public int ifUnderTime { get; set; }

        public string lateDeduction { get; set; }

        public string undertimeDeduction { get; set; }

        public string CashAdvance { get; set; }

        public string Loan { get; set; }

        public override string ToString()
        {
            return Description;
        }
    }
}
