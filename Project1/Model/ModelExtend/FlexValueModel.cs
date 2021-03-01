using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend
{
    public class FlexValueModel : LT_FLEX_VALUES
    {
        public int TotalRow { get; set; }
        public string FLEX_VALUE_CATEGORY_DEC { get; set; }
        public string FLEX_VALUE_SET_NAME { get; set; }
    }
}
