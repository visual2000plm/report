using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;

namespace ReportServiceReportView
{
    public partial class ReportServiceGenerator : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                string dsName = "pdmproduct";

                #region SQL

                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection("Data Source=srv-spider;Initial Catalog=SR_PLMS;User ID=sa;Password=visualdev;");
                SqlCommand cmd = new SqlCommand(@"SELECT     dbo.tblSketch.Thumbnail, dbo.pdmProduct.ParentReferenceID, dbo.pdmProduct.OriginalReferenceID, dbo.pdmProduct.ReferenceCode
FROM         dbo.pdmProduct INNER JOIN
                      dbo.tblSketch ON dbo.pdmProduct.SketchID = dbo.tblSketch.SketchID", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                #endregion SQL

                ReportGenerator gen = new ReportGenerator(dt, dsName);

                Stream xmlStream = gen.GeneraReport();

                StreamReader readers = new StreamReader(xmlStream);
                string rdlFile = readers.ReadToEnd();
                //   string rdlFile = readers.ToString();

                // need to running time get it !
                ReportDataSource ds = new ReportDataSource(dsName, dt);

                ReportViewer1.Reset();

                ReportViewer1.LocalReport.DataSources.Add(ds);

                //  ReportViewer1.LocalReport.DataSources.Add(ds);

                ReportViewer1.LocalReport.DisplayName = dsName;

                ReportViewer1.LocalReport.LoadReportDefinition(gen.GeneraReport());
            }
        }
    }
}