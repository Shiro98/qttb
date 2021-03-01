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
    public class FlexValueController : BaseController
    {
        Entities db = new Entities();
        ITEMS_LISTEntities dbItemList = new ITEMS_LISTEntities();
        FlexValueSetDA _flexValueSetDA = new FlexValueSetDA();
        FlexValueDA _flexValueDA = new FlexValueDA();
        FlexValueCategoryDA _flexValueCateDA = new FlexValueCategoryDA();
        BaseController _helperController = new BaseController();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: User
        [HasCredential(ControllerName = "FlexValue")]
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
        public ActionResult GetAllByPage(ModelSearchFelexValue modelSearch)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                int totalItems = 0;
                var data = _flexValueDA.GetAllByPage(modelSearch);
                if (data != null && data.Count > 0)
                    totalItems = data.FirstOrDefault().TotalRow;
                AddLog("Lấy dữ liệu theo trang bảng Danh mục( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") thành công.");
                return Json(new { data = data, totalItems = totalItems, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu theo trang bảng Danh mục( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") lỗi: " + ex.Message);
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

                var flexValueSets = _flexValueSetDA.GetSelect(string.Empty, string.Empty, null, "V");

                var flexValueColumns = _flexValueDA.GetColumn(flexValueSets.FirstOrDefault().FLEX_VALUE_SET_ID);

                AddLog("Lấy danh sách các botom được thực hiện trên from Danh mục thành công.");
                return Json(new { Buttoms = bottoms, FlexValueSets = flexValueSets, FlexValueColumns = flexValueColumns, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy danh sách các botom được thực hiện trên from Danh mục lỗi: " + ex.Message);
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
                var flexValueSets = _flexValueSetDA.GetSelect(string.Empty, string.Empty, null, "V");

                var flexValueColumns = _flexValueDA.GetColumn(flexValueSets.FirstOrDefault().FLEX_VALUE_SET_ID);
                var flexValueParents = new List<LT_FLEX_VALUES>();
                flexValueParents.AddRange(_flexValueDA.GetAll());
                var flexValueCates = _flexValueCateDA.GetAll();

                AddLog("Lấy dữ liệu danh mục thành công.");
                return Json(new { 
                    FlexValueSets = flexValueSets, 
                    FlexValueColumns = flexValueColumns, 
                    FlexValueParents = flexValueParents,
                    FlexValueCates = flexValueCates,
                    Error = false, 
                    Title = "Lấy dữ liệu thành công." }); ;
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
                var data = _flexValueDA.GetById(Id);

                var flexValueSets = _flexValueSetDA.GetSelect(string.Empty, string.Empty, null, "V");

                var flexValueColumns = _flexValueDA.GetColumn(data.FLEX_VALUE_SET_ID);
                var flexValueParents = new List<LT_FLEX_VALUES>();
                flexValueParents.AddRange(_flexValueDA.GetAll());
                var flexValueCates = _flexValueCateDA.GetAll();

                AddLog("Lấy dữ liệu theo ID bảng Danh mục(ID: " + Id + ") thành công.");
                return Json(new { FlexValueSets = flexValueSets, FlexValueColumns = flexValueColumns, Error = false, Title = "Lấy dữ liệu thành công.", data = data });
            }
            catch (Exception ex)
            {
                AddLog("Lấy dữ liệu theo ID bảng Danh mục(ID: " + Id + ") lỗi: " + ex.Message);
                return Json(new { Error = true, Title = ex.Message });
            }
        }
        [HttpPost]
        [QLTBAuthorize(role = "btnCreate")]
        public object Add(LT_FLEX_VALUES model)
        {
            ObjectMessage obj = new ObjectMessage();
            obj.Error = false;
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                model.CREATED_BY = (int)user.UserID;
                model.ENABLED_FLAG = "Y";
                model.CREATION_DATE = DateTime.Now;
                obj = _flexValueDA.Add(model);
                if (obj.Error)
                    AddLog("Thêm mới dữ liệu bảng Danh mục(FLEX_VALUE: " + model.FLEX_VALUE + ", FLEX_VALUE_SET_ID: " + model.FLEX_VALUE_SET_ID + ") lỗi: " + obj.Title);
                else
                    AddLog("Thêm mới dữ liệu bảng Danh mục(FLEX_VALUE: " + model.FLEX_VALUE + ", FLEX_VALUE_SET_ID: " + model.FLEX_VALUE_SET_ID + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Thêm mới dữ liệu bảng Danh mục(FLEX_VALUE: " + model.FLEX_VALUE + ", FLEX_VALUE_SET_ID: " + model.FLEX_VALUE_SET_ID + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }

       
        [HttpPost]
        [QLTBAuthorize(role = "btnUpdate")]
        public object Edit(LT_FLEX_VALUES model)
        {
            ObjectMessage obj = new ObjectMessage();
            obj.Error = false;
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                model.LAST_UPDATED_BY = (int)user.UserID;
                model.LAST_UPDATE_DATE = DateTime.Now;
                obj = _flexValueDA.Edit(model);
                if (obj.Error)
                    AddLog("Cập nhật dữ liệu bảng Danh mục(FLEX_VALUE: " + model.FLEX_VALUE + ", FLEX_VALUE_SET_ID: " + model.FLEX_VALUE_SET_ID + ") lỗi: " + obj.Title);
                else
                    AddLog("Cập nhật dữ liệu bảng Danh mục(FLEX_VALUE: " + model.FLEX_VALUE + ", FLEX_VALUE_SET_ID: " + model.FLEX_VALUE_SET_ID + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Cập nhật dữ liệu bảng Danh mục(FLEX_VALUE: " + model.FLEX_VALUE + ", FLEX_VALUE_SET_ID: " + model.FLEX_VALUE_SET_ID + ") lỗi: " + ex.Message);
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
                obj = _flexValueDA.Delete(Id, (int)user.UserID);
                if (obj.Error)
                    AddLog("Xóa dữ liệu bảng Danh mục(ID: " + Id + ") lỗi: " + obj.Title);
                else
                    AddLog("Xóa dữ liệu bảng Danh mục(ID: " + Id + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Xóa dữ liệu bảng Danh mục(ID: " + Id + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }
    }
}