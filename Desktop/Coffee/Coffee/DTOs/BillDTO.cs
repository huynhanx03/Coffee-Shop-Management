using Coffee.Models;
using Coffee.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DTOs
{
    public class BillDTO
    {
        public string MaHoaDon {  get; set; }
        public string MaBan { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string NgayTao { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }
        public string MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string TenBan { get; set; }
        public Dictionary<string, DetailBillDTO> _ChiTietHoaDon;

        public Dictionary<string, DetailBillDTO> ChiTietHoaDon
        {
            get { return _ChiTietHoaDon; }
            set
            {
                _ChiTietHoaDon = value;
                DanhSachChiTietHoaDon = value.Values.ToList();
            }
        }
        public List<DetailBillDTO> DanhSachChiTietHoaDon;

        public BillDTO() { }
        public BillDTO(BillModel bill)
        {

        }
    }
}
