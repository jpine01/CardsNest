using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UofLConnect.Models.Admin
{
    public class ReportsQueueModel
    {
        public string Report_Number { get; set; }
        public string Report_Time { get; set; }
        public string Report_Date { get; set; }
        public string Report_Name { get; set; }
        public string User_Name { get; set; }
        public string Report_Message { get; set; }
    }
}