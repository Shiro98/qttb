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
    public class FlexValueDA
    {
        ITEMS_LISTEntities db = new ITEMS_LISTEntities();
        DatabaseSql databaseSql = new DatabaseSql();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public List<LT_FLEX_VALUES> GetAll()
        {
            var result = new List<LT_FLEX_VALUES>();
            var data = db.LT_FLEX_VALUES.Where(x => x.ENABLED_FLAG == "Y").ToList();
            if (data != null && data.Count > 0)
                result = data;
            return result;
        }

        public LT_FLEX_VALUES GetById(int id)
        {
            return db.LT_FLEX_VALUES.FirstOrDefault(x => x.FLEX_VALUE_ID == id);
        }

        public List<FlexValueModel> GetAllByPage(ModelSearchFelexValue modelSearch)
        {
            var result = new List<FlexValueModel>();

            try
            {
                var paramList = new List<SqlParameter>
                    {
                         new SqlParameter(@"FLEX_VALUE_ID",  modelSearch.FLEX_VALUE_ID  >0? (object)modelSearch.FLEX_VALUE_ID : DBNull.Value),
                         new SqlParameter(@"FLEX_VALUE_SET_ID", modelSearch.FLEX_VALUE_SET_ID),
                         new SqlParameter(@"FLEX_VALUE", !string.IsNullOrEmpty(modelSearch.FLEX_VALUE)? (object)modelSearch.FLEX_VALUE : DBNull.Value),
                         new SqlParameter(@"LAST_UPDATE_DATE", modelSearch.LAST_UPDATE_DATE != null? (object)modelSearch.LAST_UPDATE_DATE : DBNull.Value),
                         new SqlParameter(@"LAST_UPDATED_BY", modelSearch.LAST_UPDATED_BY != null? (object)modelSearch.LAST_UPDATED_BY : DBNull.Value),
                         new SqlParameter(@"CREATION_DATE", modelSearch.CREATION_DATE != null? (object)modelSearch.CREATION_DATE : DBNull.Value),
                         new SqlParameter(@"CREATED_BY", modelSearch.CREATED_BY != null? (object)modelSearch.CREATED_BY : DBNull.Value),
                         new SqlParameter(@"DESCRIPTION", !string.IsNullOrEmpty(modelSearch.DESCRIPTION)? (object)modelSearch.DESCRIPTION : DBNull.Value),
                         new SqlParameter(@"ENABLED_FLAG", !string.IsNullOrEmpty(modelSearch.ENABLED_FLAG)? (object)modelSearch.ENABLED_FLAG : DBNull.Value),
                         new SqlParameter(@"SUMMARY_FLAG", !string.IsNullOrEmpty(modelSearch.SUMMARY_FLAG)? (object)modelSearch.SUMMARY_FLAG : DBNull.Value),
                         new SqlParameter(@"PARENT_FLEX_VALUE_ID", (modelSearch.PARENT_FLEX_VALUE_ID != null && modelSearch.PARENT_FLEX_VALUE_ID >0)? (object)modelSearch.PARENT_FLEX_VALUE_ID : DBNull.Value),
                         new SqlParameter(@"HIERARCHY_LEVEL", modelSearch.HIERARCHY_LEVEL != null? (object)modelSearch.HIERARCHY_LEVEL : DBNull.Value),
                         new SqlParameter(@"FLEX_VALUE_CATEGORY", !string.IsNullOrEmpty(modelSearch.FLEX_VALUE_CATEGORY)? (object)modelSearch.FLEX_VALUE_CATEGORY : DBNull.Value),
                         new SqlParameter(@"ATTRIBUTE1", !string.IsNullOrEmpty(modelSearch.ATTRIBUTE1)? (object)modelSearch.ATTRIBUTE1 : DBNull.Value),
                         new SqlParameter(@"ATTRIBUTE2", !string.IsNullOrEmpty(modelSearch.ATTRIBUTE2)? (object)modelSearch.ATTRIBUTE2 : DBNull.Value),
                         new SqlParameter(@"ATTRIBUTE3", !string.IsNullOrEmpty(modelSearch.ATTRIBUTE3)? (object)modelSearch.ATTRIBUTE3 : DBNull.Value),
                         new SqlParameter(@"ATTRIBUTE4", !string.IsNullOrEmpty(modelSearch.ATTRIBUTE4)? (object)modelSearch.ATTRIBUTE4 : DBNull.Value),
                         new SqlParameter(@"ATTRIBUTE5", !string.IsNullOrEmpty(modelSearch.ATTRIBUTE5)? (object)modelSearch.ATTRIBUTE5 : DBNull.Value),
                         new SqlParameter(@"ATTRIBUTE6", !string.IsNullOrEmpty(modelSearch.ATTRIBUTE6)? (object)modelSearch.ATTRIBUTE6 : DBNull.Value),
                         new SqlParameter(@"ATTRIBUTE7", !string.IsNullOrEmpty(modelSearch.ATTRIBUTE7)? (object)modelSearch.ATTRIBUTE7 : DBNull.Value),
                         new SqlParameter(@"ATTRIBUTE8", !string.IsNullOrEmpty(modelSearch.ATTRIBUTE8)? (object)modelSearch.ATTRIBUTE8 : DBNull.Value),
                         new SqlParameter(@"ATTRIBUTE9", !string.IsNullOrEmpty(modelSearch.ATTRIBUTE9)? (object)modelSearch.ATTRIBUTE9 : DBNull.Value),
                         new SqlParameter(@"ATTRIBUTE10", !string.IsNullOrEmpty(modelSearch.ATTRIBUTE10)? (object)modelSearch.ATTRIBUTE10 : DBNull.Value),
                         new SqlParameter(@"page", modelSearch.currentPage),
                         new SqlParameter(@"OrderByClause", modelSearch.SortColumn),
                         new SqlParameter(@"pageSize", modelSearch.pageSize)
                    };
                result = DatabaseSql.ExecuteProcToList<FlexValueModel>("LT_FLEX_VALUES_SEARCH", paramList, Constants.ConectionStr_ITEMS_LIST).ToList();
            }
            catch (Exception ex)
            {
                log.Error("FlexValueDA - Lấy danh sách danh mục theo trang lỗi: " + ex.Message);
                result = new List<FlexValueModel>();
            }

            return result;
        }

        public ObjectMessage Add(LT_FLEX_VALUES model)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var codeOutRole = "";
                var paramList = new List<SqlParameter>
                            {
                                new SqlParameter(@"FLEX_VALUE_SET_ID", model.FLEX_VALUE_SET_ID),
                                new SqlParameter(@"FLEX_VALUE", !string.IsNullOrEmpty(model.FLEX_VALUE)? (object)model.FLEX_VALUE : DBNull.Value),
                                new SqlParameter(@"CREATED_BY",model.CREATED_BY),
                                new SqlParameter(@"DESCRIPTION", !string.IsNullOrEmpty(model.DESCRIPTION)? (object)model.DESCRIPTION : DBNull.Value),
                                 new SqlParameter(@"ENABLED_FLAG", model.ENABLED_FLAG),
                                new SqlParameter(@"SUMMARY_FLAG", !string.IsNullOrEmpty(model.SUMMARY_FLAG)? (object)model.SUMMARY_FLAG : DBNull.Value),
                                new SqlParameter(@"PARENT_FLEX_VALUE_ID", model.PARENT_FLEX_VALUE_ID != null? (object)model.PARENT_FLEX_VALUE_ID : DBNull.Value ),
                                new SqlParameter(@"HIERARCHY_LEVEL", model.HIERARCHY_LEVEL != null? (object)model.HIERARCHY_LEVEL : DBNull.Value ),
                                new SqlParameter(@"FLEX_VALUE_CATEGORY", !string.IsNullOrEmpty(model.FLEX_VALUE_CATEGORY)? (object)model.FLEX_VALUE_CATEGORY : DBNull.Value),
                                new SqlParameter(@"ATTRIBUTE1", !string.IsNullOrEmpty(model.ATTRIBUTE1)? (object)model.ATTRIBUTE1 : DBNull.Value),
                                new SqlParameter(@"ATTRIBUTE2", !string.IsNullOrEmpty(model.ATTRIBUTE2)? (object)model.ATTRIBUTE2 : DBNull.Value),
                                new SqlParameter(@"ATTRIBUTE3", !string.IsNullOrEmpty(model.ATTRIBUTE3)? (object)model.ATTRIBUTE3 : DBNull.Value),
                                new SqlParameter(@"ATTRIBUTE4", !string.IsNullOrEmpty(model.ATTRIBUTE4)? (object)model.ATTRIBUTE4 : DBNull.Value),
                                new SqlParameter(@"ATTRIBUTE5", !string.IsNullOrEmpty(model.ATTRIBUTE5)? (object)model.ATTRIBUTE5 : DBNull.Value),
                                new SqlParameter(@"ATTRIBUTE6", !string.IsNullOrEmpty(model.ATTRIBUTE6)? (object)model.ATTRIBUTE6 : DBNull.Value),
                                new SqlParameter(@"ATTRIBUTE7", !string.IsNullOrEmpty(model.ATTRIBUTE7)? (object)model.ATTRIBUTE7 : DBNull.Value),
                                new SqlParameter(@"ATTRIBUTE8", !string.IsNullOrEmpty(model.ATTRIBUTE8)? (object)model.ATTRIBUTE8 : DBNull.Value),
                                new SqlParameter(@"ATTRIBUTE9", !string.IsNullOrEmpty(model.ATTRIBUTE9)? (object)model.ATTRIBUTE9 : DBNull.Value),
                                new SqlParameter(@"ATTRIBUTE10", !string.IsNullOrEmpty(model.ATTRIBUTE10)? (object)model.ATTRIBUTE10 : DBNull.Value),
                                new SqlParameter(@"CODE_OUT", SqlDbType.VarChar,30) { Direction = ParameterDirection.Output },
                                new SqlParameter(@"MESSAGE_OUT", SqlDbType.VarChar,1000) { Direction = ParameterDirection.Output }
                            };
                var resultPro = DatabaseSql.ExecuteProcNonQuery("LT_FLEX_VALUES_CREATE", paramList, Constants.ConectionStr_ITEMS_LIST);

                if (!string.IsNullOrEmpty(((SqlParameter)paramList[paramList.Count - 2]).Value.ToString()))
                    codeOutRole = ((SqlParameter)paramList[paramList.Count - 2]).Value.ToString();
                if (codeOutRole == "OK")
                {
                    obj.Error = false;
                    obj.Title = "Thêm mới thành công!";
                }
                else
                {
                    obj.Error = true;
                    obj.Title = "Thêm mới lỗi: " + ((SqlParameter)paramList[paramList.Count - 1]).Value.ToString();
                }


            }
            catch (Exception ex)
            {
                log.Error("FlexValueDA - Thêm danh mục (" + model.FLEX_VALUE + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
            }
            return obj;
        }
        public ObjectMessage Edit(LT_FLEX_VALUES model)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var codeOutRole = "";
                var paramList = new List<SqlParameter>
                            {
                                new SqlParameter(@"FLEX_VALUE_ID", model.FLEX_VALUE_ID),
                                new SqlParameter(@"FLEX_VALUE_SET_ID", model.FLEX_VALUE_SET_ID),
                                new SqlParameter(@"FLEX_VALUE", !string.IsNullOrEmpty(model.FLEX_VALUE)? (object)model.FLEX_VALUE : DBNull.Value),
                                new SqlParameter(@"LAST_UPDATED_BY",model.LAST_UPDATED_BY),
                                new SqlParameter(@"DESCRIPTION", !string.IsNullOrEmpty(model.DESCRIPTION)? (object)model.DESCRIPTION : DBNull.Value),
                                new SqlParameter(@"ENABLED_FLAG", !string.IsNullOrEmpty(model.ENABLED_FLAG)? (object)model.ENABLED_FLAG : DBNull.Value),
                                new SqlParameter(@"SUMMARY_FLAG", !string.IsNullOrEmpty(model.SUMMARY_FLAG)? (object)model.SUMMARY_FLAG : DBNull.Value),
                                new SqlParameter(@"PARENT_FLEX_VALUE_ID", model.PARENT_FLEX_VALUE_ID != null? (object)model.PARENT_FLEX_VALUE_ID : DBNull.Value ),
                                new SqlParameter(@"HIERARCHY_LEVEL", model.HIERARCHY_LEVEL != null? (object)model.HIERARCHY_LEVEL : DBNull.Value ),
                                new SqlParameter(@"FLEX_VALUE_CATEGORY", !string.IsNullOrEmpty(model.FLEX_VALUE_CATEGORY)? (object)model.FLEX_VALUE_CATEGORY : DBNull.Value),
                                new SqlParameter(@"ATTRIBUTE1", !string.IsNullOrEmpty(model.ATTRIBUTE1)? (object)model.ATTRIBUTE1 : DBNull.Value),
                                new SqlParameter(@"ATTRIBUTE2", !string.IsNullOrEmpty(model.ATTRIBUTE2)? (object)model.ATTRIBUTE2 : DBNull.Value),
                                new SqlParameter(@"ATTRIBUTE3", !string.IsNullOrEmpty(model.ATTRIBUTE3)? (object)model.ATTRIBUTE3 : DBNull.Value),
                                new SqlParameter(@"ATTRIBUTE4", !string.IsNullOrEmpty(model.ATTRIBUTE4)? (object)model.ATTRIBUTE4 : DBNull.Value),
                                new SqlParameter(@"ATTRIBUTE5", !string.IsNullOrEmpty(model.ATTRIBUTE5)? (object)model.ATTRIBUTE5 : DBNull.Value),
                                new SqlParameter(@"ATTRIBUTE6", !string.IsNullOrEmpty(model.ATTRIBUTE6)? (object)model.ATTRIBUTE6 : DBNull.Value),
                                new SqlParameter(@"ATTRIBUTE7", !string.IsNullOrEmpty(model.ATTRIBUTE7)? (object)model.ATTRIBUTE7 : DBNull.Value),
                                new SqlParameter(@"ATTRIBUTE8", !string.IsNullOrEmpty(model.ATTRIBUTE8)? (object)model.ATTRIBUTE8 : DBNull.Value),
                                new SqlParameter(@"ATTRIBUTE9", !string.IsNullOrEmpty(model.ATTRIBUTE9)? (object)model.ATTRIBUTE9 : DBNull.Value),
                                new SqlParameter(@"ATTRIBUTE10", !string.IsNullOrEmpty(model.ATTRIBUTE10)? (object)model.ATTRIBUTE10 : DBNull.Value),
                                new SqlParameter(@"CODE_OUT", SqlDbType.VarChar,30) { Direction = ParameterDirection.Output },
                                new SqlParameter(@"MESSAGE_OUT", SqlDbType.VarChar,1000) { Direction = ParameterDirection.Output }
                            };
                var resultPro = DatabaseSql.ExecuteProcNonQuery("LT_FLEX_VALUES_UPDATE", paramList, Constants.ConectionStr_ITEMS_LIST);
                if (!string.IsNullOrEmpty(((SqlParameter)paramList[paramList.Count - 2]).Value.ToString()))
                    codeOutRole = ((SqlParameter)paramList[paramList.Count - 2]).Value.ToString();
                if (codeOutRole == "OK")
                {
                    obj.Error = false;
                    obj.Title = "Sửa danh mục thành công!";
                }
                else
                {
                    obj.Error = true;
                    obj.Title = "Sửa danh mục lỗi: " + ((SqlParameter)paramList[paramList.Count - 1]).Value.ToString();
                }

            }
            catch (Exception ex)
            {
                log.Error("FlexValueDA - Sửa danh mục (" + model.FLEX_VALUE + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
            }
            return obj;
        }
        public ObjectMessage Delete(int Id, int updateBy)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var outCode = "";
                var paramList = new List<SqlParameter>
                    {
                        new SqlParameter(@"FLEX_VALUE_ID", Id),
                        new SqlParameter(@"LAST_UPDATED_BY", updateBy),
                        new SqlParameter(@"CODE_OUT", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Output },
                        new SqlParameter(@"MESSAGE_OUT", SqlDbType.NVarChar, 1000) { Direction = ParameterDirection.Output }
                    };
                var result = DatabaseSql.ExecuteProcNonQuery("LT_FLEX_VALUES_DELETE", paramList, Constants.ConectionStr_ITEMS_LIST);
                if (!string.IsNullOrEmpty(((SqlParameter)paramList[paramList.Count - 2]).Value.ToString()))
                    outCode = ((SqlParameter)paramList[paramList.Count - 2]).Value.ToString();

                if (outCode == "OK")
                {
                    obj.Error = false;
                    obj.Title = "Xóa thành công!";
                }
                else
                {
                    log.Error("FlexValueDA - Xóa danh mục (Id: " + Id + ", updateBy: " + updateBy + ") lỗi: " + ((SqlParameter)paramList[paramList.Count - 1]).Value.ToString());
                    obj.Error = true;
                    obj.Title = "Xóa không thành công!";
                }

                return obj;
            }
            catch (Exception ex)
            {
                log.Error("FlexValueDA - Xóa danh mục (Id: " + Id + ", updateBy: " + updateBy + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
    }
}
