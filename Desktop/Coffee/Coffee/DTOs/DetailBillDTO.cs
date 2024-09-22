using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DTOs
{
    public class DetailBillDTO
    {
        public string MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public string MaKichThuoc { get; set; }
        public string TenKichThuoc { get; set; }
        private int _SoLuong;
        public int SoLuong
        {
            get { return _SoLuong; }
            set 
            { 
                _SoLuong = value; 
                OnPropertyChanged(nameof(SoLuong)); 
            }
        }
        public decimal Gia { get; set; }
        private decimal _ThanhTien;
        public decimal ThanhTien
        {
            get { return _ThanhTien; }
            set
            {
                if (_ThanhTien != value)
                {
                    _ThanhTien = value;
                    OnPropertyChanged(nameof(ThanhTien));
                }
            }
        }

        private ProductSizeDetailDTO _SelectedProductSize;
        public ProductSizeDetailDTO SelectedProductSize
        {
            get { return _SelectedProductSize; }
            set
            {
                _SelectedProductSize = value;
                ThanhTien = SelectedProductSize.Gia * SoLuong;
                OnPropertyChanged(nameof(SelectedProductSize));
            }
        }

        public ObservableCollection<ProductSizeDetailDTO> DanhSachChiTietKichThuocSanPham { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
