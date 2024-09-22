using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DTOs
{
    public class TableDTO
    {
        public string MaBan { get; set; }
        public string TenBan { get; set; }
        public string MaLoaiBan { get; set; }
        public string TenLoaiBan { get; set; }
        public int Hang {  get; set; }
        public int Cot { get; set; }
        public string TrangThai { get; set; }
    }
}
