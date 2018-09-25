using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Capstone.Models;
using Microsoft.AspNet.Identity;

namespace Capstone.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private FriendsList friendsList = new FriendsList();

        public ActionResult Index()
        {
            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Where(u => u.Id == currentUserId).FirstOrDefault();

            if (currentUser != null)
            {
                Session["UserName"] = currentUser.FirstName;
                Session["UserId"] = currentUser.Id;
            }           

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Chat()
        {
            if (Session["UserName"] != null && Session["UserId"] != null)
            {
                Response.Redirect("~/StartChat.aspx");
            }
            return Redirect("~/StartChat.aspx");
        }

        public ActionResult AddFriend()
        {
            List<string> UserInfo = new List<string>();
            var userId = User.Identity.GetUserId();

            //Retrieve my id
            var myInfo = db.Users.FirstOrDefault(u => u.Id.Equals(userId));

            //Retrieve my language wanted
            var myLanguageWanted = myInfo.LanguageWanted;

            //Search the database for people who's primary language matches my wanted language
            var otherPeopleInfo = db.Users.Where(u => u.PrimayLanguage.Equals(myLanguageWanted)).ToList();

            return View(otherPeopleInfo);
        }

        public ActionResult AddTheFriend()
        {
            var userId = User.Identity.GetUserId();
            string myFriendId = (string)this.RouteData.Values["id"];

            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Where(u => u.Id == currentUserId).FirstOrDefault();
            var friend = db.Users.Where(u => u.Id == myFriendId).FirstOrDefault();
            var myName = currentUser.FirstName;
            var friendName = friend.FirstName;

            var userFriend = new FriendsList
            {
                MyId = myName,
                FriendId = friendName
            };
            db.FriendsLists.Add(userFriend);
            // db.Entry(userFriend).State = EntityState.Modified;
            db.SaveChanges();
            return View("Index");
        }

        //[HttpPost]
        //public async System.Threading.Tasks.Task<ActionResult> AddTheFriendAsync(FriendsList friendsList)
        //{
        //    var userId = User.Identity.GetUserId();
        //    string myFriendId = (string)this.RouteData.Values["id"];

        //    if (ModelState.IsValid)
        //    {
        //        var userFriend = new FriendsList
        //        {
        //            MyId = userId,
        //            FriendId = myFriendId
        //        };
        //        //friendsList.MyId = userId;
        //        //friendsList.FriendId = myFriendId;
        //        var result = await UserManager.CreateAsync(userFriend);
        //        //db.Entry(friendsList).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }

        //    return View("Index");
        //}

    }
}