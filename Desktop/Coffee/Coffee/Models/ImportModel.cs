using Coffee.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.Models
{
    public class ImportModel
    {
        public string MaPhieuNhapKho { get; set; }
        public string MaNhanVien { get; set; }
        public string NgayTaoPhieu { get; set; }
        public Dictionary<string, DetailImportDTO> ChiTietPhieuNhapKho { get; set; }

        //public decimal TongTien { get; set; }
    }
}
