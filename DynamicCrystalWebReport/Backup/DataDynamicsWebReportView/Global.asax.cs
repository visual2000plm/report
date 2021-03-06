﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using ReportViewSetup;

namespace DataDynamicsWebReportView
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
}