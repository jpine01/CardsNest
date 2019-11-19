using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UofLConnect.Utilities
{
    public class UserInfo
    {
        private static bool _isadmin;

        // Public property used to get the user id anytime we need it
        public static int UserID { get; set; }
        public static string FName { get; set; }
        public static string LName { get; set; }
        public static string DOB { get; set; }
        public static string Ethnicity { get; set; }
        public static string Orientation { get; set; }
        public static string Email { get; set; }
        public static string Gender { get; set; }
        public static string StudentID { get; set; }
        public static string UserName { get; set; }
        public static string Age { get; set; }
        public static List<string> RoleName { get; set; }
        public static string IsProfileCompleted { get; set; }
        public static string Grade { get; set; }
        public static string Job { get; set; }
        public static string Major { get; set; }
        public static string Industry { get; set; }
        public static string IsApproved { get; set; }
        public static string IsDenied { get; set; }
        public static List<string> PrevCourses { get; set; }
        public static List<string> CurrentCourses { get; set; }

        public static bool IsAdmin
        {
            get
            {
                return _isadmin;
            }
            set
            {
                _isadmin = value;
            }
        }
    }
}