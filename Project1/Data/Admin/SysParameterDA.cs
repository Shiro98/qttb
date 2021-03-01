using log4net;
using Model.Model;
using Model.ModelExtend;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Admin
{
    public class SysParameterDA
    {
        ITEMS_SYSTEMEntities db = new ITEMS_SYSTEMEntities();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ST_PARAMETERS GetItemByCode(string code, string appCode)
        {
            return db.ST_PARAMETERS.FirstOrDefault(x => x.PARAM_CODE == code && x.APP_CODE == appCode);
        }

        public List<SysParameterPageModel> GetAllByPage(ModelSearchUser modelSearch, ref int pageSize)
        {
            var result = new List<SysParameterPageModel>();
            pageSize = 10;
            try
            {
                var param = db.ST_PARAMETERS.FirstOrDefault(x => x.PARAM_CODE == "PageSize");
                if (param != null)
                    pageSize = Convert.ToInt32(param.PARAM_VALUE);

                var sqlString = "SELECT *, count(PARAM_ID) over() as TotalRow FROM [ST_PARAMETERS] WHERE ENABLED_FLAG = 'Y'";
                if (!string.IsNullOrEmpty(modelSearch.KeyWord))
                {
                    sqlString += " AND (PARAM_CODE LIKE N'%" + modelSearch.KeyWord + "%' OR PARAM_DESC LIKE N'%" + modelSearch.KeyWord + "%')";
                }
                if (!string.IsNullOrEmpty(modelSearch.SortColumn))
                    sqlString += " ORDER BY " + modelSearch.SortColumn;
                sqlString += " OFFSET " + ((modelSearch.currentPage - 1) * modelSearch.pageSize) + " ROWS FETCH NEXT " + modelSearch.pageSize + " ROWS ONLY;";
                result = db.Database.SqlQuery<SysParameterPageModel>(sqlString).ToList();
            }
            catch (Exception ex)
            {
                log.Error("SysParameterDA - Lấy danh sách tham số lỗi: " + ex.Message);
                result = new List<SysParameterPageModel>();
            }
            return result;
        }

        public List<ST_PARAMETERS> GetAll()
        {
            return db.ST_PARAMETERS.ToList();
        }


        public ObjectMessage Add(ST_PARAMETERS sysParameter)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                sysParameter.ENABLED_FLAG = "Y";
                db.ST_PARAMETERS.Add(sysParameter);
                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Thêm mới thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("SysParameterDA - Thêm tham số("+sysParameter.PARAM_CODE+") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
        public ObjectMessage Edit(ST_PARAMETERS sysParameter)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.ST_PARAMETERS.FirstOrDefault(x => x.PARAM_ID == sysParameter.PARAM_ID);
                data.APP_CODE = sysParameter.APP_CODE;
                data.PARAM_CODE = sysParameter.PARAM_CODE;
                data.PARAM_NAME = sysParameter.PARAM_NAME;
                data.PARAM_VALUE = sysParameter.PARAM_VALUE;
                data.PARAM_DESC = sysParameter.PARAM_DESC;
                data.PARAM_VALUE_TYPE = sysParameter.PARAM_VALUE_TYPE;
                data.APP_CAN_EDIT = sysParameter.APP_CAN_EDIT;
                data.MODULE = sysParameter.MODULE;
                data.SCOPE = sysParameter.SCOPE;
                data.DOMAIN_CODE = sysParameter.DOMAIN_CODE;
                data.UNIT_ID = sysParameter.UNIT_ID;
                data.ID_BO_PHAN = sysParameter.ID_BO_PHAN;
                data.UPDATED_BY = sysParameter.UPDATED_BY;
                data.UPDATE_DATE = sysParameter.UPDATE_DATE;
                data.DISPLAY_FLAG = sysParameter.DISPLAY_FLAG;

                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Cập nhật thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("SysParameterDA - Sửa tham số(" + sysParameter.PARAM_CODE + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
        public ObjectMessage Delete(string Id, int updateBy)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.ST_PARAMETERS.FirstOrDefault(x => x.PARAM_ID == Id);
                data.ENABLED_FLAG = "N";
                data.UPDATED_BY = updateBy;
                data.UPDATE_DATE = DateTime.Now;
                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Xóa thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("SysParameterDA - Xóa tham số(" + Id + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
    }
}
