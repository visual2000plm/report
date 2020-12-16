using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataDynamics.Reports.Extensibility;
using DataDynamics.Reports;
using System.IO;
using ReportViewSetup;
using System.Xml;
using System.Text;

namespace DataDynamicsWebReportView
{
    internal sealed class DataDynamicsCustomResourceLocator : ResourceLocator
    {
        private readonly ResourceLocator _parentResourceLocator;
        private readonly string _dataSourceType;

        public DataDynamicsCustomResourceLocator(ResourceLocator parentResourceLocator, string dataSourceType)
        {
            _parentResourceLocator = parentResourceLocator;
        }

        #region Overrides of ResourceLocator

        public override Resource GetResource(ResourceInfo resourceInfo)
        {
            Resource resource = _parentResourceLocator.GetResource(resourceInfo);
            // the rdl resource
            if (resource.Value != null && !string.IsNullOrEmpty(resourceInfo.Name) && resourceInfo.Name.ToLower().EndsWith(".rdlx"))
            {
                ReportDefinition rdl = new ReportDefinition();
                rdl.Load(new StreamReader(resource.Value));

                //need to centralize all DataSource in  one Report source,so report source    dynamically change report datasource on the fly
                for (int i = 0; i < rdl.Report.DataSources.Count; i++)
                {

                    if (_dataSourceType == EmReportDataSourceType.DWDatabase.ToString())
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

                // return the modified resource
                MemoryStream rdlStream = new MemoryStream();
                XmlTextWriter writer = new XmlTextWriter(rdlStream, Encoding.UTF8);
                rdl.Save(writer);
                writer.Flush();
                rdlStream.Seek(0, SeekOrigin.Begin);
                return new Resource(rdlStream, resource.ParentUri);
            }

            // for all other resources, just return them using the default resource locator
            return resource;
        }

        #endregion Overrides of ResourceLocator
    }
}