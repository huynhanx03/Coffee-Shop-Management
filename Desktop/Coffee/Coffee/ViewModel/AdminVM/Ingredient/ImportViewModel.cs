using Coffee.DTOs;
using Coffee.Models;
using Coffee.Services;
using Coffee.Utils;
using Coffee.Views.Admin.IngredientPage;
using Coffee.Views.MessageBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Coffee.ViewModel.AdminVM.Ingredient
{
    public partial class IngredientViewModel: BaseViewModel
    {
        #region variable
        private ObservableCollection<DetailImportDTO> _DetailImportList = new ObservableCollection<DetailImportDTO>();

        public ObservableCollection<DetailImportDTO> DetailImportList
        {
            get { return _DetailImportList; }
            set { _DetailImportList = value; OnPropertyChanged(); }
        }

        private List<DetailImportDTO> __DetailImportList = new List<DetailImportDTO>();

        private string _EmployeeName;
        public string EmployeeName
        {
            get { return _EmployeeName; }
            set { _EmployeeName = value; OnPropertyChanged(); }
        }

        private string _InvoiceDate;
        public string InvoiceDate
        {
            get { return _InvoiceDate; }
            set { _InvoiceDate = value; OnPropertyChanged(); }
        }

        private decimal _InvoiceValue;
        public decimal InvoiceValue
        {
            get { return _InvoiceValue; }
            set { _InvoiceValue = value; OnPropertyChanged(); }
        }

        private DetailImportDTO _SelectDetailImport;

        public DetailImportDTO SelectDetailImport
        {
            get { return _SelectDetailImport; }
            set { _SelectDetailImport = value; OnPropertyChanged(); }
        }

        private bool isSearchImport = false;

        #endregion

        #region ICommand
        public ICommand confirmImportIC { get; set; }
        public ICommand closeBillImportWindowIC { get; set; }
        public ICommand removeIngredientIC { get; set; }

        #endregion

        /// <summary>
        /// Xác nhận nhập kho
        /// </summary>
        private async void confirmImport(Window w)
        {
            // Xác định phiếu nhập kho
            ImportModel import = new ImportModel
            {
                MaNhanVien = Memory.user.MaNguoiDung,
                NgayTaoPhieu = InvoiceDate
            };

            foreach (DetailImportDTO detail in DetailImportList)
            {
                // Chỉnh sửa lại mã đơn vị
                UnitDTO unit = UnitList.First(u => u.TenDonVi == detail.TenDonVi);

                // Kiểm tra mã đơn vị mới có thích hợp không
                if (detail.MaDonVi == "DV0001" || detail.MaDonVi == "DV0002")
                {
                    if (unit.MaDonVi != "DV0001" && unit.MaDonVi != "DV0002")
                    {
                        MessageBoxCF ms = new MessageBoxCF(Application.Current.Resources["InappropriateUnitInIngredients"] as string + detail.TenNguyenLieu, MessageType.Error, MessageButtons.OK);
                        w.Close();
                        ms.ShowDialog();
                        return;
                    }
                }
                else
                {
                    if (unit.MaDonVi != "DV0003" && unit.MaDonVi != "DV0004")
                    {
                        MessageBoxCF ms = new MessageBoxCF(Application.Current.Resources["InappropriateUnitInIngredients"] as string + detail.TenNguyenLieu, MessageType.Error, MessageButtons.OK);
                        w.Close();
                        ms.ShowDialog();
                        return;
                    }
                }

                detail.MaDonVi = unit.MaDonVi;
            }

            import.ChiTietPhieuNhapKho = DetailImportList.ToDictionary(detail => detail.MaNguyenLieu);

            (string label, bool isCreate) = await BillImportService.Ins.createBillImport(import);

            if (isCreate)
            {
                // Thêm số lượng vào nguyên liệu
                foreach (DetailImportDTO detailImport in DetailImportList)
                {
                    await IngredientService.Ins.updateIngredientQuantity(detailImport.MaNguyenLieu, detailImport.SoLuong, detailImport.MaDonVi);
                }

                DetailImportList = new ObservableCollection<DetailImportDTO>();
                this.loadIngredientList();

                MessageBoxCF ms = new MessageBoxCF(label, MessageType.Accept, MessageButtons.OK);
                ms.ShowDialog();

                w.Close();
            }
            else
            {
                MessageBoxCF ms = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                ms.ShowDialog();
            }
        }

        /// <summary>
        /// Xoá nguyên liệu khỏi nhập kho
        /// </summary>
        public void removeIngredient()
        {
            DetailImportList.Remove(SelectDetailImport);

            DetailImportList = new ObservableCollection<DetailImportDTO>(DetailImportList);
        }

        /// <summary>
        /// Tìm kiếm nguyên liệu tại phiếu nhập
        /// </summary>
        /// <param name="text"></param>
        public void searchDetailImport(string text)
        {
            if (text != null)
            {
                // rỗng
                if (string.IsNullOrEmpty(text))
                {
                    isSearchImport = true;
                    DetailImportList = new ObservableCollection<DetailImportDTO>(__DetailImportList);
                }
                else
                {
                    if (!isSearchImport)
                        __DetailImportList = new List<DetailImportDTO>(DetailImportList);

                    isSearchImport = true;
                    DetailImportList = new ObservableCollection<DetailImportDTO>(__DetailImportList.FindAll(x => x.TenNguyenLieu.ToLower().Contains(text.ToLower())));
                }
            }
        }
    }
}
