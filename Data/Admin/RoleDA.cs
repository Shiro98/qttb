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
    public class RoleDA
    {
        ITEMS_SYSTEMEntities db = new ITEMS_SYSTEMEntities();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public List<ST_ROLES> GetAll()
        {
            return db.ST_ROLES.ToList();
        }

        public List<RolePageModel> GetAllByPage(ModelSearchUser modelSearch, ref int pageSize)
        {
            var result = new List<RolePageModel>();
            pageSize = 10;

            try
            {
                using (var context = new ITEMS_SYSTEMEntities())
                {
                    var paramList = new List<SqlParameter>
                    {
                        new SqlParameter(@"keyword", !string.IsNullOrEmpty(modelSearch.KeyWord)? (object)modelSearch.KeyWord : DBNull.Value),
                        new SqlParameter(@"page", modelSearch.currentPage),
                        new SqlParameter(@"OrderByClause", modelSearch.SortColumn),
                        new SqlParameter(@"pageSize", SqlDbType.Int) { Direction = ParameterDirection.Output }
                    };
                    result = context.Database.SqlQuery<RolePageModel>("exec [ST_ROLES_SEARCH] @keyword, @page, @OrderByClause, @pageSize OUT", paramList.ToArray()).ToList();
                    if (!string.IsNullOrEmpty(((SqlParameter)paramList[3]).Value.ToString()))
                        pageSize = Convert.ToInt32(((SqlParameter)paramList[3]).Value);

                }
            }
            catch (Exception ex)
            {
                log.Error("RoleDA - Lấy danh sách quyền theo trang lỗi: " + ex.Message);
                result = new List<RolePageModel>();
            }

            return result;
        }

        public ObjectMessage Add(ST_ROLES role, List<TreeModel> pageMenus)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var check = db.ST_ROLES.Where(x => x.APP_CODE == role.APP_CODE && x.ROLE_TYPE == role.ROLE_TYPE).ToList();
                if (check == null || check.Count == 0)
                {
                    using (ITEMS_SYSTEMEntities context = new ITEMS_SYSTEMEntities())
                    {
                        using (var dbContextTransaction = context.Database.BeginTransaction())
                        {
                            var codeOutRole = "";
                            var roleId = 0;
                            var paramList = new List<SqlParameter>
                            {
                                new SqlParameter(@"APP_CODE", !string.IsNullOrEmpty(role.APP_CODE)? (object)role.APP_CODE : DBNull.Value),
                                new SqlParameter(@"ROLE_DESC", !string.IsNullOrEmpty(role.ROLE_DESC)? (object)role.ROLE_DESC : DBNull.Value),
                                new SqlParameter(@"ROLE_TYPE", !string.IsNullOrEmpty(role.ROLE_TYPE)? (object)role.ROLE_TYPE : DBNull.Value),
                                new SqlParameter(@"SCOPE", !string.IsNullOrEmpty(role.SCOPE)? (object)role.SCOPE : DBNull.Value),
                                new SqlParameter(@"DOMAIN_CODE", !string.IsNullOrEmpty(role.DOMAIN_CODE)? (object)role.DOMAIN_CODE : DBNull.Value),
                                new SqlParameter(@"UNIT_ID", role.UNIT_ID != null? (object)role.UNIT_ID : DBNull.Value ),
                                new SqlParameter(@"ENABLED_FLAG", "Y"),
                                new SqlParameter(@"LAST_UPDATED_BY", role.LAST_UPDATED_BY),
                                new SqlParameter(@"CODE_OUT", SqlDbType.VarChar,30) { Direction = ParameterDirection.Output },
                                new SqlParameter(@"ROLE_ID", SqlDbType.Int) { Direction = ParameterDirection.Output }
                            };
                            var resultPro = context.Database.ExecuteSqlCommand("exec [ST_ROLES_CREATE] @APP_CODE, @ROLE_DESC, @ROLE_TYPE, @SCOPE, @DOMAIN_CODE, @UNIT_ID, @ENABLED_FLAG, @LAST_UPDATED_BY, @CODE_OUT OUT, @ROLE_ID OUT", paramList.ToArray());
                            if (!string.IsNullOrEmpty(((SqlParameter)paramList[paramList.Count - 2]).Value.ToString()))
                                codeOutRole = ((SqlParameter)paramList[paramList.Count - 2]).Value.ToString();
                            if (codeOutRole == "OK")
                            {
                                if (!string.IsNullOrEmpty(((SqlParameter)paramList[paramList.Count - 1]).Value.ToString()))
                                    roleId = Convert.ToInt32(((SqlParameter)paramList[paramList.Count - 1]).Value);

                                // Thêm quyền sử dụng page
                                if (roleId > 0)
                                {
                                    var success = true;
                                    var pageSelect = pageMenus.Where(x => x.@checked && x.id.IndexOf("/") <= 0).ToList();
                                    if (pageSelect != null && pageSelect.Count > 0)
                                    {
                                        for (int i = 0; i < pageSelect.Count; i++)
                                        {
                                            var pageID = pageSelect[i].pace_id;
                                            var actions = pageMenus.Where(x => x.@checked && x.pace_id == pageID && x.id.IndexOf("/") > 0).ToList();
                                            string action = "";
                                            if (actions != null && actions.Count > 0)
                                            {
                                                action = string.Join("|", actions.Select(x => x.name_control));
                                            }
                                            var codeOutRP = "";
                                            var paramListRP = new List<SqlParameter>
                                                {
                                                    new SqlParameter(@"APP_CODE", !string.IsNullOrEmpty(role.APP_CODE)? (object)role.APP_CODE : DBNull.Value),
                                                     new SqlParameter(@"ENABLED_FLAG", "Y"),
                                                     new SqlParameter(@"ROLE_ID", roleId ),
                                                     new SqlParameter(@"PAGE_ID", Convert.ToInt32(pageID) ),
                                                    new SqlParameter(@"CONTROL_STRING", !string.IsNullOrEmpty(action)? (object)action : DBNull.Value),
                                                    new SqlParameter(@"CODE_OUT", SqlDbType.VarChar,30) { Direction = ParameterDirection.Output }
                                                };
                                            var resultRP = context.Database.ExecuteSqlCommand("exec [ST_ROLE_PAGES_CREATE] @APP_CODE, @ENABLED_FLAG, @ROLE_ID, @PAGE_ID, @CONTROL_STRING, @CODE_OUT OUT", paramListRP.ToArray());
                                            if (!string.IsNullOrEmpty(((SqlParameter)paramListRP[paramListRP.Count - 1]).Value.ToString()))
                                                codeOutRP = ((SqlParameter)paramListRP[paramListRP.Count - 1]).Value.ToString();

                                            if (codeOutRP != "OK")
                                            {
                                                success = false;
                                                break;
                                            }
                                        }
                                    }
                                    if (success)
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
                                else
                                {
                                    obj.Error = true;
                                    obj.Title = "Thêm mới lỗi!";
                                    dbContextTransaction.Rollback();
                                }
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
                    obj.Title = "Mã quyền đã tồn tại!";
                }

                return obj;
            }
            catch (Exception ex)
            {
                log.Error("RoleDA - Thêm quyền (" + role.ROLE_TYPE + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
        public ObjectMessage Edit(ST_ROLES role, List<TreeModel> pageMenus)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                using (ITEMS_SYSTEMEntities context = new ITEMS_SYSTEMEntities())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        var codeOutRole = "";
                        var paramList = new List<SqlParameter>
                            {
                                new SqlParameter(@"ROLE_ID", role.ROLE_ID),
                                new SqlParameter(@"APP_CODE", !string.IsNullOrEmpty(role.APP_CODE)? (object)role.APP_CODE : DBNull.Value),
                                new SqlParameter(@"ROLE_DESC", !string.IsNullOrEmpty(role.ROLE_DESC)? (object)role.ROLE_DESC : DBNull.Value),
                                new SqlParameter(@"ROLE_TYPE", !string.IsNullOrEmpty(role.ROLE_TYPE)? (object)role.ROLE_TYPE : DBNull.Value),
                                new SqlParameter(@"SCOPE", !string.IsNullOrEmpty(role.SCOPE)? (object)role.SCOPE : DBNull.Value),
                                new SqlParameter(@"DOMAIN_CODE", !string.IsNullOrEmpty(role.DOMAIN_CODE)? (object)role.DOMAIN_CODE : DBNull.Value),
                                new SqlParameter(@"UNIT_ID", role.UNIT_ID != null? (object)role.UNIT_ID : DBNull.Value ),
                                new SqlParameter(@"LAST_UPDATED_BY", role.LAST_UPDATED_BY),
                                new SqlParameter(@"CODE_OUT", SqlDbType.VarChar,30) { Direction = ParameterDirection.Output }
                            };
                        var resultPro = context.Database.ExecuteSqlCommand("exec [ST_ROLES_UPDATE] @ROLE_ID, @APP_CODE, @ROLE_DESC, @ROLE_TYPE, @SCOPE, @DOMAIN_CODE, @UNIT_ID, @LAST_UPDATED_BY, @CODE_OUT OUT", paramList.ToArray());
                        if (!string.IsNullOrEmpty(((SqlParameter)paramList[paramList.Count - 1]).Value.ToString()))
                            codeOutRole = ((SqlParameter)paramList[paramList.Count - 1]).Value.ToString();
                        if (codeOutRole == "OK")
                        {
                            var success = true;
                            var pageSelect = pageMenus.Where(x => x.@checked && x.id.IndexOf("/") <= 0).ToList();
                            if (pageSelect != null && pageSelect.Count > 0)
                            {
                                for (int i = 0; i < pageSelect.Count; i++)
                                {
                                    var pageID = pageSelect[i].pace_id;
                                    var actions = pageMenus.Where(x => x.@checked && x.pace_id == pageID && x.id.IndexOf("/") > 0).ToList();
                                    string action = "";
                                    if (actions != null && actions.Count > 0)
                                    {
                                        action = string.Join("|", actions.Select(x => x.name_control));
                                    }
                                    var codeOutRP = "";
                                    var paramListRP = new List<SqlParameter>
                                                {
                                                    new SqlParameter(@"APP_CODE", !string.IsNullOrEmpty(role.APP_CODE)? (object)role.APP_CODE : DBNull.Value),
                                                     new SqlParameter(@"ENABLED_FLAG", "Y"),
                                                     new SqlParameter(@"ROLE_ID", role.ROLE_ID ),
                                                     new SqlParameter(@"PAGE_ID", Convert.ToInt32(pageID) ),
                                                    new SqlParameter(@"CONTROL_STRING", !string.IsNullOrEmpty(action)? (object)action : DBNull.Value),
                                                    new SqlParameter(@"CODE_OUT", SqlDbType.VarChar,30) { Direction = ParameterDirection.Output }
                                                };
                                    var resultRP = context.Database.ExecuteSqlCommand("exec [ST_ROLE_PAGES_CREATE] @APP_CODE, @ENABLED_FLAG, @ROLE_ID, @PAGE_ID, @CONTROL_STRING, @CODE_OUT OUT", paramListRP.ToArray());
                                    if (!string.IsNullOrEmpty(((SqlParameter)paramListRP[paramListRP.Count - 1]).Value.ToString()))
                                        codeOutRP = ((SqlParameter)paramListRP[paramListRP.Count - 1]).Value.ToString();

                                    if (codeOutRP != "OK")
                                    {
                                        success = false;
                                        break;
                                    }
                                }
                            }
                            if (success)
                            {
                                obj.Error = false;
                                obj.Title = "Sửa quyền thành công!";
                                dbContextTransaction.Commit();
                            }
                            else
                            {
                                obj.Error = true;
                                obj.Title = "Sửa quyền lỗi!";
                                dbContextTransaction.Rollback();
                            }
                        }
                        else
                        {
                            obj.Error = true;
                            obj.Title = "Sửa quyền lỗi!";
                            dbContextTransaction.Rollback();
                        }
                    }
                }


                return obj;
            }
            catch (Exception ex)
            {
                log.Error("RoleDA - Sửa quyền (" + role.ROLE_TYPE + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
        public ObjectMessage Delete(int Id, int updateBy)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                using (var context = new ITEMS_SYSTEMEntities())
                {
                    var outCode = "";
                    var paramList = new List<SqlParameter>
                    {
                        new SqlParameter(@"ROLE_ID", Id),
                        new SqlParameter(@"UPDATED_BY", updateBy),
                        new SqlParameter(@"CODE_OUT", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Output }
                    };
                    var result = context.Database.ExecuteSqlCommand("exec [ST_ROLES_DELETE] @ROLE_ID, @UPDATED_BY, @CODE_OUT OUT", paramList.ToArray());
                    if (!string.IsNullOrEmpty(((SqlParameter)paramList[paramList.Count -1]).Value.ToString()))
                        outCode = ((SqlParameter)paramList[paramList.Count - 1]).Value.ToString();

                    if(outCode == "OK")
                    {
                        obj.Error = false;
                        obj.Title = "Xóa thành công!";
                    }
                    else
                    {
                        obj.Error = true;
                        obj.Title = "Xóa không thành công!";
                    }
                }
               
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("RoleDA - Xóa quyền (Id: " + Id + ", updateBy: " + updateBy + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
    }
}
