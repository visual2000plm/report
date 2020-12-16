using System;

using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using ReportViewSetup;
using System.Data.SqlClient;

namespace WebApplication1
{
    public partial class CrystalReportView : System.Web.UI.Page
    {
        private string _ReportFileNmae = string.Empty;
        private string _UId = string.Empty;

        //Request.QueryString[DDSetup.ReportParameterProductReferenceID];
        private string _ProductReferenceId = string.Empty;

        private string _MutipleProductReferenceIDs = string.Empty;

        //Request.QueryString[DDSetup.ReportParameterProductReferenceID];
        private string _DatasourceType = string.Empty;



        //Request.QueryString[DDSetup.ReportParameterMainReferenceID];
        private string _MainReferenceID = string.Empty;


        //Request.QueryString[DDSetup.ReportParameterMasterReferenceID];
        private string _MasterReferenceID = string.Empty;



        protected void Page_Load(object sender, EventArgs e)
        {
            if (_ReportFileNmae != string.Empty)
            {
                if (CurrnetReportDocument != null)
                {
                    crystalReportViewer.ReportSource = CurrnetReportDocument;
                    crystalReportViewer.DataBind();
                    crystalReportViewer.EnableDatabaseLogonPrompt = false;
                    crystalReportViewer.EnableParameterPrompt = true;

                    crystalReportViewer.HasGotoPageButton = true;
                    crystalReportViewer.HasCrystalLogo = false;

                    crystalReportViewer.DisplayPage = true;
                    crystalReportViewer.SeparatePages = true;

                    string aUId = Request.QueryString[DDSetup.ReportParameterUid];

                    if ((!this.IsPostBack) && !string.IsNullOrEmpty(aUId))
                    {
                        ConfigureCrystalReportParameters(CurrnetReportDocument);
                    }
                }
            }



        }

        protected override void OnPreInit(EventArgs e)
        {
            string reportName = Request.QueryString[DDSetup.QueryReportName];
            string aUId = Request.QueryString[DDSetup.ReportParameterUid];
            _UId = aUId;

            if (!string.IsNullOrEmpty(reportName) && !string.IsNullOrEmpty(aUId))
            {
                _ReportFileNmae = reportName;
            }

            _ProductReferenceId = Request.QueryString[DDSetup.ReportParameterProductReferenceID];
            _DatasourceType  = Request.QueryString[DDSetup.QueryReportDataSourceType];

               //Request.QueryString[DDSetup.ReportParameterMainReferenceID];
             _MainReferenceID =  Request.QueryString[DDSetup.ReportParameterMainReferenceID];

            //Request.QueryString[DDSetup.ReportParameterMasterReferenceID];
             _MasterReferenceID = Request.QueryString[DDSetup.ReportParameterMasterReferenceID];


            //DDSetup.ReportParameterMutipleProductReferenceIDs

            string PdmRequestRegisterID = Request.QueryString[DDSetup.PdmRequestRegisterID];
            if (!string.IsNullOrEmpty(PdmRequestRegisterID))
            {
                // need format like this '1,2,3,4,6'

                string requestContent = DDSetup.GetPdmRequestContent(PdmRequestRegisterID);
                if (requestContent.IndexOf('\'') == -1)
                {
                    requestContent = "'" + requestContent + "'";
                }

                _MutipleProductReferenceIDs = requestContent;

                //if (runtime.Parameters[DDSetup.ReportParameterMutipleProductReferenceIDs] != null)
                //{
                //    runtime.Parameters[DDSetup.ReportParameterMutipleProductReferenceIDs].CurrentValue = requestContent;
                //    // runtime.Parameters[DDSetup.ReportParameterMutipleProductReferenceIDs].Hidden

                //}
            }

          


            base.OnPreInit(e);
            if (!this.IsPostBack)
            {
                if (_ReportFileNmae != string.Empty)
                {
                    // only create once !!
                    BindReport();
                }
            }
        }

