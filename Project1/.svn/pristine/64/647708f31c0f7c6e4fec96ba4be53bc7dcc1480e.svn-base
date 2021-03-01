using JWTWebApi.Service;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JWTWebApi.Controllers
{
    public class LogOutController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ITokenService tokenService = new TokenService();
        // GET: LogOut
        public ActionResult Index(string appcode)
        {
            var urlLogin = "/Login/Index?appcode=" + appcode;
            if (Request.Cookies["UserToken"] == null)
            {
                return Redirect(urlLogin);
            }

            var token =  Request.Cookies["UserToken"].Value;
            var getUserName = tokenService.ValidateToken(token);
            RemoveCookies("UserToken");
            AddLog("Đăng xuất( UserName: " + getUserName + "), AppCode: "+ appcode + " thành công.");
            return Redirect(urlLogin); 
        }
        public void RemoveCookies(string ckName)
        {
            if (Request.Cookies[ckName] != null)
            {
                var c = new HttpCookie(ckName);
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }
        }
        private void AddLog(string content)
        {
            log.Error("[Login][]: " + content);
        }
    }
}