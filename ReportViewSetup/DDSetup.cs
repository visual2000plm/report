using System.Data;
using System.Linq;
using System.Data.SqlClient;
using DataDynamics.Reports;
using System.Collections.Generic;
using Com.Visual2000.SystemFramework;
using System.Web;

namespace ReportViewSetup
{
    public enum EmDomainType : byte { Anonymous = 0, Vendor = 1, Customer = 2, Agent = 3, Employee = 4, AppAdmin = 5, SysAdmin = 6 }
    public enum EmReportDataSourceType    {     PLMDatabase = 1,       DWDatabase = 2,    }


 //https://e2e.visual-2000.com/mec_plms_DynamicReportViewer/DDWebReportView.aspx?ReportName=DesignLinePlanMain.rdlx&uid=1&PdmRequestRegisterID=38&ReportDataSourceType=PLMDatabase
//https://e2e.visual-2000.com/mec_plms_DynamicReportViewer/DDWebReportView.aspx?ReportName=Header.rdlx&uid=1&ProductReferenceID=1712&MainReferenceID=1712&ReportDataSourceType=PLMDatabase

    public class DDSetup
    {
        // public static readonly string PDMConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PdmConnectionString"].ConnectionString;
        public static readonly string QueryReportName = "ReportName";
		public static readonly string QueryMutipleRef = "MutipleRef";
		//  public static readonly string QueryMutipleReportNames = "MutipleReportNames";
		public static readonly string QueryFolderPath = "QueryFolderPath";

		//public static readonly string Queryuid = "Uid";
		public static readonly string QueryReportDataSourceType = "ReportDataSourceType";
		public static readonly string QueryReportBatchNumber = "ReportBatchNumber";

		public const string QueryPdmRequestRegisterID = "PdmRequestRegisterID";




		public static readonly string QueryReportJobName = "ReportJobName";

        public static readonly string RequestJobPdfId = "RequestJobPdfId";


		public static readonly  string PrintMergeGridReferencePrefixConst = "PrintMergeGridReference";



		// copy tab need to  MasterReferenceID and ProductReferenceID
		public static readonly string QueryReportParameterMainReferenceID = "MainReferenceID";

		// only for master header print 
		public static readonly string QueryReportParameterMasterReferenceID = "MasterReferenceID";




		public static readonly string ReportParameterUserAvailbeReportNameList = "UserAvailbeReportNameList";



        public static readonly string ReportParameterUid = "uid";
		//public static readonly string ReportParameterUserName = "uid";

		public static readonly string ReportParameterCurrentUserName = "CurrentUserName";
        public static readonly string ReportParameterVendorID = "VendorID";
        public static readonly string ReportParameterCurrentVendorName = "CurrentVendorName";
        public static readonly string ReportParameterCustomerID = "CustomerID";
        public static readonly string ReportParameterCurrentCustomerName = "CurrentCustomerName";

        public static readonly string CrystalReportPublishUrlSetup = "CrystalReportPublishUrl";

        
        public static readonly string ReportParameterImageUrl = "ApplicationServerUrl";

        // need to pass reportFileName and PdmRequestRegisterID for Batch print ...
        // PdmRequestRegisterID will be passed to MutipleProductReferenceIDs paramter in the report !


    


        public static readonly string ReportParameterMutipleProductReferenceIDs = "MutipleProductReferenceIDs";

        public static readonly string ReportParameterProductReferenceID = "ProductReferenceID";

     

        public static readonly string ReportParameterClientTimeZonekey = "ClientTimeZonekey";


		public static readonly string ReportParameterBlock1RwValueFilter = "block1RwValueFilter";

		public static readonly string ReportParameterBlock2RwValueFilter = "block2RwValueFilter";


		//ClientTimeZonekey


		//MainReferenceID



		public static string PLMConnectionString
        {
            get;
            set;
        }

        public static string DWDataSourceConnectionString
        {
            get;
            set;
        }

        public string ReportRootPath
        {
            get;
            set;
        }

         public string ReportPdfCompressPath
        {
            get;
            set;
        }


       //  public static readonly string PDFConvertRootPath


        public string PLMApplictionHomeURL
        {
            get;
            set;
        }
         
        public string PLMApplictionURL
        {
            get;
            set;
        }

