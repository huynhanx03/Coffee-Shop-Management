using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DTOs
{
    public class EvaluateDTO
    {
        public string MaDanhGia {  get; set; }
        public string MaSanPham {  get; set; }
        public string TenSanPham { get; set; }
        public string MaNguoiDung {  get; set; }
        public string TenKhachHang { get; set; }
        public string MaHang {  get; set; }
        public string TenHang {  get; set; }
        public string ThoiGianDanhGia {  get; set; }
        public string VanBanDanhGia { get; set; }
        public int DiemDanhGia { get; set; }
    }
}
