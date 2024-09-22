using Coffee.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.Models
{
    public class ProductModel
    {
        public string MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public string MaLoaiSanPham { get; set; }
        public string HinhAnh { get; set; }
        public int SoLuong { get; set; }
        public string Mota { get; set; }
        public int PhanTramGiam { get; set; }

        public Dictionary<string, ProductSizeDetailDTO> ChiTietKichThuocSanPham { get; set; }
        public Dictionary<string, ProductRecipeDTO> CongThuc { get; set; }
    }
}
