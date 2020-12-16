using System;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Xml;
using DataDynamics.Reports;
using DataDynamics.Reports.Extensibility;
using DataDynamics.Reports.Rendering.Excel;
using DataDynamics.Reports.Rendering.Excel.ExcelTemplateGenerator;
using DataDynamics.Reports.Rendering.IO;
using ReportViewSetup;

// //http://localhost/ReportPublishCrystal/DDWebReportView.aspx?ReportName=Report1.rdlx&uid=1&ProductReferenceID=75282&ReportDataSourceType=PLMDatabase
           
namespace DynamicWebReportView
{
    public partial class DDWebReportView : System.Web.UI.Page
    {

        //Request.QueryString[DDSetup.ReportParameterProductReferenceID];
        private string _DatasourceType = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Report1.rdlx
        }

        protected void GeneratePDF(object sender, EventArgs e)
        {
        }

        public string ConvertRelativeUrlToAbsoluteUrl(string relativeUrl)
        {
            return string.Format("http{0}://{1}{2}", (Request.IsSecureConnection) ? "s" : "", Request.Url.Host, Page.ResolveUrl(relativeUrl));
        }

        protected override void OnInit(EventArgs e)
        {
            string reportName = Request.QueryString[DDSetup.QueryReportName];
            string aUId = Request.QueryString[DDSetup.ReportParameterUid];
            _DatasourceType = Request.QueryString[DDSetup.QueryReportDataSourceType];


            if (string.IsNullOrEmpty(reportName) || string.IsNullOrEmpty(aUId))
            {
                Response.Write(" Can not find the report");
                return;
            }

            if (reportName.EndsWith(".rdlx") || reportName.EndsWith(".RDLX"))
            {  
                // RequestID
                //PdmRequestRegisterID
                string reportFileNmae = DDSetup.ReorptSetup.ReportRootPath + @"\" + Request.QueryString[DDSetup.QueryReportName];

                //

                FileInfo reportFile = new FileInfo(reportFileNmae);
                ReportDefinition rdl = new ReportDefinition(reportFile);

                DataDynamicsExport.ChangDataSourceInRentime(rdl, _DatasourceType);

                ReportRuntime runtime = new ReportRuntime(rdl);

                DDSetup.SetupDDReportRuntimeParameter(runtime, aUId);

                if (runtime.Parameters[DDSetup.ReportParameterProductReferenceID] != null)
                {
                    string productReferenceId = Request.QueryString[DDSetup.ReportParameterProductReferenceID];
                    if (!string.IsNullOrEmpty(productReferenceId))
                    {
                        if (runtime.Parameters[DDSetup.ReportParameterProductReferenceID] != null)
                        {
                            runtime.Parameters[DDSetup.ReportParameterProductReferenceID].CurrentValue = int.Parse(productReferenceId);
                        }
                        else // try to setup ReportParameterMutipleProductReferenceIDs
                        {

                            runtime.Parameters[DDSetup.ReportParameterMutipleProductReferenceIDs].CurrentValue = productReferenceId;
                        
                        }
               
                    }
                }

                // new added MainReferenceID 2014-01-17
                if (runtime.Parameters[DDSetup.ReportParameterMainReferenceID] != null)
                {
                    string mainReferenceID = Request.QueryString[DDSetup.ReportParameterMainReferenceID];
                    if (!string.IsNullOrEmpty(mainReferenceID))
                    {

                        runtime.Parameters[DDSetup.ReportParameterMainReferenceID].CurrentValue = int.Parse(mainReferenceID);
                        
                       

                    }
                }

                // new added ReportParameter MasterReferenceID 2014-01-17
                if (runtime.Parameters[DDSetup.ReportParameterMasterReferenceID] != null)
                {
                    string masterReferenceID = Request.QueryString[DDSetup.ReportParameterMasterReferenceID];
                    if (!string.IsNullOrEmpty(masterReferenceID))
                    {

                        runtime.Parameters[DDSetup.ReportParameterMasterReferenceID].CurrentValue = int.Parse(masterReferenceID);



                    }
                }

                // new added reportparamter 


                if (runtime.Parameters[DDSetup.ReportParameterImageUrl] != null)
                {
                    runtime.Parameters[DDSetup.ReportParameterImageUrl].CurrentValue = DDSetup.ReorptSetup.ReportingServerImageUrl;
                }

                string PdmRequestRegisterID = Request.QueryString[DDSetup.PdmRequestRegisterID];
                if (!string.IsNullOrEmpty(PdmRequestRegisterID))
                {
                    // need format like this '1,2,3,4,6'
                    string requestContent = DDSetup.GetPdmRequestContent(PdmRequestRegisterID);

                    if (runtime.Parameters[DDSetup.ReportParameterMutipleProductReferenceIDs] != null)
                    {
                        runtime.Parameters[DDSetup.ReportParameterMutipleProductReferenceIDs].CurrentValue = requestContent;
                        // runtime.Parameters[DDSetup.ReportParameterMutipleProductReferenceIDs].Hidden
                    }
                    else// try to set ReportParameterProductReferenceID
                    {

                        runtime.Parameters[DDSetup.ReportParameterProductReferenceID].CurrentValue = requestContent;
                    
                    }
                }

           
                this.WebReportViewer2.SetReport(runtime);
            }
            else if (reportName.EndsWith(".rpt") || reportName.EndsWith(".RPT"))
            {
                string cystalURL = string.Empty;
                string lastCharOncrystalurl = DDSetup.ReorptSetup.CrystalReportPublishUrl.Substring(DDSetup.ReorptSetup.CrystalReportPublishUrl.Length - 1, 1);
                if (lastCharOncrystalurl == @"/")
                {
                    cystalURL = DDSetup.ReorptSetup.CrystalReportPublishUrl + "CrystalReportView.aspx?" + DDSetup.QueryReportName + "=" + reportName + "&" + DDSetup.ReportParameterUid + "=" + aUId;
                }
                else
                {
                    cystalURL = DDSetup.ReorptSetup.CrystalReportPublishUrl + "/CrystalReportView.aspx?" + DDSetup.QueryReportName + "=" + reportName + "&" + DDSetup.ReportParameterUid + "=" + aUId;
                }

                this.Response.Redirect(cystalURL, true);
            }
            else
            {
                Response.Write(" Can not find the report");
            }
        }

       
        private static void ExportToExcelFile(string rdlxFileName, string exportFieName, string uid)
        {
            ExcelTransformationDevice device = new ExcelTransformationDevice();
            ExcelTemplateGenerator template = new ExcelTemplateGenerator();
            MemoryStream templateStream = new MemoryStream();

            //  rdlxFileName = DDSetup.ReorptSetup.ReportRootPath + @"\" + rdlxFileName;
            ReportDefinition def = new ReportDefinition(new FileInfo(rdlxFileName));
            template.GenerateTemplate(def, templateStream);
            templateStream.Position = 0;
            device.TemplateStream = templateStream;
            ReportRuntime runtime = new ReportRuntime(def);

            DDSetup.SetupDDReportRuntimeParameter(runtime, uid);

            runtime.Render(device, new FileStreamProvider(new DirectoryInfo(@".\OutPutExcel"), exportFieName));
        }


    }

 
}