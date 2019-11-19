using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UofLConnect.Models.Admin
{
    public class EmailBlastModel
    {
        public bool Mentor { get; set; }
        public bool Mentee { get; set; }
        public bool Alumni { get; set; }
        public string EmailSubject { get; set; }
        public string EmailContent { get; set; }
    }
}