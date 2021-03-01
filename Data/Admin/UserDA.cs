using Common;
using log4net;
using Model.Model;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq; 

namespace Data.Admin
{
    public class UserDA
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        ITEMS_SYSTEMEntities db = new ITEMS_SYSTEMEntities();
        public int Login(string userName, string password)
        {
            var result = db.ST_USERS.FirstOrDefault(x => x.LOGIN_NAME == userName);
            if (result == null)
                return 0;
            else
            {
                if (result.ENABLED_FLAG == "N")
                    return -1;
                else
                {
                    if (result.PASSWORD == password)
                        return 1;
                    else
                        return -2;
                }
            }
        }
        public ST_USERS GetItemByUserName(string userName)
        {
            return db.ST_USERS.FirstOrDefault(x => x.LOGIN_NAME == userName);
        }
        public List<string> GetListCredentials(string userName)
        {
            var query = from pm in db.ST_PAGES_MENU
                        join rp in db.ST_ROLE_PAGES on pm.PAGE_ID equals rp.PAGE_ID
                        join ur in db.ST_USER_ROLES on rp.ROLE_ID equals ur.ROLE_ID
                        join u in db.ST_USERS on ur.USER_ID equals u.USER_ID
                        where u.LOGIN_NAME == userName && !string.IsNullOrEmpty(pm.PAGE_NAME)
                        select new
                        {
                            ID = pm.PAGE_NAME
                        };

            return query.Select(x => x.ID).ToList();
        }

        
        public ObjectMessage Add(ST_USERS user, string roleIds, string Appcode)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var check = db.ST_USERS.Where(x => x.LOGIN_NAME.ToUpper() == user.LOGIN_NAME.ToUpper()).ToList();
                if (check == null || check.Count == 0)
                {
                    using (ITEMS_SYSTEMEntities context = new ITEMS_SYSTEMEntities())
                    {
                        using (var dbContextTransaction = context.Database.BeginTransaction())
                        {
                            user.PASSWORD = Encryptor.MD5Hash(user.LOGIN_NAME.ToUpper() + "123456");

                            var codeOutRole = "";
                            var paramList = new List<SqlParameter>
                            {
                                new SqlParameter(@"LOGIN_NAME", user.LOGIN_NAME),
                                new SqlParameter(@"PASSWORD", !string.IsNullOrEmpty(user.PASSWORD)? (object)user.PASSWORD : DBNull.Value),
                                new SqlParameter(@"FULL_NAME", !string.IsNullOrEmpty(user.FULL_NAME)? (object)user.FULL_NAME : DBNull.Value),
                                new SqlParameter(@"UNIT_ID", user.UNIT_ID != null? (object)user.UNIT_ID : DBNull.Value ),
                                new SqlParameter(@"START_DATE", !string.IsNullOrEmpty(user.START_DATE)? (object)user.START_DATE : DBNull.Value),
                                new SqlParameter(@"END_DATE", !string.IsNullOrEmpty(user.END_DATE)? (object)user.END_DATE : DBNull.Value),
                                new SqlParameter(@"USER_GROUP", !string.IsNullOrEmpty(user.USER_GROUP)? (object)user.USER_GROUP : DBNull.Value),
                                new SqlParameter(@"USER_CLASS", !string.IsNullOrEmpty(user.USER_CLASS)? (object)user.USER_CLASS : DBNull.Value),
                                new SqlParameter(@"USER_DESC", !string.IsNullOrEmpty(user.USER_DESC)? (object)user.USER_DESC : DBNull.Value),
                                new SqlParameter(@"ENABLED_FLAG", "Y"),
                                new SqlParameter(@"TEL_NO", !string.IsNullOrEmpty(user.TEL_NO)? (object)user.TEL_NO : DBNull.Value),
                                new SqlParameter(@"IMG_PATH", !string.IsNullOrEmpty(user.IMG_PATH)? (object)user.IMG_PATH : DBNull.Value),
                                new SqlParameter(@"UPDATED_BY", user.UPDATED_BY),
                                new SqlParameter(@"SHORT_NAME", Common.Common.LocDau(user.FULL_NAME) ),
                                new SqlParameter(@"TITLE_ID", user.TITLE_ID != null? (object)user.TITLE_ID : DBNull.Value),
                                new SqlParameter(@"LEVEL_ID", user.LEVEL_ID != null? (object)user.LEVEL_ID : DBNull.Value),
                                new SqlParameter(@"ROLE_IDS", !string.IsNullOrEmpty(roleIds)? (object)roleIds : DBNull.Value),
                                new SqlParameter(@"APP_CODE", !string.IsNullOrEmpty(Appcode)? (object)Appcode : DBNull.Value),
                                new SqlParameter(@"CODE_OUT", SqlDbType.VarChar,30) { Direction = ParameterDirection.Output }
                            };
                            var resultPro = context.Database.ExecuteSqlCommand("exec [ST_USERS_CREATE] @LOGIN_NAME,@PASSWORD,@FULL_NAME,@UNIT_ID,@START_DATE," +
                                "@END_DATE,@USER_GROUP,@USER_CLASS,@USER_DESC,@ENABLED_FLAG,@TEL_NO,@IMG_PATH,@UPDATED_BY,@SHORT_NAME,@TITLE_ID,@LEVEL_ID,@ROLE_IDS,@APP_CODE, @CODE_OUT OUT", paramList.ToArray());
                            if (!string.IsNullOrEmpty(((SqlParameter)paramList[paramList.Count - 1]).Value.ToString()))
                                codeOutRole = ((SqlParameter)paramList[paramList.Count - 1]).Value.ToString();

                            if (codeOutRole == "OK")
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
                    obj.Title = "Tên đăng nhập đã tồn tại";
                }

                return obj;
            }
            catch (Exception ex)
            {
                log.Error("UserDA - Tạo tài khoản(" + user.LOGIN_NAME + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
        public ObjectMessage Edit(ST_USERS user, string roleIds, string Appcode)
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
                                new SqlParameter(@"USER_ID", user.USER_ID),
                                new SqlParameter(@"LOGIN_NAME", user.LOGIN_NAME),
                                new SqlParameter(@"FULL_NAME", !string.IsNullOrEmpty(user.FULL_NAME)? (object)user.FULL_NAME : DBNull.Value),
                                new SqlParameter(@"UNIT_ID", user.UNIT_ID != null? (object)user.UNIT_ID : DBNull.Value ),
                                new SqlParameter(@"START_DATE", !string.IsNullOrEmpty(user.START_DATE)? (object)user.START_DATE : DBNull.Value),
                                new SqlParameter(@"END_DATE", !string.IsNullOrEmpty(user.END_DATE)? (object)user.END_DATE : DBNull.Value),
                                new SqlParameter(@"USER_GROUP", !string.IsNullOrEmpty(user.USER_GROUP)? (object)user.USER_GROUP : DBNull.Value),
                                new SqlParameter(@"USER_CLASS", !string.IsNullOrEmpty(user.USER_CLASS)? (object)user.USER_CLASS : DBNull.Value),
                                new SqlParameter(@"USER_DESC", !string.IsNullOrEmpty(user.USER_DESC)? (object)user.USER_DESC : DBNull.Value),
                                new SqlParameter(@"ENABLED_FLAG", "Y"),
                                new SqlParameter(@"TEL_NO", !string.IsNullOrEmpty(user.TEL_NO)? (object)user.TEL_NO : DBNull.Value),
                                new SqlParameter(@"IMG_PATH", !string.IsNullOrEmpty(user.IMG_PATH)? (object)user.IMG_PATH : DBNull.Value),
                                new SqlParameter(@"UPDATED_BY", user.UPDATED_BY),
                                new SqlParameter(@"SHORT_NAME", Common.Common.LocDau(user.FULL_NAME) ),
                                new SqlParameter(@"TITLE_ID", user.TITLE_ID != null? (object)user.TITLE_ID : DBNull.Value),
                                new SqlParameter(@"LEVEL_ID", user.LEVEL_ID != null? (object)user.LEVEL_ID : DBNull.Value),
                                new SqlParameter(@"ROLE_IDS", !string.IsNullOrEmpty(roleIds)? (object)roleIds : DBNull.Value),
                                new SqlParameter(@"APP_CODE", !string.IsNullOrEmpty(Appcode)? (object)Appcode : DBNull.Value),
                                new SqlParameter(@"CODE_OUT", SqlDbType.VarChar,30) { Direction = ParameterDirection.Output }
                            };
                        var resultPro = context.Database.ExecuteSqlCommand("exec [ST_USERS_UPDATE] @USER_ID, @LOGIN_NAME,@FULL_NAME,@UNIT_ID,@START_DATE," +
                            "@END_DATE,@USER_GROUP,@USER_CLASS,@USER_DESC,@ENABLED_FLAG,@TEL_NO,@IMG_PATH,@UPDATED_BY,@SHORT_NAME,@TITLE_ID,@LEVEL_ID,@ROLE_IDS,@APP_CODE, @CODE_OUT OUT", paramList.ToArray());
                        if (!string.IsNullOrEmpty(((SqlParameter)paramList[paramList.Count - 1]).Value.ToString()))
                            codeOutRole = ((SqlParameter)paramList[paramList.Count - 1]).Value.ToString();

                        if (codeOutRole == "OK")
                        {
                            obj.Error = false;
                            obj.Title = "Cập nhật thành công!";
                            dbContextTransaction.Commit();
                        }
                        else
                        {
                            obj.Error = true;
                            obj.Title = "Cập nhật lỗi!";
                            dbContextTransaction.Rollback();
                        }
                    }
                }
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("UserDA - Sửa tài khoản(" + user.LOGIN_NAME + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }

        /// <summary>
        /// thay đổi mật khẩu
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="LOGIN_NAME"></param>
        /// <param name="passwordOd"></param>
        /// <param name="passwordNew"></param>
        /// <returns></returns>
        public ObjectMessage ChangePassword(long userId, string LOGIN_NAME, string passwordOd, string passwordNew)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {

                string passwordNewMD5 = Encryptor.MD5Hash(LOGIN_NAME + passwordNew);
                string passwordOdMD5 = Encryptor.MD5Hash(LOGIN_NAME + passwordOd);
                using (var context = new ITEMS_SYSTEMEntities())
                {
                    var outCode = "";
                    var paramList = new List<SqlParameter>
                    {
                        new SqlParameter(@"USER_ID", userId),
                        new SqlParameter(@"PASSWORD_OLD", passwordOdMD5),
                        new SqlParameter(@"PASSWORD", passwordNewMD5),
                        new SqlParameter(@"CODE_OUT", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Output }
                    };
                    var result = context.Database.ExecuteSqlCommand("exec [ST_USERS_CHANGE_PASSWORD] @USER_ID, @PASSWORD_OLD, @PASSWORD, @CODE_OUT OUT", paramList.ToArray());
                    if (!string.IsNullOrEmpty(((SqlParameter)paramList[paramList.Count - 1]).Value.ToString()))
                        outCode = ((SqlParameter)paramList[paramList.Count - 1]).Value.ToString();

                    if (outCode == "OK")
                    {
                        obj.Error = false;
                        obj.Title = "Đổi mật khẩu thành công!";
                    }
                    else
                    {
                        if (outCode == "SAI_PAS_OLD")
                        {
                            obj.Error = false;
                            obj.Title = "Bạn nhập mật khẩu cũ không đúng!";
                        }
                        else
                        {
                            obj.Error = true;
                            obj.Title = "Đổi mật khẩu không thành công!";
                        }

                    }
                }
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("UserDA - Thay mật khẩu tài khoản(" + userId + ") lỗi: " + ex.Message);
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
                        new SqlParameter(@"USER_ID", Id),
                        new SqlParameter(@"UPDATED_BY", updateBy),
                        new SqlParameter(@"CODE_OUT", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Output }
                    };
                    var result = context.Database.ExecuteSqlCommand("exec [ST_USERS_DELETE] @USER_ID, @UPDATED_BY, @CODE_OUT OUT", paramList.ToArray());
                    if (!string.IsNullOrEmpty(((SqlParameter)paramList[paramList.Count - 1]).Value.ToString()))
                        outCode = ((SqlParameter)paramList[paramList.Count - 1]).Value.ToString();