        public string ReportingServerImageUrl
        {
            get;
            set;
        }


        //public string ReportDataSourceServerName
        //{
        //    get;
        //    set;
        //}

        //public string ReportDataSourceDatabaseName
        //{
        //    get;
        //    set;
        //}

        //public string ReportDataSourceUserID
        //{
        //    get;
        //    set;
        //}

        //public string ReportDataSourcePassword
        //{
        //    get;
        //    set;
        //}

        public string CrystalReportPublishUrl
        {
            get;
            set;
        }

        public string PdfCompressionSetting
        {
            get;
            set;
        }

        

        public bool IsIntegratedSecurity
        {
            get;
            set;
        }

        public bool IsReportCompressionActivate
        {
            get;
            set;
        }

        public string GhostScriptPath
        {
            get;
            set;
        }

        private static  readonly DDSetup _DDSetup;

		static DDSetup ()
		{

			PLMConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PLMConnectionString"].ConnectionString;

			DWDataSourceConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DWDataSourceConnectionString"].ConnectionString;



			_DDSetup = new DDSetup();

			 SetReportParameter();

			ReportCompressSetup();

			
		}




		//public static void InitAppSetup(string plmConnectionString, string plmDWConnectionString)
		//{
		//	if (_DDSetup == null)
		//	{
		//		PLMConnectionString = plmConnectionString;
		//		DWDataSourceConnectionString = plmDWConnectionString;
		//		SetReportParameter();
		//	}
		//}

		public static DDSetup ReorptSetup
        {
            get
            {
                if (_DDSetup == null)
                {
                    SetReportParameter();
					

				}
                return _DDSetup;
            }
        }


