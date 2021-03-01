using Data.Admin;
using log4net;
using Microsoft.Reporting.WebForms;
using Model.Model;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using WebAdmin.Service;

namespace WebAdmin.Controllers
{
    public class HomeController : Controller
    {
        private static readonly string AppCode = ConfigurationManager.AppSettings["AppCode"];
        private static readonly string ApiUri = ConfigurationManager.AppSettings["ApiUri"];
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        ITokenService _ITokenService = new TokenService();
        PageMenuDA _pageMenuDA = new PageMenuDA();
        UserDA _userDA = new UserDA();
        Entities db = new Entities();
        public ActionResult Index()
        {
            //log4net.Config.XmlConfigurator.Configure();
            //log.Error("Home");
            //string clientIPAddress = string.Empty;
            //try
            //{
            //    clientIPAddress = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(0).ToString();
            //    IPHostEntry Host = default(IPHostEntry);
            //    string Hostname = null;
            //    Hostname = System.Environment.MachineName;
            //    Host = Dns.GetHostEntry(Hostname);
            //    foreach (IPAddress IP in Host.AddressList)
            //    {
            //        if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            //        {
            //            clientIPAddress = Convert.ToString(IP);
            //        }
            //    }
            //}
            //catch
            //{
            //    clientIPAddress = "";
            //}
            //log.Error(clientIPAddress);
            if (Request.Cookies["UserToken"] != null)
            {
                var _AppCode = "";
                if (AppCode == "THIET_BI_VT1")
                    _AppCode = "THIET_BI_VT";
                var token = Request.Cookies["UserToken"].Value.ToString();
                // kiểm tra xem token còn hạn không

                var userName = _ITokenService.ValidateToken(token);
                if (!string.IsNullOrEmpty(userName))
                {
                    var user = _userDA.GetItemByUserName(userName);
                    // Kiểm xem người dùng có bị khóa không
                    var check = _userDA.CheckLock(user.USER_ID);
                    if(check == false)
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
                            log4net.GlobalContext.Properties["UserName"] = user.LOGIN_NAME;
                            log4net.Config.XmlConfigurator.Configure();
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