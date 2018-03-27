using BodyBlizzSpaVer2.Classes;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace BodyBlizzSpaVer2
{
    /// <summary>
    /// Interaction logic for AttendanceForm.xaml
    /// </summary>
    public partial class AttendanceForm : MetroWindow
    {
        public AttendanceForm()
        {
            InitializeComponent();
        }


        ConnectionDB conDB = new ConnectionDB();
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            date.Content = DateTime.Now.ToShortDateString();
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            dateTimeIn.Visibility = Visibility.Hidden;
            dateTimeOut.Visibility = Visibility.Hidden;
            dateTimeIn.Text = DateTime.Now.ToShortTimeString();
            dateTimeOut.Text = DateTime.Now.ToShortTimeString();
            fillComboTherapist(cmbTherapist);
            dateFrom.Text = DateTime.Now.ToShortDateString();
            getAttendanceRecords();

            

        }

        public void fillComboTherapist(System.Windows.Controls.ComboBox cmb)
        {

            try
            {
                string queryString = "SELECT ID, firstName, lastName, wage, description FROM dbspa.tbltherapist WHERE (isDeleted = 0)";

                MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

                while (reader.Read())
                {
                    TherapistModel tm = new TherapistModel();
                    tm.ID1 = reader["ID"].ToString();
                    tm.FirstName = reader["firstName"].ToString();
                    tm.LastName = reader["lastName"].ToString();
                    tm.Wage = reader["wage"].ToString();
                    tm.Description = tm.FirstName + " " + tm.LastName;
                    cmb.Items.Add(tm);
                }

                conDB.closeConnection();

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

        }

        private void updateLogInViaManual()
        {
            if (cmbTherapist.SelectedItem != null)
            {
                TherapistModel therapist = cmbTherapist.SelectedItem as TherapistModel;
                string recordID = "";


                try
                {

                    string strLogInTime = Properties.Settings.Default.LogInTime;
                    string strHalfday = Properties.Settings.Default.haldayTime;
                    DateTime loggedTime;
                    DateTime dateNow = DateTime.Parse(dteAttendance.Text);
                    if (chkManualIn.IsChecked == true)
                    {
                        loggedTime = DateTime.Parse(dateNow.ToShortDateString() + " " + dateTimeIn.Text);
                    }
                    else
                    {
                        loggedTime = DateTime.Parse(dateNow.ToShortDateString() + " " + time.Content);
                    }

                    DateTime logInTime = DateTime.Parse(dateNow.ToShortDateString() + " " + strLogInTime);

                    DateTime halfDay = DateTime.Parse(dateNow.ToShortDateString() + " " + strHalfday);

                    int x = loggedTime.CompareTo(logInTime);
                    int y = loggedTime.CompareTo(halfDay);

                    int deductions = 0;
                    int lateDeduction = 0;
                    int numIfLate = 0;
                    int numIfHalfDay = 0;
                    int minutes = 0;

                    //CHECK IF LOG IN TIME IS HALFDAY
                    if (y >= 0)
                    {
                        //ifHalfDay = true;
                        numIfHalfDay = 1;
                        if ((DateTime.Now.DayOfWeek == DayOfWeek.Saturday) ||
                            (DateTime.Now.DayOfWeek == DayOfWeek.Sunday))
                        {
                            //GET HALDAY DEDUCTION
                            deductions = Properties.Settings.Default.halfdayDeduction;
                        }
                        else
                        {
                            deductions = Properties.Settings.Default.halfdayDeduction;
                        }
                    }
                    else
                    {
                        //CHECK IF LOG IN TIME IS ON OR BEFORE OR AFTER SET LOG IN TIME
                        //-1 BEFORE 1PM
                        //0 - 1PM
                        //1 - after 1PM
                        if (x > 0)
                        {
                            TimeSpan span = loggedTime.Subtract(logInTime);

                            minutes = span.Minutes;

                            if (minutes < 0)
                            {
                                minutes = 0;
                            }
                            else
                            {
                                //ifLate = true;
                                numIfLate = 1;
                            }

                            lateDeduction = checkMinutesToDeduct(minutes);

                        }
                    }

                    string queryString = "";
                    List<string> parameters = new List<string>();
                    if (checkIfAlreadyLoggedIn())
                    {

                        System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("THERAPIST: " + therapist.Description + " already has a Time In! " +
                                "Update existing Time In record?", "Attendace", System.Windows.Forms.MessageBoxButtons.YesNo);

                        if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                        {
                            TherapistAttendance attM = dgvAttendance.SelectedItem as TherapistAttendance;
                            if (attM != null)
                            {
                                recordID = attM.ID;
                            }

                            queryString = "UPDATE dbspa.tblattendance SET attendanceDate = ?, timeIn = ?, minutesLate = ?, isLate =?, " +
                                " deduction = ?, ifhalfday = ?, lateDeduction = ? WHERE therapistID = ? AND dbspa.tblattendance.attendanceDate = ?";

                            DateTime date = DateTime.Parse(dteAttendance.Text);
                            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
                            parameters.Add(dateTimeIn.Text);
                            parameters.Add(minutes.ToString());
                            parameters.Add(numIfLate.ToString());
                            parameters.Add(deductions.ToString());
                            parameters.Add(numIfHalfDay.ToString());
                            parameters.Add(lateDeduction.ToString());
                            parameters.Add(therapist.ID1);
                            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

                            conDB.AddRecordToDatabase(queryString, parameters);
                            System.Windows.MessageBox.Show("Time in for " + therapist.Description + " recorded successfully!");
                            conDB.writeLogFile("TIME IN:(MANUAL) UPDATE EXISTING RECORD FOR: " + therapist.Deduction);
                        }
                        else
                        {

                        }

         
                    }
                    else
                    {
                        queryString = "INSERT INTO dbspa.tblattendance (attendanceDate, timeIn, therapistID, isDeleted, minutesLate, isLate, deduction, " +
                            "ifhalfday, lateDeduction, undertimeDeduction, ifUndertime) VALUES(?,?,?,?,?,?,?,?,?,?,?)";

                        DateTime date = DateTime.Parse(dteAttendance.Text);
                        parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

                        parameters.Add(dateTimeIn.Text);

                        parameters.Add(therapist.ID1);
                        parameters.Add(0.ToString());
                        parameters.Add(minutes.ToString());
                        parameters.Add(numIfLate.ToString());
                        parameters.Add(deductions.ToString());
                        parameters.Add(numIfHalfDay.ToString());
                        parameters.Add(lateDeduction.ToString());
                        parameters.Add(0.ToString());
                        parameters.Add(0.ToString());

                        conDB.AddRecordToDatabase(queryString, parameters);
                        System.Windows.MessageBox.Show("Time in for " + therapist.Description + " recorded successfully!");
                        conDB.writeLogFile("TIME IN:(MANUAL) SAVE NEW RECORD FOR " + therapist.Deduction);
                    }

                    

                    conDB.closeConnection();
                    
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
                finally
                {
                    getAttendanceRecords();
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Please select Therapist!");
            }
        }

        private void LogOutViaManual()
        {

        }

        private int checkMinutesToDeduct(int minutes)
        {
            Deductions deduct = new Deductions();
            int deductions = 0;

            if (minutes >= deduct.LateFrom && minutes <= deduct.LateTo)
            {
                deductions = deduct.Deduction;
            }
            else if (minutes >= deduct.LateFrom2 && minutes <= deduct.LateTo2)
            {
                deductions = deduct.Deduction2;
            }
            else if (minutes >= deduct.LateFrom3 && minutes <= deduct.LateTo3)
            {
                deductions = deduct.Deduction3;
            }
            else if (minutes >= deduct.LateFrom4 && minutes <= deduct.LateTo4)
            {
                deductions = deduct.Deduction4;
            }

            return deductions;
        }

        private bool checkIfAlreadyLoggedIn()
        {
            bool ifLogIn = false;
            int idThera = 0;
            try
            {
                if (cmbTherapist.SelectedItem != null)
                {
                    TherapistModel therapist = cmbTherapist.SelectedItem as TherapistModel;

                    string queryString = "SELECT therapistID FROM dbspa.tblattendance WHERE therapistID = ? AND attendanceDate = ? AND isDeleted = 0";
                    List<string> parameters = new List<string>();
                    parameters.Add(therapist.ID1);
                    DateTime date = DateTime.Parse(dteAttendance.Text);
                    parameters.Add(date.Year + "-" + date.Month + "-" + date.Day);
                   

                    MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

                    while (reader.Read())
                    {
                        idThera = Convert.ToInt32(reader["therapistID"].ToString());
                    }

                    conDB.closeConnection();

                    if (idThera > 0)
                    {
                        ifLogIn = true;
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Please select Therapist!");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

            return ifLogIn;
        }

        private void getAttendanceRecords()
        {

            List<TherapistAttendance> lstTherapistAttendance = new List<TherapistAttendance>();
            TherapistAttendance attendance = new TherapistAttendance();
            try
            {
                string queryString = "SELECT dbspa.tblattendance.ID, dbspa.tbltherapist.description AS 'THERAPIST', dbspa.tblattendance.attendanceDate AS 'DATE', " +
                    "dbspa.tblattendance.timeIn AS 'TIME IN', dbspa.tbltherapist.ID AS 'THERAPIST ID', dbspa.tblattendance.timeOut AS 'TIME OUT' FROM (dbspa.tblattendance INNER JOIN dbspa.tbltherapist ON " +
                    "dbspa.tblattendance.therapistID = dbspa.tbltherapist.ID) WHERE (dbspa.tblattendance.isDeleted = 0) AND (dbspa.tbltherapist.isDeleted = 0) " +
                    "AND (dbspa.tblattendance.attendanceDate = ?)";
                List<string> parameters = new List<string>();
                DateTime date = new DateTime();
                if (!string.IsNullOrEmpty(dateFrom.Text))
                {
                    date = DateTime.Parse(dateFrom.Text);
                }else
                {
                    date = DateTime.Parse(DateTime.Now.ToShortDateString());
                }
                
                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);


                MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

                while(reader.Read())
                {
                    attendance.ID = reader["ID"].ToString();
                    attendance.Therapist = reader["THERAPIST"].ToString();
                    DateTime dte = DateTime.Parse(reader["DATE"].ToString());
                    attendance.AttendanceDate = dte.ToShortDateString();
                    attendance.TimeIn = reader["TIME IN"].ToString();
                    attendance.TimeOut = reader["TIME OUT"].ToString();
                    attendance.TherapistID = reader["THERAPIST ID"].ToString();
                    lstTherapistAttendance.Add(attendance);
                    attendance = new TherapistAttendance();
                }

              
                dgvAttendance.ItemsSource = lstTherapistAttendance;
                //dgvAttendance.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#b7b739"));
                conDB.closeConnection();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

        }

        private bool checkTherapistIfTheSame()
        {
            bool ifthesame = false;

            TherapistModel therapist = cmbTherapist.SelectedItem as TherapistModel;

            TherapistAttendance attM = dgvAttendance.SelectedItem as TherapistAttendance;

            if(therapist != null && attM != null)
            {
                if (therapist.ID1.Equals(attM.TherapistID))
                {
                    ifthesame = true;
                }
            }


            return ifthesame;
        }

        private void searchAttendanceRecord()
        {
            List<TherapistAttendance> lstTherapistAttendance = new List<TherapistAttendance>();
            TherapistAttendance attendance = new TherapistAttendance();

            try
            {
                string queryString = "SELECT dbspa.tblattendance.ID, dbspa.tbltherapist.description AS 'THERAPIST', dbspa.tblattendance.attendanceDate AS 'DATE', " +
                    "dbspa.tblattendance.timeIn AS 'TIME IN', dbspa.tbltherapist.ID AS 'THERAPIST ID', dbspa.tblattendance.timeOut AS 'TIME OUT' FROM " +
                    "(dbspa.tblattendance INNER JOIN dbspa.tbltherapist ON dbspa.tblattendance.therapistID = dbspa.tbltherapist.ID) " +
                    "WHERE (dbspa.tblattendance.isDeleted = 0) AND (dbspa.tbltherapist.isDeleted = 0) AND (dbspa.tblattendance.attendanceDate = ?)";

                List<string> parameters = new List<string>();
                DateTime date = DateTime.Parse(dateFrom.Text);
                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

                MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

                while (reader.Read())
                {
                    attendance.ID = reader["ID"].ToString();
                    attendance.Therapist = reader["THERAPIST"].ToString();
                    DateTime dte = DateTime.Parse(reader["DATE"].ToString());
                    attendance.AttendanceDate = dte.ToShortDateString();
                    attendance.TimeIn = reader["TIME IN"].ToString();
                    attendance.TimeOut = reader["TIME OUT"].ToString();
                    attendance.TherapistID = reader["THERAPIST ID"].ToString();
                    lstTherapistAttendance.Add(attendance);
                    attendance = new TherapistAttendance();
                }

                dgvAttendance.ItemsSource = lstTherapistAttendance;

                conDB.closeConnection();

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void deleteAttendanceRecord(string recID)
        {
            string queryString = "UPDATE dbspa.tblAttendance SET isDeleted = 1 WHERE ID = ?";
            List<string> parameters = new List<string>();

            parameters.Add(recID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

        }

        private bool checkTimeIn(int strTherapistId, string recordID)
        {
            bool ifTimeIn = false;

            TherapistModel tm = new TherapistModel();

            try
            {
                string queryString = "SELECT dbspa.tblattendance.ID, dbspa.tbltherapist.description AS 'THERAPIST', dbspa.tblattendance.attendanceDate AS 'DATE', " +
                    "dbspa.tblattendance.timeIn AS 'TIME IN', dbspa.tblattendance.timeOut AS 'TIME OUT' FROM (dbspa.tblattendance INNER JOIN " +
                    "dbspa.tbltherapist ON dbspa.tblattendance.therapistID = dbspa.tbltherapist.ID) WHERE (dbspa.tblattendance.isDeleted = 0) AND " +
                    "(dbspa.tbltherapist.isDeleted = 0) AND (tblAttendance.therapistID = ?) AND dbspa.tblattendance.ID = ?";

                List<string> parameters = new List<string>();
                parameters.Add(strTherapistId.ToString());
                parameters.Add(recordID);

                MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

                while (reader.Read())
                {
                    tm.ID1 = reader["ID"].ToString();
                    tm.TimeIn = reader["TIME IN"].ToString();
                }

                if (!string.IsNullOrEmpty(tm.TimeIn))
                {
                    ifTimeIn = true;
                }
                conDB.closeConnection();

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

            return ifTimeIn;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // code goes here
            time.Content = DateTime.Now.ToString("hh:mm:ss tt");
        }

        private void chkManualIn_Checked(object sender, RoutedEventArgs e)
        {


            dateTimeIn.Visibility = Visibility.Visible;

            
        }

        private void chkManualOut_Checked(object sender, RoutedEventArgs e)
        {

            dateTimeOut.Visibility = Visibility.Visible;

            
        }

        private void chkManualIn_Unchecked(object sender, RoutedEventArgs e)
        {

                dateTimeIn.Visibility = Visibility.Hidden;

        }

        private void chkManualOut_Unchecked(object sender, RoutedEventArgs e)
        {

                dateTimeOut.Visibility = Visibility.Hidden;

        }

        private void btnTimeIn_Click(object sender, RoutedEventArgs e)
        {
            if (chkManualIn.IsChecked == true)
            {
                if (!string.IsNullOrEmpty(dteAttendance.Text))
                {
                    updateLogInViaManual();
                }else
                {
                    System.Windows.MessageBox.Show("Please provide date for attendance!");
                }
                
            }
            else
            {
                if (cmbTherapist.SelectedItem != null)
                {
                    TherapistModel therapist = cmbTherapist.SelectedItem as TherapistModel;

                    try
                    {

                        string strLogInTime = Properties.Settings.Default.LogInTime;
                        string strHalfday = Properties.Settings.Default.haldayTime;
                        DateTime loggedTime;
                        if (chkManualIn.IsChecked == true)
                        {
                            loggedTime = DateTime.Parse(dteAttendance.Text + " " + dateTimeIn.Text);
                        }
                        else
                        {
                            loggedTime = DateTime.Parse(dteAttendance.Text + " " + time.Content.ToString());
                        }

                        DateTime logInTime = DateTime.Parse(dteAttendance.Text + " " + strLogInTime);

                        DateTime halfDay = DateTime.Parse(dteAttendance.Text + " " + strHalfday);

                        int x = loggedTime.CompareTo(logInTime);
                        int y = loggedTime.CompareTo(halfDay);

                        int deductions = 0;
                        int lateDeduction = 0;
                        //bool ifLate = false;
                        int numIfLate = 0;
                        //bool ifHalfDay = false;
                        int numIfHalfDay = 0;
                        int minutes = 0;

                        //CHECK IF LOG IN TIME IS HALFDAY
                        if (y >= 0)
                        {
                            //ifHalfDay = true;
                            numIfHalfDay = 1;
                            if ((DateTime.Now.DayOfWeek == DayOfWeek.Saturday) ||
                                (DateTime.Now.DayOfWeek == DayOfWeek.Sunday))
                            {
                                //GET HALDAY DEDUCTION
                                deductions = Properties.Settings.Default.halfdayDeduction;
                            }
                            else
                            {
                                deductions = Properties.Settings.Default.halfdayDeduction;
                            }
                        }
                        else
                        {
                            //CHECK IF LOG IN TIME IS ON OR BEFORE OR AFTER SET LOG IN TIME
                            //-1 BEFORE 1PM
                            //0 - 1PM
                            //1 - after 1PM
                            if (x > 0)
                            {
                                TimeSpan span = loggedTime.Subtract(logInTime);

                                minutes = span.Minutes;

                                if (minutes < 0)
                                {
                                    minutes = 0;
                                }
                                else
                                {
                                    //ifLate = true;
                                    numIfLate = 1;
                                }

                                lateDeduction = checkMinutesToDeduct(minutes);

                            }
                        }

                        string recordID = "";
                        string queryString = "";
                        List<string> parameters = new List<string>();
                        if (checkIfAlreadyLoggedIn())
                        {
                      
                            System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("THERAPIST: " + therapist.Description + " already has a Time In! " + 
                                "Update existing Time In record?", "Attendace", System.Windows.Forms.MessageBoxButtons.YesNo);

                            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                            {
                                TherapistAttendance attM = dgvAttendance.SelectedItem as TherapistAttendance;
                                if (attM != null)
                                {
                                    recordID = attM.ID;
                                }

                                queryString = "UPDATE dbspa.tblattendance SET attendanceDate = ?, timeIn = ?, minutesLate = ?, isLate =?, " +
                                    " deduction = ?, ifhalfday = ?, lateDeduction = ? WHERE therapistID = ? AND dbspa.tblattendance.attendanceDate = ?";

                                DateTime date = DateTime.Parse(dteAttendance.Text);
                                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
                                parameters.Add(dateTimeIn.Text);
                                parameters.Add(minutes.ToString());
                                parameters.Add(numIfLate.ToString());
                                parameters.Add(deductions.ToString());
                                parameters.Add(numIfHalfDay.ToString());
                                parameters.Add(lateDeduction.ToString());
                                parameters.Add(therapist.ID1);

                                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

                                conDB.AddRecordToDatabase(queryString, parameters);
                                System.Windows.MessageBox.Show("Time in for " + therapist.Description + " recorded successfully!");
                                conDB.writeLogFile("TIME IN: UPDATE EXISTING RECORD FOR: " + therapist.Deduction);
                            }
                            else
                            {
                                
                            }                                                    
                        }
                    
                        else
                        {
                            queryString = "INSERT INTO dbspa.tblattendance (attendanceDate, timeIn, therapistID, isDeleted, minutesLate, isLate, deduction, " +
                                "ifhalfday, lateDeduction, undertimeDeduction, ifUndertime) VALUES(?,?,?,?,?,?,?,?,?,?,?)";

                            DateTime date = DateTime.Parse(dteAttendance.Text);
                            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

                            parameters.Add(loggedTime.ToShortTimeString());

                            parameters.Add(therapist.ID1);
                            parameters.Add(0.ToString());
                            parameters.Add(minutes.ToString());
                            parameters.Add(numIfLate.ToString());
                            parameters.Add(deductions.ToString());
                            parameters.Add(numIfHalfDay.ToString());
                            parameters.Add(lateDeduction.ToString());
                            parameters.Add(0.ToString());
                            parameters.Add(0.ToString());

                            conDB.AddRecordToDatabase(queryString, parameters);
                            System.Windows.MessageBox.Show("Time in for " + therapist.Description + " recorded successfully!");
                            conDB.writeLogFile("TIME IN: SAVE NEW RECORD FOR: " + therapist.Deduction);
                        }                     

                        conDB.closeConnection();
                        
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        getAttendanceRecords();
                    }
                }//exit IF brace
                else
                {
                    System.Windows.MessageBox.Show("Please select Therapist!");
                }
            }
        }

        private void btnTimeOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(dteAttendance.Text))
                {

                    if (checkTherapistIfTheSame())
                    {
                        if (cmbTherapist.SelectedItem != null)
                        {
                            TherapistModel therapist = cmbTherapist.SelectedItem as TherapistModel;

                            string recordID = "";

                            TherapistAttendance attM = dgvAttendance.SelectedItem as TherapistAttendance;

                            if (attM != null)
                            {
                                recordID = attM.ID;
                            }


                            if (checkTimeIn(Convert.ToInt32(therapist.ID1), attM.ID))
                            {

                                DateTime loggedOutTime;
                                string strLogOut = Properties.Settings.Default.logOutTime;
                                if (chkManualOut.IsChecked == true)
                                {
                                    loggedOutTime = DateTime.Parse(dteAttendance.Text + " " + dateTimeOut.Text);
                                }
                                else
                                {
                                    loggedOutTime = DateTime.Parse(dteAttendance.Text + " " + time.Content);
                                }

                                DateTime LogOut = DateTime.Parse(dteAttendance.Text + " " + strLogOut);
                                if (loggedOutTime.Date.Equals(LogOut.Date))
                                {
                                   loggedOutTime = loggedOutTime.AddDays(-1);
                                }
                                int x = loggedOutTime.CompareTo(LogOut);
                                int deductions = 0;
                                //bool ifUndertime = false;
                                int numIfUndertime = 0;
                                if (x < 0)
                                {
                                    deductions = Properties.Settings.Default.undertime;
                                    //ifUndertime = true;
                                    numIfUndertime = 1;
                                }

                                string queryString = "UPDATE dbspa.tblattendance SET timeOut = ?, ifUndertime = ? , undertimeDeduction = ? WHERE therapistID = ? AND ID = ?";
                                List<string> parameters = new List<string>();
                                if (chkManualOut.IsChecked == true)
                                {
                                    parameters.Add(dateTimeOut.Text);
                                }
                                else
                                {
                                    parameters.Add(time.Content.ToString());
                                }

                                parameters.Add(numIfUndertime.ToString());
                                parameters.Add(deductions.ToString());
                                parameters.Add(therapist.ID1);
                                parameters.Add(recordID);

                                conDB.AddRecordToDatabase(queryString, parameters);
                                conDB.closeConnection();
                                //getAttendanceRecords();
                                System.Windows.MessageBox.Show("Time out for " + therapist.Description + " recorded successfully!");
                                conDB.writeLogFile("TIME OUT: " + therapist.Deduction);

                            }
                            else
                            {
                                System.Windows.MessageBox.Show("No Time In record found!");
                            }
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("Please select Therapist!");
                        }

                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Selected therapist not equal to selected record!");
                    }
                }else
                {
                    System.Windows.MessageBox.Show("Please provide date for attendance!");
                }

            }

                
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                getAttendanceRecords();
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            searchAttendanceRecord();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Are you sure you want to Delete record?", "Delete Record", MessageBoxButtons.YesNo);

            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                TherapistAttendance t = dgvAttendance.SelectedItem as TherapistAttendance;

                if(t != null)
                {
                    deleteAttendanceRecord(t.ID);
                    getAttendanceRecords();
                    System.Windows.MessageBox.Show("RECORD DELETED SUCCESSFULLY!");
                    conDB.writeLogFile("DELETED ATTENDANCE RECORD: ID: " + t.ID);
                }
            }
        }
    }
}
