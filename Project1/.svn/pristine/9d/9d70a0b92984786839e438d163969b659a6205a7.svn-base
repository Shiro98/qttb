
using JWTWebApi.Models;
using JWTWebApi.Service;
using log4net; 
using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using JWTWebApi.EntityManager;

namespace JWTWebApi.Controllers
{
    public class LoginController : Controller
    { 
        private readonly ITokenService tokenService = new TokenService();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly int CookieMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["AccessTokenDurationInMinutes"].ToString());
        private readonly UserManager userManager = new UserManager();
        // GET: Login
        public ActionResult Index(string appcode)
        {
            ViewBag.ReturnUrl = appcode;
            if (Request.Cookies["UserToken"] != null)
            { 
                return RedirectToAction("Index","Home"); 
            }
            return View();
        }
        //[ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            var urlLogin = "/Login/Index?appcode=" + model.AppCode;
            if (ModelState.IsValid)
            {
                var passSha256 = Encryptor.HmacSha256Hash(model.UserName.ToUpper() + model.Password);
                var result = tokenService.CheckLogin(model.UserName, passSha256);
                if (result == 1)
                {
                    var token = tokenService.GenerateAccessToken(model.UserName);
                    var listIdValue = new HttpCookie("UserToken", token);
                    listIdValue.Expires.AddMinutes(CookieMinutes);
                    HttpContext.Response.Cookies.Add(listIdValue);
                    AddLog("Đăng nhập( UserName: " + model.UserName + "), AppCode: " + (!string.IsNullOrEmpty(model.AppCode) ? model.AppCode : "Home") + " thành công.");
                    if(!string.IsNullOrEmpty(model.AppCode))
                    {
                        var appUrl = tokenService.GetUrlByAppCode(model.AppCode);
                        if (!string.IsNullOrEmpty(appUrl))
                            return Redirect(appUrl);
                        else
                            return RedirectToAction("Index", "Home");
                    }    
                    else
                        return RedirectToAction("Index", "Home");
                }
                else if (result == 0)
                {
                    AddLog("Đăng nhập( UserName: " + model.UserName + "), AppCode: " + model.AppCode + "  lỗi: Tài khoản không tồn tại.");
                    ModelState.AddModelError("", "Tài khoản không tồn tại.");
                }
                else if (result == -1)
                {
                    AddLog("Đăng nhập( UserName: " + model.UserName + "), AppCode: " + model.AppCode + "  lỗi: Tài khoản đang bị khóa.");
                    ModelState.AddModelError("", "Tài khoản đang bị khóa.");
                }
                else if (result == -2)
                {
                    AddLog("Đăng nhập( UserName: " + model.UserName + "), AppCode: " + model.AppCode + "  lỗi: Mật khẩu không đúng.");
                    ModelState.AddModelError("", "Mật khẩu không đúng.");
                }
                else
                {
                    AddLog("Đăng nhập( UserName: " + model.UserName + "), AppCode: " + model.AppCode + "  lỗi: Đăng nhập không thành công.");
                    ModelState.AddModelError("", "Đăng nhập không thành công.");
                }

            }
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