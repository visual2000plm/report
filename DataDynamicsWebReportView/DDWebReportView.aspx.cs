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
using System.Collections.Generic;
using System.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using DataDynamics.Reports.Expressions.ExpressionObjectModel;


// //http://localhost/ReportPublishCrystal/DDWebReportView.aspx?ReportName=Report1.rdlx&uid=1&ProductReferenceID=75282&ReportDataSourceType=PLMDatabase
//http://localhost/ReportPublishDynamic/DDWebReportView.aspx?ReportName=202&uid=8b5fdf73-b04e-4ede-9e5a-4151ab7d36aa&ReportDataSourceType=PLMDatabase

//reference#                         12852


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

		protected override void OnInit(EventArgs e)
		{
			// test traceing
			// DDSetup.WriteReportTrace("test admin", " testtt");
			string reportName = Request.QueryString[DDSetup.QueryReportName];
			string sessionUid = Request.QueryString[DDSetup.ReportParameterUid];
			_DatasourceType = Request.QueryString[DDSetup.QueryReportDataSourceType];


			string reportFileName = string.Empty;

			int? userid = DDSetup.GetUserIdFromSessionId(sessionUid);


			if (!userid.HasValue)
			{
				Response.Write(" access denied ");
				return;
			}
			else
			{
				reportFileName = DDSetup.GetUserReportFileName(userid.Value, reportName);

				if (string.IsNullOrEmpty(reportFileName))
				{

					Response.Write(" access denied ");
					return;
				}

			}
			string reportFileNmae = DDSetup.ReorptSetup.ReportRootPath + @"\" + reportFileName;

			FileInfo reportFile = new FileInfo(reportFileNmae);
			ReportDefinition rdl = new ReportDefinition(reportFile);
			ReportRuntime runtime = new ReportRuntime(rdl);

			DataDynamicsExport.ChangDataSourceInRentime(rdl, _DatasourceType);



			DDSetup.SetupDDReportRuntimeParameter(runtime, userid);

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
			if (runtime.Parameters[DDSetup.QueryReportParameterMainReferenceID] != null)
			{
				string mainReferenceID = Request.QueryString[DDSetup.QueryReportParameterMainReferenceID];
				if (!string.IsNullOrEmpty(mainReferenceID))
				{

					runtime.Parameters[DDSetup.QueryReportParameterMainReferenceID].CurrentValue = int.Parse(mainReferenceID);



				}
			}

			// new added ReportParameter MasterReferenceID 2014-01-17
			if (runtime.Parameters[DDSetup.QueryReportParameterMasterReferenceID] != null)
			{
				string masterReferenceID = Request.QueryString[DDSetup.QueryReportParameterMasterReferenceID];
				if (!string.IsNullOrEmpty(masterReferenceID))
				{

					runtime.Parameters[DDSetup.QueryReportParameterMasterReferenceID].CurrentValue = int.Parse(masterReferenceID);



				}
			}

			// new added reportparamter 


			if (runtime.Parameters[DDSetup.ReportParameterImageUrl] != null)
			{
				runtime.Parameters[DDSetup.ReportParameterImageUrl].CurrentValue = DDSetup.ReorptSetup.ReportingServerImageUrl;
			}

			string PdmRequestRegisterID = Request.QueryString[DDSetup.QueryPdmRequestRegisterID];
			if (!string.IsNullOrEmpty(PdmRequestRegisterID))
			{
				// need format like this '1,2,3,4,6'
				DDSetup.SetupDynamicReportRequesttRegisterId(runtime, PdmRequestRegisterID);

			}


			this.WebReportViewer2.SetReport(runtime);


		}

	


		//public string ConvertRelativeUrlToAbsoluteUrl(string relativeUrl)
		//{
		//    return string.Format("http{0}://{1}{2}", (Request.IsSecureConnection) ? "s" : "", Request.Url.Host, Page.ResolveUrl(relativeUrl));
		//}
	}


}