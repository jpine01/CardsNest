using PusherServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UofLConnect.Models.Home;
using UofLConnect.Utilities;

namespace UofLConnect.Controllers
{
    public class AuthController : Controller
    {
        private Pusher pusher;

        //class constructor
        public AuthController()
        {

            var options = new PusherOptions();
            options.Cluster = "us2";

            pusher = new Pusher(
               "874291",
               "0ea0e88480fa88552547",
               "6dd5cbc4d876800a7f0c",
               options
           );
        }

        public ActionResult Login()
        {
            string user_name = UserInfo.UserName;

            if (user_name.Trim() == "")
            {
                return Redirect("/");
            }

            using (var db = new ChatContext())
            {

                UserModel user = db.Users.FirstOrDefault(u => u.name == user_name);

                if (user == null)
                {
                    user = new UserModel { name = user_name, id = UserInfo.UserID, created_at = DateTime.Now };

                    db.Users.Add(user);
                    db.SaveChanges();
                }

                Session["user"] = user;
            }

            return Redirect("/chat");
        }

        public JsonResult AuthForChannel(string channel_name, string socket_id)
        {
            if (Session["user"] == null)
            {
                return Json(new { status = "error", message = "User is not logged in" });
            }

            var currentUser = (UserModel)Session["user"];

            if (channel_name.IndexOf("presence") >= 0)
            {

                var channelData = new PresenceChannelData()
                {
                    user_id = currentUser.id.ToString(),
                    user_info = new
                    {
                        id = currentUser.id,
                        name = currentUser.name
                    },
                };

                var presenceAuth = pusher.Authenticate(channel_name, socket_id, channelData);

                return Json(presenceAuth);

            }

            if (channel_name.IndexOf(currentUser.id.ToString()) == -1)
            {
                return Json(new { status = "error", message = "User cannot join channel" });
            }

            var auth = pusher.Authenticate(channel_name, socket_id);

            return Json(auth);
        }
    }
}