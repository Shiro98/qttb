using Common;
using log4net;
using Model.Model;
using Model.ModelExtend;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Data.Admin
{
    public class FlexValueSetDA
    {
        ITEMS_LISTEntities db = new ITEMS_LISTEntities();
        DatabaseSql databaseSql = new DatabaseSql();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public List<LT_FLEX_VALUE_SETS> GetSelect(string appCode, string domainCode, int? unitId, string flexType)
        {
            var result = new List<LT_FLEX_VALUE_SETS>();

            try
            {
                var paramList = new List<SqlParameter>
                    {
                        new SqlParameter(@"APP_CODE", !string.IsNullOrEmpty(appCode)? (object)appCode : DBNull.Value),
                        new SqlParameter(@"DOMAIN_CODE", !string.IsNullOrEmpty(domainCode)? (object)domainCode: DBNull.Value),
                        new SqlParameter(@"UNIT_ID", unitId != null ? (object)unitId: DBNull.Value),
                        new SqlParameter(@"FLEX_TYPE", !string.IsNullOrEmpty(flexType)? (object)flexType: DBNull.Value)
                    };
                result = DatabaseSql.ExecuteProcToList<LT_FLEX_VALUE_SETS>("LT_FLEX_VALUE_SETS_SEARCH", paramList, Constants.ConectionStr_ITEMS_LIST).ToList();
            }
            catch (Exception ex)
            {
                log.Error("FlexValueSetDA - Lấy danh sách danh mục lỗi: " + ex.Message);
                result = new List<LT_FLEX_VALUE_SETS>();
            }

            return result;
        }

    }
}
