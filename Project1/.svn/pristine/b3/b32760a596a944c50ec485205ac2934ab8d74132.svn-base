using Common;
using Data.Admin;
using log4net;
using Model.Model;
using Model.ModelExtend;
using Newtonsoft.Json;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using WebApp.Common;
using WebAdmin.Models.API;

namespace WebAdmin.Controllers
{
    public class LoginController : Controller
    {
        PageMenuDA _pageMenuDA = new PageMenuDA();
        UserDA _userDA = new UserDA();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string ApiUri = ConfigurationManager.AppSettings["ApiUri"];
        private static readonly string AppCode = ConfigurationManager.AppSettings["AppCode"];

        // GET: Login
        public ActionResult Index()
        {
            if (Request.Cookies["UserToken"] != null)
            {
                var userToken = JsonConvert.DeserializeObject<TokenModel>(Request.Cookies["UserToken"].Value);
                // kiểm tra xem token còn hạn không

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.BaseAddress = new Uri(userToken.ApiUri);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", parameter: userToken.Token + ":" + userToken.LoginName);
                    var response = client.GetAsync(requestUri: "TokenManager/GetValidateToken");
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var resultMessage = response.Result.Content.ReadAsStringAsync().Result;
                        string ReturnMessage = JsonConvert.DeserializeObject<string>(resultMessage);
                        var userSession = new UserLogin();
                        var user = _userDA.GetItemByUserName(userToken.LoginName);
                        userSession.UserName = user.LOGIN_NAME;
                        userSession.UserID = user.USER_ID;
                        userSession.Name = user.FULL_NAME;
                        userSession.Appcode = "THIET_BI_VT";
                        var listPermission = _userDA.GetListCredentials(userToken.LoginName);

                        var menus = _pageMenuDA.GetMenuByUser(user.USER_ID, userSession.Appcode);
                        Session.Add("CEDENTIALS_SESSION", listPermission);
                        Session.Add("Menus", menus);
                        Session.Add("USER_SESSION", userSession);
                        log4net.GlobalContext.Properties["UserName"] = user.LOGIN_NAME;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        Session.Clear();
                        Request.Cookies.Clear();
                    }
                }
            }
            else
            {
                Session.Clear();
                Request.Cookies.Clear();
            }

            return View();
        }
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _userDA.Login(model.UserName, Encryptor.MD5Hash(model.UserName.ToUpper()+model.Password));
                if (result == 1)
                {
                    // Gọi API lấy token
                    var token = GetToken(model.UserName);
                    if (!string.IsNullOrEmpty(token))
                    {
                        var tokenModel = new TokenModel() { LoginName = model.UserName , Token = token, ApiUri = ApiUri };
                        // Tạo Cookie
                        var listIdValue = new HttpCookie("UserToken", JsonConvert.SerializeObject(tokenModel));
                        listIdValue.Expires.AddDays(1);
                        HttpContext.Response.Cookies.Add(listIdValue);
                    }

                    var userSession = new UserLogin();
                    var user = _userDA.GetItemByUserName(model.UserName);
                    userSession.UserName = user.LOGIN_NAME;
                    userSession.UserID = user.USER_ID;
                    userSession.Name = user.FULL_NAME;
                    userSession.Appcode = "THIET_BI_VT";
                    var listPermission = _userDA.GetListCredentials(model.UserName);

                    var menus = _pageMenuDA.GetMenuByUser(user.USER_ID, userSession.Appcode);
                    Session.Add("CEDENTIALS_SESSION", listPermission);
                    Session.Add("Menus", menus);
                    Session.Add("USER_SESSION", userSession);
                    log4net.GlobalContext.Properties["UserName"] = user.LOGIN_NAME;
                    AddLog("Đăng nhập( UserName: " + user.LOGIN_NAME + ") thành công.");
                    return RedirectToAction("Index", "Home");
                }
                else if (result == 0)
                {
                    AddLog("Đăng nhập( UserName: " + model.UserName + ") lỗi: Tài khoản không tồn tại.");
                    ModelState.AddModelError("", "Tài khoản không tồn tại.");
                }
                else if (result == -1)
                {
                    AddLog("Đăng nhập( UserName: " + model.UserName + ") lỗi: Tài khoản đang bị khóa.");
                    ModelState.AddModelError("", "Tài khoản đang bị khóa.");
                }
                else if (result == -2)
                {
                    AddLog("Đăng nhập( UserName: " + model.UserName + ") lỗi: Mật khẩu không đúng.");
                    ModelState.AddModelError("", "Mật khẩu không đúng.");
                }
                else
                {
                    AddLog("Đăng nhập( UserName: " + model.UserName + ") lỗi: Đăng nhập không thành công.");
                    ModelState.AddModelError("", "Đăng nhập không thành công.");
                }

            }
            return View("Index");
        }

        public string ConvertPathImageToBase64(string path)
        {
            string base64String = "";
            var pathRoot = Server.MapPath("~/FileUpload");

            var arrayPath = path.IndexOf("FileUpload");
            string pathGet = path.Substring(0, arrayPath) + "FileUpload";
            string patttt = path.Replace(pathGet, pathRoot);


            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    using (Image image = Image.FromFile(path))
                    {
                        using (MemoryStream m = new MemoryStream())
                        {
                            image.Save(m, image.RawFormat);
                            byte[] imageBytes = m.ToArray();

                            // Convert byte[] to Base64 String
                            base64String = "data:image/jpeg;base64," + Convert.ToBase64String(imageBytes);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return base64String;
        }

        /// <summary>
        /// Lấy token
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string GetToken(string userName)
        {
            var token = "";
            string url = "TokenManager/GenerateToken?userName=" + userName;
            HttpResponseMessage response = (ApiBase.GetToken(url)).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                token = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result).ToString();
            }
            else
            {
                var message = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result).ToString();
                AddLog(message);
            }

            return token;
                
        }
        
        public ActionResult Logout()
        {
            var user = Session["USER_SESSION"] as UserLogin;
            AddLog("Đăng xuất( UserName: " + user.UserName + ") thành công.");
            Session["USER_SESSION"] = null;
            Session["CEDENTIALS_SESSION"] = null;
            Session["Menus"] = null;
            Request.Cookies.Remove("UserToken");
            if (Request.Cookies["UserToken"] != null)
            {
                Response.Cookies["UserToken"].Expires = DateTime.Now.AddDays(-1);
            }
            return Redirect(ApiUri + "LogOut/Index?appcode=" + AppCode);
        }

        private void AddLog(string content)
        {
            log.Error("[Login][]: " + content);
        }
    }
}