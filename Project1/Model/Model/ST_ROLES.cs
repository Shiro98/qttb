//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Model.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class ST_ROLES
    {
        public int ROLE_ID { get; set; }
        public string APP_CODE { get; set; }
        public string ROLE_DESC { get; set; }
        public string ROLE_TYPE { get; set; }
        public string SCOPE { get; set; }
        public string DOMAIN_CODE { get; set; }
        public Nullable<int> UNIT_ID { get; set; }
        public string ENABLED_FLAG { get; set; }
        public Nullable<System.DateTime> LAST_UPDATE_DATE { get; set; }
        public Nullable<int> LAST_UPDATED_BY { get; set; }
    }
}