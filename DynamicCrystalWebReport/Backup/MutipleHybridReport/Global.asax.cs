using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using ReportViewSetup;

namespace MutipleHybridReport
{
    public class Global : System.Web.HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
            string PLMConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PLMConnectionString"].ConnectionString;
            string DWDataSourceConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DWDataSourceConnectionString"].ConnectionString;
            DDSetup.InitAppSetup(PLMConnectionString, DWDataSourceConnectionString); 

        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

        }

        void Session_End(object sender, EventArgs e)
        {
        

        }

    }
}
