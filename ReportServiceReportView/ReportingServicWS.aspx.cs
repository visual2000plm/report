using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ReportServiceReportView.ServiceReference1;
using System.Web.Services.Protocols;

//C:\Program Files\Microsoft SQL Server\MSRS10_50.MSSQLSERVER\Reporting Services\ReportServer

namespace ReportServiceReportView
{
    public enum EmReportItemType { Foder = 1, Report = 2, DataSource = 5, DataSet = 8, Component =9}

    public partial class ReportingServicWS : System.Web.UI.Page
    {

        public readonly string reportQuery = @"SELECT TOP 1000 [ItemID]
      ,[Path]
      ,[Name]
      ,[ParentID]
      ,[Type]
      ,[Content]
      ,[Intermediate]
      ,[SnapshotDataID]
      ,[LinkSourceID]
      ,[Property]
      ,[Description]
      ,[Hidden]
      ,[CreatedByID]
      ,[CreationDate]
      ,[ModifiedByID]
      ,[ModifiedDate]
      ,[MimeType]
      ,[SnapshotLimit]
      ,[Parameter]
      ,[PolicyID]
      ,[PolicyRoot]
      ,[ExecutionFlag]
      ,[ExecutionTime]
      ,[SubType]
      ,[ComponentID]
  FROM [ReportServer].[dbo].[Catalog] where type=2";
        protected void Page_Load(object sender, EventArgs e)
        {
            //<Parameters>    <UserProfileState>0</UserProfileState>    <Parameter>      <Name>@ReportYear</Name>      <Type>Object</Type>      <Nullable>False</Nullable>      <AllowBlank>False</AllowBlank>      <MultiValue>True</MultiValue>      <UsedInQuery>True</UsedInQuery>      <State>MissingValidValue</State>      <PromptUser>True</PromptUser>    </Parameter>    <Parameter>      <Name>@ReportMonth</Name>      <Type>Object</Type>      <Nullable>False</Nullable>      <AllowBlank>False</AllowBlank>      <MultiValue>True</MultiValue>      <UsedInQuery>True</UsedInQuery>      <State>MissingValidValue</State>      <PromptUser>True</PromptUser>    </Parameter>    <Parameter>      <Name>@EmployeeID</Name>      <Type>Object</Type>      <Nullable>False</Nullable>      <AllowBlank>False</AllowBlank>      <MultiValue>True</MultiValue>      <UsedInQuery>True</UsedInQuery>      <State>MissingValidValue</State>      <PromptUser>True</PromptUser>    </Parameter>  </Parameters>
            ///AdventureWorks 2008R2/AdventureWorks2008R2_Base
           // ReportExecutionServiceSoapClient service = new ReportExecutionServiceSoapClient();
            //service
          //  service.Credentials = new System.Net.NetworkCredential("username", "password", "domain");

          //  ReportingService2010SoapClient service = new ReportingService2010SoapClient();
          //  service.LogonUser (TrustedUserHeader. 

            ReportServerInfoService aReportServerInfoService = new ReportServerInfoService();

          var result =  aReportServerInfoService.ListReportCollection("/",true);

            // Response.Output.WriteLine (  @"catalogItem.Path --  catalogItem.Name --catalogItem.Size --catalogItem.TypeName --catalogItem.VirtualPath") ;

            var sortResult = result.OrderBy (o=>o.Name);
             foreach (CatalogItem catalogItem in sortResult)
             {
                 Response.Output.WriteLine ( catalogItem.Name +"|" +catalogItem.TypeName  + "<br/>"  );
                // Response.Output.NewLine = ";";
             }

          //   Response.Output.WriteLine("______________");
          //foreach (CatalogItem catalogItem in result)
          //{
          //    Response.Output.WriteLine  ( catalogItem.Path + "|" +  catalogItem.Name + "|" +catalogItem.Size + "|" +catalogItem.TypeName + "|" +catalogItem.VirtualPath) ;
          //}


        }
    }

    public class ReportServerInfoService
    {
        public CatalogItem[] ListReportCollection(string itemPath, bool recursive)
        {
            CatalogItem[] reportCollection = { };

            try
            {
                ReportingService2010SoapClient client = new ReportingService2010SoapClient();

                //NT LAN Manager (NTLM) is a suite of Microsoft security protocols that provides authentication, integrity, and confidentiality to users.[1][2][3] NTLM is the successor to the authentication protocol in Microsoft LAN Manager (LANMAN), 
                client.ClientCredentials.Windows.AllowNtlm = true;
                client.ClientCredentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;

                    //
        // Summary:
        //     The server process can impersonate the client's security context on its local
        //     system. The server cannot impersonate the client on remote systems.
      //  Impersonation = 3,

        //       Finally figure it out.
         //       My Reporting Services were configured to a local account while my Application Pool for IIS was configured to ApplicationPoolIdentity. 
          //      I changed my Application Pool to LocalSystem and it fixed it. Hopefully this information will be useful to others as I wasted several hours figuring this out.

            client.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

                CatalogItem aCatalogItem = new CatalogItem();


               // aCatalogItem.TypeName

                // Summary:
                //     The server process can impersonate the client's security context on remote
                //     systems.
               // client.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Delegation;

                client.Open();
                TrustedUserHeader t = new TrustedUserHeader();

                try
                {
                    // I need to list of children of a specified folder.
                    client.ListChildren(t, itemPath, recursive, out reportCollection); // see http://msdn.microsoft.com/en-us/library/reportservice2010.reportingservice2010.listchildren.aspx
                }
                catch (SoapException ex)
                {
                   // _logger.Error("ReportServerManagementService--" + ex);
                }
            }
            catch (SoapException ex)
            {
               // _logger.Error("ReportServerManagementService--" + ex);
            }

            return reportCollection;
        }
    }
}