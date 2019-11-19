using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace UofLConnect.Models.Home
{
    public class ProfileModel
    {
        const int DATE_MAX_LENGTH = 22; // Max length for dates
        const int EMAIL_MAX_LENGTH = 30; // Max length for email
        const int NAME_MAX_LENGTH = 20; // Max length for the name
        private int _major;
        private string _email;
        private bool _complete;


        [MaxLength(DATE_MAX_LENGTH)]
        public string DOB { get; set; }

        [MaxLength(EMAIL_MAX_LENGTH)]
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value?.Trim().ToLower();
            }
        }

        public string StudentID { get; set; }

        [Required]
        [MaxLength(NAME_MAX_LENGTH)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(NAME_MAX_LENGTH)]
        public string LastName { get; set; }

        public string Ethnicity { get; set; }

        public string SexualOrientation { get; set; }

        public string Gender { get; set; }

        public string[] SelectedCourses { get; set; }

        public IEnumerable<SelectListItem> CourseList { get; set; }

        public string[] CurrentCourses { get; set; }

        public IEnumerable<SelectListItem> CurrentCourseList { get; set; }

        public string[] PreviousCourses { get; set; }

        public IEnumerable<SelectListItem> PreviousCourseList { get; set; }

       
        // Read only property to populate report drop down
        public IEnumerable<SelectListItem> Grade_List
        {

            get
            {
                List<SelectListItem> gradeList = new List<SelectListItem>();


                gradeList.Add(new SelectListItem { Text = "Freshman", Value = "1" });
                gradeList.Add(new SelectListItem { Text = "Sophomore", Value = "2" });
                gradeList.Add(new SelectListItem { Text = "Junior", Value = "3" });
                gradeList.Add(new SelectListItem { Text = "Senior", Value = "4" });

                return gradeList;

            }
        }

        public IEnumerable<SelectListItem> Industry_List
        {

            get
            {
                List<SelectListItem> gradeList = new List<SelectListItem>();


                gradeList.Add(new SelectListItem { Text = "Software Devleopment", Value = "1" });
                gradeList.Add(new SelectListItem { Text = "Information Security", Value = "2" });
                gradeList.Add(new SelectListItem { Text = "Business Process Management", Value = "3" });
                gradeList.Add(new SelectListItem { Text = "Help Desk", Value = "4" });
                gradeList.Add(new SelectListItem { Text = "Marketing", Value = "5" });
                gradeList.Add(new SelectListItem { Text = "Academic Research", Value = "6" });

                return gradeList;

            }
        }


        public int Grade { get; set; } 

        public string Job { get; set; }

        public bool No_Path { get; set; }

        public bool Web_Dev { get; set; }

        public bool Info_Sec { get; set; }

        public bool BPM { get; set; }

        public int Major
        {

            get
            {

                return _major;

            }
            set
            {
                //if (No_Path == true)
                //{
                //    _major = 0;
                //}
                //else if (Web_Dev == true)
                //{
                //    _major = 1;

                //}
                //else if (Info_Sec == true)
                //{
                //    _major = 2;
                //}
                //else if (BPM)
                //{
                //    _major = 3;
                //}
                //else if (Info_Sec == true && Web_Dev == true)
                //{
                //    _major = 12;
                //}
                //else if (BPM == true && Info_Sec == true)
                //{
                //    _major = 23;
                //}
                //else if (BPM == true && Web_Dev == true)
                //{
                //    _major = 13;
                //}
                //else
                //{
                //    _major = 123;
                //}

                _major = value;

            }

        }


        public int ProfessionalInterest { get; set; }

        public int PersonalInterest { get; set; }

        public int Industry { get; set; }

        public bool IsProfileComplete {
            get
            {
                return _complete;
            }

            set
            {
                _complete = true;

                _complete = value;

            }

        }

    }
}