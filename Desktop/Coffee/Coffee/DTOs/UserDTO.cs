using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DTOs
{
    public class UserDTO
    {
        public string MaNguoiDung { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string CCCD_CMND { get; set; }
        public string DiaChi { get; set; }
        public string NgaySinh { get; set; }
        public string NgayTao { get; set; }
        public string TaiKhoan { get; set; }    
        public string MatKhau { get; set; }    
        public int VaiTro { get; set; }
        public string HinhAnh { get; set; }
    }
}
