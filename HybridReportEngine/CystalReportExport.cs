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
using System.Data;
//using DynamicWebReportView;

//http://localhost/ReportPublishCrystal/MutipleCrystalReportPdfView.aspx?ReportName=Test1CrystalReport.rpt|Test2CrystalReport.rpt&uid=1&ProductReferenceID=75282&ReportDataSourceType=PLMDatabase
//http://localhost/ReportPublishCrystal/MutipleCrystalReportPdfView.aspx?ReportName=Test1CrystalReport.rpt|Test2CrystalReport.rpt|Report1.rdlx|CrystalGetTab1.rpt&uid=1&ProductReferenceID=75282&ReportDataSourceType=PLMDatabase

namespace ReportViewSetup
{
    public class CystalReportExport
    {
        public static Stream GetCrystalPdfStream(string reportFileNmae, int? aUId, string productReferenceId, string PdmRequestRegisterID, string dataSourceType,string mainReferenceID, string masterReferenceID)
        {
            //
            using (ReportDocument reportDocument = BindCrystalReport(reportFileNmae, dataSourceType))
            {
                if (reportDocument != null)
                {
                    ConfigureCrystalReportParamter(reportDocument, aUId, productReferenceId, PdmRequestRegisterID, mainReferenceID, masterReferenceID);

                    try
                    {
                        //return reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                        Stream reportSteam = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);
                        //reportDocument.Close();
                        //reportDocument.Dispose();
                        return reportSteam;

                    }
                    catch
                    {
                        //reportDocument.Close();
                        //reportDocument.Dispose();
                        return null;

                    };
                }

            }

            return null;

        }

        private static ReportDocument BindCrystalReport(string reportFileNmae, string datasourceType)
        {
            ConnectionInfo ConnInfo = new ConnectionInfo();


            if (datasourceType == EmReportDataSourceType.DWDatabase.ToString())
            {

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(DDSetup.DWDataSourceConnectionString);
                ConnInfo.UserID = builder.UserID;
                ConnInfo.Password = builder.Password;
                ConnInfo.DatabaseName = builder.InitialCatalog;
                ConnInfo.ServerName = builder.DataSource;

            }
            else
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(DDSetup.PLMConnectionString);
                ConnInfo.UserID = builder.UserID;
                ConnInfo.Password = builder.Password;
                ConnInfo.DatabaseName = builder.InitialCatalog;
                ConnInfo.ServerName = builder.DataSource;

            }


            ReportDocument reportDocument = new ReportDocument();

            string fullpath = DDSetup.ReorptSetup.ReportRootPath + @"\" + reportFileNmae;

            try
            {
                // cannot find the report !
                reportDocument.Load(fullpath);


                Tables RepTbls = reportDocument.Database.Tables;
                foreach (CrystalDecisions.CrystalReports.Engine.Table RepTbl in RepTbls)
                {
                    TableLogOnInfo RepTblLogonInfo = RepTbl.LogOnInfo;
                    RepTblLogonInfo.ConnectionInfo = ConnInfo;
                    RepTbl.ApplyLogOnInfo(RepTblLogonInfo);
                }

                return reportDocument;

            }
            catch
            {
                return null;
            }



        }

