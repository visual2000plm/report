using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ReportViewSetup;
using System.Data.SqlClient;
using System.IO;

namespace DynamicWebReportView
{
    public partial class RequestJobPdfFile : System.Web.UI.Page
    {
        protected override void OnPreInit(EventArgs e)
        {
                    
            string jobId = Request.QueryString[DDSetup.RequestJobPdfId];
            string reportPdfFilename = string.Empty ;

            using (SqlConnection conn = new SqlConnection(DDSetup.PLMConnectionString))
            {
                conn.Open();
                string querysql = " select [FileName] from  [pdmPrintJob] where  PrintJobID = @jobId";

                SqlCommand cmd = new SqlCommand(querysql, conn);

                cmd.Parameters.Add("@jobId", int.Parse(jobId));

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    reportPdfFilename = cmd.ExecuteScalar().ToString();
                }
        
            }

            if (!string.IsNullOrEmpty(reportPdfFilename))
            {
                try
                {

                    byte[] buffer = File.ReadAllBytes(reportPdfFilename);

                    Response.ContentType = "application/pdf";

                    Response.OutputStream.Write(buffer, 0, buffer.Length);
                    Response.Flush();
                    Response.Close();

                }
                catch
                {
                    Response.Write("Cannot find a report");

                }
            
            }
         
        }

    }
}