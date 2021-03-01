using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend
{
    public class UserPageModel : ST_USERS
    {
        public string UNIT_NAME { get; set; }
        public string ROLE_DESC { get; set; }
        public int TotalRow { get; set; }
        public int? IsLock { get; set; }
    }
}
