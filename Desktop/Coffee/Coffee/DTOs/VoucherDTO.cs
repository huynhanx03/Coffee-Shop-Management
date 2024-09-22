using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DTOs
{
    public class VoucherDTO
    {
        public string MaPhieuGiamGia { get; set; }
        public string HangToiThieu { get; set; }
        public string TenHangToiThieu { get; set; } 
        public string NgayHetHan { get; set; }
        public string NgayPhatHanh { get; set; }
        public string NoiDung { get; set; }
        public int PhanTramGiam { get; set; }
    }
}
