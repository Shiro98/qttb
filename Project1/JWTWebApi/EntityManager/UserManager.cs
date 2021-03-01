using System.Linq; 
using JWTWebApi.Entities;


namespace JWTWebApi.EntityManager
{
    public class UserManager
    {
        public ST_USERS GetInfoByUserName(string userName)
        {
            var user = new ST_USERS();
            using (var cs = new ITEMS_SYSTEMEntities1())
            {
                user = (from b in cs.ST_USERS where b.LOGIN_NAME == userName select b).FirstOrDefault();
            }
            return user;
        }
        public bool CheckLock(int userId)
        {
            var flag = false;
            try
            {
                using (var cs = new ITEMS_SYSTEMEntities1())
                {
                   var user = (from b in cs.USER_LOCK where b.USER_LOCKED_ID == userId && b.ENABLED_FLAG == "Y" select b).FirstOrDefault();
                    if (user != null)
                        flag = true;
                    else
                        return flag;
                }
            }
            catch { return true; }
            return flag;
        }
    }
}