using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend
{
    public class ModelSearchFlexValueCategory : LT_FLEX_VALUE_CATEGORIES
    {
        public int currentPage { get; set; }
        public int pageSize { get; set; }
        public string SortColumn { get; set; }
        public string AppCode { get; set; }
    }
}
