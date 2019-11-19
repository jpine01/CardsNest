using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UofLConnect.Models.Admin
{
    public class AccountDeniedModel
    {
        public string UserID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public string StudentID { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public string Type { get; set; }
    }
}