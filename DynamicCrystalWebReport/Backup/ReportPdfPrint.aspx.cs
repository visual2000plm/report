using System;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using ReportViewSetup;
using System.Data.SqlClient;
using System.IO;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using System.Collections.Generic;
using DynamicWebReportView;
using System.Diagnostics;
using System.Data;

// need to set  IIS applcation pool    idle time as short as possible so that  IIS recylce 
// <httpRuntime executionTimeout="3000"/> The default is 110 seconds.( 2 min), normal set 1200 Seconds

//Specifies the maximum number of seconds ( 2400 ) for big report that a request is allowed to execute before being automatically shut down by ASP.NET.

//http://localhost/ReportPublishCrystal/ReportPdfPrint.aspx?ReportName=Crystal_OSC_GetTab1.rpt^21836|OSC_Proto Summary.rdlx^21863&uid=1ReportDataSourceType=PLMDatabase&MutipleRef=1

//http://localhost/ReportPublishCrystal/ReportPdfPrint.aspx?ReportName=Crystal_OSC_GetTab1.rpt^21836|OSC_Proto Summary.rdlx^21863&uid=1ReportDataSourceType=PLMDatabase&MutipleRef=0
//MutipleRef=0

namespace WebApplication1
{
    public partial class ReportPdfPrint : System.Web.UI.Page
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            string reportName = Request.QueryString[DDSetup.QueryReportName];
            string aUId = Request.QueryString[DDSetup.ReportParameterUid];
            string allReportFileNameWithRefId = string.Empty;
           


            if (!string.IsNullOrEmpty(reportName) && !string.IsNullOrEmpty(aUId))
            {
                allReportFileNameWithRefId = reportName;
            }
            else
            {
                return;
            }

    
            string PdmRequestRegisterID = Request.QueryString[DDSetup.PdmRequestRegisterID];

            if (string.IsNullOrEmpty(PdmRequestRegisterID))
            {
                PdmRequestRegisterID = string.Empty;
            }

            string dataSourceType = Request.QueryString[DDSetup.QueryReportDataSourceType];

            string mainreferenceID = Request.QueryString[DDSetup.ReportParameterMainReferenceID];
            string masterReferenceId = Request.QueryString[DDSetup.ReportParameterMasterReferenceID];


            if (!this.IsPostBack)
            {

                PrintUserReaTimeReport(aUId, allReportFileNameWithRefId, PdmRequestRegisterID, dataSourceType, mainreferenceID,  masterReferenceId);

            }
        }
        private void PrintUserReaTimeReport(string aUId, string allReportFileNmae,  string PdmRequestRegisterID, string dataSourceType,string mainreferenceID, string masterReferenceId )
        {

            //allReportFileNmae=ReportName=Crystal_OSC_GetTab1.rpt^21836|OSC_Proto Summary.rdlx^21863
            if (allReportFileNmae != string.Empty)
            {
                // only create once !!

                string[] reportnames = allReportFileNmae.Split('|');

                List<KeyValuePair<string, string>> mutipleReportAndReference = new List<KeyValuePair<string, string>>();

                List<Stream> pdfFileStream = new List<Stream>();

                  foreach (String reportFileNameWithRef in reportnames)
                   {

                        string [] reportNameAndRef =  reportFileNameWithRef.Split('^');
                        if (reportNameAndRef.Length != 2)
                            continue;

                        string  reportFileName = reportNameAndRef[0];
                        string  reportRefId = reportNameAndRef[1];
                          

                        // Cyrstal report
                        if (reportFileName.EndsWith(".rpt") || reportFileName.EndsWith(".RPT"))
                        {

                            Stream result = CystalReportExport.GetCrystalPdfStream(reportFileName, aUId, reportRefId, PdmRequestRegisterID, dataSourceType, mainreferenceID, masterReferenceId);
                            if (result != null)
                            {
                                pdfFileStream.Add(result);
                            }
                        }
                        // Data Dynamics 
                        else if (reportFileName.EndsWith(".rdlx") || reportFileName.EndsWith(".RDLX"))
                        {
                            Stream result = DataDynamicsExport.GetDataDynamicPdfStream(reportFileName, aUId, reportRefId, PdmRequestRegisterID, dataSourceType, mainreferenceID, masterReferenceId);

                            if (result != null)
                            {
                                pdfFileStream.Add(result);
                            }

                        }
                    }
   

                using (PdfDocument outputPdfDocument = new PdfDocument())
                {

                    foreach (Stream stream in pdfFileStream)
                    {
                        // Open the document to import pages from it.
                        if (stream.Length > 0)
                        {
                            using (PdfDocument inputDocument = PdfReader.Open(stream, PdfDocumentOpenMode.Import))
                            {
                                // Iterate pages
                                int count = inputDocument.PageCount;
                                for (int idx = 0; idx < count; idx++)
                                {
                                    // Get the page from the external document...
                                    PdfPage page = inputDocument.Pages[idx];
                                    // ...and add it to the output document.
                                    outputPdfDocument.AddPage(page);
                                }
                                stream.Close();
                                stream.Dispose();
                            }
                        }
                    }

                    if (DDSetup.ReorptSetup.IsReportCompressionActivate)
                    {
                        string fileID = Guid.NewGuid().ToString();
                        string FileNameOrigin = DDSetup.ReorptSetup.ReportPdfCompressPath + "Origin_" + fileID + ".pdf";
                        outputPdfDocument.Save(FileNameOrigin);

                        string FileNameDestination = DDSetup.ReorptSetup.ReportPdfCompressPath + "Dest_" + fileID + ".pdf";
                        ReportJobManagement.PdfCompression(FileNameOrigin, FileNameDestination);
                        File.Delete(FileNameOrigin);
                        OutputPdfFile(FileNameDestination);


                    }
                    else // need to compress PDF 
                    {
                        MemoryStream memoStream = new MemoryStream();

                        outputPdfDocument.Save(memoStream, false);
                        OutPutResponse(memoStream);

                    }

                }

                // need to dispsoe output to release memeory
                //  outputPdfDocument.Dispose();
                // need to dispost 


            }
        }
        private void OutputPdfFile(string FileNameDestination)
        {
            try
            {

                byte[] buffer = File.ReadAllBytes(FileNameDestination);
                Response.ContentType = "application/pdf";
                Response.OutputStream.Write(buffer, 0, buffer.Length);
                Response.Flush();
                Response.Close();

                File.Delete(FileNameDestination);


            }
            catch
            {
                Response.Write("Cannot find a report");

            }
        }
        private void OutPutResponse(MemoryStream memoStream)
        {
            try
            {

                byte[] buffer = new byte[memoStream.Length];

                memoStream.Read(buffer, 0, (int)memoStream.Length);
                Response.ContentType = "application/pdf";

                Response.OutputStream.Write(buffer, 0, buffer.Length);
                Response.Flush();
                Response.Close();

                memoStream.Close();
                memoStream.Dispose();


            }
            catch
            {
                Response.Write("Cannot find a report");

            }
        }


    }
}