using JWTWebApi.EntityManager;
using JWTWebApi.Service;
using System; 
using System.Web.Mvc;
using JWTWebApi.Models;

namespace JWTWebApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITokenService tokenService = new TokenService();
        private readonly UserManager userManager = new UserManager();
        private readonly AppManager appManager = new AppManager();
        public ActionResult Index()
        {
            ViewBag.Title = "Trang chủ";
            if (Request.Cookies["UserToken"] != null)
            { 
                var token = Request.Cookies["UserToken"].Value.ToString();
                // kiểm tra xem token còn hạn không 
                var userName = tokenService.ValidateToken(token);
                if (!string.IsNullOrEmpty(userName))
                { 
                    var user = userManager.GetInfoByUserName(userName);
                    // Kiểm xem người dùng có bị khóa không
                    var check = userManager.CheckLock(user.USER_ID);
                    if (check == false)
                    {
                        ViewBag.ListSite = appManager.GetAllApp();
                        var session = (UserLoginModel)Session["USER_SESSION"];
                        if (session == null)
                        {
                            var userSession = new UserLoginModel();
                            userSession.UserName = user.LOGIN_NAME;
                            userSession.UserID = user.USER_ID;
                            userSession.Name = user.FULL_NAME; 
                            Session.Add("USER_SESSION", userSession);

                            ViewBag.UserInfo = userSession;
                        }
                        else
                        {
                            ViewBag.UserInfo = session;
                        }    
                    }
                    else
                    {
                        Session["USER_SESSION"] = null; 
                        if (Request.Cookies["UserToken"] != null)
                        {
                            Response.Cookies["UserToken"].Expires = DateTime.Now.AddDays(-1);
                        }
                        return RedirectToAction("Index", "Login");
                    }
                }
                else
                {
                    Session["USER_SESSION"] = null; 
                    Request.Cookies.Remove("UserToken");
                    if (Request.Cookies["UserToken"] != null)
                    {
                        Response.Cookies["UserToken"].Expires = DateTime.Now.AddDays(-1);
                    }
                    return RedirectToAction("Index", "Login");
                }
            }
            else
            {
                Session["USER_SESSION"] = null; 
                Request.Cookies.Remove("UserToken");
                if (Request.Cookies["UserToken"] != null)
                {
                    Response.Cookies["UserToken"].Expires = DateTime.Now.AddDays(-1);
                }
                return RedirectToAction("Index","Login");
            }
            return View();
        }
    }
}