        private static void ConfigureCrystalReportParamter(ReportDocument crystalReportDocument, int? aUId, string productReferenceId, string PdmRequestRegisterID, string mainReferenceID, string masterReferenceID)
        {
            if (aUId.HasValue)
            {
                SetupReportParameter(crystalReportDocument, DDSetup.ReportParameterUid, aUId);

                string userName;
                int? v2kuid;
                string timeZoneInfoToken = string.Empty;
                EmDomainType aDomainType = DDSetup.GetDomainTypeAndV2kUID(aUId, out v2kuid, out userName, out timeZoneInfoToken);


				if (!string.IsNullOrEmpty(userName))
				{
					if (IsExistParaName(crystalReportDocument, DDSetup.ReportParameterCurrentUserName))
					{
						SetupReportParameter(crystalReportDocument, DDSetup.ReportParameterCurrentUserName, userName);
					}
					else// try to setup _MutipleProductReferenceIDs
					{

						SetupReportParameter(crystalReportDocument, DDSetup.ReportParameterCurrentUserName, userName);
					}

				}

				if (v2kuid.HasValue)
                {
                    if (aDomainType == EmDomainType.Vendor)
                    {
                        SetupReportParameter(crystalReportDocument, DDSetup.ReportParameterVendorID, v2kuid.Value);
                    }
                    else if (aDomainType == EmDomainType.Customer)
                    {
                    }
                }

                if (!string.IsNullOrEmpty(productReferenceId))
                {
                    if (IsExistParaName(crystalReportDocument, DDSetup.ReportParameterProductReferenceID))
                    {
                        SetupReportParameter(crystalReportDocument, DDSetup.ReportParameterProductReferenceID, productReferenceId);
                    }
                    else// try to setup _MutipleProductReferenceIDs
                    {

                        SetupReportParameter(crystalReportDocument, DDSetup.ReportParameterMutipleProductReferenceIDs, productReferenceId);
                    }

                }

                // need 2014-02-17  to add two paramters to mainReferenceID,masterReferenceID


                if (!string.IsNullOrEmpty(mainReferenceID))
                {
                    if (IsExistParaName(crystalReportDocument, DDSetup.QueryReportParameterMainReferenceID))
                    {
                        SetupReportParameter(crystalReportDocument, DDSetup.QueryReportParameterMainReferenceID, mainReferenceID);
                    }
                   

                }

                if (!string.IsNullOrEmpty(timeZoneInfoToken))
                {
                    if (IsExistParaName(crystalReportDocument, DDSetup.ReportParameterClientTimeZonekey))
                    {
                        SetupReportParameter(crystalReportDocument, DDSetup.ReportParameterClientTimeZonekey, timeZoneInfoToken);
                    }


                }


                if (!string.IsNullOrEmpty(masterReferenceID))
                {
                    if (IsExistParaName(crystalReportDocument, DDSetup.QueryReportParameterMasterReferenceID))
                    {
                        SetupReportParameter(crystalReportDocument, DDSetup.QueryReportParameterMasterReferenceID, masterReferenceID);
                    }


                }


                if (!string.IsNullOrEmpty(PdmRequestRegisterID))
				{
					// need format like this '1,2,3,4,6'

					SetupCrystalReportRequestRegisterId(crystalReportDocument, PdmRequestRegisterID);
				}

				// need to set up ReportParameterApplicationServerUrl for report iamge url
				if (IsExistParaName(crystalReportDocument, DDSetup.ReportParameterImageUrl))
                {
                    SetupReportParameter(crystalReportDocument, DDSetup.ReportParameterImageUrl, DDSetup.ReorptSetup.ReportingServerImageUrl);
                }



                // if(  this._ProductReferenceId
            }
        }

		public static void SetupCrystalReportRequestRegisterId(ReportDocument crystalReportDocument, string PdmRequestRegisterID)
		{
			string requestContent = DDSetup.GetPdmRequestContent(PdmRequestRegisterID);


			// doest include  '\''
			//if (requestContent.IndexOf('\'') == -1)
			//{
			//	requestContent = "'" + requestContent + "'";
			//}

			if(! requestContent.Contains('\''.ToString ()) )
			{
				requestContent = "'" + requestContent + "'";

			}


			// it is new dyniac request !!!

			if (requestContent.StartsWith(DDSetup.PrintMergeGridReferencePrefixConst))
			{
				SetupReportRequestParameter(crystalReportDocument, PdmRequestRegisterID, requestContent);

			}
			else // it is old request dont change !!!
			{

				if (!string.IsNullOrEmpty(requestContent))
				{
					if (IsExistParaName(crystalReportDocument, DDSetup.ReportParameterMutipleProductReferenceIDs))
					{
						SetupReportParameter(crystalReportDocument, DDSetup.ReportParameterMutipleProductReferenceIDs, requestContent);
					}
					else// try to setup _MutipleProductReferenceIDs
					{

						SetupReportParameter(crystalReportDocument, DDSetup.ReportParameterProductReferenceID, requestContent);
					}

				}

			}


		}

