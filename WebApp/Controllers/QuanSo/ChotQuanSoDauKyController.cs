using Model.Model;
//using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;
using Data.Home;
using Model.ModelExtend;

namespace WebApp.Controllers.QuanSo
{
    public class ChotQuanSoDauKyController : Controller
    {
        QS_QUAN_TRANGEntities db = new QS_QUAN_TRANGEntities();
        QuanSoDA quanSoDA = new QuanSoDA();

        // GET: ChotQuanSoDauKy
        public ActionResult Index(int? txnId)
        { 
            return View();
        }

        public PartialViewResult _AddFileChotQuanSoDauKy()
        {
            return PartialView("_addFileChotQuanSoDauKy");
        }
        //[HttpPost]
        //public ActionResult ImportFileExel(HttpPostedFileBase file)
        //{

        //    if (file.ContentLength > 0)
        //    {
        //        string _FileName = Path.GetFileName(file.FileName);
        //        string _path = Path.Combine(Server.MapPath("~/Content/"), _FileName);
        //        file.SaveAs(_path);
        //    }



        //    List<TMP_CB_BIET_PHAI> tmpBietPhai = new List<TMP_CB_BIET_PHAI>();
        //    List<TMP_CB_CHO_HUU> tmpChoHuu = new List<TMP_CB_CHO_HUU>();
        //    List<TMP_CB_CONG_TAC> tmpCongTac = new List<TMP_CB_CONG_TAC>();
        //    List<TMP_CB_DI_HOC> tmpDiHoc = new List<TMP_CB_DI_HOC>();
        //    List<TMP_CB_TUYEN_MOI> tmpTuyenMoi = new List<TMP_CB_TUYEN_MOI>();
        //    List<TMP_CS_NGHIA_VU> tmpNghiaVu = new List<TMP_CS_NGHIA_VU>();
        //    ExcelPackage.LicenseContext = LicenseContext.Commercial;
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //    FileInfo fileInfo = new FileInfo(Server.MapPath("~/Content/template1.xlsx"));

        //    ExcelPackage package = new ExcelPackage(fileInfo);

        //    //CB Công tác
        //    ExcelWorksheet wsCongTac = package.Workbook.Worksheets[0];

        //    for (int i = wsCongTac.Dimension.Start.Row; i <= wsCongTac.Dimension.End.Row; i++)
        //    {

        //    }
        //    //CB Tuyển mới
        //    ExcelWorksheet wsTuyenMoi = package.Workbook.Worksheets[1];

        //    for (int i = wsTuyenMoi.Dimension.Start.Row; i <= wsTuyenMoi.Dimension.End.Row; i++)
        //    {

        //    }
        //    //CB Chờ Hưu
        //    ExcelWorksheet wsChoHuu = package.Workbook.Worksheets[2];

        //    for (int i = wsChoHuu.Dimension.Start.Row; i <= wsChoHuu.Dimension.End.Row; i++)
        //    {

        //    }
        //    //CB Biệt phái
        //    ExcelWorksheet wsBietPhai = package.Workbook.Worksheets[3];

        //    for (int i = wsBietPhai.Dimension.Start.Row; i <= wsBietPhai.Dimension.End.Row; i++)
        //    {

        //    }
        //    //CB Nghĩa Vụ
        //    ExcelWorksheet wsNghiaVu = package.Workbook.Worksheets[4];

        //    for (int i = wsNghiaVu.Dimension.Start.Row; i <= wsNghiaVu.Dimension.End.Row; i++)
        //    {

        //    }
        //    //CB Đi học
        //    ExcelWorksheet wsDiHoc = package.Workbook.Worksheets[5];

        //    for (int i = wsDiHoc.Dimension.Start.Row; i <= wsDiHoc.Dimension.End.Row; i++)
        //    {

        //    }
        //    return Json("OK");
        //}
        #region Danh Sách chốt quân số đầu kỳ (Import excel)
        [HttpPost]
        public ActionResult DanhSachTp_CbCongTac(long? txnId)
        {
            try
            { 
                var data = quanSoDA.DanhSachTp_CbCongTac(txnId); 
                return Json(new {data = data, Error = false, Title = "Lấy dữ liệu thành công." });
            }
            catch(Exception ex)
            {
                return Json(new {Error = true, Title = "Lấy dữ liệu không thành công: "+ ex});
            } 
        }
        [HttpPost]
        public ActionResult DanhSachTp_CbChoHuu(long? txnId)
        {
            try
            {
                var data = quanSoDA.DanhSachTp_CbChoHuu(txnId);
                return Json(new { data = data, Error = false, Title = "Lấy dữ liệu thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { Error = true, Title = "Lấy dữ liệu không thành công: " + ex });
            }
        }
        [HttpPost]
        public ActionResult DanhSachTp_BietPhai(long? txnId)
        {
            try
            {
                var data = quanSoDA.DanhSachTp_BietPhai(txnId);
                return Json(new { data = data, Error = false, Title = "Lấy dữ liệu thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { Error = true, Title = "Lấy dữ liệu không thành công: " + ex });
            }
        }
        [HttpPost]
        public ActionResult DanhSachTp_CbDiHoc(long? txnId)
        {
            try
            {
                var data = quanSoDA.DanhSachTp_CbDiHoc(txnId);
                return Json(new { data = data, Error = false, Title = "Lấy dữ liệu thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { Error = true, Title = "Lấy dữ liệu không thành công: " + ex });
            }
        }
        [HttpPost]
        public ActionResult DanhSachTp_CsNghiaVu(long? txnId)
        {
            try
            {
                var data = quanSoDA.DanhSachTp_CsNghiaVu(txnId);
                return Json(new { data = data, Error = false, Title = "Lấy dữ liệu thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { Error = true, Title = "Lấy dữ liệu không thành công: " + ex });
            }
        }
        [HttpPost]
        public ActionResult DanhSachTp_TuyenMoi(long? txnId)
        {
            try
            {
                var data = quanSoDA.DanhSachTp_TuyenMoi(txnId);
                return Json(new { data = data, Error = false, Title = "Lấy dữ liệu thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { Error = true, Title = "Lấy dữ liệu không thành công: " + ex });
            }
        }
        #endregion
    }
}