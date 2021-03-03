using Model.Model;
using Model.ModelExtend;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data.DanhMuc;

namespace WebApp.Controllers.Danh_Muc
{
    public class Don_vi_tinhController : Controller
    {
        ITEMS_LISTEntities db = new ITEMS_LISTEntities();
        Don_vi_tinhDA da = new Don_vi_tinhDA();
        // GET: Don_vi
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DanhSach(int? tID)
        {
            try
            {
                var data = da.DanhSach(tID);
                return Json(new { data = data, Error = false, Title = "Lấy dữ liệu thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { Error = true, Title = "Lấy dữ liệu không thành công: " + ex });
            }
        }
    }
}