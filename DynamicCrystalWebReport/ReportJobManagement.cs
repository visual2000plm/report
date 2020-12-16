using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using DynamicWebReportView;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;

//http://localhost/ReportPublishCrystal/MutipleCrystalReportPdfView.aspx?ReportName=Test1CrystalReport.rpt|Test2CrystalReport.rpt&uid=1&ProductReferenceID=75282&ReportDataSourceType=PLMDatabase
//http://localhost/ReportPublishCrystal/MutipleCrystalReportPdfView.aspx?ReportName=Test1CrystalReport.rpt|Test2CrystalReport.rpt|Report1.rdlx|CrystalGetTab1.rpt&uid=1&ProductReferenceID=75282&ReportDataSourceType=PLMDatabase

namespace ReportViewSetup
{

    public class ReportJobProcessor
    {
    
        public static readonly Queue<ReportJobDesc> ReportJobQueue = new Queue<ReportJobDesc>();
        public static readonly string reportExecPrintLocaltion = @"C:\Development_Workspace\PLMS Project\PLM Report Solution\DynamicWebReportView\ReportPrintExe\bin\Debug\ReportPrintExe.exe";

        static ReportJobProcessor()
        {
            //   = new Thread (

            Thread bkThread = new Thread(() =>
              {

                  PorcessReportJob();

              });
            bkThread.IsBackground = true;
            bkThread.Start();

        }

        static void PorcessReportJob()
        {
            while (true)
            {
                while (ReportJobQueue.Count > 0)
                {
                    ReportJobDesc aReportJobDesc = ReportJobQueue.Dequeue();
                    string atgements = aReportJobDesc.ReportJobName + "," + aReportJobDesc.UId + "," + aReportJobDesc.AllReportFileNmae + "," + aReportJobDesc.ProductReferenceId + "," + aReportJobDesc.PdmRequestRegisterID + "," + aReportJobDesc.DataSourceType;
                    System.Console.WriteLine(atgements); 
                    Process process = new Process();
                    process.StartInfo.FileName = reportExecPrintLocaltion;
                    //   process.StartInfo.Arguments = GetPdfCompressionSetting(FileNameOrigin, FileNameDestination, DDSetup.ReorptSetup.PdfCompressionSetting);
                    process.StartInfo.Arguments =   "\""+ atgements+"\"";
                    process.StartInfo.UseShellExecute = true;
                 //   process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    process.WaitForExit();
                }

                Thread.Sleep(10 * 1000);

            }
        }



    }
}
