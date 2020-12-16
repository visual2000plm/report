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
using Com.Visual2000.SystemFramework;

// need to set  IIS applcation pool    idle time as short as possible so that  IIS recylce 
// <httpRuntime executionTimeout="3000"/> The default is 110 seconds.( 2 min), normal set 1200 Seconds

//Specifies the maximum number of seconds ( 2400 ) for big report that a request is allowed to execute before being automatically shut down by ASP.NET.



//http://localhost/ReportPublishCrystal/ReportPdfPrint.aspx?ReportName=18^1466&uid=bd40d8a7-4ca4-4844-890e-b095dc66a923&MainReferenceID=1466&ReportDataSourceType=PLMDatabase


//MutipleRef=0

namespace WebApplication1
{
    public partial class ReportPdfPrint : System.Web.UI.Page
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            string allReportFileNmaes = Request.QueryString[DDSetup.QueryReportName];
            string sessionUid = Request.QueryString[DDSetup.ReportParameterUid];
           

            int? userid = DDSetup.GetUserIdFromSessionId(sessionUid);

			ApplicationLog.WriteError("IsReportCompressionActivate:" + DDSetup.ReorptSetup.IsReportCompressionActivate);

			if (!userid.HasValue)
            {
                Response.Write(" access denied ");
                return;
            }
           

    
            string PdmRequestRegisterID = Request.QueryString[DDSetup.QueryPdmRequestRegisterID];

            if (string.IsNullOrEmpty(PdmRequestRegisterID))
            {
                PdmRequestRegisterID = string.Empty;
            }


            string dataSourceType = Request.QueryString[DDSetup.QueryReportDataSourceType];

            string mainreferenceID = Request.QueryString[DDSetup.QueryReportParameterMainReferenceID];
            string masterReferenceId = Request.QueryString[DDSetup.QueryReportParameterMasterReferenceID];


			string reportBatchNumber = Request.QueryString[DDSetup.QueryReportBatchNumber];

			if (string.IsNullOrEmpty(reportBatchNumber))
			{
				reportBatchNumber = string.Empty;
			}






			if (!this.IsPostBack)
            {


                PrintUserReaTimeReport(userid, allReportFileNmaes, PdmRequestRegisterID, dataSourceType, mainreferenceID, masterReferenceId, reportBatchNumber);

            }
        }

