//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JWTWebApi.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class USER_LOCK
    {
        public int Id { get; set; }
        public int USER_LOCKED_ID { get; set; }
        public int USER_LOCK_BY { get; set; }
        public int UPDATED_BY { get; set; }
        public System.DateTime UPDATE_DATE { get; set; }
        public string ENABLED_FLAG { get; set; }
        public string CONTENT_LOCK { get; set; }
    }
}
