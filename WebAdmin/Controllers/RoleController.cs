using Common;
using Model.Model;
using Model.ModelExtend;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Data.Admin;
using log4net;
using WebApp.Common;

namespace WebAdmin.Controllers
{
    public class RoleController : BaseController
    {
        Entities db = new Entities();
        RoleDA _roleDA = new RoleDA();
        RolePageDA _rolePageDA = new RolePageDA();
        PageMenuDA _pageMenuDA = new PageMenuDA();
        PageActionDA _pageActionDA = new PageActionDA();
        BaseController _helperController = new BaseController();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: User
        [HasCredential(ControllerName = "Role")]
        public ActionResult Index()
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
        [HttpPost]
        public ActionResult GetAllByPage(ModelSearchUser modelSearch)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                int totalItems = 0;
                int pageSize = 0;
                var data = _roleDA.GetAllByPage(modelSearch, ref pageSize);
                if (data != null && data.Count > 0)
                    totalItems = data.FirstOrDefault().TotalRow;
                AddLog("Lấy dữ liệu theo trang bảng Quyền( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") thành công.");
                return Json(new { data = data, totalItems = totalItems, pageSize = pageSize, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu theo trang bảng Quyền( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") lỗi: " + ex.Message);
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


        private void AddLog(string content)
        {
            var user = Session["USER_SESSION"] as UserLogin;
            log.Error("[ST_ROLES][" + user.UserName + "]: " + content);
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
                var pageMenus = _pageMenuDA.GetAll();
                var pageActions = _pageActionDA.GetAll();
                List<TreeModel> lstTreeModel = new List<TreeModel>();
                ConvertTreePageMenu(lstTreeModel, pageMenus, pageActions, null, new List<ST_ROLE_PAGES>());
                lstTreeModel = lstTreeModel.OrderBy(e => e.name).ToList();

                // lấy danh sách đơn vị
                var units = db.ST_UNITS.Where(x => x.ENABLED_FLAG == "Y").OrderBy(x => x.UNIT_NAME).ToList();

                // lấy danh sách phòng ban
                var appCodes = db.APPS_CONF.OrderBy(x => x.DESCRIPTION).ToList();


                AddLog("Lấy dữ liệu danh mục thành công.");
                return Json(new { TreeDatas = lstTreeModel, Units = units, AppCodes = appCodes, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu danh mục lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        [HttpPost]
        public object GetItemByID(int Id)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var data = db.ST_ROLES.FirstOrDefault(x => x.ROLE_ID == Id);

                var rolePages = new List<ST_ROLE_PAGES>();
                if (Id > 0)
                {
                    rolePages = _rolePageDA.GetAllByRole(Id);
                }
                var pageMenus = _pageMenuDA.GetAll();
                var pageActions = _pageActionDA.GetAll();
                List<TreeModel> lstTreeModel = new List<TreeModel>();
                ConvertTreePageMenu(lstTreeModel, pageMenus, pageActions, null, rolePages);
                lstTreeModel = lstTreeModel.OrderBy(e => e.name).ToList();
                AddLog("Lấy dữ liệu theo ID bảng Quyền(ID: " + Id + ") thành công.");
                return Json(new { TreeDatas = lstTreeModel, Error = false, Title = "Lấy dữ liệu thành công.", data = data });
            }
            catch (Exception ex)
            {
                AddLog("Lấy dữ liệu theo ID bảng Quyền(ID: " + Id + ") lỗi: " + ex.Message);
                return Json(new { Error = true, Title = ex.Message });
            }
        }
        [HttpPost]
        [QLTBAuthorize(role = "btnCreate")]
        public object Add(ST_ROLES role, List<TreeModel> pageMenus)
        {
            ObjectMessage obj = new ObjectMessage();
            obj.Error = false;
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                role.LAST_UPDATED_BY = (int)user.UserID;
                role.ENABLED_FLAG = "Y";
                //role.APP_CODE = user.Appcode;
                role.LAST_UPDATE_DATE = DateTime.Now;
                obj = _roleDA.Add(role, pageMenus);
                if (obj.Error)
                    AddLog("Thêm mới dữ liệu bảng Quyền(ROLE_TYPE: " + role.ROLE_TYPE + ", ROLE_DESC: " + role.ROLE_DESC + ") lỗi: " + obj.Title);
                else
                    AddLog("Thêm mới dữ liệu bảng Quyền(ROLE_TYPE: " + role.ROLE_TYPE + ", ROLE_DESC: " + role.ROLE_DESC + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Thêm mới dữ liệu bảng Quyền(ROLE_TYPE: " + role.ROLE_TYPE + ", ROLE_DESC: " + role.ROLE_DESC + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        /// <summary>
        /// Tạo tree chức năng
        /// </summary>
        /// <param name="lstTreeModel"></param>
        /// <param name="pageMenu"></param>
        /// <param name="pageFunction"></param>
        /// <param name="parentId"></param>
        public void ConvertTreePageMenu(List<TreeModel> lstTreeModel, List<ST_PAGES_MENU> pageMenu, List<ST_PAGE_FUNCTIONS> pageFunction, int? parentId, List<ST_ROLE_PAGES> rolePages)
        {
            if (pageMenu != null && pageMenu.Count > 0)
            {
                var lstPageMenu = new List<ST_PAGES_MENU>();
                if (parentId > 0)
                {
                    lstPageMenu = pageMenu.Where(x => x.PARENT_ID == (int)parentId && x.ENABLED_FLAG == "Y").ToList();
                }
                else
                    lstPageMenu = pageMenu.Where(x => x.PARENT_ID == null || x.PARENT_ID == 0).ToList();
                if (lstPageMenu.Count > 0)
                {
                    foreach (var item in lstPageMenu)
                    {
                        var tree = new TreeModel();
                        var rolePage = rolePages.FirstOrDefault(x => x.PAGE_ID == item.PAGE_ID && x.ENABLED_FLAG == "Y");

                        tree = new TreeModel
                        {
                            id = item.PAGE_ID.ToString() + "_",
                            name = item.PAGE_NAME,
                            pace_id = item.PAGE_ID.ToString(),
                            pId = (item.PARENT_ID == null || item.PARENT_ID == 0) ? "0" : item.PARENT_ID.ToString() + "_",
                            @checked = (rolePage != null && rolePage.PAGE_ID > 0) ? true : false,
                            open = (rolePage != null && rolePage.PAGE_ID > 0) ? true : false
                        };
                        lstTreeModel.Add(tree);

                        var pf = pageFunction.Where(x => x.PAGE_ID == item.PAGE_ID).ToList();
                        if (pf.Count > 0)
                        {
                            var listAction = new List<string>();
                            if (rolePage != null && rolePage.PAGE_ID > 0 && !string.IsNullOrEmpty(rolePage.CONTROL_STRING))
                            {
                                listAction = rolePage.CONTROL_STRING.Split('|').ToList();
                            }
                            foreach (var itemF in pf)
                            {
                                tree = new TreeModel
                                {
                                    @checked = listAction.Contains(itemF.CONTROL_NAME),
                                    id = "_" + itemF.PAGE_ID.ToString() + "/" + itemF.PAGE_ID,
                                    name = itemF.CONTROL_DESC ?? itemF.CONTROL_NAME,
                                    name_control = itemF.CONTROL_NAME,
                                    pace_id = itemF.PAGE_ID.ToString(),
                                    pId = item.PAGE_ID.ToString() + "_"
                                };
                                lstTreeModel.Add(tree);
                            }
                        }
                        ConvertTreePageMenu(lstTreeModel, pageMenu, pageFunction, item.PAGE_ID, rolePages);
                    }
                }
            }
        }

        [HttpPost]
        [QLTBAuthorize(role = "btnUpdate")]
        public object Edit(ST_ROLES role, List<TreeModel> pageMenus)
        {
            ObjectMessage obj = new ObjectMessage();
            obj.Error = false;
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                role.LAST_UPDATED_BY = (int)user.UserID;
                role.LAST_UPDATE_DATE = DateTime.Now;
                obj = _roleDA.Edit(role, pageMenus);
                if (obj.Error)
                    AddLog("Cập nhật dữ liệu bảng Quyền(ROLE_TYPE: " + role.ROLE_TYPE + ", ROLE_DESC: " + role.ROLE_DESC + ") lỗi: " + obj.Title);
                else
                    AddLog("Cập nhật dữ liệu bảng Quyền(ROLE_TYPE: " + role.ROLE_TYPE + ", ROLE_DESC: " + role.ROLE_DESC + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Cập nhật dữ liệu bảng Quyền(ROLE_TYPE: " + role.ROLE_TYPE + ", ROLE_DESC: " + role.ROLE_DESC + ") lỗi: " + ex.Message);
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
                obj = _roleDA.Delete(Id, (int)user.UserID);
                if (obj.Error)
                    AddLog("Xóa dữ liệu bảng Quyền(ID: " + Id + ") lỗi: " + obj.Title);
                else
                    AddLog("Xóa dữ liệu bảng Quyền(ID: " + Id + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Xóa dữ liệu bảng Quyền(ID: " + Id + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }
    }
}