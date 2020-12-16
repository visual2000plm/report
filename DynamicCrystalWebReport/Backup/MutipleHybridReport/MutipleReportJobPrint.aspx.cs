using System;

using ReportViewSetup;
using System.Data.SqlClient;
using System.IO;

using System.Collections.Generic;
using DynamicWebReportView;
using System.Diagnostics;
using System.Data;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

// need to set  IIS applcation pool    idle time as long as possible so that  IIS dont recyle application pool (2 hours
// <httpRuntime executionTimeout="110"/> The default is 110 seconds.( 2 min), n
//Specifies the maximum number of seconds ( 2400 ) for big report that a request is allowed to execute before being automatically shut down by ASP.NET.


namespace WebApplication1
{
    public partial class MutipleReportJobPrint : System.Web.UI.Page
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            string reportName = Request.QueryString[DDSetup.QueryReportName];
            string aUId = Request.QueryString[DDSetup.ReportParameterUid];
            string allReportFileNmae = string.Empty;
            string allReportFileNameWithRefId = Request.QueryString[DDSetup.ReportJobName];

            if (!string.IsNullOrEmpty(reportName) && !string.IsNullOrEmpty(aUId))
            {
                allReportFileNmae = reportName;
            }
            else
            {
                Response.Write(" Cannot find report file !");
                return;
            }

            string productReferenceId = Request.QueryString[DDSetup.ReportParameterProductReferenceID];
            string PdmRequestRegisterID = Request.QueryString[DDSetup.PdmRequestRegisterID];
            string dataSourceType = Request.QueryString[DDSetup.QueryReportDataSourceType];


            string mainReferenceID = Request.QueryString[DDSetup.ReportParameterMainReferenceID];
            string masterReferenceID = Request.QueryString[DDSetup.ReportParameterMasterReferenceID];
         

            if (string.IsNullOrEmpty(PdmRequestRegisterID))
            {
                PdmRequestRegisterID = string.Empty;
            }

            if (!this.IsPostBack)
            {

               if (string.IsNullOrEmpty(allReportFileNameWithRefId))
                {
                    Response.Write(" Report Job name is missing !");
                    return;
                }
                else// need to create a new JOB
                {
                    // create a report job and add to the queue
                     ReportJobDesc aReportJobDesc = new ReportJobDesc ();
                     aReportJobDesc.ReportJobName   = allReportFileNameWithRefId   ;
                     aReportJobDesc.UId   =   aUId   ;       
                     aReportJobDesc.AllReportFileNmae   = allReportFileNmae   ;
                     aReportJobDesc.ProductReferenceId   = productReferenceId  ;
                     aReportJobDesc.PdmRequestRegisterID = PdmRequestRegisterID ;
                     aReportJobDesc.DataSourceType = dataSourceType;
                     aReportJobDesc.MainReferenceID = mainReferenceID;
                     aReportJobDesc.MasterReferenceID = masterReferenceID;

                     ReportJobProcessor.ReportJobQueue.Enqueue(aReportJobDesc);
                  

                     Response.Write(" the Print Job was added to print queue  !");

                }

            }
        }
    
    
     
       
     
    }
}