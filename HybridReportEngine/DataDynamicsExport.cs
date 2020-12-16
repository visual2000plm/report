using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataDynamics.Reports.Rendering.Excel;
using DataDynamics.Reports.Rendering.Excel.ExcelTemplateGenerator;
using System.IO;
using DataDynamics.Reports;
using DataDynamics.Reports.Rendering.IO;
using DataDynamics.Reports.Rendering.Word;
using DataDynamics.Reports.Rendering.Pdf;
using ReportViewSetup;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using DynamicWebReportView;
using DataDynamicsWebReportView;
using System.Data;
//Namespace:  System.IO.Packaging
//Assembly:  WindowsBase (in WindowsBase.dll)

namespace DynamicWebReportView
{
	public class DataDynamicsExport
	{
		public static Stream GetDataDynamicPdfStream(string rdlxFileName, int? aUId, string productReferenceId, string PdmRequestRegisterID, string dataSourceType, string mainReferenceID, string masterReferenceID)
		{
			string fullpath = DDSetup.ReorptSetup.ReportRootPath + @"\" + rdlxFileName;

			try
			{
				PdfRenderingExtension device = new PdfRenderingExtension();

				ReportDefinition def = new ReportDefinition(new FileInfo(fullpath));

				ChangDataSourceInRentime(def, dataSourceType);

				using (ReportRuntime runtime = new ReportRuntime(def))
				{

					SetupReportParamter(runtime, aUId, productReferenceId, PdmRequestRegisterID, mainReferenceID, masterReferenceID);

					//   FileStreamProvider aFileStreamProvider=    new FileStreamProvider(new DirectoryInfo(@".\"), exportFieName);

					DataDynamics.Reports.Rendering.IO.MemoryStreamProvider memoryStreamProvider = new MemoryStreamProvider();
					runtime.Render(device, memoryStreamProvider);
					var pInfo = memoryStreamProvider.GetPrimaryStream();
					return pInfo.OpenStream();

				}


				//  ReportRuntime runtime = new ReportRuntime(rdl);

			}

			catch (Exception ex)
			{
				string exStrt = ex.ToString();
				return null;

			}

		}


		public static void ChangDataSourceInRentime(ReportDefinition rdl, string dataSourceType)
		{
			//need to centralize all DataSource in  one Report source,so report source    dynamically change report datasource on the fly

			for (int i = 0; i < rdl.Report.DataSources.Count; i++)
			{

				if (dataSourceType == EmReportDataSourceType.DWDatabase.ToString())
				{
					rdl.Report.DataSources[i].DataSourceReference = null;

					rdl.Report.DataSources[i].ConnectionProperties.ConnectString = DDSetup.DWDataSourceConnectionString;
					rdl.Report.DataSources[i].ConnectionProperties.DataProvider = "SQL";
				}
				else
				{
					rdl.Report.DataSources[i].DataSourceReference = null;

					rdl.Report.DataSources[i].ConnectionProperties.ConnectString = DDSetup.PLMConnectionString;
					rdl.Report.DataSources[i].ConnectionProperties.DataProvider = "SQL";

				}


			}

			//rdl.Report.
			rdl.ResourceLocator = new DataDynamicsCustomResourceLocator(rdl.ResourceLocator, dataSourceType);
		}





		private static void SetupReportParamter(ReportRuntime runtime, int? aUId, string productReferenceId, string PdmRequestRegisterID, string mainReferenceID, string masterReferenceID)
		{
			DDSetup.SetupDDReportRuntimeParameter(runtime, aUId);

			if (runtime.Parameters[DDSetup.ReportParameterProductReferenceID] != null)
			{

				runtime.Parameters[DDSetup.ReportParameterProductReferenceID].CurrentValue = productReferenceId;

			}

			// new add string mainReferenceID, string masterReferenceID 2014-02-17
			if (runtime.Parameters[DDSetup.QueryReportParameterMainReferenceID] != null)
			{

				runtime.Parameters[DDSetup.QueryReportParameterMainReferenceID].CurrentValue = mainReferenceID;

			}

			if (runtime.Parameters[DDSetup.QueryReportParameterMasterReferenceID] != null)
			{

				runtime.Parameters[DDSetup.QueryReportParameterMasterReferenceID].CurrentValue = masterReferenceID;

			}




			if (runtime.Parameters[DDSetup.ReportParameterImageUrl] != null)
			{
				runtime.Parameters[DDSetup.ReportParameterImageUrl].CurrentValue = DDSetup.ReorptSetup.ReportingServerImageUrl;
			}



			if (!string.IsNullOrEmpty(PdmRequestRegisterID))
			{
				// need format like this '1,2,3,4,6'
				//string requestContent = DDSetup.GetPdmRequestContent(PdmRequestRegisterID);




				//if (runtime.Parameters[DDSetup.ReportParameterMutipleProductReferenceIDs] != null)
				//{
				//    runtime.Parameters[DDSetup.ReportParameterMutipleProductReferenceIDs].CurrentValue = requestContent;
				//    // runtime.Parameters[DDSetup.ReportParameterMutipleProductReferenceIDs].Hidden
				//}
				//else// try to set ReportParameterProductReferenceID
				//{

				//    runtime.Parameters[DDSetup.ReportParameterProductReferenceID].CurrentValue = requestContent;

				//}


				DDSetup.SetupDynamicReportRequesttRegisterId(runtime, PdmRequestRegisterID);
			}
		}





		private static void ZipFiles(string _zipFileName, string folderPath)
		{
			Crc32 crc = new Crc32();
			ZipOutputStream zs = null;

			try
			{


				using (zs = new ZipOutputStream(File.Create(_zipFileName)))
				{
					byte[] buffer = new byte[32768];

					foreach (string name in Directory.GetFileSystemEntries(folderPath, "*.pdf"))
					{

						ZipOnefile(crc, zs, buffer, name);
					} // foreach file
					zs.Finish();
				}

				//return true;
			}
			catch (Exception exc)
			{

				// return false;
			}
			finally
			{
				if (zs != null)
					zs.Close();
			}
		}

		private static void ZipOnefile(Crc32 crc, ZipOutputStream zs, byte[] buffer, string fileName)
		{
			FileInfo file = new FileInfo(fileName);


			using (Stream fs = file.OpenRead())
			{
				AddOneStreamToZipOutputStream(crc, zs, buffer, fileName, fs);
			}

		}

		private static void AddOneStreamToZipOutputStream(Crc32 crc32, ZipOutputStream zipOutputStream, byte[] buffer, string fileName, Stream appendStream)
		{
			ZipEntry entry = new ZipEntry(ZipEntry.CleanName(fileName));
			entry.DateTime = System.DateTime.Now;
			entry.Size = appendStream.Length;


			crc32.Reset();
			long len = appendStream.Length;
			while (len > 0)
			{
				int readSoFar = appendStream.Read(buffer, 0, buffer.Length);
				crc32.Update(buffer, 0, readSoFar);
				len -= readSoFar;
			}
			entry.Crc = crc32.Value;
			zipOutputStream.PutNextEntry(entry);

			// add appendStream to the 
			len = appendStream.Length;
			appendStream.Seek(0, SeekOrigin.Begin);
			while (len > 0)
			{
				int readSoFar = appendStream.Read(buffer, 0, buffer.Length);
				zipOutputStream.Write(buffer, 0, readSoFar);
				len -= readSoFar;
			}
		}

	}
}