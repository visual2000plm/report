using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportViewSetup
{
    public class ReportJobDesc
    {
        public   string ReportJobName
        {
            get;set;
        }
        public int? UId
        {
            get;
            set;
        }

       
        // will include report name and Reference ID !!!
        public string  AllReportFileNmae
        {
            get;
            set;
        }
        public  string ProductReferenceId
        {
            get;
            set;
        }

        public string  DataSourceType
        {
            get;
            set;
        }

        public string PdmRequestRegisterID
        {
            get;
            set;
        }

        
         public string MainReferenceID
        {
            get;
            set;
        }

        public string MasterReferenceID
        {
            get;
            set;
        }
       

    }
}
