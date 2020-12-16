using System;
using System.Collections.Generic;

using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Data.SqlClient;
using System.Data;
using ReportViewSetup;

namespace DynamicWebReportView
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
             string PLMConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PLMConnectionString"].ConnectionString;

             string DWDataSourceConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DWDataSourceConnectionString"].ConnectionString;


             DDSetup.InitAppSetup(PLMConnectionString, DWDataSourceConnectionString); 
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            

        }

        protected void Application_Error(object sender, EventArgs e)
        {
          
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }

//    public enum EmDomainType : byte { Anonymous = 0, Vendor = 1, Customer = 2, Agent = 3, Employee = 4, AppAdmin = 5, SysAdmin = 6 }
//    public class DDSetup
//    {

//        public static readonly string PDMConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PdmConnectionString"].ConnectionString;
//        public static readonly string QueryReportName = "ReportName";
//        public static readonly string QueryFolderPath = "QueryFolderPath";
//        public static readonly string Queryuid = "Uid";
//        public static readonly string ReportParameterUid = "Uid";
//        public static readonly string ReportParameterVendorID = "VendorID";
//        public static readonly string ReportParameterCustomerID = "CustomerID";

       

//        public string ReportRootPath
//        {
//            get;
//            set;
//        }
//        public string PLMApplictionHomeURL
//        {
//            get;
//            set;
//        }

        


//        static DDSetup _DDSetup;
//        internal static void InitAppSetup()
//        {
//            if (_DDSetup == null)
//            {

//                SetReportParameter();
//            }

//        }


//        public static DDSetup ReorptSetup
//        {

//            get
//            {
//                if (_DDSetup == null)
//                {

//                    SetReportParameter();
//                }
//                return _DDSetup;

//            }

//        }

//        private static void SetReportParameter()
//        {
//            _DDSetup = new DDSetup();


//            using (SqlConnection conn = new SqlConnection(PDMConnectionString))
//            {
//                conn.Open();

//                string qeuryDynamicReportRepositoryPath = @" select SetupValue from pdmsetup where SetupCode= 'DynamicReportRepositoryPath' ";

//                SqlCommand cmd = new SqlCommand(qeuryDynamicReportRepositoryPath, conn);

//                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
//                System.Data.DataTable resultTabel = new DataTable();
//                adapter.Fill(resultTabel);
//                if (resultTabel.Rows.Count > 0)
//                {
//                    _DDSetup.ReportRootPath = resultTabel.Rows[0][0].ToString();

//                }
//                else
//                {
//                    _DDSetup.ReportRootPath = string.Empty;
//                }



//                //

//                string qeuryPLMApplictionHomeURL = @" select SetupValue from pdmsetup where SetupCode= 'ApplicationUrl' ";
//                cmd = new SqlCommand(qeuryPLMApplictionHomeURL, conn);

//                adapter = new SqlDataAdapter(cmd);
//                resultTabel = new DataTable();
//                adapter.Fill(resultTabel);
//                if (resultTabel.Rows.Count > 0)
//                {
//                    _DDSetup.PLMApplictionHomeURL = resultTabel.Rows[0][0].ToString() + "WebPages/DesktopModule/UserDesktop.aspx";

//                }
//                else
//                {
//                    _DDSetup.PLMApplictionHomeURL = string.Empty;
//                }

//                //ApplicationUrl



//            }
//        }


//        public static EmDomainType GetDomainTypeAndV2kUID(string uid, out int? matchV2kUserID)
//        {
//            EmDomainType aDomainType = EmDomainType.Anonymous;
//            matchV2kUserID = null;
//              using (SqlConnection conn = new SqlConnection(PDMConnectionString))
//            {
//                conn.Open();


//                string qeuryDynamicReportRepositoryPath = string.Format(@" select    pdmSecurityRegDomain.PersonType, pdmSecurityWebUser.UserID,MatchV2kUserID
//                       FROM    pdmSecurityWebUser INNER JOIN    pdmSecurityRegDomain ON pdmSecurityWebUser.DomainID = pdmSecurityRegDomain.DomainID
//                        where UserID= {0}", uid);

//                SqlCommand cmd = new SqlCommand(qeuryDynamicReportRepositoryPath, conn);

//                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
//                System.Data.DataTable resultTabel = new DataTable();
//                adapter.Fill(resultTabel);
//                if (resultTabel.Rows.Count > 0)
//                {
//                    DataRow aRow = resultTabel.Rows[0];
//                    object v2kid = aRow["MatchV2kUserID"];
//                    object personType = aRow["PersonType"];
//                    if (v2kid != null && personType != null)
//                    {
//                        int domainType = int.Parse(personType.ToString());
//                        int result;
//                        if (int.TryParse(v2kid.ToString(), out result))
//                        {

//                            matchV2kUserID = result;

//                        }

//                        if (domainType == (int)EmDomainType.Employee)
//                        {
//                            aDomainType = EmDomainType.Employee;


//                        }

//                        else if (domainType == (int)EmDomainType.Vendor)
//                        {
//                            aDomainType = EmDomainType.Vendor;


//                        }

//                        else if (domainType == (int)EmDomainType.Customer)
//                        {
//                            aDomainType = EmDomainType.Customer;


//                        }

//                        else if (domainType == (int)EmDomainType.Agent)
//                        {
//                            aDomainType = EmDomainType.Agent;


//                        }

//                        else if (domainType == (int)EmDomainType.SysAdmin)
//                        {
//                            aDomainType = EmDomainType.SysAdmin;


//                        }

//                        else if (domainType == (int)EmDomainType.AppAdmin)
//                        {
//                            aDomainType = EmDomainType.AppAdmin;


//                        }
                        
//                    }
                  

                  

//                }
              
               



//            }
//              return aDomainType;
            
            
        
//        }





//    }
}