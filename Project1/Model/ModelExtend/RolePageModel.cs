﻿using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend
{
    public class RolePageModel:ST_ROLES
    {
        public int TotalRow { get; set; }
        public string UNIT_NAME { get; set; }
    }
}
