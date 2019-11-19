using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UofLConnect.Models.Home
{
    public class RegisterModel
    {
        const int NAME_MAX_LENGTH = 20; // Max length for the name
        const int EMAIL_MAX_LENGTH = 30; // Max length for email

        private string _email;
        private string _studentEmail;
        private string _alumniEmail;
        private string _username;
        private string _password;

        [Required]
        public string UserName
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value?.Trim();
            }
        }

        [Required]
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value?.Trim();
            }
        }

        public string StudentID { get; set; }

        [Required]
        public string RegisterType { get; set; }

        public string SchoolYear { get; set; }

        public string AlumniRole { get; set; }

        public IEnumerable<SelectListItem> AlumniRole_List
        {

            get
            {
                List<SelectListItem> list = new List<SelectListItem>();

                list.Add(new SelectListItem { Text = "Normal", Value = "Normal" });
                list.Add(new SelectListItem { Text = "Mentor", Value = "Mentor" });

                return list;

            }
        }

        public IEnumerable<SelectListItem> SchoolYear_List
        {

            get
            {
                List<SelectListItem> list = new List<SelectListItem>();

                list.Add(new SelectListItem { Text = "Freshman", Value = "Freshman" });
                list.Add(new SelectListItem { Text = "Sophomore", Value = "Sophomore" });
                list.Add(new SelectListItem { Text = "Junior", Value = "Junior" });
                list.Add(new SelectListItem { Text = "Senior", Value = "Senior" });

                return list;

            }
        }

        [Required] [MaxLength(NAME_MAX_LENGTH)]
        public string FirstName { get; set; }

        [Required] [MaxLength(NAME_MAX_LENGTH)]
        public string LastName { get; set; }
        
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

        [MaxLength(EMAIL_MAX_LENGTH)]
        public string StudentEmail
        {
            get
            {
                return _studentEmail;
            }
            set
            {
                _studentEmail = value?.Trim().ToLower();
            }
        }

        [MaxLength(EMAIL_MAX_LENGTH)]
        public string AlumniEmail
        {
            get
            {
                return _alumniEmail;
            }
            set
            {
                _alumniEmail = value?.Trim().ToLower();
            }
        }
    }
}