//	Teckpack Print
//http://localhost/ReportPublishCrystal/ReportPdfPrint.aspx?ReportName=2^4489&uid=66e7fed1-1dfc-4003-89ad-f0831a0b4138&MainReferenceID=4489&ReportDataSourceType=PLMDatabase





        private void PrintUserReaTimeReport(int ? aUId, string mutipleReportFiles,  string PdmRequestRegisterID, string dataSourceType,string mainreferenceID, string masterReferenceId,string reportBatchNumber)
        {

            //allReportFileNmae=ReportName=Crystal_OSC_GetTab1.rpt^21836|OSC_Proto Summary.rdlx^21863
            if (mutipleReportFiles != string.Empty)
			{
				// only create once !!

				List<Stream> pdfFileStream = new List<Stream>();


				// it is batch print 
				if (!string.IsNullOrWhiteSpace(reportBatchNumber))
				{
					List<string> requestRegistIds = DDSetup.GetPdmRequestRegisterIdsByBatchNimber(reportBatchNumber);

					foreach ( string requestRegisterID in requestRegistIds)
					{

						// only for single report !!
						string singleReportId = mutipleReportFiles;

						List<Stream> pdfFileStreamFromSearchView = PrintSearchViewPdf(aUId, singleReportId, requestRegisterID, dataSourceType, mainreferenceID, masterReferenceId);

						pdfFileStream.AddRange(pdfFileStreamFromSearchView);

					}
				}
				else // it is NOT BATCH print, need to process singe request 
				{
					if (string.IsNullOrWhiteSpace(PdmRequestRegisterID))
					{
						List<Stream> pdfFileStreamTeckpacks = PrintTechPackPdf(aUId, mutipleReportFiles, string.Empty, dataSourceType, mainreferenceID, masterReferenceId);

						pdfFileStream.AddRange(pdfFileStreamTeckpacks);

					}
					else // it is searchView print Calls  for each single request PdmRequestRegisterID
					{
						string singleReportId = mutipleReportFiles;

						List<Stream> pdfFileStreamFromSearchView = PrintSearchViewPdf(aUId, singleReportId, PdmRequestRegisterID, dataSourceType, mainreferenceID, masterReferenceId);

						pdfFileStream.AddRange(pdfFileStreamFromSearchView);
					}


				}







				using (PdfDocument outputPdfDocument = new PdfDocument())
				{

					try
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

						ApplicationLog.WriteError("IsReportCompressionActivate:" + DDSetup.ReorptSetup.IsReportCompressionActivate);

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
					catch (Exception ex)
					{

						Response.Write("Print pdf Failed" + ex.ToString());

					}



				}

				// need to dispsoe output to release memeory
				//  outputPdfDocument.Dispose();
				// need to dispost 


			}
		}

		private static List<Stream> PrintTechPackPdf(int? aUId, string mutipleReportFiles, string PdmRequestRegisterID, string dataSourceType, string mainreferenceID, string masterReferenceId)
		{
			string[] reportnames = mutipleReportFiles.Split('|');

			//  var dictReportNameAndFileName = DDSetup.GetDictUserReportNameAndFileNameReports(aUId.Value);

			List<KeyValuePair<string, string>> mutipleReportAndReference = new List<KeyValuePair<string, string>>();

			List<Stream> pdfFileStream = new List<Stream>();

			foreach (String reportFileNameWithRef in reportnames)
			{

				string[] reportNameAndRef = reportFileNameWithRef.Split('^');
				if (reportNameAndRef.Length != 2)
					continue;





				string reportId = reportNameAndRef[0];


				string reportFileName = DDSetup.GetUserReportFileName(aUId.Value, reportId);

				if (string.IsNullOrEmpty(reportFileName))
				{

					// Response.Write(" access denied ");
					continue;
				}






				string reportRefId = reportNameAndRef[1];


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

			return pdfFileStream;
		}

		//http://localhost/ReportPublishDynamic/DDWebReportView.aspx?ReportName=90&uid=573aa0c3-528a-40a9-890e-701e097a1fb5&PdmRequestRegisterID=134&ReportDataSourceType=PLMDatabase
		//Search Print
		//http://localhost/ReportPublishCrystal/ReportPdfPrint.aspx?ReportName=90&uid=66e7fed1-1dfc-4003-89ad-f0831a0b4138&PdmRequestRegisterID=133&ReportDataSourceType=PLMDatabase

		private static List<Stream> PrintSearchViewPdf(int? aUId, string reportId, string PdmRequestRegisterID, string dataSourceType, string mainreferenceID, string masterReferenceId)
		{
			

			List<Stream> pdfFileStream = new List<Stream>();


				string reportFileName = DDSetup.GetUserReportFileName(aUId.Value, reportId);

				


				// Cyrstal report
				if (reportFileName.EndsWith(".rpt") || reportFileName.EndsWith(".RPT"))
				{

					Stream result = CystalReportExport.GetCrystalPdfStream(reportFileName, aUId, null, PdmRequestRegisterID, dataSourceType, mainreferenceID, masterReferenceId);
					if (result != null)
					{
						pdfFileStream.Add(result);
					}
				}
				// Data Dynamics 
				else if (reportFileName.EndsWith(".rdlx") || reportFileName.EndsWith(".RDLX"))
				{
					Stream result = DataDynamicsExport.GetDataDynamicPdfStream(reportFileName, aUId, null, PdmRequestRegisterID, dataSourceType, mainreferenceID, masterReferenceId);

					if (result != null)
					{
						pdfFileStream.Add(result);
					}

				}
		

			return pdfFileStream;
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