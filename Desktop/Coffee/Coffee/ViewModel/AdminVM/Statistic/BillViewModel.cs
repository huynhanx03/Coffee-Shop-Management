using Coffee.DALs;
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
    public partial class StatisticViewModel:BaseViewModel
    {
        #region variable
        public string _TenNhanVien { get; set; }
        public string TenNhanVien
        {
            get { return _TenNhanVien; }
            set { _TenNhanVien = value; OnPropertyChanged(); }
        }
        public string _NgayTao { get; set; }
        public string NgayTao
        {
            get { return _NgayTao; }
            set { _NgayTao = value; OnPropertyChanged(); }
        }
        public decimal _TongTien { get; set; }
        public decimal TongTien
        {
            get { return _TongTien; }
            set { _TongTien = value; OnPropertyChanged(); }
        }
        private ObservableCollection<DetailBillDTO> _DetailBillList;
        public ObservableCollection<DetailBillDTO> DetailBillList
        {
            get { return _DetailBillList; }
            set { _DetailBillList = value; OnPropertyChanged(); }
        }
        #endregion

        #region
        public ICommand closeBillViewWindowIC { get; set; }
        #endregion

        /// <summary>
        /// lấy thông tin nhân viên
        /// </summary>
        public async Task getNameEmployee(string id)
        {
            EmployeeDTO employee = await EmployeeService.Ins.getDetailEmployee(id);

            if (employee != null)
            {
                TenNhanVien = employee.HoTen;
            }
        }
        /// <summary>
        /// lấy danh sách chi tiết của bill
        /// </summary>
        public async Task getListDetailBill(string MaHoaDon)
        {
            //(string label, List<DetailBillModel> detailbilllist) = await BillService.Ins.getDetailBill(MaHoaDon);

            //if (detailbilllist != null)
            //{
            //    DetailBillList = new ObservableCollection<DetailBillModel>(detailbilllist);
            //}
        }
    }
}
