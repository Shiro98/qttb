using log4net;
using Model.Model;
using Model.ModelExtend;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Data.Admin
{
    public class PageMenuDA
    {
        ITEMS_SYSTEMEntities db = new ITEMS_SYSTEMEntities();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ST_PAGES_MENU GetItemByName(string name)
        {
            return db.ST_PAGES_MENU.FirstOrDefault(x => x.PAGE_NAME == name);
        }

        public List<ST_PAGES_MENU> GetAllByPage(int page)
        {
            var pageSize = 10;
            var param = db.ST_PARAMETERS.FirstOrDefault(x => x.PARAM_CODE == "PageSize");
            if (param != null)
                pageSize = Convert.ToInt32(param.PARAM_VALUE);
            return db.ST_PAGES_MENU.Where(x => x.ENABLED_FLAG == "Y").Skip(pageSize * (page - 1)).Take(pageSize).ToList();
        }

        public List<ST_PAGES_MENU> GetAll()
        {
            return db.ST_PAGES_MENU.Where(x => x.ENABLED_FLAG == "Y").ToList();
        }

        public List<MenuModel> GetMenuAdmin()
        {
            var result = new List<MenuModel>();
            try
            {
                var pageMenus = db.ST_PAGES_MENU.Where(x => x.ENABLED_FLAG == "Y").ToList();
                if (pageMenus != null && pageMenus.Count > 0)
                {
                    for (int i = 0; i < pageMenus.Count; i++)
                    {
                        var model = new MenuModel
                        {
                            PAGE_ID = pageMenus[i].PAGE_ID,
                            APP_CODE = pageMenus[i].APP_CODE,
                            PAGE_NAME = pageMenus[i].PAGE_NAME,
                            PAGE_DESC = pageMenus[i].PAGE_DESC,
                            FILE_NAME = pageMenus[i].FILE_NAME,
                            ORDER_LEVEL = pageMenus[i].ORDER_LEVEL,
                            MODULE_CODE = pageMenus[i].MODULE_CODE,
                            CSS_CLASS = pageMenus[i].CSS_CLASS,
                            HREF_URL = pageMenus[i].HREF_URL,
                            PERMISSION_REQUIRE = pageMenus[i].PERMISSION_REQUIRE,
                            ENABLED_FLAG = pageMenus[i].ENABLED_FLAG,
                            LAST_UPDATE_DATE = pageMenus[i].LAST_UPDATE_DATE,
                            LAST_UPDATED_BY = pageMenus[i].LAST_UPDATED_BY,
                            Actions = GetAllByPageMenu(pageMenus[i].PAGE_ID)
                        };
                        result.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("PageMenuDA - Lấy danh sách menu cho tài khoản Admin lỗi: " + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Lấy danh sách action trên 1 page menu
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public string GetAllByPageMenu(int page)
        {
            var result = "";
            try
            {
                var actions = db.ST_PAGE_FUNCTIONS.Where(x => x.PAGE_ID == page).Select(x => x.CONTROL_NAME).ToArray();
                if (actions != null && actions.Length > 0)
                {
                    result = string.Join("|", actions);
                }
            }
            catch (Exception ex)
            {
                log.Error("PageMenuDA - Lấy danh sách action trên 1 page menu("+page+") lỗi: " + ex.Message);
                result = "";
            }
            return result;
        }
        /// <summary>
        /// Lấy danh sách menu theo user ID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<MenuModel> GetMenuByUser(int userID, string appCode)
        {
            var result = new List<MenuModel>();
            try
            {
                // lấy thông tin user
                var user = db.ST_USERS.FirstOrDefault(x => x.USER_ID == userID);

                // Lấy danh sách menu
                using (var context = new ITEMS_SYSTEMEntities())
                {
                    var paramList = new List<SqlParameter>
                    {
                        new SqlParameter(@"appCode", !string.IsNullOrEmpty(appCode)? (object)appCode : DBNull.Value),
                        new SqlParameter(@"userId", userID),
                        new SqlParameter(@"unitId", user.UNIT_ID)
                    };
                    result = context.Database.SqlQuery<MenuModel>("exec [ST_PAGES_MENU_GET_MENU_BY_USER] @appCode, @userId, @unitId", paramList.ToArray()).ToList();

                }
            }
            catch (Exception ex)
            {
                log.Error("PageMenuDA - Lấy danh sách menu theo user ID(UserId:" + userID + ", App_Code: "+appCode+") lỗi: " + ex.Message);
                result = new List<MenuModel>();
            }
            return result;
        }

        public ObjectMessage Add(ST_PAGES_MENU pageMenu)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                db.ST_PAGES_MENU.Add(pageMenu);
                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Thêm mới thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("PageMenuDA - Thêm mới menu(FILE_NAME: " + pageMenu.FILE_NAME + ", HREF_URL: " + pageMenu.HREF_URL+ ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
        public ObjectMessage Edit(ST_PAGES_MENU pageMenu)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.ST_PAGES_MENU.FirstOrDefault(x => x.PAGE_ID == pageMenu.PAGE_ID);
                data.APP_CODE = pageMenu.APP_CODE;
                data.PAGE_NAME = pageMenu.PAGE_NAME;
                data.PAGE_DESC = pageMenu.PAGE_DESC;
                data.FILE_NAME = pageMenu.FILE_NAME;
                data.ORDER_LEVEL = pageMenu.ORDER_LEVEL;
                data.MODULE_CODE = pageMenu.MODULE_CODE;
                data.CSS_CLASS = pageMenu.CSS_CLASS;
                data.HREF_URL = pageMenu.HREF_URL;
                data.PERMISSION_REQUIRE = pageMenu.PERMISSION_REQUIRE;
                data.LAST_UPDATE_DATE = pageMenu.LAST_UPDATE_DATE;
                data.LAST_UPDATED_BY = pageMenu.LAST_UPDATED_BY;

                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Cập nhật thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("PageMenuDA - Sửa menu(FILE_NAME: " + pageMenu.FILE_NAME + ", HREF_URL: " + pageMenu.HREF_URL + ") lỗi: " + ex.Message);
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
                var data = db.ST_PAGES_MENU.FirstOrDefault(x => x.PAGE_ID == Id);
                data.ENABLED_FLAG = "N";
                data.LAST_UPDATE_DATE = DateTime.Now;
                data.LAST_UPDATED_BY = updateBy;

                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Xóa thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("PageMenuDA - Xóa menu(Id: " + Id + ", updateBy: " + updateBy + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
    }
}
