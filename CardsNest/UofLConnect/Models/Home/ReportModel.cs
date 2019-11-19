using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UofLConnect.Models.Home
{
    public class ReportModel
    {
        // Holds current date/time
        private string _SetDefaultTime = DateTime.Now.ToString("hh:mm:ss.0000000");

        private string _SetDefaultDate = DateTime.Now.ToShortDateString();

        private bool _SetDefaultSeen = false;

        private string _SetUserName = "";

      //  [DataType(DataType.Time)]
      //  [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh:mm:ss[.nnnnnnn]}")]
        public string Report_Time
        {
            get
            {
                return _SetDefaultTime;
            }

            set
            {
                _SetDefaultTime = value;
            }

        }

     //   [DataType(DataType.Date)]
      //  [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public string Report_Date
        {
            get
            {
                return _SetDefaultDate;
            }

            set
            { 
                _SetDefaultDate = value;
            }

        }


        [Required]
        public string Report_Name { get; set; }
        

        public string User_Name
        {

            get
            {
                return _SetUserName;
            }
            set
            {
                _SetUserName = value;
            }

        }


        [Required]
        public string Report_Message { get; set; }


        public bool Seen
        {
            get
            {
                return _SetDefaultSeen;
            }

            set
            {
                _SetDefaultSeen = value;
            }

        }


        // Read only property to populate report drop down
        public IEnumerable<SelectListItem> Report_List
        {

            get
            {
                List<SelectListItem> reportList = new List<SelectListItem>();


                reportList.Add(new SelectListItem { Text = "Crisis", Value = "Crisis Report" });
                reportList.Add(new SelectListItem { Text = "Harrasment", Value = "Harrasment" });
                reportList.Add(new SelectListItem { Text = "Complaint", Value = "Complaint" });
                reportList.Add(new SelectListItem { Text = "Bug", Value = "Bug" });
                reportList.Add(new SelectListItem { Text = "Fraudulent User", Value = "Fraudulent User" });

                return reportList;

            }
        }

    }

}