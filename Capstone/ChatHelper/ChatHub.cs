using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Capstone.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Google.Cloud.Translation;
using Google.Cloud.Translation.V2;

namespace Capstone.ChatHelper
{
    [HubName("chatHub")]
    public class ChatHub : Hub
    {
        #region---Data Members---
        static List<UserDetail> ConnectedUsers = new List<UserDetail>();
        static List<MessageDetail> CurrentMessage = new List<MessageDetail>();
        ApplicationDbContext db = new ApplicationDbContext();
        #endregion

        #region---Methods---

        public void Connect(string UserName, string UserID)
        {
            var id = Context.ConnectionId;

            //var myFriends = db.FriendsLists.Where(x => x.MyId == UserID);

            var myFriendsId = (from x in db.FriendsLists
                               where x.MyId == UserName
                               select x.FriendId).First();

            var myCurrentId = (from y in db.FriendsLists
                               where y.MyId == myFriendsId
                               select y.MyId).First();

            if (myFriendsId == myCurrentId)
            {
                //Add the current user to the connected table
                if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
                {
                    ConnectedUsers.Add(new UserDetail { ConnectionId = id, UserName = UserName + "-" + UserID, UserID = UserID });
                }

                UserDetail CurrentUser = ConnectedUsers.Where(u => u.ConnectionId == id).FirstOrDefault();
                var connection = ConnectedUsers;

                // send to caller           
                Clients.Caller.onConnected(CurrentUser.UserID.ToString(), CurrentUser.UserName, ConnectedUsers, CurrentMessage, CurrentUser.UserID);

                // send to all except caller client           
                Clients.AllExcept(CurrentUser.ConnectionId).onNewUserConnected(CurrentUser.UserID.ToString(), CurrentUser.UserName, CurrentUser.UserID);
            }
 
        }

        public void SendMessageToAll(string userName, string message)
        {
            // store last 100 messages in cache
            //AddMessageinCache(userName, message);

            // Broad cast message
            //Clients.All.messageReceived(userName, message);
        }

        public void SendPrivateMessage(string toUserId, string message)
        {
            //Bad word filter
            string newMessage = message.Replace("shit", "****");

            //Translation portion
            TranslationClient client = TranslationClient.Create();
            var detection = client.DetectLanguage(message);
            var language = "en";
            if (detection.Language == "zh-CN")
            {
                language = "en";
            }
            else if (detection.Language == "en")
            {
                language = "zh-CN";
            }
            if (detection.Language == "es")
            {
                language = "en";
            }

            var response = client.TranslateText(message, language);
            var translatedText = response.TranslatedText;
            translatedText = newMessage + "\n" + translatedText;

            //Send the message
            try
            {
                string fromconnectionid = Context.ConnectionId;
                string strfromUserId = (ConnectedUsers.Where(u => u.ConnectionId == Context.ConnectionId).Select(u => u.UserID).FirstOrDefault()).ToString();
                string _fromUserId = strfromUserId;
                
                string _toUserId = toUserId;
                
                List<UserDetail> FromUsers = ConnectedUsers.Where(u => u.UserID == _fromUserId).ToList();
                List<UserDetail> ToUsers = ConnectedUsers.Where(x => x.UserID == _toUserId).ToList();

                if (FromUsers.Count != 0 && ToUsers.Count() != 0)
                {
                    foreach (var ToUser in ToUsers)
                    {
                        // send to                                                                                            //Chat Title
                        Clients.Client(ToUser.ConnectionId).sendPrivateMessage(_fromUserId.ToString(), FromUsers[0].UserName, FromUsers[0].UserName, translatedText);
                    }


                    foreach (var FromUser in FromUsers)
                    {
                        // send to caller user                                                                                //Chat Title
                        Clients.Client(FromUser.ConnectionId).sendPrivateMessage(_toUserId.ToString(), FromUsers[0].UserName, ToUsers[0].UserName, newMessage);
                    }
                    // send to caller user
                    //Clients.Caller.sendPrivateMessage(_toUserId.ToString(), FromUsers[0].UserName, message);
                    //ChatDB.Instance.SaveChatHistory(_fromUserId, _toUserId, message);
                    MessageDetail _MessageDeail = new MessageDetail { FromUserID = _fromUserId, FromUserName = FromUsers[0].UserName, ToUserID = _toUserId, ToUserName = ToUsers[0].UserName, Message = message };
                    AddMessageinCache(_MessageDeail);
                }
            }
            catch { }
        }

        public void RequestLastMessage(string FromUserID, string ToUserID)
        {
            List<MessageDetail> CurrentChatMessages = (from u in CurrentMessage where ((u.FromUserID == FromUserID && u.ToUserID == ToUserID) || (u.FromUserID == ToUserID && u.ToUserID == FromUserID)) select u).ToList();
            //send to caller user
            Clients.Caller.GetLastMessages(ToUserID, CurrentChatMessages);
        }

        public void SendUserTypingRequest(string toUserId)
        {
            string strfromUserId = (ConnectedUsers.Where(u => u.ConnectionId == Context.ConnectionId).Select(u => u.UserID).FirstOrDefault()).ToString();

            string _toUserId = toUserId;
            //int.TryParse(toUserId, out _toUserId);
            List<UserDetail> ToUsers = ConnectedUsers.Where(x => x.UserID == _toUserId).ToList();

            foreach (var ToUser in ToUsers)
            {
                // send to                                                                                            
                Clients.Client(ToUser.ConnectionId).ReceiveTypingRequest(strfromUserId);
            }
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                ConnectedUsers.Remove(item);
                if (ConnectedUsers.Where(u => u.UserID == item.UserID).Count() == 0)
                {
                    var id = item.UserID.ToString();
                    Clients.All.onUserDisconnected(id, item.UserName);
                }
            }
            return base.OnDisconnected(stopCalled);
        }
        #endregion

        #region---private Messages---
        private void AddMessageinCache(MessageDetail _MessageDetail)
        {
            CurrentMessage.Add(_MessageDetail);
            if (CurrentMessage.Count > 100)
                CurrentMessage.RemoveAt(0);
        }
        #endregion
    }
}