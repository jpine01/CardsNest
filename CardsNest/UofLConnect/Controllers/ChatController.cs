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
    public class ChatController : Controller
    {
        private Pusher pusher;

        //class constructor
        public ChatController()
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

        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                return Redirect("/");
            }

            if (UserInfo.IsApproved == "False")
                return RedirectToAction("Information", "Home", new { msg = "Your account must be approved by an Admin before you can access the system." });

            var currentUser = (UserModel)Session["user"];

            using (var db = new ChatContext())
            {

                ViewBag.allUsers = db.Users.Where(u => u.name != currentUser.name)
                                 .ToList();
            }


            ViewBag.currentUser = currentUser;


            return View();
        }

        public JsonResult ConversationWithContact(int contact)
        {
            if (Session["user"] == null)
            {
                return Json(new { status = "error", message = "User is not logged in" });
            }

            var currentUser = (UserModel)Session["user"];

            var conversations = new List<Conversation>();

            using (var db = new ChatContext())
            {
                conversations = db.Conversations.
                                  Where(c => (c.receiver_id == currentUser.id
                                      && c.sender_id == contact) ||
                                      (c.receiver_id == contact
                                      && c.sender_id == currentUser.id))
                                  .OrderBy(c => c.created_at)
                                  .ToList();
            }

            return Json(
                new { status = "success", data = conversations },
                JsonRequestBehavior.AllowGet
            );
        }

        [HttpPost]
        public JsonResult SendMessage()
        {
            if (Session["user"] == null)
            {
                return Json(new { status = "error", message = "User is not logged in" });
            }

            var currentUser = (UserModel)Session["user"];
            var contact = Convert.ToInt32(Request.Form["contact"]);
            string socket_id = Request.Form["socket_id"];

            Conversation convo = new Conversation
            {
                sender_id = currentUser.id,
                message = Request.Form["message"],
                receiver_id = Convert.ToInt32(Request.Form["contact"]),
                created_at = DateTime.Now
            };

            using (var db = new ChatContext())
            {
                db.Conversations.Add(convo);
                db.SaveChanges();
            }

            var conversationChannel = getConvoChannel(currentUser.id, contact);

            pusher.TriggerAsync(
              conversationChannel,
              "new_message",
              convo,
              new TriggerOptions() { SocketId = socket_id });

            return Json(convo);
        }

        [HttpPost]
        public JsonResult MessageDelivered(int message_id)
        {
            Conversation convo = null;
            using (var db = new ChatContext())
            {
                convo = db.Conversations.FirstOrDefault(c => c.id == message_id);
                if (convo != null)
                {
                    convo.status = Conversation.messageStatus.Delivered;
                    db.Entry(convo).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }

            }
            string socket_id = Request.Form["socket_id"];
            var conversationChannel = getConvoChannel(convo.sender_id, convo.receiver_id);
            pusher.TriggerAsync(
              conversationChannel,
              "message_delivered",
              convo,
              new TriggerOptions() { SocketId = socket_id });
            return Json(convo);
        }

        private String getConvoChannel(int user_id, int contact_id)
        {
            if (user_id > contact_id)
            {
                return "private-chat-" + contact_id + "-" + user_id;
            }

            return "private-chat-" + user_id + "-" + contact_id;
        }
    }
}