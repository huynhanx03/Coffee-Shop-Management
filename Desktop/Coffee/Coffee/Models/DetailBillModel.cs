using Coffee.DTOs;
using Coffee.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.Models
{
    public class DetailBillModel
    {
        public string MaHoaDon {  get; set; }
        public string MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public string MaKichThuoc { get; set; }
        public string TenKichThuoc { get; set; }
        public int SoLuong {  get; set; }
        public decimal ThanhTien { get; set; }
    

        // lấy tên sản phẩm từ mã sản phẩm
        public async Task LoadProductName()
        {
            ProductDTO p = await ProductService.Ins.getNameProduct(this.MaSanPham);
            this.TenSanPham = p.TenSanPham;
        }
        // lấy tên kích thước từ mã kích thước
        public async Task LoadSizeName()
        {
            ProductSizeDetailDTO s = await ProductSizeDetailService.Ins.getNameSize(this.MaKichThuoc);
            this.TenKichThuoc = s.TenKichThuoc;
        }
    }
}
