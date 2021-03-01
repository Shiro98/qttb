using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChatApp.Common;
using ChatApp.Models.ModelCustom;
using Data.Admin;
using Microsoft.AspNet.SignalR;
using Model.Model;
using Simple.Base;
using WebApp.Service;

namespace ChatApp.Controllers
{
    public class ChatController : Controller
    {
        private static readonly string AppCode = ConfigurationManager.AppSettings["AppCode"];
        private static readonly string ApiUri = ConfigurationManager.AppSettings["ApiUri"];
        //private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        ITokenService _ITokenService = new TokenService();
        PageMenuDA _pageMenuDA = new PageMenuDA();
        UserDA _userDA = new UserDA();
        Entities db = new Entities();
        ChatDA _ChatDA = new ChatDA();
        // GET: Chat
        public ActionResult Index()
        {
            if (Request.Cookies["UserToken"] != null)
            {
                var _AppCode = "";
                //if (AppCode == "CHAT_APP")
                //    _AppCode = "CHAT_APP";
                var token = Request.Cookies["UserToken"].Value.ToString();
                // kiểm tra xem token còn hạn không

                var userName = _ITokenService.ValidateToken(token);
                ViewBag.UserName = userName;
                if (!string.IsNullOrEmpty(userName))
                {
                    var user = _userDA.GetItemByUserName(userName);
                    // Kiểm xem người dùng có bị khóa không
                    var check = _userDA.CheckLock(user.USER_ID);
                    if (check == false)
                    {
                        var session = (UserLogin)Session["USER_SESSION"];
                        if (session == null)
                        {
                            var userSession = new UserLogin();
                            userSession.UserName = user.LOGIN_NAME;
                            userSession.UserID = user.USER_ID;
                            userSession.Name = user.FULL_NAME;
                            userSession.Appcode = _AppCode;
                            var listPermission = _userDA.GetListCredentials(userName);

                            var menus = _pageMenuDA.GetMenuByUser(user.USER_ID, userSession.Appcode);
                            Session.Add("CEDENTIALS_SESSION", listPermission);
                            Session.Add("Menus", menus);
                            Session.Add("USER_SESSION", userSession);
                        }
                    }
                    else
                    {
                        Session["USER_SESSION"] = null;
                        Session["CEDENTIALS_SESSION"] = null;
                        Session["Menus"] = null;
                        Request.Cookies.Remove("UserToken");
                        if (Request.Cookies["UserToken"] != null)
                        {
                            Response.Cookies["UserToken"].Expires = DateTime.Now.AddDays(-1);
                        }
                        return Redirect(ApiUri + "Login/Index?appcode=" + AppCode);
                    }
                }
                else
                {
                    Session["USER_SESSION"] = null;
                    Session["CEDENTIALS_SESSION"] = null;
                    Session["Menus"] = null;
                    Request.Cookies.Remove("UserToken");
                    if (Request.Cookies["UserToken"] != null)
                    {
                        Response.Cookies["UserToken"].Expires = DateTime.Now.AddDays(-1);
                    }
                    return Redirect(ApiUri + "Login/Index?appcode=" + AppCode);
                }
            }
            else
            {
                Session["USER_SESSION"] = null;
                Session["CEDENTIALS_SESSION"] = null;
                Session["Menus"] = null;
                Request.Cookies.Remove("UserToken");
                if (Request.Cookies["UserToken"] != null)
                {
                    Response.Cookies["UserToken"].Expires = DateTime.Now.AddDays(-1);
                }
                return Redirect(ApiUri + "Login/Index?appcode=" + AppCode);
            }
            return View();
        }
        public ActionResult ChatBox()
        {
            return View();
        }

        #region Lấy dữ liệu lịch sử chat
        public object GetHistoryMessage(string toUser)
        {
            var session = (UserLogin)Session["USER_SESSION"];
            var dataHistoryMessage = _ChatDA.GetHistoryMesage(session.UserName, toUser);
            var CurrentMessagePrivate = new List<MessagePrivate>();
            CurrentMessagePrivate = dataHistoryMessage.Select(x => new MessagePrivate
            {
                fromUser = x.SEND_USER,
                toUser = x.RECIVE_USER,
                Message = x.MESSAGE,
                Time = x.CREATED_DATE.ToString(),
                IsRead = x.ISREAD,
                IsMedia = x.ATTACH_FILE_ID
            }).OrderBy(x => x.Time).ToList();
            return Json(CurrentMessagePrivate);
        }
        #endregion
        #region send file attach
        public object SendAttachFile()
        {
            var obj = new MessageFile();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                                                            //Use the following properties to get file's name, size and MIMEType
                obj.FileSize = file.ContentLength;
                obj.FileName = file.FileName;
                obj.MimeType = file.ContentType;
                obj.Path = "~/Content/ChatFile/" + obj.FileName;
                System.IO.Stream fileContent = file.InputStream;
                //To save file, use SaveAs method
                file.SaveAs(Server.MapPath("~/Content/ChatFile/") + obj.FileName); //File will be saved in application root
            }
            return Json(obj);
        }
        #endregion
    }
    public class MessageFile
    {
        public int FileSize { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public string Path { get; set; }
    }
}