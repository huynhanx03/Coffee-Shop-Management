using Coffee.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.Models
{
    public class BillModel
    {
        public string MaHoaDon { get; set; }
        public string MaBan { get; set; }
        public string MaNhanVien { get; set; }
        public string MaKhachHang { get; set; }
        public string NgayTao { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }
        public Dictionary<string, DetailBillModel> ChiTietHoaDon { get; set; }

        public BillModel () { }
        public BillModel(BillDTO bill)
        {
            this.MaHoaDon = bill.MaHoaDon;
            this.MaBan = bill.MaBan;
            this.MaNhanVien = bill.MaNhanVien;
            this.MaKhachHang = bill.MaKhachHang;
            this.NgayTao = bill.NgayTao;
            this.TrangThai = bill.TrangThai;
        }
    }
}
