using JWTWebApi.Entities;
using System.Collections.Generic;
using System.Linq;
using JWTWebApi.Models;

namespace JWTWebApi.EntityManager
{
    public class AppManager
    {
        public List<AppModel> GetAllApp()
        {
            var lstApp = new List<AppModel>();
            using (var cs = new ITEMS_SYSTEMEntities1())
            {
                lstApp = (from b in cs.APPs 
                          select new AppModel
                          {
                              Id = b.Id,
                              AppCode = b.AppCode,
                              AppName = b.AppName,
                              Url = b.Url
                          }).ToList();
            }
            return lstApp;
        }
    }
}