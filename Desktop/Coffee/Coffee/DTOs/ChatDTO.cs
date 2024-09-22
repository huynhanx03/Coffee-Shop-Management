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
        private string _MaKH;
        public string MaKH
        {
            get { return _MaKH; }
            set
            {
                _MaKH = value;
                IsReceived = !string.IsNullOrEmpty(value);
            }
        }
        
        public string NoiDung { get; set; }
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
