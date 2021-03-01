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

namespace WebApp.Controllers
{
    public class FlexValueCategoryController : BaseController
    {
        ITEMS_LISTEntities db = new ITEMS_LISTEntities();
        FlexValueSetDA _flexValueSetDA = new FlexValueSetDA();
        FlexValueCategoryDA _flexValueCategoryDA = new FlexValueCategoryDA();
        BaseController _helperController = new BaseController();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: User
        [HasCredential(ControllerName = "FlexValueCategory")]
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
        public ActionResult GetAllByPage(ModelSearchFlexValueCategory modelSearch)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                if (modelSearch.pageSize == 0)
                    modelSearch.pageSize = 10;
                int totalItems = 0;
                int pageSize = 0;
                var data = _flexValueCategoryDA.GetAllByPage(modelSearch);
                if (data != null && data.Count > 0)
                    totalItems = data.FirstOrDefault().TotalRow;
                AddLog("Lấy dữ liệu theo trang bảng Danh mục Nhóm/Loại( FLEX_VALUE_SET_ID: " + modelSearch.FLEX_VALUE_SET_ID + ", page: " + modelSearch.currentPage + ") thành công.");
                return Json(new { data = data, totalItems = totalItems, pageSize = pageSize, Error = false, Title = "Lấy dữ liệu thành công." }, JsonRequestBehavior.AllowGet); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu theo trang bảng Danh mục Nhóm/Loại( FLEX_VALUE_SET_ID: " + modelSearch.FLEX_VALUE_SET_ID + ", page: " + modelSearch.currentPage + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        [HttpPost]
        public ActionResult GetColumnByFlexValueSet(int flexValueSetId)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                var flexValueColumns = _flexValueCategoryDA.GetColumn(flexValueSetId);

                AddLog("Lấy danh sách các cột theo flex_value_set.");
                return Json(new { FlexValueColumns = flexValueColumns, Error = false, Title = "Lấy dữ liệu thành công." }, JsonRequestBehavior.AllowGet); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy danh sách các cột theo flex_value_set lỗi: " + ex.Message);
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
                var flexValueSets = _flexValueSetDA.GetSelect(string.Empty, string.Empty, null, "C");

                var flexValueColumns = _flexValueCategoryDA.GetColumn(flexValueSets.FirstOrDefault().FLEX_VALUE_SET_ID);

                var menu = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");
                var bottoms = _helperController.GetBottomRoleByController(controllerName, menu);
                AddLog("Lấy danh sách các botom được thực hiện trên from Người dùng thành công.");
                return Json(new { FlexValueSets = flexValueSets, FlexValueColumns = flexValueColumns, Buttoms = bottoms, Error = false, Title = "Lấy dữ liệu thành công." }, JsonRequestBehavior.AllowGet); ;
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
        public ActionResult GetDanhMuc(string flexValueSetId)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                var flexValueColumns = _flexValueCategoryDA.GetColumn(Convert.ToInt32(flexValueSetId));

                AddLog("Lấy dữ liệu danh mục thành công.");
                return Json(new { 
                    FlexValueColumns = flexValueColumns, 
                    Error = false, 
                    Title = "Lấy dữ liệu thành công." }, JsonRequestBehavior.AllowGet); ;
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
                var data = _flexValueCategoryDA.GetById(Id);

                var flexValueColumns = _flexValueCategoryDA.GetColumn(data.FLEX_VALUE_SET_ID);

                AddLog("Lấy dữ liệu theo ID bảng Danh mục Nhóm/Loại(ID: " + Id + ") thành công.");
                return Json(new
                {
                    FlexValueColumns = flexValueColumns,
                    Error = false, Title = "Lấy dữ liệu thành công.", data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                AddLog("Lấy dữ liệu theo ID bảng Danh mục Nhóm/Loại(ID: " + Id + ") lỗi: " + ex.Message);
                return Json(new { Error = true, Title = ex.Message });
            }
        }
        [HttpPost]
        [QLTBAuthorize(role = "btnCreate")]
        public object Add(LT_FLEX_VALUE_CATEGORIES model)
        {
            ObjectMessage obj = new ObjectMessage();
            obj.Error = false;
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                model.CREATED_BY = (int)user.UserID;
                //model.ENABLED_FLAG = "Y";
                model.CREATION_DATE = DateTime.Now;
                obj = _flexValueCategoryDA.Add(model);
                if (obj.Error)
                    AddLog("Thêm mới dữ liệu bảng Danh mục Nhóm/Loại(FLEX_VALUE_CATEGORY: " + model.FLEX_VALUE_CATEGORY + ", FLEX_VALUE_SET_ID: " + model.FLEX_VALUE_SET_ID + ") lỗi: " + obj.Title);
                else
                    AddLog("Thêm mới dữ liệu bảng Danh mục Nhóm/Loại(FLEX_VALUE_CATEGORY: " + model.FLEX_VALUE_CATEGORY + ", FLEX_VALUE_SET_ID: " + model.FLEX_VALUE_SET_ID + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Thêm mới dữ liệu bảng Danh mục Nhóm/Loại(FLEX_VALUE_CATEGORY: " + model.FLEX_VALUE_CATEGORY + ", FLEX_VALUE_SET_ID: " + model.FLEX_VALUE_SET_ID + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        
        [HttpPost]
        [QLTBAuthorize(role = "btnUpdate")]
        public object Edit(LT_FLEX_VALUE_CATEGORIES model)
        {
            ObjectMessage obj = new ObjectMessage();
            obj.Error = false;
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                model.LAST_UPDATED_BY = (int)user.UserID;
                model.LAST_UPDATE_DATE = DateTime.Now;
                obj = _flexValueCategoryDA.Edit(model);
                if (obj.Error)
                    AddLog("Cập nhật dữ liệu bảng Danh mục Nhóm/Loại(FLEX_VALUE_CATEGORY: " + model.FLEX_VALUE_CATEGORY + ", FLEX_VALUE_SET_ID: " + model.FLEX_VALUE_SET_ID + ") lỗi: " + obj.Title);
                else
                    AddLog("Cập nhật dữ liệu bảng Danh mục Nhóm/Loại(FLEX_VALUE_CATEGORY: " + model.FLEX_VALUE_CATEGORY + ", FLEX_VALUE_SET_ID: " + model.FLEX_VALUE_SET_ID + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Cập nhật dữ liệu bảng Danh mục Nhóm/Loại(FLEX_VALUE_CATEGORY: " + model.FLEX_VALUE_CATEGORY + ", FLEX_VALUE_SET_ID: " + model.FLEX_VALUE_SET_ID + ") lỗi: " + ex.Message);
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
                obj = _flexValueCategoryDA.Delete(Id, (int)user.UserID);
                if (obj.Error)
                    AddLog("Xóa dữ liệu bảng Danh mục Nhóm/Loại(ID: " + Id + ") lỗi: " + obj.Title);
                else
                    AddLog("Xóa dữ liệu bảng Danh mục Nhóm/Loại(ID: " + Id + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Xóa dữ liệu bảng Danh mục Nhóm/Loại(ID: " + Id + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }
    }
}