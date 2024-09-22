using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DTOs
{
    public class ProductOrderDTO
    {
        public string MaSanPham {  get; set; }
        public decimal Gia {  get; set; }
        public string MaKichThuoc { get; set; }
        private string _KichThuoc;
        public string KichThuoc
        {
            get
            {
                return _KichThuoc;
            }
            set
            {
                switch (value)
                {
                    case "S":
                        value = "Nhỏ";
                        MaKichThuoc = "KT0001";
                        break;

                    case "M":
                        value = "Vừa";
                        MaKichThuoc = "KT0002";
                        break;

                    case "L":
                        value = "Lớn";
                        MaKichThuoc = "KT0003";
                        break;
                }

                _KichThuoc = value;
            }
        }
        public int SoLuong {  get; set; }
        public string TenSanPham {  get; set; }
    }
}
