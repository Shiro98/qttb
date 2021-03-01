using System; 

namespace Model.ModelExtend
{
    public class TC_PHAM_NHANModel
    {
        public int ID { get; set; }
        public int? NHOM_TIEU_CHUAN_ID { get; set; }
        public long? SP_ID { get; set; }
        public int? LOAI_PHAM_ID { get; set; }
        public int? GIOI_TINH { get; set; }
        public int NAM { get; set; }
        public double? SO_LUONG_TC { get; set; }
        public int? NIEN_HAN { get; set; }
        public DateTime? NGAY_TAO { get; set; }
        public DateTime? NGAY_CAP_NHAT { get; set; }
        public int? NGUOI_CAP_NHAT { get; set; }
    }
}