		private static void SetupReportRequestParameter(ReportDocument crystalReportDocument, string PdmRequestRegisterID, string requestContent)
		{
            //referenceId , blockId, rowValueId
            // truncate 18000,5834,6bb03992-e5ff-4b01-a41b-438ca9a7e560
            //paraNameValue: classId: 1,2,3 |colorId: 2,3 |productReferenceId: 1,2,3 | 
            int? mainBlockId = DDSetup.GetPdmRequestMainBlockId(PdmRequestRegisterID);

			if (mainBlockId.HasValue)
			{

				string dataFieldString = requestContent.Substring(requestContent.IndexOf(':') + 1);

				// column1: ReferenceID, Column2: BlockId, column3: RowValueGuid
				DataTable result = DDSetup.ReadCSVContentDataTable(dataFieldString, 3, "|", ",");

				if (result.Rows.Count > 0)
				{
					string productReferenceId = result.Rows[0][0].ToString();

					


					if (IsExistParaName(crystalReportDocument, DDSetup.ReportParameterProductReferenceID))
					{
						SetupReportParameter(crystalReportDocument, DDSetup.ReportParameterProductReferenceID, productReferenceId);
					}
					else// try to setup _MutipleProductReferenceIDs
					{

						SetupReportParameter(crystalReportDocument, DDSetup.ReportParameterMutipleProductReferenceIDs, productReferenceId);
					}



					Dictionary<string, List<DataRow>> dictBlockIdDataRowList = result.AsEnumerable().GroupBy(o => o["2"]).ToDictionary(o => o.Key.ToString(), o => o.ToList());


					// only one block it is mainblock
					if (dictBlockIdDataRowList.Count == 1)
					{

						var firstKeyList = dictBlockIdDataRowList.First();

						var rowValueConcString = firstKeyList.Value.Select(o => o["3"].ToString()).Aggregate((current, next) => current + ", " + next);

						// it is main block  (first block
						//GetMergeBlockPrintGrid 3879, 5834, '6BB03992-E5FF-4B01-A41B-438CA9A7E560', -1, '2B963DC6-3D85-46C9-B100-2F19ADECA94B', 18000

						if (firstKeyList.Key.ToString() == mainBlockId.ToString())
						{

							SetupReportParameter(crystalReportDocument, DDSetup.ReportParameterBlock1RwValueFilter, rowValueConcString);


							SetupReportParameter(crystalReportDocument, DDSetup.ReportParameterBlock2RwValueFilter, System.Guid.NewGuid().ToString());

							

						}
						else // it is not main block, it is second block
						{

							SetupReportParameter(crystalReportDocument, DDSetup.ReportParameterBlock1RwValueFilter, System.Guid.NewGuid().ToString());

							SetupReportParameter(crystalReportDocument, DDSetup.ReportParameterBlock2RwValueFilter, rowValueConcString);



						}




					}
					else // two blockId
					{


						foreach (string blockIdkey in dictBlockIdDataRowList.Keys)
						{

							var rowValueConcString = dictBlockIdDataRowList[blockIdkey].Select(o => o["3"].ToString()).Aggregate((current, next) => current + ", " + next);


							// it is first block
							if (blockIdkey == mainBlockId.ToString())
							{
									SetupReportParameter(crystalReportDocument, DDSetup.ReportParameterBlock1RwValueFilter, rowValueConcString);


							}
							else // it is second block
							{
								SetupReportParameter(crystalReportDocument, DDSetup.ReportParameterBlock2RwValueFilter, rowValueConcString);


							}



						}

					}


				}


			}
		}


		private static ParameterFieldDefinition SetupReportParameter(ReportDocument cryRpt, string parameterName, object value)
        {
            ParameterFieldDefinitions crParameterFieldDefinitions = cryRpt.DataDefinition.ParameterFields;

            if (!IsExistParaName(cryRpt, parameterName))
                return null;

            ParameterFieldDefinition crParameterFieldDefinition = crParameterFieldDefinitions[parameterName];

            //!!!!!!!!!!! if it is allow, it disalbe the fornter prompe, means run time can change the value
            crParameterFieldDefinition.EnableAllowEditingDefaultValue = true;

            //  crParameterFieldDefinition.ApplyMinMaxValues(value, value);

            ParameterDiscreteValue crParameterDiscreteValue = new ParameterDiscreteValue();
            crParameterDiscreteValue.Value = value;
            crParameterDiscreteValue.IsRange = false;

            ParameterValues crParameterValues = crParameterFieldDefinition.CurrentValues;
            crParameterValues.Add(crParameterDiscreteValue);

            crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

            crParameterFieldDefinition.ApplyDefaultValues(crParameterValues);

            //crParameterFieldDefinition.PromptText = value.ToString();
            crParameterFieldDefinition.DiscreteOrRangeKind = DiscreteOrRangeKind.DiscreteValue;

            if (crParameterFieldDefinition.HasCurrentValue)
            {
                crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);
            }

            return crParameterFieldDefinition;
        }

        private static bool IsExistParaName(ReportDocument cryRpt, string parameterName)
        {
            ParameterFieldDefinitions crParameterFieldDefinitions = cryRpt.DataDefinition.ParameterFields;

            foreach (ParameterFieldDefinition aParDef in crParameterFieldDefinitions)
            {
                if (aParDef.Name == parameterName)
                    return true;
            }

            return false;
        }
       

    }
}
