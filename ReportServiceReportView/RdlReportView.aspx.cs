using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;

namespace ReportServiceReportView
{
    public partial class RdlReportView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            // report managment  http://dev-sean/Reports/Pages/Folder.aspx
            // report publisjh   http://dev-sean/reportserver");
            // must use VS 2010 WSL tools to generael WS Reference.cs file !!!!! need to use wsdl tools to generate DLL file
            //wsdl /language:CS /n:"Microsoft.SqlServer.ReportingServices2010" http://dev-sean/reportserver/reportservice2010.asmx?wsdl
           
            if (!this.IsPostBack)
            {
                ReportViewer1.ProcessingMode = ProcessingMode.Remote;

                ReportViewer1.ServerReport.ReportServerUrl = new Uri(@"http://dev-xianghao/Reports_SQL2008R2");
                //ReportViewer1.ServerReport.pa
                ReportViewer1.ServerReport.ReportPath = @"/testNoDataSource";

                // dont put @ name here ( it is not store procedure)
               // ReportParameter reportParameter = new ReportParameter("ReportParameter1", new string[] { "dynamical change paramter value is 5" });
                 var paramteColelction = ReportViewer1.ServerReport.GetParameters().ToDictionary (o=>o.Name,o=>o);

                 if (paramteColelction.ContainsKey("ReportParameter1"))
                 {
                     ReportParameter reportParameter = new ReportParameter("ReportParameter1", new string[] { "dynamical change paramter value is 5" });
                     ReportViewer1.ServerReport.SetParameters(reportParameter);
                 }
                      
                      //   para.Values.Add( "dynamical change paramter valusss ?/" );

                 this.ReportViewerPublish.ProcessingMode = ProcessingMode.Remote;

                 ReportViewerPublish.ServerReport.ReportServerUrl = new Uri(@"http://dev-sean/reportserver");
                 //ReportViewer1.ServerReport.pa
               //  ReportViewerPublish.ServerReport.ReportPath = @"/plmBlockmatrixWithConnection";
                 //fabcd/abcd_REPORTS/sharedatasource

                 ReportViewerPublish.ServerReport.ReportPath = @"/abcd/abcd_REPORTS/plmBlockmatrix";

               

               //
            }

            // ReportViewer1.ServerReport.su
        }
    }
}