using Model.ModelExtend;
using Model.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace WebApp.Controllers.Danh_Muc
{
    public class Don_vi_tinhController : Controller
    {
        ITEMS_LISTEntities db = new ITEMS_LISTEntities();
        // GET: Don_vi
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DanhSach()
        {
            try
            {
                var data = new List<DM_DON_VI_TINH_Model>();
                using (var ds = new ITEMS_LISTEntities())
                {
                    data = ds.Database.SqlQuery<DM_DON_VI_TINH_Model>("Select * from DM_DON_VI_TINH").ToList();
                }
               // var ID = db.DM_DON_VI_TINH.Select(x => new { ID = x.ID}).OrderBy(x => x.ID).ToList();
                return Json(new { Data = data, Error = false, Title = "Lấy dữ liệu thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { Error = true, Title = "Lấy dữ liệu không thành công: " + ex });
            }
        }
    }
}