        private void BindReport()
        {
            ConnectionInfo ConnInfo = new ConnectionInfo();


            if (this._DatasourceType == EmReportDataSourceType.DWDatabase.ToString ())
            {

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(DDSetup.DWDataSourceConnectionString );
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

          
            ReportDocument rep = new ReportDocument();

            string fullpath = DDSetup.ReorptSetup.ReportRootPath + @"\" + _ReportFileNmae;

            // rep.Load(Server.MapPath(fullpath));
            rep.Load(fullpath);
            this.crystalReportViewer.ReportSource = rep;

            Tables RepTbls = rep.Database.Tables;
            foreach (CrystalDecisions.CrystalReports.Engine.Table RepTbl in RepTbls)
            {
                TableLogOnInfo RepTblLogonInfo = RepTbl.LogOnInfo;
                RepTblLogonInfo.ConnectionInfo = ConnInfo;
                RepTbl.ApplyLogOnInfo(RepTblLogonInfo);
            }

            Session[_ReportFileNmae] = rep;
        }

        public ReportDocument CurrnetReportDocument
        {
            get
            {
                return Session[_ReportFileNmae] as ReportDocument;
            }
        }

        public void ConfigureCrystalReportParameters(ReportDocument customerReport)
        {
            if (!string.IsNullOrEmpty(this._UId))
            {
                SetupReportParameter(customerReport, DDSetup.ReportParameterUid, int.Parse(_UId));

                string userName;
                int? v2kuid;
                string timeZoneInfoToken = string.Empty;

                EmDomainType aDomainType = DDSetup.GetDomainTypeAndV2kUID(_UId, out v2kuid, out userName, out timeZoneInfoToken);
                if (v2kuid.HasValue)
                {
                    if (aDomainType == EmDomainType.Vendor)
                    {
                        SetupReportParameter(customerReport, DDSetup.ReportParameterVendorID, v2kuid.Value);
                    }
                    else if (aDomainType == EmDomainType.Customer)
                    {
                    }
                }

                if (!string.IsNullOrEmpty(timeZoneInfoToken))
                {
                    if (IsExistParaName(customerReport, DDSetup.ReportParameterClientTimeZonekey))
                    {
                        SetupReportParameter(customerReport, DDSetup.ReportParameterClientTimeZonekey, timeZoneInfoToken);
                    }
                   

                }


                if (!string.IsNullOrEmpty(this._ProductReferenceId))
                {
                    if (IsExistParaName(customerReport, DDSetup.ReportParameterProductReferenceID))
                    {
                        SetupReportParameter(customerReport, DDSetup.ReportParameterProductReferenceID, _ProductReferenceId);
                    }
                    else// try to setup _MutipleProductReferenceIDs
                    {

                        SetupReportParameter(customerReport, DDSetup.ReportParameterMutipleProductReferenceIDs, _ProductReferenceId);
                    }
                    
                }

                // new add 2014-02-17 report paramters


                if (!string.IsNullOrEmpty(this._MainReferenceID))
                {
                    if (IsExistParaName(customerReport, DDSetup.ReportParameterMainReferenceID))
                    {
                        SetupReportParameter(customerReport, DDSetup.ReportParameterMainReferenceID, _MainReferenceID);
                    }
                  
                }

                if (!string.IsNullOrEmpty(this._MasterReferenceID ))
                {
                    if (IsExistParaName(customerReport, DDSetup.ReportParameterMasterReferenceID))
                    {
                        SetupReportParameter(customerReport, DDSetup.ReportParameterMasterReferenceID, _MasterReferenceID);
                    }

                }



                if (!string.IsNullOrEmpty(this._MutipleProductReferenceIDs))
                {
                    if (IsExistParaName(customerReport, DDSetup.ReportParameterMutipleProductReferenceIDs))
                    {
                        SetupReportParameter(customerReport, DDSetup.ReportParameterMutipleProductReferenceIDs, _MutipleProductReferenceIDs);
                    }
                    else// try to setup _MutipleProductReferenceIDs
                    {

                        SetupReportParameter(customerReport, DDSetup.ReportParameterProductReferenceID, _MutipleProductReferenceIDs);
                    }

                   
                }

                // need to set up ReportParameterApplicationServerUrl for report iamge url
                if (IsExistParaName(customerReport, DDSetup.ReportParameterImageUrl))
                {
                    SetupReportParameter(customerReport, DDSetup.ReportParameterImageUrl, DDSetup.ReorptSetup.ReportingServerImageUrl);
                }


              

             

                // if(  this._ProductReferenceId
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