using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Capstone.Models;
using Microsoft.AspNet.Identity;
using PusherServer;

namespace Capstone.Controllers
{
    public class ChatController : Controller
    {
        private Pusher pusher;
        private ApplicationDbContext db = new ApplicationDbContext();

        //class constructor
        public ChatController()
        {
            var options = new PusherOptions();
            options.Cluster = "us2";
            

            pusher = new Pusher(
               "582528",
               "5ca413a0c03488fa6148",
               "8efc93ab956a7ab442c8",
               options
           );
        }

        // GET: Chat
        public ActionResult Index()
        {
            var currentUserId = User.Identity.GetUserId();

            var user = db.Users.Where(u => u.Id == currentUserId).FirstOrDefault();
            Session["user"] = user;
            var currentUser = (Models.ApplicationUser)Session["user"];
            if (Session["user"] == null)
            {
                return Redirect("/");
            }

            //var currentUser = (Models.ApplicationUser)Session["user"];

            //var currentUserId = User.Identity.GetUserId();
            //var currentUser = db.Users.Where(u => u.Id == currentUserId).FirstOrDefault();

            using (var db = new Models.ApplicationDbContext())
            {
                ViewBag.allUsers = db.Users.Where(u => u.FirstName != currentUser.FirstName).ToList();
            }

            ViewBag.currentUser = currentUser;

            return View();
        }

        public JsonResult ConversationWithContact(string contact)
        {
            if (Session["user"] == null)
            {
                return Json(new { status = "error", message = "User is not logged in" });
            }

            var currentUser = (Models.ApplicationUser)Session["user"];

            var conversations = new List<Models.Conversation>();

            using (var db = new Models.ApplicationDbContext())
            {
                conversations = db.Conversations.Where(
                    c => (c.Receiver_Id == currentUser.Id
                    && c.Sender_Id == contact) ||
                    (c.Receiver_Id == contact
                    && c.Sender_Id == currentUser.Id)).OrderBy(c => c.Created_At).ToList();
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

            var currentUser = (ApplicationUser)Session["user"];
            var contact = Request.Form["contact"];
            string socket_id = Request.Form["socket_id"];

            Conversation convo = new Conversation
            {
                Sender_Id = currentUser.Id,
                Message = Request.Form["Message"],
                Receiver_Id = contact
            };

            using (var db = new Models.ApplicationDbContext())
            {
                db.Conversations.Add(convo);
                db.SaveChanges();
            }

            var conversationChannel = getConvoChannel(currentUser.Id, contact);

            pusher.TriggerAsync(
              conversationChannel,
              "new_message",
              convo,
              new TriggerOptions() { SocketId = socket_id });

            return Json(convo);
        }

        private String getConvoChannel(string user_id, string contact_id)
        {
            //if (user_id > contact_id)
            //{
            //    return "private-chat-" + contact_id + "-" + user_id;
            //}

            return "private-chat-" + user_id + "-" + contact_id;
        }

        [HttpPost, Route("pusher/auth")]
        public JsonResult Auth(string channel_name, string socket_id)
        {
            if (Session["user"] == null)
            {
                return Json(new { status = "error", message = "User is not logged in" });
            }
            var currentUser = (ApplicationUser)Session["user"];

            var options = new PusherOptions();
            options.Cluster = "us2";

            var pusher = new Pusher(
            "582528",
            "5ca413a0c03488fa6148",
            "8efc93ab956a7ab442c8", options);
            var currentId = "ad1a7ce2-8dd1-4658-a3c8-1687583bccb0";

            if (channel_name.IndexOf(currentId.ToString()) == -1)
            {
                return Json(
                  new { status = "error", message = "User cannot join channel" }
                );
            }

            var auth = pusher.Authenticate(channel_name, socket_id);

            return Json(auth);
        }
    }
}