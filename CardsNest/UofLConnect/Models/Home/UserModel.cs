using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UofLConnect.Models.Home
{
    public class UserModel
    {
        public UserModel()
        {
        }

        public int id { get; set; }
        public string name { get; set; }
        public DateTime created_at { get; set; }
    }
}