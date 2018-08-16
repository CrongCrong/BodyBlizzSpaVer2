using BodyBlizzSpaVer2.Classes;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Windows;

namespace BodyBlizzSpaVer2
{
    /// <summary>
    /// Interaction logic for ReportForm.xaml
    /// </summary>
    public partial class ReportForm : Window
    {
        public ReportForm()
        {
            InitializeComponent();
        }

        ClientModel clientModel;
        ClientForm clientForm;
        TherapistModel therapist;
        List<TherapistModel> lstTherapist;
        private ServiceTypeModel serviceTypeModel = new ServiceTypeModel();
        List<ServiceMadeModel> lstServiceMade;
        List<ExpensesModel> lstExpenses;
        List<CashAdvanceModel> lstCashAdvance;
        List<LoanModel> lstLoans;
        List<ServiceReportModel> lstServiceReport;

        string dateReport;

        public ReportForm(ClientForm cf, ClientModel cm)
        {
            clientForm = cf;
            clientModel = cm;
            InitializeComponent();
        }

        public ReportForm(TherapistModel tm, List<TherapistModel> lstTM)
        {
            therapist = tm;
            lstTherapist = lstTM;
            InitializeComponent();
        }

        public ReportForm(List<ServiceMadeModel> smm, List<ExpensesModel> exm, List<CashAdvanceModel> cam, List<LoanModel> llm, string strDate)
        {
            lstServiceMade = smm;
            lstExpenses = exm;
            lstCashAdvance = cam;
            lstLoans = llm;
            dateReport = strDate;
            InitializeComponent();
        }

        public ReportForm(List<ServiceReportModel> lstSRF)
        {
            lstServiceReport = lstSRF;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (clientModel != null)
            {
                if (clientModel.IfPrintWaiver)
                {
                    printWaiverForm();
                }
                else
                {
                    printClientForm();
                }

            }
            if (therapist != null)
            {
                printTherapistSalary();
            }

            if(lstServiceMade != null)
            {
                printExpenses();
            }

            if(lstServiceReport != null)
            {
                printServiceForm();
            }

        }

