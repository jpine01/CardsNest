using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UofLConnect.Models.Home
{
    public class InterestsModel
    {

        // Read only property to populate report drop down
        public List<SelectListItem> ProfessionalInterest_List { get; set; }

        // Read only property to populate report drop down
        public List<SelectListItem> PersonalInterest_List { get; set; }

        public string[] PersonalInterests { get; set; }

        public string[] ProfessionalInterests { get; set; }

        public List<SelectListItem> SelectedProfessionalInterests { get; set; }

        public List<SelectListItem> SelectedProfessionalInterestsList { get; set; }

        public List<SelectListItem> SelectedPersonalInterests { get; set; }

        public List<SelectListItem> SelectedPersonalInterestsList { get; set; }

        public int[] SelectedProfessionalInterestIDs { get; set; }

        public int[] SelectedPersonallInterestIDs { get; set; }

        public string[] SelectedProfessionalInterestNames { get; set; }

        public string[] SelectedPersonalInterestNames { get; set; }


        // Individual record data

        public int Personal_Interest_ID { get; set; }

        public string Personal_Interest_Name { get; set; }

        public int User_ID { get; set; }

        public int Professional_Interest_ID { get; set; }

        public string Professional_Interest_Name { get; set; }

        public int Professional_Industry { get; set; }
    }
}