                    if (outCode == "OK")
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
                log.Error("UserDA - Xóa tài khoản(Id: " + Id + ", updateBy: " + updateBy + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }

        /// <summary>
        /// Kiểm tra xem user có bị khóa không
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool CheckLock(int userId)
        {
            var result = false;

            using (var context = new ITEMS_SYSTEMEntities())
            {
                var paramList = new List<SqlParameter>
                    {
                        new SqlParameter(@"userId", userId)
                    };
                var resultPro = context.Database.SqlQuery<USER_LOCK>("exec [ST_USERS_CHECK_LOCK] @userId", paramList.ToArray()).ToList();
                if (resultPro != null && resultPro.Count > 0)
                    result = true;
                else
                    result = false;
            }

            return result;
        }

        /// <summary>
        /// Mở khóa tài khoản
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ObjectMessage UnLockUser(int userId, int updateBy)
        {
            ObjectMessage result = new ObjectMessage();

            using (ITEMS_SYSTEMEntities context = new ITEMS_SYSTEMEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    var codeOutRole = "";
                    var paramList = new List<SqlParameter>
                            {
                                new SqlParameter(@"USER_LOCKED_ID", userId),
                                new SqlParameter(@"UPDATED_BY", updateBy),
                                new SqlParameter(@"CODE_OUT", SqlDbType.VarChar,30) { Direction = ParameterDirection.Output }
                            };
                    var resultPro = context.Database.ExecuteSqlCommand("exec [ST_USERS_UNLOCK] @USER_LOCKED_ID,@UPDATED_BY, @CODE_OUT OUT", paramList.ToArray());
                    if (!string.IsNullOrEmpty(((SqlParameter)paramList[paramList.Count - 1]).Value.ToString()))
                        codeOutRole = ((SqlParameter)paramList[paramList.Count - 1]).Value.ToString();

                    if (codeOutRole == "OK")
                    {
                        result.Error = false;
                        result.Title = "Mở khóa thành công!";
                        dbContextTransaction.Commit();
                    }
                    else
                    {
                        result.Error = true;
                        result.Title = "Mở khóa lỗi!";
                        dbContextTransaction.Rollback();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Khóa tài khoản
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ObjectMessage LockUser(int userId, int updateBy, string content)
        {
            ObjectMessage result = new ObjectMessage();

            using (ITEMS_SYSTEMEntities context = new ITEMS_SYSTEMEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    var codeOutRole = "";
                    var paramList = new List<SqlParameter>
                            {
                                new SqlParameter(@"USER_LOCKED_ID", userId),
                                new SqlParameter(@"USER_LOCK_BY", updateBy),
                                new SqlParameter(@"UPDATED_BY", userId),
                                new SqlParameter(@"CONTENT_LOCK", updateBy),
                                new SqlParameter(@"CODE_OUT", SqlDbType.VarChar,30) { Direction = ParameterDirection.Output }
                            };
                    var resultPro = context.Database.ExecuteSqlCommand("exec [ST_USERS_CREATE_LOCK] @USER_LOCKED_ID, @USER_LOCK_BY , @UPDATED_BY , @CONTENT_LOCK, @CODE_OUT OUT", paramList.ToArray());
                    if (!string.IsNullOrEmpty(((SqlParameter)paramList[paramList.Count - 1]).Value.ToString()))
                        codeOutRole = ((SqlParameter)paramList[paramList.Count - 1]).Value.ToString();

                    if (codeOutRole == "OK")
                    {
                        result.Error = false;
                        result.Title = "Khóa thành công!";
                        dbContextTransaction.Commit();
                    }
                    else
                    {
                        result.Error = true;
                        result.Title = "Khóa lỗi!";
                        dbContextTransaction.Rollback();
                    }
                }
            }

            return result;
        }
    }
}
