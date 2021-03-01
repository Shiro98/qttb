using Data.Admin;
using Model.Model;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Service;
using WebGrease;

namespace ChatApp.Controllers
{
    public class HomeController : Controller
    {
        private static readonly string AppCode = ConfigurationManager.AppSettings["AppCode"];
        private static readonly string ApiUri = ConfigurationManager.AppSettings["ApiUri"];
        //private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        ITokenService _ITokenService = new TokenService();
        PageMenuDA _pageMenuDA = new PageMenuDA();
        UserDA _userDA = new UserDA();
        Entities db = new Entities();
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
    }
}