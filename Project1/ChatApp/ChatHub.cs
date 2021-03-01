using ChatApp.Common;
using ChatApp.Models.Entity;
using ChatApp.Models.ModelCustom;
using Data.Admin;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatApp
{
    public class ChatHub : Hub
    {
        static List<Users> ConnectedUsers = new List<Users>();
        static List<Users> ConnectedUserOnline = new List<Users>();
        static List<Messages> CurrentMessage = new List<Messages>();
        static List<MessagePrivate> CurrentMessagePrivate = new List<MessagePrivate>();
        //ConnClass ConnC = new ConnClass();
        UserDA _userDA = new UserDA();
        ChatDA _ChatDA = new ChatDA();
        public ChatHub()
        {
            string logintime = DateTime.Now.ToString();
            var lstUser = _userDA.GetListUser();
            ConnectedUserOnline = ConnectedUsers.Where(x => x.ConnectionId != null).ToList();
            ConnectedUsers = lstUser.Select(x => new Users
            {
                UserName = x.LOGIN_NAME,
                FullName = x.FULL_NAME,
                IsOnline = false,
                LoginTime = logintime,
                UserImage = x.IMG_PATH
            }).ToList();
            foreach(var item in ConnectedUserOnline)
            {
                var data = ConnectedUsers.FirstOrDefault(x => x.UserName.ToLower() == item.UserName.ToLower());
                data.ConnectionId = item.ConnectionId;
                data.IsOnline = true;
            }
        }
        public void Connect(string userName)
        {
            var id = Context.ConnectionId;
            string logintime = DateTime.Now.ToString();
            if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
            {
                string UserImg = "";
                if (ConnectedUsers.Count(x => x.UserName == userName) > 0)
                {
                    var data = ConnectedUsers.FirstOrDefault(x => x.UserName == userName);
                    data.ConnectionId = id;
                    data.IsOnline = true;
                }
                else
                {
                    ConnectedUsers.Add(new Users { ConnectionId = id, UserName = userName, UserImage = UserImg, LoginTime = logintime, IsOnline = true });
                }

                // send to caller
                Clients.Caller.onConnected(id, userName, ConnectedUsers, CurrentMessagePrivate);

                // send to all except caller client
                Clients.AllExcept(id).onNewUserConnected(id, userName, UserImg, logintime);
            }
        }

        public void SendMessageToAll(string userName, string message, string time)
        {
            string UserImg = "";
            // store last 100 messages in cache
            AddMessageinCache(userName, message, time, UserImg);

            // Broad cast message
            Clients.All.messageReceived(userName, message, time, UserImg);

        }
        
        private void AddMessageinCache(string userName, string message, string time, string UserImg)
        {
            CurrentMessage.Add(new Messages { UserName = userName, Message = message, Time = time, UserImage = UserImg });

            if (CurrentMessage.Count > 100)
                CurrentMessage.RemoveAt(0);

        }
        private void AddMessageinCachePrivate(string fromUser, string toUser, string message, string time)
        {
            CurrentMessagePrivate.Add(new MessagePrivate { fromUser = fromUser, toUser = toUser, Message = message, Time = time });

            if (CurrentMessagePrivate.Count > 100)
                CurrentMessagePrivate.RemoveAt(0);

        }
        // Clear Chat History
        public void clearTimeout()
        {
            CurrentMessage.Clear();
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                //ConnectedUsers.Remove(item);

                //var id = Context.ConnectionId;
                //Clients.All.onUserDisconnected(id, item.UserName);
                item.ConnectionId = null;
            }
            return base.OnDisconnected(stopCalled);
        }

        public void SendPrivateMessage(string toUserId, string toUserName, string message, int isMedia)
        {

            string fromUserId = Context.ConnectionId;

            var toUser = ConnectedUsers.FirstOrDefault(x => x.UserName == toUserName);
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromUserId);

            if (toUser != null && fromUser != null)
            {
                if (!string.IsNullOrEmpty(toUser.ConnectionId))
                {
                    string CurrentDateTime = DateTime.Now.ToString();
                    string UserImg = "";
                    // send to 
                    Clients.Client(toUserId).sendPrivateMessage(fromUserId, fromUser.UserName, message, UserImg, CurrentDateTime);

                    // send to caller user
                    Clients.Caller.sendPrivateMessage(toUserId, fromUser.UserName, message, UserImg, CurrentDateTime);
                    CONVERSATION conver = new CONVERSATION();
                    conver.SEND_USER = fromUser.UserName;
                    conver.RECIVE_USER = toUser.UserName;
                    conver.MESSAGE = message;
                    conver.ISREAD = false;
                    conver.CREATED_DATE = DateTime.Now;
                    conver.ATTACH_FILE_ID = isMedia;
                    _ChatDA.AddMessage(conver);
                    //AddMessageinCachePrivate(fromUser.UserName, toUser.UserName, message, CurrentDateTime);
                }
                else
                {
                    CONVERSATION conver = new CONVERSATION();
                    conver.SEND_USER = fromUser.UserName;
                    conver.RECIVE_USER = toUser.UserName;
                    conver.MESSAGE = message;
                    conver.ISREAD = false;
                    conver.CREATED_DATE = DateTime.Now;
                    conver.ATTACH_FILE_ID = isMedia;
                    _ChatDA.AddMessage(conver);
                }
            }

        }
    }
}