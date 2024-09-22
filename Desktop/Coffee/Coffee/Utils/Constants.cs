using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.Utils
{
    public class Constants
    {
        public class StatusTable
        {
            public static readonly string FREE = "Trống";
            public static readonly string BOOKED = "Đã đặt";
        }

        public class StatusBill
        {
            public static readonly string UNPAID = "Chưa thanh toán";
            public static readonly string PAID = "Đã thanh toán";
        }

        public class StatusOrder
        {
            public static readonly string CANCEL = "Đã huỷ";
            public static readonly string WAITTING = "Chờ xác nhận";
            public static readonly string CONFIRMED = "Đã xác nhận";
        }

        public class API
        {
            public static readonly string IP = "http://localhost:3000";
        }
    }
}
