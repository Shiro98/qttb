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
using WebApp.Models;

namespace WebApp.Controllers
{
    
    public class Nhap_KhoController : Controller
    {
        Entities dbHT = new Entities();
        THIET_BI_VTEntities1 db = new THIET_BI_VTEntities1();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Nhap_Kho
        public ActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public ActionResult dsNhapKho(NhapXuatKho nhap)
        {

            try
            {
                nhap.NGUON_TIEP_NHAN_ID = nhap.NGUON_TIEP_NHAN_ID == null ? 0 : nhap.NGUON_TIEP_NHAN_ID;
                nhap.NGUOI_NHAP_NHAN_ID = nhap.NGUOI_NHAP_NHAN_ID == null ? 0 : nhap.NGUOI_NHAP_NHAN_ID;
                nhap.DON_VI_NHAP_NHAN_ID = nhap.DON_VI_NHAP_NHAN_ID == null ? 0 : nhap.DON_VI_NHAP_NHAN_ID;
                nhap.OrderByClause = nhap.OrderByClause == null ? "ID_NHAP_XUAT" : nhap.OrderByClause;
                var data = new List<NhapXuatKho>();
                int pageSize = 0;
                using (var context = new THIET_BI_VTEntities1())
                {
                    var paramList = new List<SqlParameter>
                            {
                                new SqlParameter(@"SO_KE_HOACH", !string.IsNullOrEmpty(nhap.SO_KE_HOACH)? (object)nhap.SO_KE_HOACH : DBNull.Value),
                                new SqlParameter(@"NGUON_TIEP_NHAN_ID", nhap.NGUON_TIEP_NHAN_ID),
                                new SqlParameter(@"NGUOI_NHAP_NHAN_ID", nhap.NGUOI_NHAP_NHAN_ID),
                                new SqlParameter(@"DON_VI_NHAP_NHAN_ID", nhap.DON_VI_NHAP_NHAN_ID),
                                new SqlParameter(@"TU_NGAY", !string.IsNullOrEmpty(nhap.TU_NGAY)? (object)nhap.TU_NGAY : DBNull.Value),
                                new SqlParameter(@"DEN_NGAY", !string.IsNullOrEmpty(nhap.DEN_NGAY)? (object)nhap.DEN_NGAY : DBNull.Value),
                                new SqlParameter(@"TRANG_THAI", !string.IsNullOrEmpty(nhap.TRANG_THAI)? (object)nhap.TRANG_THAI : DBNull.Value),
                                new SqlParameter(@"page", nhap.currentPage),
                                new SqlParameter(@"OrderByClause", !string.IsNullOrEmpty(nhap.OrderByClause)? (object)nhap.OrderByClause : DBNull.Value),
                                new SqlParameter(@"pageSize", SqlDbType.Int,8) { Direction = ParameterDirection.Output }
                            };
                    data = context.Database.SqlQuery<NhapXuatKho>("exec [KH_NHAP_XUAT_KHO_GET] @SO_KE_HOACH,@NGUON_TIEP_NHAN_ID,@NGUOI_NHAP_NHAN_ID," +
                                    "@DON_VI_NHAP_NHAN_ID,@TU_NGAY,@DEN_NGAY,@TRANG_THAI,@page,@OrderByClause,@pageSize OUT", paramList.ToArray()).ToList();

                      pageSize = Convert.ToInt32(paramList[paramList.Count - 1].Value.ToString());
                }
                
                return Json(new { data = data, pageSize = pageSize, Error = false, Title = "Lấy dữ liệu thành công." });
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
                if (nhap.ID_NHAP_XUAT == 0)
                {
                        using (THIET_BI_VTEntities1 context = new THIET_BI_VTEntities1())
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
                                int stt = 1;
                                foreach (NhapXuatKhoChiTiet item in hang)
                                {
                                    workRow = dt_hang.NewRow();
                                    workRow["SO_TT"] = stt;
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
                                    stt++;
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
                                new SqlParameter(@"lIST_NHAP_XUAT_CT", dt_hang),
                                new SqlParameter(@"CODE_OUT", SqlDbType.VarChar,10) { Direction = ParameterDirection.Output }
                            };
                                ((SqlParameter)paramList[paramList.Count - 2]).SqlDbType = SqlDbType.Structured;
                                ((SqlParameter)paramList[paramList.Count - 2]).TypeName = "dbo.lIST_KH_NHAP_XUAT_KHO_CT";
                                var resultPro = context.Database.ExecuteSqlCommand("exec [KH_NHAP_XUAT_KHO_INSERT] @SO_KE_HOACH,@NGUON_TIEP_NHAN_ID,@NGUOI_NHAP_NHAN_ID," +
                                    "@NGUOI_NHAP_NHAN,@DON_VI_NHAP_NHAN_ID,@DON_VI_NHAP_NHAN,@NGAY_NHAP_NHAN,@GHI_CHU,@LOAI,@NGUOI_TAO,@lIST_NHAP_XUAT_CT,@CODE_OUT OUT", paramList.ToArray());
                                string outCode = paramList[paramList.Count - 1].Value.ToString();
                                if (outCode == "OK")
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
                        using (THIET_BI_VTEntities1 context = new THIET_BI_VTEntities1())
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
                                int stt = 1;
                                foreach (NhapXuatKhoChiTiet item in hang)
                                {
                                    workRow = dt_hang.NewRow();
                                    workRow["SO_TT"] = stt;
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
                                    stt++;
                                }

                                var paramList = new List<SqlParameter>
                            {
                                new SqlParameter(@"ID_NHAP_XUAT", nhap.ID_NHAP_XUAT),
                                new SqlParameter(@"SO_KE_HOACH", nhap.SO_KE_HOACH),
                                new SqlParameter(@"NGUON_TIEP_NHAN_ID", nhap.NGUON_TIEP_NHAN_ID),
                                new SqlParameter(@"NGUOI_NHAP_NHAN_ID", nhap.NGUOI_NHAP_NHAN_ID),
                                new SqlParameter(@"NGUOI_NHAP_NHAN", nhap.NGUOI_NHAP_NHAN),
                                new SqlParameter(@"DON_VI_NHAP_NHAN_ID", nhap.DON_VI_NHAP_NHAN_ID),
                                new SqlParameter(@"DON_VI_NHAP_NHAN", nhap.DON_VI_NHAP_NHAN),
                                new SqlParameter(@"NGAY_NHAP_NHAN", nhap.NGAY_NHAP_NHAN),
                                new SqlParameter(@"GHI_CHU", !string.IsNullOrEmpty(nhap.GHI_CHU)? (object)nhap.GHI_CHU : DBNull.Value),
                                new SqlParameter(@"LOAI", nhap.LOAI),
                                new SqlParameter(@"NGUOI_SUA", user.UserID),
                                new SqlParameter(@"lIST_NHAP_XUAT_CT", dt_hang),
                                new SqlParameter(@"CODE_OUT", SqlDbType.VarChar,10) { Direction = ParameterDirection.Output }
                            };
                                ((SqlParameter)paramList[paramList.Count - 2]).SqlDbType = SqlDbType.Structured;
                                ((SqlParameter)paramList[paramList.Count - 2]).TypeName = "dbo.lIST_KH_NHAP_XUAT_KHO_CT";
                                var resultPro = context.Database.ExecuteSqlCommand("exec [KH_NHAP_XUAT_KHO_UPDATE] @ID_NHAP_XUAT,@SO_KE_HOACH,@NGUON_TIEP_NHAN_ID,@NGUOI_NHAP_NHAN_ID," +
                                    "@NGUOI_NHAP_NHAN,@DON_VI_NHAP_NHAN_ID,@DON_VI_NHAP_NHAN,@NGAY_NHAP_NHAN,@GHI_CHU,@LOAI,@NGUOI_SUA,@lIST_NHAP_XUAT_CT,@CODE_OUT OUT", paramList.ToArray());
                                string outCode = paramList[paramList.Count - 1].Value.ToString();
                                if (outCode == "OK")
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
              
                return Json(new { Error = obj.Error, Title = obj.Title });
               
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return Json(new { Error = true, Title = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult laydlSua(Int64 id)
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
                var nhap = db.KH_NHAP_XUAT_KHO.Where(x => x.ID_NHAP_XUAT == id).Select(x => new {
                    ID_NHAP_XUAT = x.ID_NHAP_XUAT,
                    SO_KE_HOACH = x.SO_KE_HOACH,
                    NGUON_TIEP_NHAN_ID = x.NGUON_TIEP_NHAN_ID,
                    NGUOI_NHAP_NHAN_ID = x.NGUOI_NHAP_NHAN_ID,
                    NGUOI_NHAP_NHAN = x.NGUOI_NHAP_NHAN,
                    DON_VI_NHAP_NHAN_ID = x.DON_VI_NHAP_NHAN_ID,
                    DON_VI_NHAP_NHAN = x.DON_VI_NHAP_NHAN,
                    NGAY_NHAP_NHAN = x.NGAY_NHAP_NHAN,
                    CAN_CU_ID = x.CAN_CU_ID,
                    NGUOI_DUYET_ID = x.NGUOI_DUYET_ID,
                    NGUOI_DUYET = x.NGUOI_DUYET,
                    GHI_CHU = x.GHI_CHU,
                    LOAI = x.LOAI,
                    NGUOI_TAO = x.NGUOI_TAO,
                    NGAY_TAO = x.NGAY_TAO,
                    NGUOI_SUA = x.NGUOI_SUA,
                    NGAY_SUA = x.NGAY_SUA,
                    TRANG_THAI = x.TRANG_THAI
                }).ToList();

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


        [HttpPost]
        public ActionResult xoa(Int64 id)
        {

            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                ObjectMessage obj = new ObjectMessage();
              
                  
                        using (THIET_BI_VTEntities1 context = new THIET_BI_VTEntities1())
                        {
                            using (var dbContextTransaction = context.Database.BeginTransaction())
                            {
                                var paramList = new List<SqlParameter>
                            {
                                new SqlParameter(@"ID_NHAP_XUAT", id),
                                 new SqlParameter(@"CODE_OUT", SqlDbType.VarChar,10) { Direction = ParameterDirection.Output }
                            };
                               
                                var resultPro = context.Database.ExecuteSqlCommand("exec [KH_NHAP_XUAT_KHO_DELETE] @ID_NHAP_XUAT,@CODE_OUT OUT", paramList.ToArray());

                                if (resultPro > 0)
                                {
                                    obj.Error = false;
                                    obj.Title = "Xóa bản ghi thành công!";
                                    dbContextTransaction.Commit();
                                }
                                else
                                {
                                    obj.Error = true;
                                    obj.Title = "Xóa bản ghi lỗi!";
                                    dbContextTransaction.Rollback();
                                }
                            }
                        }
                  
                return Json(new { Error = obj.Error, Title = obj.Title });

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return Json(new { Error = true, Title = ex.Message });
            }
        }


    }
}