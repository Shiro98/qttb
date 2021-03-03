using Model.Model;
using Model.ModelExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DanhMuc
{
    public class Don_vi_tinhDA
    {
        public List<DM_DON_VI_TINHModel> DanhSach(int? tID)
        {
            try
            {
                using (var db = new ITEMS_LISTEntities())
                {
                    var query = from rs in db.DM_DON_VI_TINH
                                select new DM_DON_VI_TINHModel
                                {
                                    ID = rs.ID,
                                    TEN_DVT = rs.TEN_DVT,
                                    MO_TA = rs.MO_TA
                                };
                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<DM_DON_VI_TINHModel>();
            }
        }
    }
}
