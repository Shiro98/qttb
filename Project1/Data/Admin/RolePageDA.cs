using Common;

using Model.Model;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Admin
{
    public class RolePageDA
    {
        ITEMS_SYSTEMEntities db = new ITEMS_SYSTEMEntities();

        public List<ST_ROLE_PAGES> GetAllByRole(int roleID)
        {
            return db.ST_ROLE_PAGES.Where(x=>x.ROLE_ID == roleID && x.ENABLED_FLAG == "Y").ToList();
        }

        public ObjectMessage Add(ST_ROLE_PAGES rolePage)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                rolePage.ENABLED_FLAG = "Y";
                db.ST_ROLE_PAGES.Add(rolePage);

                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Thêm mới thành công!";
                return obj;
            }catch(Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
        public ObjectMessage Edit(ST_ROLE_PAGES rolePage)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.ST_ROLE_PAGES.FirstOrDefault(x => x.ROLE_ID == rolePage.ROLE_ID 
                && rolePage.PAGE_ID == x.PAGE_ID && x.APP_CODE == rolePage.APP_CODE && x.ENABLED_FLAG == "Y");
                data.CONTROL_STRING = rolePage.CONTROL_STRING;

                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Cập nhật thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
        public ObjectMessage Delete(int roleId, int pageId, string appCode)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.ST_ROLE_PAGES.FirstOrDefault(x => x.ROLE_ID == roleId && pageId == x.PAGE_ID && x.APP_CODE == appCode);
                data.ENABLED_FLAG = "N";
                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Xóa thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
    }
}
