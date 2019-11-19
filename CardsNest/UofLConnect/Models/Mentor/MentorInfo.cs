using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UofLConnect.Models.Mentor
{
    public class MentorInfo
    {
        private string _industry;

        public string UserID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string DOB { get; set; }
        public string Ethnicity { get; set; }
        public string Orientation { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string UserName { get; set; }
        public string Age { get; set; }
        public string RoleName { get; set; }
        public string Grade { get; set; }
        public string Job { get; set; }
        public string Major { get; set; }
        public string StudentID { get; set; }
        public string IsProfileCompleted { get; set; }
        public string MatchPercentage { get; set; }
        public string Industry
        {
            get { return _industry; }
            set
            {
                switch (value)
                {
                    case "1":
                        _industry = "Software Development";
                        break;
                    case "2":
                        _industry = "Information Security";
                        break;
                    case "3":
                        _industry = "BPM";
                        break;
                    case "4":
                        _industry = "Help Desk";
                        break;
                    case "5":
                        _industry = "Marketing";
                        break;
                    case "6":
                        _industry = "Academic Research";
                        break;
                }
            }
        }
        public List<string> PrevCourses { get; set; }
        public List<string> CurrentCourses { get; set; }
    }
}