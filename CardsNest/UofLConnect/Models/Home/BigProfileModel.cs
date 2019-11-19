using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UofLConnect.Models.Home
{
    public class BigProfileModel
    {

        public ProfileModel ProfileModel { get; set; }

        public PersonalInterstsModel PersonalInterestsModel { get; set; }

        public ProfessionalInterestsModel ProfessionalInterestsModel { get; set; }
    }
}