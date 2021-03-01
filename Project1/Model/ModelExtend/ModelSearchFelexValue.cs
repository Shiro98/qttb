using Model.Model;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend
{
    public class ModelSearchFelexValue : LT_FLEX_VALUES
    {
        public int currentPage { get; set; }
        public int pageSize { get; set; }
        public string SortColumn { get; set; }
        public string AppCode { get; set; }

    }
}
