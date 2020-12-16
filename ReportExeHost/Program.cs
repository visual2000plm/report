using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ReportViewSetup;
using System.Diagnostics;

namespace ReportExeHost
{
    class Program
    {
        static void Main(string[] args)
        {
            StartProgram();
            System.Console.ReadLine();
  
        }


        public static readonly Queue<ReportJobDesc> ReportJobQueue = new Queue<ReportJobDesc>();
        public static readonly string reportExecPrintLocaltion = @"C:\Development_Workspace\PLMS Project\PLM Report Solution\DynamicWebReportView\ReportPrintExe\bin\Debug\HybridReportEngine.exe";
      
        static void StartProgram()
        {
            //   = new Thread (

            AddJobToQueue();
            AddJobToQueue();
            Thread bkThread = new Thread(() =>
              {

                  PorcessReportJob();

              });
            bkThread.IsBackground = true;
            bkThread.Start();

        }


        static void TestExtreal()
        {
            Process ExternalProcess = new Process();
            ExternalProcess.StartInfo.FileName = "Notepad.exe";
            ExternalProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            ExternalProcess.Start();
            ExternalProcess.WaitForExit();

        }

        static void PorcessReportJob()
        {
            while (true)
            {
                while (ReportJobQueue.Count > 0)
                {
                    ReportJobDesc aReportJobDesc = ReportJobQueue.Dequeue();
                    string atgements = "\""+aReportJobDesc.ReportJobName + "," + aReportJobDesc.UId + "," + aReportJobDesc.AllReportFileNmae + "," + aReportJobDesc.ProductReferenceId + "," + aReportJobDesc.PdmRequestRegisterID + "," + aReportJobDesc.DataSourceType +"\"";
                    Process process = new Process();
                    process.StartInfo.FileName = reportExecPrintLocaltion;
                    process.StartInfo.Arguments = atgements;
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    process.WaitForExit();
                }
               
                Thread.Sleep(1 * 1000);

            }
        }

        static void AddJobToQueue()
        {



            // need to from Database 
            //select PrintJobID ,CreatedDate  from  [pdmPrintJob] where [filename] is null  order by CreatedDate desc
            string atgements = "Ref,1,OSC_Proto Summary.rdlx|OSC_First_Offline_Report.rdlx,21836,,PLMDatabase";

            string[] inPutPara = atgements.Split(',');
            if (inPutPara.Length != 6)
                return;

            ReportJobDesc aReportJobDesc = new ReportJobDesc();
            aReportJobDesc.ReportJobName = inPutPara[0];
            aReportJobDesc.UId =  ControlTypeValueConverter.ConvertValueToInt( inPutPara[1]);
            aReportJobDesc.AllReportFileNmae = inPutPara[2];
            aReportJobDesc.ProductReferenceId = inPutPara[3];
            aReportJobDesc.PdmRequestRegisterID = inPutPara[4];
            aReportJobDesc.DataSourceType = inPutPara[5];

            ReportJobQueue.Enqueue(aReportJobDesc);

        }


    }
    


}
