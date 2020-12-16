using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
//using DynamicWebReportView;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using DynamicWebReportView;
using Com.Visual2000.SystemFramework;
//using DynamicWebReportView;

// http://localhost/MutipleHybridReport/MutipleReportJobPrint.aspx?ReportName=Test Crystal Report^15210|Test Dynamic Report^15210&uid=26a1f8cf-f291-456e-88a5-f8791b86caeb&MainReferenceID=15210&ReportDataSourceType=DWDatabase&ReportJobName=my test report job name 

namespace ReportViewSetup
{
    public class ReportJobManagement
    {
       
       internal static void PrintUserReportJob(ReportJobDesc aReportJobDesc)
        {
            string reportJobName = aReportJobDesc.ReportJobName;
            int? aUId = aReportJobDesc.UId;
            string allReportFileNameAndReferenceID = aReportJobDesc.AllReportFileNmae;
           // string productReferenceId = aReportJobDesc.ProductReferenceId;
            string PdmRequestRegisterID = aReportJobDesc.PdmRequestRegisterID;
            string dataSourceType = aReportJobDesc.DataSourceType;

             string mainReferenceID = aReportJobDesc.MainReferenceID;
             string masterReferenceID = aReportJobDesc.MasterReferenceID;

          




            if (allReportFileNameAndReferenceID != string.Empty)
            {
                // only create once !!

               // var dictReportNameAndFileName = DDSetup.GetDictUserReportNameAndFileNameReports(aUId.Value);

                string[] reportnames = allReportFileNameAndReferenceID.Split('|');

                List<Stream> pdfFileStream = new List<Stream>();

                foreach (String reportFileNameWithRef in reportnames)
                {

                    if (string.IsNullOrEmpty(reportFileNameWithRef))
                        continue;

                    string[] reportNameAndRef = reportFileNameWithRef.Split('^');
                    if (reportNameAndRef.Length != 2)
                        continue;

                    string reportName = reportNameAndRef[0];

                    string reportFileName = DDSetup.GetUserReportFileName(aUId.Value, reportName);

                    if (string.IsNullOrEmpty(reportFileName))
                    {
                        continue;
                    }





                    string reportRefId = reportNameAndRef[1];




                    // Cyrstal report
                    if (reportFileName.EndsWith(".rpt") || reportFileName.EndsWith(".RPT"))
                    {

                        Stream result = CystalReportExport.GetCrystalPdfStream(reportFileName, aUId, reportRefId, PdmRequestRegisterID, dataSourceType, mainReferenceID, masterReferenceID);
                        if (result != null)
                        {
                            pdfFileStream.Add(result);
                        }
                    }
                    // Data Dynamics 
                    else if (reportFileName.EndsWith(".rdlx") || reportFileName.EndsWith(".RDLX"))
                    {
                        Stream result = DataDynamicsExport.GetDataDynamicPdfStream(reportFileName, aUId, reportRefId, PdmRequestRegisterID, dataSourceType, mainReferenceID, masterReferenceID);

                        if (result != null)
                        {
                            pdfFileStream.Add(result);
                        }

                    }
                }


             
                string fileID = Guid.NewGuid().ToString();
                string needToSaveDBFileName = string.Empty ;
                string FileNameOrigin = string.Empty;

                using (PdfDocument outputDocument = new PdfDocument())
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
                                    outputDocument.AddPage(page);
                                }
                                stream.Close();
                                stream.Dispose();

                            }
                        }
                    }
                
                    FileNameOrigin = DDSetup.ReorptSetup.ReportPdfCompressPath + "Origin_" + fileID + ".pdf";
                    needToSaveDBFileName = FileNameOrigin;
                    outputDocument.Save(FileNameOrigin);
               
                }

                if (DDSetup.ReorptSetup.IsReportCompressionActivate)
                {

                  string FileNameDestination = DDSetup.ReorptSetup.ReportPdfCompressPath + "Dest_" + fileID + ".pdf";

                  bool result = ReportJobManagement.PdfCompression(FileNameOrigin, FileNameDestination);
                 if (result)
                 {
                     File.Delete(FileNameOrigin);
                     needToSaveDBFileName = FileNameDestination;  
                 
                 }
                }

                using (SqlConnection conn = new SqlConnection(DDSetup.PLMConnectionString))
                {
                    conn.Open();
                    string insertsql = "Insert into [pdmPrintJob] (CreatedBy,CreatedDate,Name,FileName) VALUES( @CreatedBy, @CreatedDate,@Name, @FileName)";

                    SqlCommand insertCmd = new SqlCommand(insertsql, conn);

                    insertCmd.Parameters.Add("@CreatedBy",aUId);  

                    insertCmd.Parameters.Add("@CreatedDate", System.DateTime.UtcNow);

                    insertCmd.Parameters.Add("@Name", reportJobName);

                    insertCmd.Parameters.Add("@FileName", needToSaveDBFileName);
   
                    insertCmd.ExecuteNonQuery();

                    conn.Close();

                }
            }
        }

       public static bool PdfCompression(string FileNameOrigin, string FileNameDestination)
        {

            string GhostScriptPath = DDSetup.ReorptSetup.GhostScriptPath;

         //   GhostScriptPath = @"C:\Program Files (x86)\gs\gs9.00\bin\gswin32.exe";

            //  string FileNameOrigin = path + "\\" + reportId + ".pdf";
            //   string FileNameDestination = path + "\\" + reportId + "_compressed.pdf";

            FileNameOrigin = "\"" + FileNameOrigin + "\"";
            FileNameDestination = "\"" + FileNameDestination + "\"";

            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();

                process.StartInfo.FileName = "\"" + GhostScriptPath + "\"";
                //version slow but works without bleu screen on pictures ...
                process.StartInfo.Arguments = GetPdfCompressionSetting(FileNameOrigin, FileNameDestination, DDSetup.ReorptSetup.PdfCompressionSetting);
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                process.StartInfo.CreateNoWindow = false;

                process.Start();
                process.WaitForExit();

                return true;
            }
            catch (Exception ex)
            {
                string error = "Filename Origin " + FileNameOrigin + " - File Destination : " + FileNameDestination + " -  GhostScriptPath : " + GhostScriptPath;
				// Logger.LogLocation = @"C:\temp\ErrorPRINTJOB.txt";
				//  Logger.LogException("Error in print job scanner.", ex, LogCategory.Critical);
				//  Logger.LogException("Error in print job scanner.", ex, LogCategory.Critical);

				ApplicationLog.WriteError("error:" + error);

				ApplicationLog.WriteError("FileName name: " + GhostScriptPath);


				ApplicationLog.WriteError("augument name: " + GetPdfCompressionSetting(FileNameOrigin, FileNameDestination, DDSetup.ReorptSetup.PdfCompressionSetting));






				return false;
            }

        }
       internal static string GetPdfCompressionSetting(string FileNameOrigin, string FileNameDestination, string PdfCompressionSetting)
        {
            //string settings = " -q -dSAFER -dNOPAUSE -dBATCH -sDEVICE=pdfwrite -dColorConversionStrategy=/sRGB -dPDFSETTINGS=";

            string settings = " -q -dSAFER -dNOPAUSE -dBATCH -sDEVICE=pdfwrite -dPDFSETTINGS=";

            if (PdfCompressionSetting.Replace(" ", "") != "")
            {
                settings += PdfCompressionSetting.Replace(" ", "");
            }
            else
            {
                settings += "/printer";
            }

            settings += " -sOUTPUTFILE=" + FileNameDestination + " -f " + FileNameOrigin;

            return settings;
        }

    
    
       

    }
}
