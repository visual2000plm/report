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

// http://localhost/MutipleHybridReport/MutipleReportJobPrint.aspx?ReportName=Test Crystal Report^15210|Test Dynamic Report^15210&uid=26a1f8cf-f291-456e-88a5-f8791b86caeb&MainReferenceID=15210&ReportDataSourceType=DWDatabase&ReportJobName=my test report job name 

namespace WebApplication1
{
    public partial class MutipleReportJobPrint : System.Web.UI.Page
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            //ReportName=Test Crystal Report^15210|Test Dynamic Report^15210
            string mutipleReportNamesWithReferenceIds = Request.QueryString[DDSetup.QueryReportName];
            string sessionUid = Request.QueryString[DDSetup.ReportParameterUid];

            string reportJobName = Request.QueryString[DDSetup.QueryReportJobName];



            int? userid = DDSetup.GetUserIdFromSessionId(sessionUid);
            if (!userid.HasValue)
            {
                Response.Write(" access denied ");
                return;
            }

            if (string.IsNullOrEmpty(mutipleReportNamesWithReferenceIds))
            {
                Response.Write(" no report file name ");
                return;
 
            }

          

            string productReferenceId = Request.QueryString[DDSetup.ReportParameterProductReferenceID];
            string PdmRequestRegisterID = Request.QueryString[DDSetup.QueryPdmRequestRegisterID];
            string dataSourceType = Request.QueryString[DDSetup.QueryReportDataSourceType];


            string mainReferenceID = Request.QueryString[DDSetup.QueryReportParameterMainReferenceID];
            string masterReferenceID = Request.QueryString[DDSetup.QueryReportParameterMasterReferenceID];
         

            if (string.IsNullOrEmpty(PdmRequestRegisterID))
            {
                PdmRequestRegisterID = string.Empty;
            }

            if (!this.IsPostBack)
            {

               if (string.IsNullOrEmpty(reportJobName))
                {
                    Response.Write(" Report Job name is missing !");
                    return;
                }
                else// need to create a new JOB
                {
                    // create a report job and add to the queue
                     ReportJobDesc aReportJobDesc = new ReportJobDesc ();
                     aReportJobDesc.ReportJobName   = reportJobName   ;
                     aReportJobDesc.UId   =   userid   ;
                     aReportJobDesc.AllReportFileNmae = mutipleReportNamesWithReferenceIds;
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