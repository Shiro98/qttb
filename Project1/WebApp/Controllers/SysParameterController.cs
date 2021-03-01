using Common;
using Data.Admin;
using log4net;
using Model.Model;
using Model.ModelExtend;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class SysParameterController : BaseController
    {
        ITEMS_SYSTEMEntities db = new ITEMS_SYSTEMEntities();
        SysParameterDA _sysParameterDA = new SysParameterDA();
        BaseController _helperController = new BaseController();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: ST_PARAMETERS
        [HasCredential(ControllerName = "SysParameter")]
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
        public ActionResult GetAll(ModelSearchUser modelSearch)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                int totalItems = 0;
                int pageSize = 0;
                var data = _sysParameterDA.GetAllByPage(modelSearch, ref pageSize);
                if (data != null && data.Count > 0)
                    totalItems = data.FirstOrDefault().TotalRow;
                AddLog("Lấy dữ liệu theo trang bảng Tham số( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") thành công.");
                return Json(new { data = data, totalItems = totalItems, Error = false, Title = "Lấy dữ liệu thành công.", pageSize = pageSize }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu theo trang bảng Tham số( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") lỗi: " + ex.Message);

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
            log.Error("[SysParameter][" + user.UserName + "]: " + content);
        }


        [HttpPost]
        public object GetItemByID(string Id)
        {
            try
            {
                var data = db.ST_PARAMETERS.FirstOrDefault(x => x.PARAM_ID == Id);
                AddLog("Lấy dữ liệu theo ID bảng Tham số( ID: " + Id + ") thành công.");
                return Json(new { Error = false, Title = "Lấy dữ liệu thành công.", data = data });
            }
            catch (Exception ex)
            {
                AddLog("Lấy dữ liệu theo ID bảng Tham số( ID: " + Id + ") lỗi: " + ex.Message);
                return Json(new { Error = true, Title = ex.Message });
            }
        }
        [HttpPost]
        public object Add(ST_PARAMETERS sysParameter)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                obj = _sysParameterDA.Add(sysParameter);
                if (obj.Error)
                    AddLog("Thêm mới dữ liệu Tham số(ParamCode: " + sysParameter.PARAM_CODE + ", ParamValue: " + sysParameter.PARAM_VALUE + ", Desctiption: " + sysParameter.PARAM_DESC + ") lỗi: " + obj.Title);
                else
                    AddLog("Thêm mới dữ liệu Tham số(ParamCode: " + sysParameter.PARAM_CODE + ", ParamValue: " + sysParameter.PARAM_VALUE + ", Desctiption: " + sysParameter.PARAM_DESC + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Thêm mới dữ liệu Tham số(ParamCode: " + sysParameter.PARAM_CODE + ", ParamValue: " + sysParameter.PARAM_VALUE + ", Desctiption: " + sysParameter.PARAM_DESC + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        [HttpPost]
        public object Edit(ST_PARAMETERS sysParameter)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                obj = _sysParameterDA.Edit(sysParameter);
                if (obj.Error)
                    AddLog("Cập nhật dữ liệu Tham số(ParamCode: " + sysParameter.PARAM_CODE + ", ParamValue: " + sysParameter.PARAM_VALUE + ", Desctiption: " + sysParameter.PARAM_DESC + ") lỗi: " + obj.Title);
                else
                    AddLog("Cập nhật dữ liệu Tham số(ParamCode: " + sysParameter.PARAM_CODE + ", ParamValue: " + sysParameter.PARAM_VALUE + ", Desctiption: " + sysParameter.PARAM_DESC + ") thành công.");

                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Cập nhật dữ liệu Tham số(ParamCode: " + sysParameter.PARAM_CODE + ", ParamValue: " + sysParameter.PARAM_VALUE + ", Desctiption: " + sysParameter.PARAM_DESC + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }
        [HttpPost]
        public object Delete(string Id)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                obj = _sysParameterDA.Delete(Id, (int)user.UserID);
                if (obj.Error)
                    AddLog("Xóa dữ liệu Tham số( ID: " + Id + " ) lỗi: " + obj.Title);
                else
                    AddLog("Xóa dữ liệu Tham số( ID: " + Id + " ) thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Xóa dữ liệu Tham số( ID: " + Id + " ) lỗi: " + ex.Message + ".");
                return Json(obj);
            }
        }
    }
}