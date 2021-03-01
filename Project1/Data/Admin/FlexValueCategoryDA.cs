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
    public class FlexValueCategoryDA
    {
        ITEMS_LISTEntities db = new ITEMS_LISTEntities();
        DatabaseSql databaseSql = new DatabaseSql();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public List<LT_FLEX_VALUE_CATEGORIES> GetAll()
        {
            db.Configuration.ProxyCreationEnabled = false;

            var result = new List<LT_FLEX_VALUE_CATEGORIES>();
            var data = db.LT_FLEX_VALUE_CATEGORIES.Where(x => x.ENABLED_FLAG == "Y").ToList();
            if (data != null && data.Count > 0)
                result = data;
            return result;
        }

        public List<LT_FLEX_VALUE_CATEGORIES> GetByFlexValueSet(int flexValueSetId)
        {
            db.Configuration.ProxyCreationEnabled = false;

            var result = new List<LT_FLEX_VALUE_CATEGORIES>();
            var data = db.LT_FLEX_VALUE_CATEGORIES.Where(x => x.ENABLED_FLAG == "Y" && x.FLEX_VALUE_SET_ID == flexValueSetId).ToList();
            if (data != null && data.Count > 0)
                result = data;
            return result;
        }

        /// <summary>
        /// Lấy danh sách tiêu đề, hiển thị của các cột
        /// </summary>
        /// <param name="FLEX_VALUE_SET_ID"></param>
        /// <returns></returns>
        public List<FlexColumnModel> GetColumn(int FLEX_VALUE_SET_ID)
        {
            List<FlexColumnModel> result = new List<FlexColumnModel>();

            try
            {
                var paramList = new List<SqlParameter>
                    {
                        new SqlParameter(@"FLEX_VALUE_SET_ID", FLEX_VALUE_SET_ID)
                    };
                var resultPro = DatabaseSql.ExecuteProcToList<FlexColumnModel>("LT_DESC_FLEX_COLUMN_USAGES_GET_BY_FVS", paramList, Constants.ConectionStr_ITEMS_LIST).ToList();

                // lấy option khi là controll là select
                if(resultPro != null && resultPro.Count > 0)
                {
                    foreach (var item in resultPro)
                    {
                        item.Options = new List<SelectModel>() { new SelectModel { Value= "", Name="Không chọn"} };
                        if (item.CONTROL_NAME == "select" && item.IS_CATEGORY_COLUMN == "Y" && item.FLEX_COLUMN_NAME != "FLEX_VALUE_SET_ID")
                        {
                            var paramListSelect = new List<SqlParameter>
                                {
                                    new SqlParameter(@"FLEX_VALUE_SET_NAME", item.FLEX_VALUE_CATEGORY),
                                    new SqlParameter(@"IdOrCode", (item.FLEX_COL_DATA_TYPE == "int" ? 1 : 0))
                                };
                            item.Options.AddRange(DatabaseSql.ExecuteProcToList<SelectModel>("LT_FLEX_VALUE_CATEGORIES_GET_SELECT", paramListSelect, Constants.ConectionStr_ITEMS_LIST).ToList());
                        }
                        result.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("FlexValueCategoryDA - Lấy danh sách danh mục theo trang lỗi: " + ex.Message);
                result = new List<FlexColumnModel>();
            }

            return result;
        }

        ///// <summary>
        ///// Lấy danh sách tiêu đề, hiển thị của các cột
        ///// </summary>
        ///// <param name="modelSearch"></param>
        ///// <returns></returns>
        //public FlexValueCategoryColumn GetColumn(ModelSearchFelexValue modelSearch)
        //{
        //    FlexValueCategoryColumn result = new FlexValueCategoryColumn();

        //    try
        //    {
        //        var paramList = new List<SqlParameter>
        //            {
        //                new SqlParameter(@"FLEX_VALUE_SET_ID", modelSearch.FLEX_VALUE_SET_ID)
        //            };
        //        var resultPro = DatabaseSql.ExecuteProcToList<LT_DESC_FLEX_COLUMN_USAGES>("LT_FLEX_VALUES_SEARCH", paramList, Constants.ConectionStr_ITEMS_LIST).ToList();

        //        if (resultPro != null && resultPro.Count > 0)
        //        {
        //            LT_FLEX_VALUE_CATEGORIES child = new LT_FLEX_VALUE_CATEGORIES();
        //            var flColumn = result.GetType().GetProperties();
        //            foreach (System.Reflection.PropertyInfo pi in child.GetType().GetProperties())
        //            {
        //                for (int i = 0; i < resultPro.Count; i++)
        //                {
        //                    if (pi.Name == resultPro[i].FLEX_COLUMN_NAME)
        //                    {
        //                        var flColumn1 = flColumn.Where(x => x.Name.Contains(pi.Name)).ToList();
        //                        foreach (System.Reflection.PropertyInfo fl in flColumn1)
        //                        {
        //                            if (fl.Name.Contains("_TITLE"))
        //                                fl.SetValue(result, resultPro[i].END_USER_COLUMN_NAME, null);

        //                            if (fl.Name.Contains("_SHOW"))
        //                                fl.SetValue(result, true, null);
        //                        }
        //                    }
        //                }

        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("FlexValueCategoryDA - Lấy danh sách cột của nhóm/loại lỗi: " + ex.Message);
        //        result = new FlexValueCategoryColumn();
        //    }

        //    return result;
        //}

        public LT_FLEX_VALUE_CATEGORIES GetById(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db.LT_FLEX_VALUE_CATEGORIES.FirstOrDefault(x => x.FLEX_VALUE_CATEGORY_ID == id);
        }

        public List<FlexValueCategoryModel> GetAllByPage(ModelSearchFlexValueCategory modelSearch)
        {
            var result = new List<FlexValueCategoryModel>();

            try
            {
                var paramList = new List<SqlParameter>
                    {
                        new SqlParameter(@"FLEX_VALUE_CATEGORY_ID", modelSearch.FLEX_VALUE_CATEGORY_ID),
                        new SqlParameter(@"FLEX_VALUE_SET_ID", modelSearch.FLEX_VALUE_SET_ID),
                        new SqlParameter(@"FLEX_VALUE_CATEGORY", !string.IsNullOrEmpty(modelSearch.FLEX_VALUE_CATEGORY)? (object)modelSearch.FLEX_VALUE_CATEGORY : DBNull.Value),
                        new SqlParameter(@"DESCRIPTION", !string.IsNullOrEmpty(modelSearch.DESCRIPTION)? (object)modelSearch.DESCRIPTION : DBNull.Value),
                        new SqlParameter(@"ENABLED_FLAG", !string.IsNullOrEmpty(modelSearch.ENABLED_FLAG)? (object)modelSearch.ENABLED_FLAG : DBNull.Value),
                        new SqlParameter(@"PARENT_FLEX_VALUE_CATEGORY", modelSearch.PARENT_FLEX_VALUE_CATEGORY != null? (object)modelSearch.PARENT_FLEX_VALUE_CATEGORY : DBNull.Value),
                        new SqlParameter(@"NOTE", !string.IsNullOrEmpty(modelSearch.NOTE)? (object)modelSearch.NOTE : DBNull.Value),
                        new SqlParameter(@"DATA_SOURCE", !string.IsNullOrEmpty(modelSearch.DATA_SOURCE)? (object)modelSearch.DATA_SOURCE : DBNull.Value),
                        new SqlParameter(@"LAST_UPDATE_DATE", modelSearch.LAST_UPDATE_DATE  != null? (object)modelSearch.LAST_UPDATE_DATE : DBNull.Value),
                        new SqlParameter(@"LAST_UPDATED_BY", modelSearch.LAST_UPDATED_BY != null? (object)modelSearch.LAST_UPDATED_BY : DBNull.Value),
                        new SqlParameter(@"CREATION_DATE", modelSearch.CREATION_DATE != null ? (object)modelSearch.CREATION_DATE : DBNull.Value),
                        new SqlParameter(@"CREATED_BY", modelSearch.CREATED_BY != null? (object)modelSearch.CREATED_BY : DBNull.Value),
                        new SqlParameter(@"page", modelSearch.currentPage),
                        new SqlParameter(@"OrderByClause", modelSearch.SortColumn),
                        new SqlParameter(@"pageSize", modelSearch.pageSize)
                    };
                result = DatabaseSql.ExecuteProcToList<FlexValueCategoryModel>("LT_FLEX_VALUE_CATEGORIES_SEARCH", paramList, Constants.ConectionStr_ITEMS_LIST).ToList();
            }
            catch (Exception ex)
            {
                log.Error("FlexValueCategoryDA - Lấy danh sách nhóm/loại theo trang lỗi: " + ex.Message);
                result = new List<FlexValueCategoryModel>();
            }

            return result;
        }

        public ObjectMessage Add(LT_FLEX_VALUE_CATEGORIES model)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var codeOutRole = "";
                var paramList = new List<SqlParameter>
                            {
                                new SqlParameter(@"FLEX_VALUE_SET_ID", model.FLEX_VALUE_SET_ID),
                                new SqlParameter(@"FLEX_VALUE_CATEGORY", !string.IsNullOrEmpty(model.FLEX_VALUE_CATEGORY)? (object)model.FLEX_VALUE_CATEGORY : DBNull.Value),
                                new SqlParameter(@"DESCRIPTION", !string.IsNullOrEmpty(model.DESCRIPTION)? (object)model.DESCRIPTION : DBNull.Value),
                                new SqlParameter(@"@CREATED_BY",model.LAST_UPDATED_BY),
                                new SqlParameter(@"PARENT_FLEX_VALUE_CATEGORY", model.PARENT_FLEX_VALUE_CATEGORY != null? (object)model.PARENT_FLEX_VALUE_CATEGORY : DBNull.Value ),
                                new SqlParameter(@"NOTE", !string.IsNullOrEmpty(model.NOTE)? (object)model.NOTE : DBNull.Value),
                                new SqlParameter(@"DATA_SOURCE", !string.IsNullOrEmpty(model.DATA_SOURCE)? (object)model.DATA_SOURCE : DBNull.Value),
                                new SqlParameter(@"ENABLED_FLAG", model.ENABLED_FLAG),
                                new SqlParameter(@"CODE_OUT", SqlDbType.VarChar,30) { Direction = ParameterDirection.Output },
                                new SqlParameter(@"MESSAGE_OUT", SqlDbType.VarChar,1000) { Direction = ParameterDirection.Output }
                            };
                var resultPro = DatabaseSql.ExecuteProcNonQuery("LT_FLEX_VALUE_CATEGORIES_CREATE", paramList, Constants.ConectionStr_ITEMS_LIST);

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
                log.Error("FlexValueCategoryDA - Thêm nhóm/loại (" + model.FLEX_VALUE_CATEGORY + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
            }
            return obj;
        }
        public ObjectMessage Edit(LT_FLEX_VALUE_CATEGORIES model)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var codeOutRole = "";
                var paramList = new List<SqlParameter>
                            {
                                new SqlParameter(@"FLEX_VALUE_CATEGORY_ID", model.FLEX_VALUE_CATEGORY_ID),
                                new SqlParameter(@"FLEX_VALUE_SET_ID", model.FLEX_VALUE_SET_ID),
                                new SqlParameter(@"FLEX_VALUE_CATEGORY", !string.IsNullOrEmpty(model.FLEX_VALUE_CATEGORY)? (object)model.FLEX_VALUE_CATEGORY : DBNull.Value),
                                new SqlParameter(@"DESCRIPTION", !string.IsNullOrEmpty(model.DESCRIPTION)? (object)model.DESCRIPTION : DBNull.Value),
                                new SqlParameter(@"ENABLED_FLAG", !string.IsNullOrEmpty(model.ENABLED_FLAG)? (object)model.ENABLED_FLAG : DBNull.Value),
                                new SqlParameter(@"LAST_UPDATED_BY",model.LAST_UPDATED_BY),
                                new SqlParameter(@"PARENT_FLEX_VALUE_CATEGORY", model.PARENT_FLEX_VALUE_CATEGORY != null? (object)model.PARENT_FLEX_VALUE_CATEGORY : DBNull.Value ),
                                new SqlParameter(@"NOTE", !string.IsNullOrEmpty(model.NOTE)? (object)model.NOTE : DBNull.Value),
                                new SqlParameter(@"DATA_SOURCE", !string.IsNullOrEmpty(model.DATA_SOURCE)? (object)model.DATA_SOURCE : DBNull.Value),
                                new SqlParameter(@"CODE_OUT", SqlDbType.VarChar,30) { Direction = ParameterDirection.Output },
                                new SqlParameter(@"MESSAGE_OUT", SqlDbType.VarChar,1000) { Direction = ParameterDirection.Output }
                            };
                var resultPro = DatabaseSql.ExecuteProcNonQuery("LT_FLEX_VALUE_CATEGORIES_UPDATE", paramList, Constants.ConectionStr_ITEMS_LIST);
                if (!string.IsNullOrEmpty(((SqlParameter)paramList[paramList.Count - 2]).Value.ToString()))
                    codeOutRole = ((SqlParameter)paramList[paramList.Count - 2]).Value.ToString();
                if (codeOutRole == "OK")
                {
                    obj.Error = false;
                    obj.Title = "Sửa nhóm/loại thành công!";
                }
                else
                {
                    obj.Error = true;
                    obj.Title = "Sửa nhóm/loại lỗi: " + ((SqlParameter)paramList[paramList.Count - 1]).Value.ToString();
                }

            }
            catch (Exception ex)
            {
                log.Error("FlexValueCategoryDA - Sửa nhóm/loại (" + model.FLEX_VALUE_CATEGORY + ") lỗi: " + ex.Message);
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
                        new SqlParameter(@"FLEX_VALUE_CATEGORY_ID", Id),
                        new SqlParameter(@"LAST_UPDATED_BY", updateBy),
                        new SqlParameter(@"CODE_OUT", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Output },
                        new SqlParameter(@"MESSAGE_OUT", SqlDbType.NVarChar, 1000) { Direction = ParameterDirection.Output }
                    };
                var result = DatabaseSql.ExecuteProcNonQuery("LT_FLEX_VALUE_CATEGORIES_DELETE", paramList, Constants.ConectionStr_ITEMS_LIST);
                if (!string.IsNullOrEmpty(((SqlParameter)paramList[paramList.Count - 2]).Value.ToString()))
                    outCode = ((SqlParameter)paramList[paramList.Count - 2]).Value.ToString();

                if (outCode == "OK")
                {
                    obj.Error = false;
                    obj.Title = "Xóa thành công!";
                }
                else
                {
                    log.Error("FlexValueCategoryDA - Xóa nhóm/loại (Id: " + Id + ", updateBy: " + updateBy + ") lỗi: " + ((SqlParameter)paramList[paramList.Count - 1]).Value.ToString());
                    obj.Error = true;
                    obj.Title = "Xóa không thành công!";
                }

                return obj;
            }
            catch (Exception ex)
            {
                log.Error("FlexValueCategoryDA - Xóa nhóm/loại (Id: " + Id + ", updateBy: " + updateBy + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
    }
}
