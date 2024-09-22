using Coffee.DTOs;
using Coffee.Models;
using Coffee.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Coffee.ViewModel.AdminVM.Statistic
{
    public partial class StatisticViewModel : BaseViewModel
    {
        #region variable
        public string _TenNhanVienNhapKho { get; set; }
        public string TenNhanVienNhapKho
        {
            get { return _TenNhanVienNhapKho; }
            set { _TenNhanVienNhapKho = value; OnPropertyChanged(); }
        }
        public string _NgayTaoPhieu { get; set; }
        public string NgayTaoPhieu
        {
            get { return _NgayTaoPhieu; }
            set { _NgayTaoPhieu = value; OnPropertyChanged(); }
        }
        public decimal _TongTienNhap { get; set; }
        public decimal TongTienNhap
        {
            get { return _TongTienNhap; }
            set { _TongTienNhap = value; OnPropertyChanged(); }
        }
        private ObservableCollection<DetailImportDTO> _DetailBillImportList;
        public ObservableCollection<DetailImportDTO> DetailImportBillList
        {
            get { return _DetailBillImportList; }
            set { _DetailBillImportList = value; OnPropertyChanged(); }
        }
        #endregion

        #region
        public ICommand closeBillImportViewWindowIC { get; set; }
        #endregion

        /// <summary>
        /// lấy thông tin nhân viên
        /// </summary>
        public async Task getNameEmployeeImport(string id)
        {
            EmployeeDTO employee = await EmployeeService.Ins.getDetailEmployee(id);

            if (employee != null)
            {
                TenNhanVienNhapKho = employee.HoTen;
            }
        }
        /// <summary>
        /// lấy danh sách chi tiết của bill nhập kho
        /// </summary>
        public async Task getListDetailBillImport(string MaHoaDon)
        {
            (string label, List<DetailImportDTO> detailbillimportlist) = await BillImportService.Ins.getDetailBillImport(MaHoaDon);

            if (detailbillimportlist != null)
            {
                DetailImportBillList = new ObservableCollection<DetailImportDTO>(detailbillimportlist);
            }
        }
    }
}