        private static void SetReportParameter()
        {
           

         

            using (SqlConnection conn = new SqlConnection(PLMConnectionString))
            {
                conn.Open();

                string qeuryDynamicReportRepositoryPath = @" select SetupValue from pdmsetup where SetupCode= 'DynamicReportRepositoryPath' ";

                SqlCommand cmd = new SqlCommand(qeuryDynamicReportRepositoryPath, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                System.Data.DataTable resultTabel = new DataTable();
                adapter.Fill(resultTabel);
                if (resultTabel.Rows.Count > 0)
                {
                    _DDSetup.ReportRootPath = resultTabel.Rows[0][0].ToString();
                }
                else
                {
                    _DDSetup.ReportRootPath = string.Empty;
                }

                //GhostScriptPath


             //   ReportCompressSetup(conn, ref cmd, ref adapter, ref resultTabel);


                string qeuryPLMApplictionHomeURL = @" select SetupValue from pdmsetup where SetupCode= 'ApplicationUrl' ";
                cmd = new SqlCommand(qeuryPLMApplictionHomeURL, conn);

                adapter = new SqlDataAdapter(cmd);
                resultTabel = new DataTable();
                adapter.Fill(resultTabel);
                if (resultTabel.Rows.Count > 0)
                {
                    _DDSetup.PLMApplictionHomeURL = resultTabel.Rows[0][0].ToString() + "WebPages/DesktopModule/UserDesktop.aspx";
                    _DDSetup.PLMApplictionURL = resultTabel.Rows[0][0].ToString();
                }
                else
                {
                    _DDSetup.PLMApplictionHomeURL = string.Empty;
                }



                //ReportingServerImageUrl
                string qeuryReportingServerImageUrl = @" select SetupValue from pdmsetup where SetupCode= 'ReportingServerImageUrl' ";
                cmd = new SqlCommand(qeuryReportingServerImageUrl, conn);

                adapter = new SqlDataAdapter(cmd);
                resultTabel = new DataTable();
                adapter.Fill(resultTabel);
                if (resultTabel.Rows.Count > 0)
                {
                   
                    _DDSetup.ReportingServerImageUrl = resultTabel.Rows[0][0].ToString();
                }
                else
                {
                    _DDSetup.ReportingServerImageUrl = _DDSetup.PLMApplictionURL;
                }



                //Crystal Report ApplicationUrl

                string queryCrystalReportPublishUrl = @" select SetupValue from pdmsetup where SetupCode= 'CrystalReportPublishUrl' ";
                cmd = new SqlCommand(queryCrystalReportPublishUrl, conn);

                adapter = new SqlDataAdapter(cmd);
                resultTabel = new DataTable();
                adapter.Fill(resultTabel);
                if (resultTabel.Rows.Count > 0)
                {
                    _DDSetup.CrystalReportPublishUrl = resultTabel.Rows[0][0].ToString();
                }
                else
                {
                    _DDSetup.CrystalReportPublishUrl = string.Empty;
                }
            }
        }

        private static void ReportCompressSetup()
        {

            using (SqlConnection conn = new SqlConnection(PLMConnectionString))
            {
                conn.Open();

                string qeuryDynamicReportRepositoryPath = @" select SetupValue from pdmsetup where SetupCode= 'DynamicReportRepositoryPath' ";

                SqlCommand cmd = new SqlCommand(qeuryDynamicReportRepositoryPath, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                System.Data.DataTable resultTabel = new DataTable();

                string qeuryGhostScriptPath = @" select SetupValue from pdmsetup where SetupCode= 'GhostScriptPath' ";

                cmd = new SqlCommand(qeuryGhostScriptPath, conn);

                adapter = new SqlDataAdapter(cmd);
                resultTabel = new DataTable();
                adapter.Fill(resultTabel);
                if (resultTabel.Rows.Count > 0)
                {
                    _DDSetup.GhostScriptPath = resultTabel.Rows[0][0].ToString();
                }
                else
                {
                    _DDSetup.GhostScriptPath = string.Empty;
                }

                // ReportCompressionActivate





                string qeuryReportCompressionActivate = @" select SetupValue from pdmsetup where SetupCode= 'ReportCompressionActivate' ";

                cmd = new SqlCommand(qeuryReportCompressionActivate, conn);

                adapter = new SqlDataAdapter(cmd);
                resultTabel = new DataTable();
                adapter.Fill(resultTabel);
                if (resultTabel.Rows.Count > 0)
                {
                    string aPdmSetupValue = resultTabel.Rows[0][0].ToString();
                    _DDSetup.IsReportCompressionActivate = (aPdmSetupValue == "1" || aPdmSetupValue == bool.TrueString);
                }
                else
                {
                    _DDSetup.IsReportCompressionActivate = false;
                }


                //ReportPdfComressPath
                string qeuryReportPdfComressPath = @" select SetupValue from pdmsetup where SetupCode= 'ReportPdfCompressPath' ";

                cmd = new SqlCommand(qeuryReportPdfComressPath, conn);

                adapter = new SqlDataAdapter(cmd);
                resultTabel = new DataTable();
                adapter.Fill(resultTabel);
                if (resultTabel.Rows.Count > 0)
                {
                    string aPdmSetupValue = resultTabel.Rows[0][0].ToString();
                    _DDSetup.ReportPdfCompressPath = aPdmSetupValue;
                }
                else
                {
                    _DDSetup.ReportPdfCompressPath = string.Empty;
                }



                //ReportPdfComressPath
                //PdfCompressionSetting

                string qeuryPdfCompressionSetting = @" select SetupValue from pdmsetup where SetupCode= 'PdfCompressionSetting' ";

                cmd = new SqlCommand(qeuryPdfCompressionSetting, conn);

                adapter = new SqlDataAdapter(cmd);
                resultTabel = new DataTable();
                adapter.Fill(resultTabel);
                if (resultTabel.Rows.Count > 0)
                {
                    string aPdmSetupValue = resultTabel.Rows[0][0].ToString();
                    _DDSetup.PdfCompressionSetting = aPdmSetupValue;
                }
                else
                {
                    _DDSetup.PdfCompressionSetting = string.Empty;
                }

            }
           
        }



        public static EmDomainType GetDomainTypeAndV2kUID(int? uid, out int? matchV2kUserID, out string currentUserName, out string timeZoneInfoToken)
        {
            EmDomainType aDomainType = EmDomainType.Anonymous;
            matchV2kUserID = null;
            currentUserName = string.Empty;
            timeZoneInfoToken = string.Empty;
            using (SqlConnection conn = new SqlConnection(PLMConnectionString))
            {
                conn.Open();

                string qeuryDynamicReportRepositoryPath = string.Format(@" select  pdmSecurityWebUser.TimeZoneInfoToken,   pdmSecurityRegDomain.PersonType, pdmSecurityWebUser.UserID, pdmSecurityWebUser.UserName, MatchV2kUserID
                       FROM    pdmSecurityWebUser INNER JOIN    pdmSecurityRegDomain ON pdmSecurityWebUser.DomainID = pdmSecurityRegDomain.DomainID
                        where UserID= {0}", uid);

                SqlCommand cmd = new SqlCommand(qeuryDynamicReportRepositoryPath, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                System.Data.DataTable resultTabel = new DataTable();
                adapter.Fill(resultTabel);
                if (resultTabel.Rows.Count > 0)
                {
                    DataRow aRow = resultTabel.Rows[0];
                    object v2kid = aRow["MatchV2kUserID"];
                    object personType = aRow["PersonType"];

                    currentUserName = aRow["UserName"] as string;
                    timeZoneInfoToken = aRow["TimeZoneInfoToken"] as string;


                    if (v2kid != null && personType != null)
                    {
                        int domainType = int.Parse(personType.ToString());
                        int result;
                        if (int.TryParse(v2kid.ToString(), out result))
                        {
                            matchV2kUserID = result;
                        }

                        if (domainType == (int)EmDomainType.Employee)
                        {
                            aDomainType = EmDomainType.Employee;
                        }

                        else if (domainType == (int)EmDomainType.Vendor)
                        {
                            aDomainType = EmDomainType.Vendor;
                        }

                        else if (domainType == (int)EmDomainType.Customer)
                        {
                            aDomainType = EmDomainType.Customer;
                        }

                        else if (domainType == (int)EmDomainType.Agent)
                        {
                            aDomainType = EmDomainType.Agent;
                        }

                        else if (domainType == (int)EmDomainType.SysAdmin)
                        {
                            aDomainType = EmDomainType.SysAdmin;
                        }

                        else if (domainType == (int)EmDomainType.AppAdmin)
                        {
                            aDomainType = EmDomainType.AppAdmin;
                        }
                    }
                }
            }

            //WriteReportTrace(currentUserName);

            return aDomainType;
        }

       

        public static void SetupDDReportRuntimeParameter(ReportRuntime ddRuntime, int ? aUId)
        {
            if (aUId != null)
            {
                if (ddRuntime.Parameters[DDSetup.ReportParameterUid] != null)
                {
                    ddRuntime.Parameters[DDSetup.ReportParameterUid].CurrentValue = aUId;

					

				}

                //string userName;
                //int? v2kuid;
                //EmDomainType aDomainType = DDSetup.GetDomainTypeAndV2kUID(aUId, out v2kuid, out userName);

                string userName = string.Empty;
                string timeZoneInfoToken = string.Empty;

                int? v2kuid;
                EmDomainType aDomainType = DDSetup.GetDomainTypeAndV2kUID(aUId, out v2kuid, out userName, out timeZoneInfoToken);


				if (ddRuntime.Parameters[DDSetup.ReportParameterCurrentUserName] != null)
				{
					ddRuntime.Parameters[DDSetup.ReportParameterCurrentUserName].CurrentValue = userName;
				}


				if (ddRuntime.Parameters[DDSetup.ReportParameterClientTimeZonekey] != null)
                {
                    ddRuntime.Parameters[DDSetup.ReportParameterClientTimeZonekey].CurrentValue = timeZoneInfoToken; ;
                }

                if (v2kuid.HasValue)
                {
                    if (ddRuntime.Parameters[DDSetup.ReportParameterCurrentUserName] != null)
                    {
                        ddRuntime.Parameters[DDSetup.ReportParameterCurrentUserName].CurrentValue = userName;
                    }

                    if (aDomainType == EmDomainType.Vendor)
                    {
                        if (ddRuntime.Parameters[DDSetup.ReportParameterVendorID] != null)
                        {
                            ddRuntime.Parameters[DDSetup.ReportParameterVendorID].CurrentValue = v2kuid.Value.ToString();
                        }
                    }
                    else if (aDomainType == EmDomainType.Customer)
                    {
                        if (ddRuntime.Parameters[DDSetup.ReportParameterCustomerID] != null)
                        {
                            ddRuntime.Parameters[DDSetup.ReportParameterCustomerID].CurrentValue = v2kuid.Value.ToString(); ;
                        }
                    }
                }
            }
        }


        public static Dictionary<string, string> GetReportDataSoureType(List<string> reportFileNames)
        {

            Dictionary<string, string> toRetrun = new Dictionary<string, string>();

            string query = @" select  ReportFileName, WebReportViewer from [pdmReportWebPublish] where";

            string inClause = GenerateColumnInClauseWithAndCondition(reportFileNames, "ReportFileName", false);

            query = query + inClause;

            System.Data.DataTable resultTabel = new DataTable();
             using (SqlConnection conn = new SqlConnection(PLMConnectionString))
             {
                 conn.Open();
                 SqlCommand cmd = new SqlCommand(query, conn);
                 SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                 adapter.Fill(resultTabel);
                
             }

             foreach (DataRow aRow in resultTabel.Rows)
             {
 
                 int ? dataSourceType = aRow["WebReportViewer"] as int ?;
                 if(! dataSourceType.HasValue )
                 {
                     dataSourceType = (int)EmReportDataSourceType.DWDatabase; 
                 }

                 string stDatasourceType =  ((EmReportDataSourceType)dataSourceType.Value ).ToString ();
                 toRetrun.Add(aRow["ReportFileName"] as string, stDatasourceType);
             
             
             }


             return toRetrun;

        }


        public static int ? GetUserIdFromSessionId(string userSessionId)
        {
            using (SqlConnection conn = new SqlConnection(DDSetup.PLMConnectionString))
            {
                conn.Open();

                string qeurycontent = @" select UserID from pdmSecurityWebUserSession   where SessionID=@userSessionId ";
                SqlCommand cmd = new SqlCommand(qeurycontent, conn);
                cmd.Parameters.Add(new SqlParameter("@userSessionId", userSessionId));
                object value = cmd.ExecuteScalar();
                if (value == null)
                {
                    return null;
                }
                else
                {

                    return ControlTypeValueConverter.ConvertValueToInt(value);
                }
                    


               
            }
        }






		public static EmDomainType GetUserDomainType(int? userId)
        {

            string userName;
            int? v2kuid;
            string timeZoneInfoToken = string.Empty;

            EmDomainType aDomainType = DDSetup.GetDomainTypeAndV2kUID(userId, out v2kuid, out userName, out timeZoneInfoToken);

            return aDomainType;
        
        }

        public static string GetPdmRequestContent(string pdmrequestId)
        {
            using (SqlConnection conn = new SqlConnection(DDSetup.PLMConnectionString))
            {
                conn.Open();

                string qeurycontent = @" select RequestContent from PdmRequestRegister where RequestID=@RequestID ";
                SqlCommand cmd = new SqlCommand(qeurycontent, conn);
                cmd.Parameters.Add(new SqlParameter("@RequestID", pdmrequestId));
                string content = cmd.ExecuteScalar() as string;

                //string deletecontent = @" delete from PdmRequestRegister where RequestID=@RequestID ";
                //SqlCommand deletecmd = new SqlCommand(deletecontent, conn);
                //deletecmd.Parameters.Add(new SqlParameter("@RequestID", pdmrequestId));
                //deletecmd.ExecuteNonQuery();

                return content;
            }
        }


		public static List<string> GetPdmRequestRegisterIdsByBatchNimber(string BatchNumber)
		{
			List<string> toReturn = new List<string>();

			using (SqlConnection conn = new SqlConnection(DDSetup.PLMConnectionString))
			{
				conn.Open();

				string qeurycontent = @" SELECT  [RequestID]    FROM  [PdmRequestRegister]   where BatchNumber = @BatchNumber ";
				SqlCommand cmd = new SqlCommand(qeurycontent, conn);
				cmd.Parameters.Add(new SqlParameter("@BatchNumber", BatchNumber));
				 SqlDataReader reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					toReturn.Add(reader["RequestID"].ToString());

				}

				reader.Close();
				conn.Close();

				//deletecmd.ExecuteNonQuery();

				return toReturn;
			}
		}


		public static DataTable  GetPdmRequestDataTable(string pdmrequestId)
		{
			 DataTable dataTable = new DataTable();
			using (SqlConnection conn = new SqlConnection(DDSetup.PLMConnectionString))
			{
				conn.Open();

				string qeurycontent = @" 		SELECT RequestID, RequestContent, ReportID, pdmReferenceView.GridBlockID as MainBlockId
						FROM            PdmRequestRegister INNER JOIN
						 pdmReferenceView ON PdmRequestRegister.ReferenceViewID = pdmReferenceView.ReferenceViewID
						wheree RequestID=@RequestID ";
				SqlCommand cmd = new SqlCommand(qeurycontent, conn);
				cmd.Parameters.Add(new SqlParameter("@RequestID", pdmrequestId));


				SqlDataAdapter da = new SqlDataAdapter(cmd);
				// this will query your database and return the result to your datatable
				da.Fill(dataTable);
			
			}

			return dataTable;

		}


		public static int? GetPdmRequestMainBlockId(string pdmrequestId)
		{
			DataTable dataTable = new DataTable();
			using (SqlConnection conn = new SqlConnection(DDSetup.PLMConnectionString))
			{
				conn.Open();

				string qeurycontent = @" SELECT  pdmReferenceView.GridBlockID as MainBlockId
						FROM            PdmRequestRegister INNER JOIN
						 pdmReferenceView ON PdmRequestRegister.ReferenceViewID = pdmReferenceView.ReferenceViewID  where RequestID=@RequestID ";
				SqlCommand cmd = new SqlCommand(qeurycontent, conn);
				cmd.Parameters.Add(new SqlParameter("@RequestID", pdmrequestId));
				int? mainBlockId = cmd.ExecuteScalar() as int?;

				//string deletecontent = @" delete from PdmRequestRegister where RequestID=@RequestID ";
				//SqlCommand deletecmd = new SqlCommand(deletecontent, conn);
				//deletecmd.Parameters.Add(new SqlParameter("@RequestID", pdmrequestId));
				//deletecmd.ExecuteNonQuery();

				return mainBlockId;

			}

		}




		// key: reportName value:int
		public static Dictionary<string, string> GetDictUserReportNameAndFileNameReportss(int userId)
        {

            Dictionary<string, string> dictToReturn = new Dictionary<string, string>();

            string qeuryReportNameAndFileName = string.Empty;

            EmDomainType userDomainType = GetUserDomainType(userId);

            if (userDomainType == EmDomainType.SysAdmin)
            {
                qeuryReportNameAndFileName = @" SELECT   distinct        pdmReportWebPublish.ReportName, pdmReportWebPublish.ReportFileName      FROM         pdmReportWebPublish ";
            }
            else
            {
                qeuryReportNameAndFileName = string.Format(@" SELECT   distinct    
                      pdmReportWebPublish.ReportName, pdmReportWebPublish.ReportFileName
                      FROM         pdmReportPublishSecurity INNER JOIN
                      pdmReportWebPublish ON pdmReportPublishSecurity.ReportID = pdmReportWebPublish.ReportID
                      where  pdmReportPublishSecurity.SecurityWebUserID = {0} or 
                      pdmReportPublishSecurity.SecurityUserGroupID in ( 
                                             SELECT      pdmSecurityGroupMember.GroupID 
                                    FROM         pdmSecurityGroupMember  where  pdmSecurityGroupMember.UserID ={1}
                      
                       )", userId, userId);

            
            }
            



            using (SqlConnection conn = new SqlConnection(PLMConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(qeuryReportNameAndFileName, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                System.Data.DataTable resultTabel = new DataTable();
                adapter.Fill(resultTabel);

                if (resultTabel.Rows.Count > 0)
                {
                    foreach (DataRow row in resultTabel.Rows)
                    {
                        string reportname = row["ReportName"].ToString();
                        string reportFileName = row["ReportFileName"].ToString();
                        if (! dictToReturn.ContainsKey(reportname))
                        {
                            dictToReturn.Add(reportname, reportFileName);
                        
                        }
                    
                    }
                  
                }

                
            }

            return dictToReturn;
        
        }

        public static string GetUserReportFileName(int userId,object reportId)
        {

            string dictToReturn = string.Empty;

            string qeuryReportNameAndFileName = string.Empty;

          //  EmDomainType userDomainType = GetUserDomainType(userId);



            string userName;
            int? v2kuid;
            string timeZoneInfoToken = string.Empty;

            EmDomainType userDomainType = DDSetup.GetDomainTypeAndV2kUID(userId, out v2kuid, out userName, out timeZoneInfoToken);





            if (userDomainType == EmDomainType.SysAdmin)
            {
                qeuryReportNameAndFileName = @" SELECT         pdmReportWebPublish.ReportFileName      FROM         pdmReportWebPublish where ReportId=@ReportId  ";
            }
            else
            {
                qeuryReportNameAndFileName = string.Format(@" SELECT       
                       pdmReportWebPublish.ReportFileName
                      FROM         pdmReportPublishSecurity INNER JOIN
                      pdmReportWebPublish ON pdmReportPublishSecurity.ReportID = pdmReportWebPublish.ReportID
                      where  pdmReportWebPublish.ReportID = @ReportId and (  pdmReportPublishSecurity.SecurityWebUserID = {0} or 
                      pdmReportPublishSecurity.SecurityUserGroupID in ( 
                         SELECT      pdmSecurityGroupMember.GroupID 
                             FROM         pdmSecurityGroupMember  where  pdmSecurityGroupMember.UserID ={1}
                      
                       ))
                       ", userId, userId);


            }




            using (SqlConnection conn = new SqlConnection(PLMConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(qeuryReportNameAndFileName, conn);
                cmd.Parameters.Add(new SqlParameter("@ReportId", reportId));

                dictToReturn = cmd.ExecuteScalar() as string;

             


            }

            WriteReportTrace(userName, dictToReturn);

            return dictToReturn;

        }

       public static void WriteReportTrace(  string currentUserName, string Reportname)
		{
			if (!ApplicationLog.TracingEnabled)
				return;

		//	WriteWhoPrintReportHistoryToDatabase(currentUserName, Reportname);


			//   dictToReturn = cmd.ExecuteScalar() as string;

			// ApplicationLog.WriteWarning("UserName: " + currentUserName + " And From " + fromHost + " Reportname " + Reportname);
		}

		private static void WriteWhoPrintReportHistoryToDatabase(string currentUserName, string Reportname)
		{
			using (SqlConnection conn = new SqlConnection(PLMConnectionString))
			{
				conn.Open();
				string fromHost = string.Empty;
				if (HttpContext.Current != null)
				{
					fromHost = "From Host:" + HttpContext.Current.Request.UserHostAddress;
				}

				// string insert = @" SELECT         pdmReportWebPublish.ReportFileName      FROM         pdmReportWebPublish where ReportId=@ReportId  ";

				string insert = @"INSERT INTO pdmReportLogTrack
                   ([UserName]
                   ,[ReportName]
                   ,[LogDateTime]
                   ,[RequestFrom])
                   VALUES
                   (
                    @UserName
                   ,@ReportName
                   ,@LogDateTime
                   ,@RequestFrom)";

				SqlCommand cmd = new SqlCommand(insert, conn);
				cmd.Parameters.Add(new SqlParameter("@UserName", currentUserName));
				cmd.Parameters.Add(new SqlParameter("@ReportName", Reportname));
				cmd.Parameters.Add(new SqlParameter("@LogDateTime", System.DateTime.Now));
				cmd.Parameters.Add(new SqlParameter("@RequestFrom", fromHost));

				cmd.ExecuteNonQuery();


			}
		}

		public static string GenerateColumnInClauseWithAndCondition(IEnumerable<string> ids, string IDColumnName, bool isInsertAnd)
        {
            string inclause = string.Empty;

            if (ids != null)
            {
                foreach (string pid in ids)
                {
                    inclause += "'" + pid + "',";
                }
            }

            if (inclause != string.Empty)
            {
                inclause = inclause.Substring(0, inclause.Length - 1);
                if (isInsertAnd)
                {
                    inclause = "  and  " + IDColumnName + " in ( " + inclause + " ) ";
                }
                else
                {
                    inclause = "  " + IDColumnName + " in ( " + inclause + " ) ";
                }
            }
            return inclause;
        }


		public static DataTable ReadCSVContentDataTable(string content, int noOfColumn,   string delimte, string endOfLine)
		{
			DataTable returnDataTable = new DataTable();
			for ( int i= 1; i<= noOfColumn; i++)
			{
				returnDataTable.Columns.Add(i.ToString ());

			}

			string [] listLine = content.Split(endOfLine.ToCharArray());

			foreach ( string oneLineString in listLine)
			{
				DataRow dataRow = returnDataTable.NewRow();
				returnDataTable.Rows.Add(dataRow);


				string[] cells = oneLineString.Split(delimte.ToCharArray ());

				for (int i = 0; i < returnDataTable.Columns.Count; i++)
				{
					dataRow[i] = cells[i];

				}

			}

			return returnDataTable;
		}


		public static void SetupDynamicReportRequesttRegisterId(ReportRuntime runtime, string PdmRequestRegisterID)
		{
			string requestContent = DDSetup.GetPdmRequestContent(PdmRequestRegisterID);
            // Dunamic paramter List 
            //paraNameValue: classId: 1,2,3 |colorId: 2,3 |productReferenceId: 1,2,3 | 

            if (requestContent.StartsWith(DDSetup.PrintMergeGridReferencePrefixConst))
			{
				SetupDataDynamicMergeReport(runtime, PdmRequestRegisterID, requestContent);

			}
			else // it is old request dont change !!!
			{
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
		}

		private static void SetupDataDynamicMergeReport(ReportRuntime runtime, string PdmRequestRegisterID, string requestContent)
		{
			// referenceId , blockId, rowValueId
			// truncate 18000,5834,6bb03992-e5ff-4b01-a41b-438ca9a7e560

			int? mainBlockId = DDSetup.GetPdmRequestMainBlockId(PdmRequestRegisterID);

			if (mainBlockId.HasValue)
			{

				string dataFieldString = requestContent.Substring(requestContent.IndexOf(':') + 1);

				// column1: ReferenceID, Column2: BlockId, column3: RowValueGuid
				DataTable result = DDSetup.ReadCSVContentDataTable(dataFieldString, 3, "|", ",");

				if (result.Rows.Count > 0)
				{
					string productReferenceId = result.Rows[0][0].ToString();

					if (runtime.Parameters[DDSetup.ReportParameterMutipleProductReferenceIDs] != null)
					{
						runtime.Parameters[DDSetup.ReportParameterMutipleProductReferenceIDs].CurrentValue = productReferenceId;
						// runtime.Parameters[DDSetup.ReportParameterMutipleProductReferenceIDs].Hidden
					}
					else// try to set ReportParameterProductReferenceID
					{

						runtime.Parameters[DDSetup.ReportParameterProductReferenceID].CurrentValue = productReferenceId;

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
							if (runtime.Parameters[DDSetup.ReportParameterBlock1RwValueFilter] != null)
							{
								runtime.Parameters[DDSetup.ReportParameterBlock1RwValueFilter].CurrentValue = rowValueConcString;

							}

							// need to set seond as   System.Guid.NewGuid() to leav empty row
							if (runtime.Parameters[DDSetup.ReportParameterBlock2RwValueFilter] != null)
							{
								runtime.Parameters[DDSetup.ReportParameterBlock2RwValueFilter].CurrentValue = System.Guid.NewGuid().ToString();

							}

						}
						else // it is not main block, it is second block
						{

							// need to set first as   System.Guid.NewGuid() to leav empty row
							if (runtime.Parameters[DDSetup.ReportParameterBlock1RwValueFilter] != null)
							{
								runtime.Parameters[DDSetup.ReportParameterBlock1RwValueFilter].CurrentValue = System.Guid.NewGuid().ToString();

							}


							if (runtime.Parameters[DDSetup.ReportParameterBlock2RwValueFilter] != null)
							{
								runtime.Parameters[DDSetup.ReportParameterBlock2RwValueFilter].CurrentValue = rowValueConcString;

							}


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
								//MainBlockId


								if (runtime.Parameters[DDSetup.ReportParameterBlock1RwValueFilter] != null)
								{
									runtime.Parameters[DDSetup.ReportParameterBlock1RwValueFilter].CurrentValue = rowValueConcString;

								}




							}
							else // it is second block
							{


								if (runtime.Parameters[DDSetup.ReportParameterBlock2RwValueFilter] != null)
								{
									runtime.Parameters[DDSetup.ReportParameterBlock2RwValueFilter].CurrentValue = rowValueConcString;

								}


							}



						}

					}


				}


			}
		}



	}
}