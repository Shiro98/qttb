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
    
    public partial class DM_CAP_BAC_HAM
    {
        public int ID { get; set; }
        public int LUC_LUONG_ID { get; set; }
        public int CAP_BAC_ID { get; set; }
        public int LOAI_HAM_ID { get; set; }
    
        public virtual DM_CAP_BAC DM_CAP_BAC { get; set; }
    }
}
