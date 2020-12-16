using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReportViewSetup;
using System.IO;

namespace ReportPrintExe
{
    class Program
    {
        static void Main(string[] args)
        {


            if (args == null || args.Length == 0)
            {
                System.Console.Write("cannot find args");  
                return;
            }
            string atgements = args[0];

          //  string atgements = "Ref,1,Crystal_OSC_GetTab1.rpt|OSC_Proto Summary.rdlx|OSC_First_Offline_Report.rdlx,21836,,PLMDatabase";
            //HybridReportEngine "Ref# 21836 PDP Test 1,1,Crystal_OSC_GetTab1.rpt^21836,,PLMDatabase"
           

            string[] inPutPara = atgements.Split(',');
            if (inPutPara.Length != 5)
                return;


            Application_Start();
            ReportJobDesc aReportJobDesc = new ReportJobDesc();
            aReportJobDesc.ReportJobName = inPutPara[0];
            aReportJobDesc.UId = ControlTypeValueConverter.ConvertValueToInt( inPutPara[1]);
            aReportJobDesc.AllReportFileNmae = inPutPara[2];
      
            aReportJobDesc.PdmRequestRegisterID = inPutPara[3];
            aReportJobDesc.DataSourceType = inPutPara[4];
            aReportJobDesc.MainReferenceID = inPutPara[5];
            aReportJobDesc.MasterReferenceID = inPutPara[6];

 


            ReportJobManagement.PrintUserReportJob(aReportJobDesc);

          //  File.WriteAllLines("c:\\Test.txt", new string[] { atgements });

        }

        protected static void Application_Start()
        {
            //string PLMConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PLMConnectionString"].ConnectionString;

           // string DWDataSourceConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DWDataSourceConnectionString"].ConnectionString;


            //DDSetup.InitAppSetup(PLMConnectionString, DWDataSourceConnectionString);
        }

    }
}
