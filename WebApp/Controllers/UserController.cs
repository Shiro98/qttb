using Common;
using Data.Admin;
using log4net;
using Model.Model;
using Model.ModelExtend;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Common;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class UserController : BaseController
    {
        ITEMS_SYSTEMEntities db = new ITEMS_SYSTEMEntities();
        UserDA _userDA = new UserDA();
        BaseController _helperController = new BaseController();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: User
        [HasCredential(ControllerName = "User")]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetProfile(int Id)
        {
            var model = db.ST_USERS.Select(x => new ViewProfile
            {
                ID = x.USER_ID,
                Name = x.FULL_NAME,
            }).FirstOrDefault(x => x.ID == Id);

            if (!string.IsNullOrEmpty(model.Avartar))
            {
                model.Avartar = GetBase64Avatar(model.Avartar);
            }

            AddLog("Lấy dữ liệu chi tiết bảng Người dùng( ID: " + Id + ") thành công.");
            return View(model);
        }

        private void AddLog(string content)
        {
            var user = Session["USER_SESSION"] as UserLogin;
            log.Error("[User][" + user.UserName + "]: " + content);
        }
        public ActionResult ChangePassword(int Id)
        {
            return View();
        }

        public ActionResult _Add()
        {
            return PartialView("_add");
        }
        public ActionResult _Edit()
        {
            return PartialView("_edit");
        }

        public ActionResult _LockUser()
        {
            return PartialView("_lockUser");
        }
        [HttpPost]
        public ActionResult GetListUser(ModelSearchUser modelSearch)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                var pageSize = 0;
                var data = new List<UserPageModel>();
                int totalItems = 0;
                using (var context = new ITEMS_SYSTEMEntities())
                {
                    var paramList = new List<SqlParameter>
                    {
                        new SqlParameter(@"keyword", !string.IsNullOrEmpty(modelSearch.KeyWord)? (object)modelSearch.KeyWord : DBNull.Value),
                        new SqlParameter(@"page", modelSearch.currentPage),
                        new SqlParameter(@"OrderByClause", modelSearch.SortColumn),
                        new SqlParameter(@"pageSize", SqlDbType.Int) { Direction = ParameterDirection.Output }
                    };
                    data = context.Database.SqlQuery<UserPageModel>("exec [ST_USERS_SEARCH] @keyword, @page, @OrderByClause, @pageSize OUT", paramList.ToArray()).ToList();
                    if (!string.IsNullOrEmpty(((SqlParameter)paramList[3]).Value.ToString()))
                        pageSize = Convert.ToInt32(((SqlParameter)paramList[3]).Value);

                    if (data != null && data.Count > 0)
                        totalItems = data.FirstOrDefault().TotalRow;
                }
                AddLog("Lấy dữ liệu theo trang bảng Người dùng( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") thành công.");

                return Json(new { data = data, totalItems = totalItems, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu theo trang bảng Người dùng( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }


        [HttpPost]
        public ActionResult GetDanhMuc()
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                // lấy danh sách quyền
                var roles = db.ST_ROLES.Select(x => new
                {
                    ID = x.ROLE_ID,
                    Name = x.ROLE_TYPE
                })
                .OrderBy(x => x.Name).ToList();

                // lấy danh sách đơn vị
                var units = db.ST_UNITS.Where(x => x.ENABLED_FLAG == "Y").OrderBy(x => x.UNIT_NAME).ToList();

                return Json(new { Roles = roles, Units = units, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                return Json(obj);
            }
        }

        [HttpPost]
        public object GetItemByID(int? Id)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var data = db.ST_USERS.FirstOrDefault(x => x.USER_ID == Id);
                var roleId = db.ST_USER_ROLES.Where(x => x.USER_ID == data.USER_ID).Select(x => x.ROLE_ID);

                var START_DATE = "";
                if (!string.IsNullOrEmpty(data.START_DATE))
                {
                    START_DATE = data.START_DATE.Substring(4, 2) + "/" + data.START_DATE.Substring(6, 2) + "/" + data.START_DATE.Substring(0, 4);
                }
                var END_DATE = "";
                if (!string.IsNullOrEmpty(data.END_DATE))
                {
                    END_DATE = data.END_DATE.Substring(4, 2) + "/" + data.END_DATE.Substring(6, 2) + "/" + data.END_DATE.Substring(0, 4);
                }

                AddLog("Lấy dữ liệu theo ID bảng Người dùng( ID: " + Id + ") thành công.");
                return Json(new { Error = false, Title = "Lấy dữ liệu thành công.", data = data, RoleId = roleId, START_DATE = START_DATE, END_DATE = END_DATE });
            }
            catch (Exception ex)
            {
                AddLog("Lấy dữ liệu theo ID bảng Người dùng( ID: " + Id + ") lỗi: " + ex.Message);
                return Json(new { Error = true, Title = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ConvertPathImageToBase64(string path)
        {
            try
            {
                var pathRoot = Server.MapPath("~/FileUpload");

                var arrayPath = path.IndexOf("FileUpload");
                string pathGet = path.Substring(0, arrayPath) + "FileUpload";
                string patttt = path.Replace(pathGet, pathRoot);

                string base64String = "";
                try
                {
                    if (!string.IsNullOrEmpty(patttt))
                    {
                        using (Image image = Image.FromFile(patttt))
                        {
                            using (MemoryStream m = new MemoryStream())
                            {
                                image.Save(m, image.RawFormat);
                                byte[] imageBytes = m.ToArray();

                                // Convert byte[] to Base64 String
                                base64String = Convert.ToBase64String(imageBytes);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                var result = Json(new { data = base64String }, JsonRequestBehavior.AllowGet);
                result.MaxJsonLength = int.MaxValue;
                return result;
            }
            catch (Exception)
            {
                return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        public string GetBase64Avatar(string path)
        {
            string base64String = "";
            var pathRoot = Server.MapPath("~/FileUpload");

            var arrayPath = path.IndexOf("FileUpload");
            if (arrayPath >= 0)
            {
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
            }
            else
            {
                base64String = path;
            }

            return base64String;
        }

        [HttpPost]
        [QLTBAuthorize(role = "btnCreate")]
        public object Add(ST_USERS user, string fileName, string roleId)
        {
            ObjectMessage obj = new ObjectMessage();
            obj.Error = false;
            try
            {
                var userDN = Session["USER_SESSION"] as UserLogin;
                var checkTrungUser = db.ST_USERS.Where(x => x.LOGIN_NAME == user.LOGIN_NAME).ToList();
                if (checkTrungUser == null || checkTrungUser.Count == 0)
                {
                    if (!string.IsNullOrEmpty(user.IMG_PATH))
                    {
                        var pathPhoto = CoppyPhoto(user.IMG_PATH, fileName, user.LOGIN_NAME, user.FULL_NAME);
                        user.IMG_PATH = pathPhoto;
                    }
                    user.ENABLED_FLAG = "Y";
                    user.UPDATED_BY = (int)userDN.UserID;
                    user.UPDATE_DATE = DateTime.Now;
                    obj = _userDA.Add(user, roleId, userDN.Appcode);
                    if (obj.Error)
                    {
                        if (!string.IsNullOrEmpty(user.IMG_PATH))
                            DeleteAvatar(user.IMG_PATH, user.LOGIN_NAME, user.FULL_NAME);
                        AddLog("Thêm mới dữ liệu bảng Người dùng(LOGIN_NAME: " + user.LOGIN_NAME + ", FULL_NAME: " + user.FULL_NAME + ") lỗi: " + obj.Title);
                    }
                    else
                        AddLog("Thêm mới dữ liệu bảng Người dùng(LOGIN_NAME: " + user.LOGIN_NAME + ", FULL_NAME: " + user.FULL_NAME + ") thành công.");
                }
                else
                {
                    obj.Error = true;
                    obj.Title = "Trùng tên đăng nhập.";
                    AddLog("Thêm mới dữ liệu bảng Người dùng(LOGIN_NAME: " + user.LOGIN_NAME + ", FULL_NAME: " + user.FULL_NAME + ") lỗi: Trùng tên đăng nhập.");
                }
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Thêm mới dữ liệu bảng Người dùng(LOGIN_NAME: " + user.LOGIN_NAME + ", FULL_NAME: " + user.FULL_NAME + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        /// <summary>
        /// Coppy ảnh lên server
        /// </summary>
        /// <param name="fileAnhCreate"></param>
        /// <returns></returns>
        public string CoppyPhoto(string base64, string fileName, string userName, string name)
        {
            string destFile = "";
            var pathFolder = Server.MapPath("~/FileUpload/Avatar");
            if (!Directory.Exists(pathFolder))
            {
                Directory.CreateDirectory(pathFolder);
            }
            var countFoleder = 0;
            var directory = System.IO.Directory.GetDirectories(pathFolder);
            if (directory != null)
                countFoleder = directory.Length + 1;
            var folder = Path.Combine(pathFolder + "\\", countFoleder.ToString());
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            try
            {
                destFile = Path.Combine(folder + "\\", fileName);
                var bytes = Convert.FromBase64String(base64);
                using (var imageFile = new FileStream(destFile, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }
                AddLog("Coppy file ảnh của người dùng(UserName: " + userName + ", Name: " + name + ") thành công.");
            }
            catch (Exception ex)
            {
                destFile = "";
                AddLog("Coppy file ảnh của người dùng(UserName: " + userName + ", Name: " + name + ") lỗi: " + ex.Message);
            }

            var folderServer = Server.MapPath("~/FileUpload");
            if (!string.IsNullOrEmpty(destFile))
                destFile = "/FileUpload" + destFile.Replace(folderServer, "");

            return destFile;
        }

        /// <summary>
        /// Coppy ảnh lên server
        /// </summary>
        /// <param name="fileAnhCreate"></param>
        /// <returns></returns>
        public void DeleteAvatar(string path, string userName, string name)
        {
            try
            {
                var pathRoot = Server.MapPath("~/FileUpload");

                var arrayPath = path.IndexOf("FileUpload");
                string pathGet = path.Substring(0, arrayPath) + "FileUpload";
                string patttt = path.Replace(pathGet, pathRoot);

                System.IO.File.Delete(path);
                AddLog("Xóa file ảnh của người dùng(UserName: " + userName + ", Name: " + name + ") thành công.");
            }
            catch (Exception ex)
            {
                AddLog("Xóa file ảnh của người dùng(UserName: " + userName + ", Name: " + name + ") lỗi: " + ex.Message);
            }
        }

        [HttpPost]
        [QLTBAuthorize(role = "btnUpdate")]
        public object Edit(ST_USERS user, string fileName, string roleId)
        {
            ObjectMessage obj = new ObjectMessage();
            obj.Error = false;
            try
            {
                var userDN = Session["USER_SESSION"] as UserLogin;
                if (!string.IsNullOrEmpty(user.IMG_PATH) && !string.IsNullOrEmpty(fileName))
                {
                    // xóa file cũ
                    var userOld = db.ST_USERS.FirstOrDefault(x => x.USER_ID == user.USER_ID);
                    if (!string.IsNullOrEmpty(userOld.IMG_PATH))
                    {
                        DeleteAvatar(user.IMG_PATH, user.LOGIN_NAME, user.FULL_NAME);
                    }
                    var pathPhoto = CoppyPhoto(user.IMG_PATH, fileName, user.LOGIN_NAME, user.FULL_NAME);
                    user.IMG_PATH = pathPhoto;
                }
                user.UPDATED_BY = (int)userDN.UserID;
                user.UPDATE_DATE = DateTime.Now;
                obj = _userDA.Edit(user, roleId, userDN.Appcode);
                if (obj.Error)
                {
                    if (!string.IsNullOrEmpty(user.IMG_PATH) && !string.IsNullOrEmpty(fileName))
                        DeleteAvatar(user.IMG_PATH, user.LOGIN_NAME, user.FULL_NAME);
                    AddLog("Cập nhật dữ liệu bảng Người dùng(LOGIN_NAME: " + user.LOGIN_NAME + ", FULL_NAME: " + user.FULL_NAME + ") lỗi: " + obj.Title);
                }
                else
                    AddLog("Cập nhật dữ liệu bảng Người dùng(LOGIN_NAME: " + user.LOGIN_NAME + ", FULL_NAME: " + user.FULL_NAME + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Cập nhật dữ liệu bảng Người dùng(LOGIN_NAME: " + user.LOGIN_NAME + ", FULL_NAME: " + user.FULL_NAME + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }
        [HttpPost]
        [QLTBAuthorize(role = "btnDelete")]
        public object Delete(int Id)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                obj = _userDA.Delete(Id, (int)user.UserID);
                if (obj.Error)
                    AddLog("Xóa dữ liệu bảng Người dùng(ID: " + Id + ") lỗi: " + obj.Title);
                else
                    AddLog("Xóa dữ liệu bảng Người dùng(ID: " + Id + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Xóa dữ liệu bảng Người dùng(ID: " + Id + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        [HttpPost]
        public object PostChangePassword(string passOld, string passNew)
        {
            var user = Session["USER_SESSION"] as UserLogin;
            ObjectMessage obj = new ObjectMessage();
            obj.Error = false;
            try
            {
                obj = _userDA.ChangePassword(user.UserID, user.UserName, passOld, passNew);

                if (obj.Error)
                {
                    AddLog("Thay đổi mật khẩu Người dùng(UserName: " + user.UserName + ", Name: " + user.Name + ") lỗi: " + obj.Title);
                }
                else
                {
                    AddLog("Thay đổi mật khẩu Người dùng(UserName: " + user.UserName + ", Name: " + user.Name + ") thành công.");
                    Session["USER_SESSION"] = null;
                }

                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Thay đổi mật khẩu Người dùng(UserName: " + user.UserName + ", Name: " + user.Name + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        [HttpPost]
        public ActionResult GetBottomAction()
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                var menu = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");
                var bottoms = _helperController.GetBottomRoleByController(controllerName, menu);
                AddLog("Lấy danh sách các botom được thực hiện trên from Người dùng thành công.");
                return Json(new { Buttoms = bottoms, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy danh sách các botom được thực hiện trên from Người dùng lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        [HttpPost]
        [QLTBAuthorize(role = "btnLock")]
        public object LockUser(int userId, string content)
        {
            ObjectMessage obj = new ObjectMessage();
            obj.Error = false;
            try
            {
                var userDN = Session["USER_SESSION"] as UserLogin;

                obj = _userDA.LockUser(userId, (int)userDN.UserID, content);
                if (obj.Error)
                    AddLog("Khóa Người dùng lỗi: " + obj.Title);
                else
                    AddLog("Khóa Người dùng thành công.");

                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Khóa Người dùng lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        [HttpPost]
        [QLTBAuthorize(role = "btnUnlock")]
        public object UnLockUser(int userId)
        {
            ObjectMessage obj = new ObjectMessage();
            obj.Error = false;
            try
            {
                var userDN = Session["USER_SESSION"] as UserLogin;

                obj = _userDA.UnLockUser(userId, (int)userDN.UserID);
                if (obj.Error)
                    AddLog("Khóa Người dùng lỗi: " + obj.Title);
                else
                    AddLog("Khóa Người dùng thành công.");

                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Khóa Người dùng lỗi: " + ex.Message);
                return Json(obj);
            }
        }

    }
}