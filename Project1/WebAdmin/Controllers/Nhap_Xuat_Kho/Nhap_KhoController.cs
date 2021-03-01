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
using WebAdmin.Models;

namespace WebAdmin.Controllers.Nhap_Xuat_Kho
{
    
    public class Nhap_KhoController : Controller
    {
        Entities dbHT = new Entities();
        THIET_BI_VTEntities db = new THIET_BI_VTEntities();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Nhap_Kho
        public ActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public ActionResult dsNhapKho()
        {

            try
            {
                var data = new List<NhapXuatKho>();
                using (var context = new THIET_BI_VTEntities())
                {
                    data = context.Database.SqlQuery<NhapXuatKho>("exec [KH_NHAP_XUAT_KHO_GET]").ToList();
                }

                return Json(new { data = data, Error = false, Title = "Lấy dữ liệu thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { Error = true, Title = ex.Message });
            }
            
        }

        [HttpPost]
        public ActionResult GetDanhMuc()
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                // lấy danh sách hàng
                var hang = db.DM_THIET_BI_VT.Where(x => x.HIEU_LUC == "Y").OrderBy(x => x.TEN).ToList();

                dbHT.Configuration.ProxyCreationEnabled = false;
                // lấy danh user
                var users = dbHT.ST_USERS.Select(x => new { ID = x.USER_ID, NAME = x.FULL_NAME}).OrderBy(x => x.NAME).ToList();

                // lấy danh sách đơn vị
                var units = dbHT.ST_UNITS.Where(x => x.ENABLED_FLAG == "Y").OrderBy(x => x.UNIT_NAME).ToList();

                return Json(new { User = users, Units = units, dsHang = hang, Error = false, Title = "Lấy dữ liệu thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { Error = true, Title = ex.Message });
            }
        }


        [HttpPost]
        public ActionResult themMoi(NhapXuatKho nhap, List<NhapXuatKhoChiTiet> hang)
        {
           
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                ObjectMessage obj = new ObjectMessage();
                var check = db.KH_NHAP_XUAT_KHO.Where(x => x.SO_KE_HOACH.ToUpper() == nhap.SO_KE_HOACH.ToUpper()).ToList();
                if (check == null || check.Count == 0)
                {
                    using (THIET_BI_VTEntities context = new THIET_BI_VTEntities())
                    {
                        using (var dbContextTransaction = context.Database.BeginTransaction())
                        {
                            DataTable dt_hang = new DataTable();
                            dt_hang.Columns.Add("SO_TT");
                            dt_hang.Columns.Add("ID_KHO");
                            dt_hang.Columns.Add("ID_THIET_BI");
                            dt_hang.Columns.Add("HANG_HOA");
                            dt_hang.Columns.Add("SO_LUONG");
                            dt_hang.Columns.Add("ID_CHAT_LUONG");
                            dt_hang.Columns.Add("GHI_CHU");
                            dt_hang.Columns.Add("NGUON_GOC");
                            dt_hang.Columns.Add("NHA_SAN_XUAT");
                            dt_hang.Columns.Add("NAM_SAN_XUAT");
                            DataRow workRow;
                            foreach (NhapXuatKhoChiTiet item in hang)
                            {
                                workRow = dt_hang.NewRow();
                                workRow["SO_TT"] = item.SO_TT;
                                workRow["ID_KHO"] = item.ID_KHO;
                                workRow["ID_THIET_BI"] = item.ID_THIET_BI;
                                workRow["HANG_HOA"] = item.HANG_HOA;
                                workRow["SO_LUONG"] = item.SO_LUONG;
                                workRow["ID_CHAT_LUONG"] = item.ID_CHAT_LUONG;
                                workRow["GHI_CHU"] = item.GHI_CHU;
                                workRow["NGUON_GOC"] = item.NGUON_GOC;
                                workRow["NHA_SAN_XUAT"] = item.NHA_SAN_XUAT;
                                workRow["NAM_SAN_XUAT"] = item.NAM_SAN_XUAT;
                                dt_hang.Rows.Add(workRow);
                            }

                            var paramList = new List<SqlParameter>
                            {
                                new SqlParameter(@"SO_KE_HOACH", nhap.SO_KE_HOACH),
                                new SqlParameter(@"NGUON_TIEP_NHAN_ID", nhap.NGUON_TIEP_NHAN_ID),
                                new SqlParameter(@"NGUOI_NHAP_NHAN_ID", nhap.NGUOI_NHAP_NHAN_ID),
                                new SqlParameter(@"NGUOI_NHAP_NHAN", nhap.NGUOI_NHAP_NHAN),
                                new SqlParameter(@"DON_VI_NHAP_NHAN_ID", nhap.DON_VI_NHAP_NHAN_ID),
                                new SqlParameter(@"DON_VI_NHAP_NHAN", nhap.DON_VI_NHAP_NHAN),
                                new SqlParameter(@"NGAY_NHAP_NHAN", nhap.NGAY_NHAP_NHAN),
                                new SqlParameter(@"GHI_CHU", !string.IsNullOrEmpty(nhap.GHI_CHU)? (object)nhap.GHI_CHU : DBNull.Value),
                                new SqlParameter(@"LOAI", nhap.LOAI),
                                new SqlParameter(@"NGUOI_TAO", user.UserID),
                                new SqlParameter(@"lIST_NHAP_XUAT_CT", dt_hang)
                            };
                            ((SqlParameter)paramList[paramList.Count - 1]).SqlDbType = SqlDbType.Structured;
                            ((SqlParameter)paramList[paramList.Count - 1]).TypeName = "dbo.lIST_KH_NHAP_XUAT_KHO_CT";
                            var resultPro = context.Database.ExecuteSqlCommand("exec [KH_NHAP_XUAT_KHO_INSERT] @SO_KE_HOACH,@NGUON_TIEP_NHAN_ID,@NGUOI_NHAP_NHAN_ID," +
                                "@NGUOI_NHAP_NHAN,@DON_VI_NHAP_NHAN_ID,@DON_VI_NHAP_NHAN,@NGAY_NHAP_NHAN,@GHI_CHU,@LOAI,@NGUOI_TAO,@lIST_NHAP_XUAT_CT", paramList.ToArray());
                            
                            if (resultPro > 0)
                            {
                                obj.Error = false;
                                obj.Title = "Thêm mới thành công!";
                                dbContextTransaction.Commit();
                            }
                            else
                            {
                                obj.Error = true;
                                obj.Title = "Thêm mới lỗi!";
                                dbContextTransaction.Rollback();
                            }
                        }
                    }
                }
                else
                {
                    obj.Error = true;
                    obj.Title = "Số kế hoạch đã tồn tại";
                }
                return Json(new { Error = obj.Error, Title = obj.Title });
               
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return Json(new { Error = true, Title = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult laydlSua(int id)
        {

            try
            {
                //DataSet data = new DataSet();
                //using (var context = new THIET_BI_VTEntities())
                //{
                //    var paramList = new List<SqlParameter>
                //    {
                //        new SqlParameter(@"ID_NHAP_XUAT", id)
                //    };
                //   var result = context.Database.SqlQuery<NhapXuatKho>("exec [KH_NHAP_XUAT_KHO_GET_BY_ID]", paramList.ToArray());

                //}
                db.Configuration.ProxyCreationEnabled = false;
                // lấy danh sách nhap kho
                var nhap = db.KH_NHAP_XUAT_KHO.Where(x => x.ID_NHAP_XUAT == id).ToList();

                var chitiet = db.KH_NHAP_XUAT_KHO_CT.Where(x => x.ID_NHAP_XUAT == id).ToList();

                var result = Json(new { Nhap = nhap, Nhap_ct = chitiet, Error = false, Title = "Lấy dữ liệu thành công." }, JsonRequestBehavior.AllowGet); 
                result.MaxJsonLength = int.MaxValue;
                return result;
            }
            catch (Exception ex)
            {
                return Json(new { Error = true, Title = ex.Message });
            }

        }


    }
}