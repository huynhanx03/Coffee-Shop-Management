using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DTOs
{
    public class ProductDTO
    {
        public string MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public int PhanTramGiam { get; set; }
        public string LoaiSanPham { get; set; }
        public string MaLoaiSanPham { get; set; }
        public string HinhAnh { get; set; }
        public int SoLuong { get; set; }
        public string Mota { get; set; }

        private Dictionary<string, ProductSizeDetailDTO> _ChiTietKichThuocSanPham;
        public Dictionary<string, ProductSizeDetailDTO> ChiTietKichThuocSanPham 
        { 
            get { return _ChiTietKichThuocSanPham; }
            set 
            {
                _ChiTietKichThuocSanPham = value;
                DanhSachChiTietKichThuocSanPham = value.Values.ToList();
            }
        }
        private Dictionary<string, ProductRecipeDTO> _CongThuc { get; set; }
        public Dictionary<string, ProductRecipeDTO> CongThuc
        {
            get { return _CongThuc; }
            set
            {
                _CongThuc = value;
                DanhSachCongThuc = value.Values.ToList();
            }
        }

        public List<ProductSizeDetailDTO> DanhSachChiTietKichThuocSanPham { get; set; }
        public List<ProductRecipeDTO> DanhSachCongThuc { get; set; }
        
    }
}
