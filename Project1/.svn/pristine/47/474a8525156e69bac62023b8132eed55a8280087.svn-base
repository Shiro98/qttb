using ChatApp.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatApp.Common
{
    public class ChatDA
    {
        CHAT_APPEntities db = new CHAT_APPEntities();

        public object AddMessage(CONVERSATION conversation)
        {
            try
            {
                conversation.CONVERSATION_ID = Guid.NewGuid();
                db.CONVERSATIONS.Add(conversation);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public List<CONVERSATION> GetHistoryMesage(string fromUser, string toUser)
        {
            try
            {
                return db.CONVERSATIONS.Where(x => (x.SEND_USER == fromUser && x.RECIVE_USER == toUser) || (x.SEND_USER == toUser && x.RECIVE_USER == fromUser)).ToList();
            }
            catch (Exception ex)
            {
                return new List<CONVERSATION>();
            }
        }
    }
}