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
    
    public partial class DM_DINH_MUC
    {
        public int ID { get; set; }
        public string SO_QUYET_DINH { get; set; }
        public Nullable<System.DateTime> NGAY_QUYET_DINH { get; set; }
        public string NGUOI_KY { get; set; }
        public Nullable<System.DateTime> NGAY_TAO { get; set; }
        public Nullable<System.DateTime> NGAY_CAP_NHAT { get; set; }
        public Nullable<int> NGUOI_CAP_NHAT { get; set; }
        public Nullable<int> STATUS { get; set; }
        public Nullable<double> HAO_HUT { get; set; }
    }
}
