using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UofLConnect.Models.Home
{
    public class LoginModel
    {
        private string _email;
        private string _password;

        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value?.Trim();
            }
        }
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
    }
}