using Common;
using log4net;
using Model.Model;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Admin
{
    public class PageActionDA
    {
        ITEMS_SYSTEMEntities db = new ITEMS_SYSTEMEntities();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ST_PAGE_FUNCTIONS GetItemByPage(int pageId)
        {
            return db.ST_PAGE_FUNCTIONS.FirstOrDefault(x => x.PAGE_ID == pageId);
        }

        public List<ST_PAGE_FUNCTIONS> GetAllByPage(int page)
        {
            var pageSize = 10;
            var param = db.ST_PARAMETERS.FirstOrDefault(x => x.PARAM_CODE == "PageSize");
            if (param != null)
                pageSize = Convert.ToInt32(param.PARAM_VALUE);
            return db.ST_PAGE_FUNCTIONS.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
        }

        /// <summary>
        /// Lấy danh sách action trên 1 page menu
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<ST_PAGE_FUNCTIONS> GetAllByPageMenu(int page)
        {
            return db.ST_PAGE_FUNCTIONS.Where(x=>x.PAGE_ID == page).ToList();
        }

        public List<ST_PAGE_FUNCTIONS> GetAll()
        {
            return db.ST_PAGE_FUNCTIONS.ToList();
        }

        public ObjectMessage Add(ST_PAGE_FUNCTIONS pageAction)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                db.ST_PAGE_FUNCTIONS.Add(pageAction);
                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Thêm mới thành công!";
                return obj;
            }catch(Exception ex)
            {
                log.Error("PageActionDA - Thêm action cho 1 trang(" + pageAction.CONTROL_NAME + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
        public ObjectMessage Edit(ST_PAGE_FUNCTIONS pageAction)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.ST_PAGE_FUNCTIONS.FirstOrDefault(x => x.PAGE_ID == pageAction.PAGE_ID);
                data.CONTROL_NAME = pageAction.CONTROL_NAME;
                data.CONTROL_DESC = pageAction.CONTROL_DESC;

                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Cập nhật thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("PageActionDA - Sửa action cho 1 trang(" + pageAction.CONTROL_NAME + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
        public ObjectMessage Delete(int Id)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var itemDelete = db.ST_PAGE_FUNCTIONS.Find(Id);
                db.ST_PAGE_FUNCTIONS.Remove(itemDelete);
                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Xóa thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("PageActionDA - Xóa action cho 1 trang(" + Id + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
    }
}
