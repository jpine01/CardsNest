using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UofLConnect.Models.Home
{
    public class CalendarEventsModel
    {
        public string id { get; set; }
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string Description { get; set; }
        public string Visibility { get; set; }
        public bool allDay { get; set; }
        public bool RepeatingEvent { get; set; }

        public IEnumerable<SelectListItem> Visibility_List
        {

            get
            {
                List<SelectListItem> reportList = new List<SelectListItem>();


                reportList.Add(new SelectListItem { Text = "Private", Value = "Private" });
                reportList.Add(new SelectListItem { Text = "Public", Value = "Public" });

                return reportList;

            }
        }

        public string Mode { get; set; }
    }
}