        private void printClientForm()
        {
            try
            {
                reportViewer.ProcessingMode = ProcessingMode.Local;
                LocalReport localReport = reportViewer.LocalReport;

                localReport.ReportPath = "Reports/ClientReport.rdlc";
                reportViewer.RefreshReport();

                //Create the list of parameters that will be passed to the report
                List<ReportParameter> paramList = new List<ReportParameter>();

                ReportParameter param = new ReportParameter("name");
                param.Values.Add(clientModel.FirstName + " " + clientModel.LastName);
                paramList.Add(param);

                param = new ReportParameter("date");
                DateTime date = DateTime.Parse(clientModel.DateServiced);
                param.Values.Add(date.ToShortDateString());
                paramList.Add(param);

                param = new ReportParameter("serialNumber");
                param.Values.Add(clientModel.SerialNumber);
                paramList.Add(param);

                param = new ReportParameter("cod");
                param.Values.Add(clientModel.Cod);
                paramList.Add(param);

                param = new ReportParameter("address");
                param.Values.Add(clientModel.Address);
                paramList.Add(param);

                param = new ReportParameter("serviceMode");
                param.Values.Add(clientModel.ServiceMode);
                paramList.Add(param);

                param = new ReportParameter("timeIn");
                param.Values.Add(clientModel.TimeIn);
                paramList.Add(param);

                param = new ReportParameter("timeOut");
                if (string.IsNullOrEmpty(clientModel.TimeOut))
                {
                    param.Values.Add(" ");
                }else
                {
                    param.Values.Add(clientModel.TimeOut);
                }
                
                paramList.Add(param);


                double total = 0.0;
                double prodTotal = 0.0;
                string[] strArr;
                string[] priceArr;

                if (clientModel.LstServiceTypeModel.Count < 1)
                {
                    strArr = new string[1];
                    priceArr = new string[1];
                    priceArr[0] = "\r" + "0.0" + "\n";
                    strArr[0] = "\r" + "None" + "\n";

                }
                else
                {
                    strArr = new string[clientModel.LstServiceTypeModel.Count];
                    priceArr = new string[clientModel.LstServiceTypeModel.Count];

                    for (int i = 0; i < clientModel.LstServiceTypeModel.Count; i++)
                    {
                        ServiceTypeModel stm = clientModel.LstServiceTypeModel[i];
                        double x = 0.0;
                        x = x + Convert.ToDouble(stm.Price);
                        total += x;
                        priceArr[i] = "\r" + stm.Price + "\n";
                        strArr[i] = "\r" + stm.ServiceType + "\n";
                    }

                }

                string[] strThera = new string[1];
                if (clientModel.LstTherapistModel.Count < 1)
                {
                    
                    strThera[0] = "\r" + "-" + "\n";
                }
                else
                {
                    strThera = new string[clientModel.LstTherapistModel.Count];
                    for (int i = 0; i < clientModel.LstTherapistModel.Count; i++)
                    {         
                        TherapistModel tm = clientModel.LstTherapistModel[i];
                        strThera[i] = "\r" + tm.ToString() + "\n";
                    }
                }


                string[] strProducts = new string[1];
                string[] strProductPrice = new string[1];

                if (clientModel.lstProductsBought.Count < 1)
                {
                    strProducts[0] = "\r" + "None" + "\n";
                    strProductPrice[0] = "\r" + "0.0" + "\n";
                }
                else
                {
                    strProducts = new string[clientModel.lstProductsBought.Count];
                    strProductPrice = new string[clientModel.lstProductsBought.Count];
                    for (int x = 0; x < clientModel.lstProductsBought.Count; x++)
                    {
                        ProductBoughtModel prodBought = clientModel.lstProductsBought[x];
                        double pp = Convert.ToDouble(String.Format("{0:0.00}", prodBought.ProductPrice));
                        strProducts[x] = "\r" + prodBought.ProductName + "\n";
                        strProductPrice[x] = "\r" + prodBought.ProductPrice + "\n";
                        prodTotal = prodTotal + pp;


                    }
                }

                
                total = total + prodTotal;

                param = new ReportParameter("total");
                param.Values.Add(total.ToString());
                paramList.Add(param);

                //MULTI LEVEL PARAMS
                ReportParameter multiValueParam = new ReportParameter("serviceType");
                multiValueParam.Values.AddRange(strArr);
                paramList.Add(multiValueParam);

                multiValueParam = new ReportParameter("amounts");
                multiValueParam.Values.AddRange(priceArr);
                paramList.Add(multiValueParam);

                multiValueParam = new ReportParameter("therapist");
                multiValueParam.Values.AddRange(strThera);
                paramList.Add(multiValueParam);

                multiValueParam = new ReportParameter("products");
                multiValueParam.Values.AddRange(strProducts);
                paramList.Add(multiValueParam);

                multiValueParam = new ReportParameter("productprice");
                multiValueParam.Values.AddRange(strProductPrice);
                paramList.Add(multiValueParam);
                //ADD PARAMETERS                        

                //Set the ReportViewers parameters to the list of ReportParameters we just created

                this.reportViewer.LocalReport.SetParameters(paramList.ToArray());

                // Refresh the report  
                reportViewer.RefreshReport();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            this.reportViewer.RefreshReport();
            this.reportViewer.RefreshReport();
        }

        private void printWaiverForm()
        {
            try
            {
                reportViewer.ProcessingMode = ProcessingMode.Local;
                LocalReport localReport = reportViewer.LocalReport;

                localReport.ReportPath = "Reports/Waiver.rdlc";
                reportViewer.RefreshReport();

                //Create the list of parameters that will be passed to the report
                List<ReportParameter> paramList = new List<ReportParameter>();

                ReportParameter param = new ReportParameter("date");
                DateTime date = DateTime.Parse(clientModel.DateServiced);
                param.Values.Add(date.ToShortDateString());
                paramList.Add(param);

                param = new ReportParameter("serialNumber");
                param.Values.Add(clientModel.SerialNumber);
                paramList.Add(param);

                param = new ReportParameter("cod");
                param.Values.Add(clientModel.Cod);
                paramList.Add(param);

                param = new ReportParameter("clientName");
                param.Values.Add(clientModel.FirstName + " " + clientModel.LastName);
                paramList.Add(param);

                //Set the ReportViewers parameters to the list of ReportParameters we just created

                this.reportViewer.LocalReport.SetParameters(paramList.ToArray());

                // Refresh the report  
                reportViewer.RefreshReport();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            this.reportViewer.RefreshReport();
            this.reportViewer.RefreshReport();
        }

        private void printTherapistSalary()
        {
            reportViewer.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer.LocalReport;

            localReport.ReportPath = "Reports/TherapistSalary.rdlc";
            reportViewer.RefreshReport();

            //Create the list of parameters that will be passed to the report
            List<ReportParameter> paramList = new List<ReportParameter>();

            ReportParameter param = new ReportParameter("name");
            param.Values.Add(therapist.Description);
            paramList.Add(param);

            param = new ReportParameter("totaldailywage");
            param.Values.Add(therapist.TotalWage);
            paramList.Add(param);

            param = new ReportParameter("dayswork");
            param.Values.Add(lstTherapist.Count.ToString());
            paramList.Add(param);

            //param = new ReportParameter("totalWage");
            //param.Values.Add(therapist.TotalWage);
            //paramList.Add(param);

            param = new ReportParameter("total");
            param.Values.Add((Convert.ToDouble(therapist.TotalCommission) + Convert.ToDouble(therapist.TotalWage)).ToString());
            paramList.Add(param);

            //param = new ReportParameter("cashAdvance");
            //param.Values.Add(therapist.CashAdvance);
            //paramList.Add(param);

            //param = new ReportParameter("loan");
            //param.Values.Add(therapist.Loan);
            //paramList.Add(param);

            double totalDeductions = 0.0;
            foreach (TherapistModel tm in lstTherapist)
            {
                double d = Convert.ToDouble(tm.Deduction) + Convert.ToDouble(tm.lateDeduction) + Convert.ToDouble(tm.undertimeDeduction);
                totalDeductions = totalDeductions + d;
            }

            int totalWage = Convert.ToInt32(therapist.TotalWage);
            double com = Convert.ToDouble(therapist.TotalCommission);
            double netPay = (totalWage + com) - totalDeductions;
            double dblCA = Convert.ToDouble(therapist.CashAdvance);
            netPay = netPay - dblCA;

            //param = new ReportParameter("netPay");
            //param.Values.Add(String.Format("{0:0.00}", netPay));
            //paramList.Add(param);

            //MULTI LEVEL PARAMS
            ReportParameter multiValueParam = new ReportParameter("servicesRendered");
            multiValueParam.Values.AddRange(therapist.Services);
            paramList.Add(multiValueParam);

            multiValueParam = new ReportParameter("amount");
            multiValueParam.Values.AddRange(therapist.Amount);
            paramList.Add(multiValueParam);

            multiValueParam = new ReportParameter("clientNames");
            multiValueParam.Values.AddRange(therapist.ClientName);
            paramList.Add(multiValueParam);

            //multiValueParam = new ReportParameter("penalties");
            //multiValueParam.Values.AddRange(therapist.Penalties);
            //paramList.Add(multiValueParam);

            this.reportViewer.LocalReport.SetParameters(paramList.ToArray());

            // Refresh the report  
            reportViewer.RefreshReport();
        }

        private void printExpenses()
        {
            reportViewer.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer.LocalReport;

            localReport.ReportPath = "Reports/ExpensesReport.rdlc";
            reportViewer.RefreshReport();

            double totalSales = 0;
            double totalCommission = 0;
            double totalExpenses = 0;
            double sales = 0;
            double totalCA = 0;
            double totalLoans = 0;

            foreach(ServiceMadeModel s in lstServiceMade)
            {
                totalSales = totalSales + Convert.ToDouble(s.Price);
                totalCommission = totalCommission + Convert.ToDouble(s.Commission);
            }

            foreach(ExpensesModel e in lstExpenses)
            {
                totalExpenses = totalExpenses + Convert.ToDouble(e.CashOut);
            }

            foreach(CashAdvanceModel c in lstCashAdvance)
            {
                totalCA = totalCA + Convert.ToDouble(c.Cash);
            }

            foreach (LoanModel l in lstLoans)
            {
                totalLoans += Convert.ToDouble(l.LoanBalance);
            }

            sales = totalSales;
            //totalSales = totalSales - totalExpenses;
            //totalSales = totalSales - totalCA;
            //totalSales = totalSales - totalLoans;

            //Create the list of parameters that will be passed to the report
            List<ReportParameter> paramList = new List<ReportParameter>();

            ReportParameter param = new ReportParameter("date");
            param.Values.Add(dateReport);
            paramList.Add(param);


            param = new ReportParameter("sales");
            param.Values.Add(sales.ToString());
            paramList.Add(param);

            param = new ReportParameter("totalSales");
            param.Values.Add(totalSales.ToString());
            paramList.Add(param);

            param = new ReportParameter("totalCommission");
            param.Values.Add(totalCommission.ToString());
            paramList.Add(param);

            param = new ReportParameter("totalExpenses");
            param.Values.Add(totalExpenses.ToString());
            paramList.Add(param);

            param = new ReportParameter("totalCA");
            param.Values.Add(totalCA.ToString());
            paramList.Add(param);

            param = new ReportParameter("totalLoans");
            param.Values.Add(totalLoans.ToString());
            paramList.Add(param);

            this.reportViewer.LocalReport.SetParameters(paramList.ToArray());

            // Refresh the report  
            reportViewer.RefreshReport();
        }

        private void printServiceForm()
        {
            ReportDataSource rds = new ReportDataSource();

            rds = new ReportDataSource("DataSet1", lstServiceReport);

            reportViewer.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer.LocalReport;

            localReport.ReportPath = "Reports/ServiceReport.rdlc";
            reportViewer.RefreshReport();

            System.Drawing.Printing.PageSettings ps = new System.Drawing.Printing.PageSettings();
            ps.Landscape = true;

            ps.PaperSize = new System.Drawing.Printing.PaperSize("A4", 827, 1170);
            ps.PaperSize.RawKind = (int)System.Drawing.Printing.PaperKind.A4;
            reportViewer.SetPageSettings(ps);

            reportViewer.LocalReport.DataSources.Add(rds);

            // Refresh the report  
            reportViewer.RefreshReport();
        }
    }
}
