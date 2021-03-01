using log4net;
using Model.Model;
using Model.ModelExtend; 
using System;
using System.Collections.Generic;
using System.Data; 
using System.Linq;
using Common.Enum;

namespace Data.Home
{
    public class QuanSoDA
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); 

        #region Chốt quân số đầu kỳ
        public List<TC_PHAM_NHANModel> DanhSachTp_PhamNhan(int loaiPham)
        {
            try
            {
                using (var db = new QS_QUAN_TRANGEntities())
                {
                    var query =  from rs in db.TC_PHAM_NHAN
                                 where rs.LOAI_PHAM_ID == loaiPham
                                 select new TC_PHAM_NHANModel
                                 {
                                     ID = rs.ID,
                                     NHOM_TIEU_CHUAN_ID = rs.NHOM_TIEU_CHUAN_ID
                                 };
                    return query.ToList(); 
                }  
            }
            catch (Exception ex)
            {
                return new List<TC_PHAM_NHANModel>();
            } 
        }
        public List<TP_CB_CONG_TACModel> DanhSachTp_CbCongTac(long? txnId)
        {
            try
            {
                var lstData = new List<TP_CB_CONG_TACModel>();
                if (txnId == null) return lstData;
                using (var db = new QS_QUAN_TRANGEntities())
                {
                    //CB Công tác = 1
                    var query = from rs in db.TP_CB_CONG_TAC
                                       where rs.TXN_ID == txnId
                                       select new TP_CB_CONG_TACModel
                                       {
                                           ID = rs.ID,
                                           SO_HIEU = rs.SO_HIEU,
                                           GIOI_TINH = rs.GIOI_TINH,
                                           CAP_BAC_ID = rs.CAP_BAC_ID,
                                           LOAI_HAM_ID = rs.LOAI_HAM_ID,
                                           CHUC_VU = rs.CHUC_VU,
                                           DON_VI_ID = rs.DON_VI_ID,
                                           LUC_LUONG_ID = rs.LUC_LUONG_ID,
                                           CO_GIAY = rs.CO_GIAY,
                                           CO_MU = rs.CO_MU,
                                           CO_QA = rs.CO_QA,
                                           LOAI = rs.LOAI,
                                           LOAI_TUYEN = rs.LOAI_TUYEN,
                                           HIEU_LUC = rs.HIEU_LUC,
                                           TXN_ID = rs.TXN_ID,
                                           DANG_KY_HV = rs.DANG_KY_HV,
                                           DOI_TIEU_CHUAN = rs.DOI_TIEU_CHUAN
                                       };
                    if (query.Any())
                        lstData = query.ToList();
                }
                return lstData;
            }
            catch (Exception ex)
            {
                return new List<TP_CB_CONG_TACModel>();
            }
        }
        public List<TP_CB_CHO_HUUModel> DanhSachTp_CbChoHuu(long? txnId)
        {
            try
            {
                var lstData = new List<TP_CB_CHO_HUUModel>();
                if (txnId == null) return lstData;
                using (var db = new QS_QUAN_TRANGEntities())
                { 
                    var query = from rs in db.TP_CB_CHO_HUU
                                where rs.TXN_ID == txnId
                                select new TP_CB_CHO_HUUModel
                                {
                                    ID = rs.ID,
                                    SO_HIEU = rs.SO_HIEU,
                                    GIOI_TINH = rs.GIOI_TINH,
                                    CAP_BAC_ID = rs.CAP_BAC_ID,
                                    LOAI_HAM_ID = rs.LOAI_HAM_ID,
                                    CHUC_VU = rs.CHUC_VU,
                                    DON_VI_ID = rs.DON_VI_ID,
                                    LUC_LUONG_ID = rs.LUC_LUONG_ID,
                                    CO_GIAY = rs.CO_GIAY,
                                    CO_MU = rs.CO_MU,
                                    CO_QA = rs.CO_QA,
                                    LOAI = rs.LOAI,
                                    LOAI_TUYEN = rs.LOAI_TUYEN,
                                    HIEU_LUC = rs.HIEU_LUC,
                                    TXN_ID = rs.TXN_ID,
                                    DANG_KY_HV = rs.DANG_KY_HV
                                };
                    if (query.Any())
                        lstData = query.ToList();
                }
                return lstData;
            }
            catch (Exception ex)
            {
                return new List<TP_CB_CHO_HUUModel>();
            }
        }
        public List<TP_CB_BIET_PHAIModel> DanhSachTp_BietPhai(long? txnId)
        {
            try
            {
                var lstData = new List<TP_CB_BIET_PHAIModel>();
                if (txnId == null) return lstData;
                using (var db = new QS_QUAN_TRANGEntities())
                {
                    var query = from rs in db.TP_CB_BIET_PHAI
                                where rs.TXN_ID == txnId
                                select new TP_CB_BIET_PHAIModel
                                {
                                    ID = rs.ID,
                                    SO_HIEU = rs.SO_HIEU,
                                    GIOI_TINH = rs.GIOI_TINH,
                                    CAP_BAC_ID = rs.CAP_BAC_ID,
                                    LOAI_HAM_ID = rs.LOAI_HAM_ID,
                                    CHUC_VU = rs.CHUC_VU,
                                    DON_VI_ID = rs.DON_VI_ID,
                                    LUC_LUONG_ID = rs.LUC_LUONG_ID,
                                    CO_GIAY = rs.CO_GIAY,
                                    CO_MU = rs.CO_MU,
                                    CO_QA = rs.CO_QA,
                                    LOAI = rs.LOAI,
                                    LOAI_TUYEN = rs.LOAI_TUYEN,
                                    HIEU_LUC = rs.HIEU_LUC,
                                    TXN_ID = rs.TXN_ID,
                                    DANG_KY_HV = rs.DANG_KY_HV
                                };
                    if (query.Any())
                        lstData = query.ToList();
                }
                return lstData;
            }
            catch (Exception ex)
            {
                return new List<TP_CB_BIET_PHAIModel>();
            }
        }
        public List<TP_CB_DI_HOCModel> DanhSachTp_CbDiHoc(long? txnId)
        {
            try
            {
                var lstData = new List<TP_CB_DI_HOCModel>();
                if (txnId == null) return lstData;
                using (var db = new QS_QUAN_TRANGEntities())
                {
                    var query = from rs in db.TP_CB_DI_HOC
                                where rs.TXN_ID == txnId
                                select new TP_CB_DI_HOCModel
                                {
                                    ID = rs.ID,
                                    SO_HIEU = rs.SO_HIEU,
                                    GIOI_TINH = rs.GIOI_TINH,
                                    CAP_BAC_ID = rs.CAP_BAC_ID,
                                    LOAI_HAM_ID = rs.LOAI_HAM_ID,
                                    CHUC_VU = rs.CHUC_VU,
                                    DON_VI_ID = rs.DON_VI_ID,
                                    LUC_LUONG_ID = rs.LUC_LUONG_ID,
                                    CO_GIAY = rs.CO_GIAY,
                                    CO_MU = rs.CO_MU,
                                    CO_QA = rs.CO_QA,
                                    LOAI = rs.LOAI,
                                    LOAI_TUYEN = rs.LOAI_TUYEN,
                                    HIEU_LUC = rs.HIEU_LUC,
                                    TXN_ID = rs.TXN_ID,
                                    DANG_KY_HV = rs.DANG_KY_HV
                                };
                    if (query.Any())
                        lstData = query.ToList();
                }
                return lstData;
            }
            catch (Exception ex)
            {
                return new List<TP_CB_DI_HOCModel>();
            }
        }
        public List<TP_CS_NGHIA_VUModel> DanhSachTp_CsNghiaVu(long? txnId)
        {
            try
            {
                var lstData = new List<TP_CS_NGHIA_VUModel>();
                if (txnId == null) return lstData;
                using (var db = new QS_QUAN_TRANGEntities())
                {
                    var query = from rs in db.TP_CS_NGHIA_VU
                                where rs.TXN_ID == txnId
                                select new TP_CS_NGHIA_VUModel
                                {
                                    ID = rs.ID,
                                    SO_HIEU = rs.SO_HIEU,
                                    GIOI_TINH = rs.GIOI_TINH,
                                    CAP_BAC_ID = rs.CAP_BAC_ID,
                                    LOAI_HAM_ID = rs.LOAI_HAM_ID,
                                    CHUC_VU = rs.CHUC_VU,
                                    DON_VI_ID = rs.DON_VI_ID,
                                    LUC_LUONG_ID = rs.LUC_LUONG_ID,
                                    CO_GIAY = rs.CO_GIAY,
                                    CO_MU = rs.CO_MU,
                                    CO_QA = rs.CO_QA,
                                    LOAI = rs.LOAI,
                                    LOAI_TUYEN = rs.LOAI_TUYEN,
                                    HIEU_LUC = rs.HIEU_LUC,
                                    TXN_ID = rs.TXN_ID
                                };
                    if (query.Any())
                        lstData = query.ToList();
                }
                return lstData;
            }
            catch (Exception ex)
            {
                return new List<TP_CS_NGHIA_VUModel>();
            }
        }
        public List<TP_CB_TUYEN_MOIModel> DanhSachTp_TuyenMoi(long? txnId)
        {
            try
            {
                var lstData = new List<TP_CB_TUYEN_MOIModel>();
                if (txnId == null) return lstData;
                using (var db = new QS_QUAN_TRANGEntities())
                {
                    var query = from rs in db.TP_CB_TUYEN_MOI
                                where rs.TXN_ID == txnId
                                select new TP_CB_TUYEN_MOIModel
                                {
                                    ID = rs.ID,
                                    SO_HIEU = rs.SO_HIEU,
                                    GIOI_TINH = rs.GIOI_TINH,
                                    CAP_BAC_ID = rs.CAP_BAC_ID,
                                    LOAI_HAM_ID = rs.LOAI_HAM_ID,
                                    CHUC_VU = rs.CHUC_VU,
                                    DON_VI_ID = rs.DON_VI_ID,
                                    LUC_LUONG_ID = rs.LUC_LUONG_ID,
                                    CO_GIAY = rs.CO_GIAY,
                                    CO_MU = rs.CO_MU,
                                    CO_QA = rs.CO_QA,
                                    NGANH_NGOAI = rs.NGANH_NGOAI,
                                    TRUONG_CAND = rs.TRUONG_CAND,
                                    TBINH_TUYEN_LAI = rs.TBINH_TUYEN_LAI,
                                    LOAI = rs.LOAI,
                                    LOAI_TUYEN = rs.LOAI_TUYEN,
                                    HIEU_LUC = rs.HIEU_LUC,
                                    TXN_ID = rs.TXN_ID,
                                    DANG_KY_HV = rs.DANG_KY_HV
                                };
                    if (query.Any())
                        lstData = query.ToList();
                }
                return lstData;
            }
            catch (Exception ex)
            {
                return new List<TP_CB_TUYEN_MOIModel>();
            }
        } 
        #endregion
    }
}
