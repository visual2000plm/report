using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;

namespace ReportServiceReportView
{
    public partial class ReportView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                ReportViewer1.Reset();
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("Report.rdl");
                // ReportViewer1.LocalReport.DataSources.Clear();
                //   ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", SqlDataSource1));
                ReportViewer1.LocalReport.Refresh();
            }
        }
    }
}