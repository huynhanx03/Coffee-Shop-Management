using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DTOs
{
    public class ChatDTO
    {
        private string _MaKhachHang;
        public string MaKhachHang
        {
            get { return _MaKhachHang; }
            set
            {
                _MaKhachHang = value;
                IsReceived = !string.IsNullOrEmpty(value);
            }
        }

        private string _MaNhanVien;
        public string MaNhanVien
        {
            get { return _MaKhachHang; }
            set
            {
                _MaKhachHang = value;
                IsReceived = string.IsNullOrEmpty(value);
            }
        }

        public string ChiTiet { get; set; }
        private string _ThoiGian;
        public string ThoiGian
        {
            get { return _ThoiGian; }
            set
            {
                _ThoiGian = value;
                ThoiGiandt = DateTime.ParseExact(ThoiGian, "HH:mm:ss dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
        }

        public bool IsReceived { get; set; }
        public DateTime ThoiGiandt { get; set; }
    }
}
