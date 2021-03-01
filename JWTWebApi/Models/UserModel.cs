using System;  

namespace JWTWebApi.Models
{
    public class UserModel
    {
        public int USER_ID { get; set; }
        public string LOGIN_NAME { get; set; }
        public string PASSWORD { get; set; }
        public string FULL_NAME { get; set; }
        public Nullable<int> UNIT_ID { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
        public string USER_GROUP { get; set; }
        public string USER_CLASS { get; set; }
        public Nullable<System.DateTime> LAST_SIGNED_IN { get; set; }
        public string USER_DESC { get; set; }
        public Nullable<int> FAIL_LOGIN_COUNT { get; set; }
        public string ENABLED_FLAG { get; set; }
        public string TEL_NO { get; set; }
        public string IMG_PATH { get; set; }
        public Nullable<int> UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
        public string SHORT_NAME { get; set; }
        public Nullable<int> TITLE_ID { get; set; }
        public Nullable<int> LEVEL_ID { get; set; }
    }
}