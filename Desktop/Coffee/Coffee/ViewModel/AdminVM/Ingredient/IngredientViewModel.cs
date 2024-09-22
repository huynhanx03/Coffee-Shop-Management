using CloudinaryDotNet.Actions;
using Coffee.DALs;
using Coffee.DTOs;
using Coffee.Services;
using Coffee.Utils;
using Coffee.Views.Admin.EmployeePage;
using Coffee.Views.Admin.IngredientPage;
using Coffee.Views.MessageBox;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Coffee.ViewModel.AdminVM.Ingredient
{
    public partial class IngredientViewModel : BaseViewModel, IConstraintViewModel
    {
        #region variable
        public Grid MaskName { get; set; }

        private ObservableCollection<IngredientDTO> _IngredientList;

        public ObservableCollection<IngredientDTO> IngredientList
        {
            get { return _IngredientList; }
            set { _IngredientList = value; OnPropertyChanged(); }
        }

        private List<IngredientDTO> __IngredientList;

        private ObservableCollection<UnitDTO> _UnitList;

        public ObservableCollection<UnitDTO> UnitList
        {
            get { return _UnitList; }
            set { _UnitList = value; OnPropertyChanged(); }
        }
        
        private IngredientDTO _SelectedIngredient;

        public IngredientDTO SelectedIngredient
        {
            get { return _SelectedIngredient; }
            set { _SelectedIngredient = value; OnPropertyChanged(); }
        }

        public string _HeaderOperation { get; set; }
        public string HeaderOperation
        {
            get { return _HeaderOperation; }
            set { _HeaderOperation = value; OnPropertyChanged(); }
        }

        private bool _IsLoading;

        public bool IsLoading
        {
            get { return _IsLoading; }
            set { _IsLoading = value; OnPropertyChanged(); }
        }

        #endregion

        #region ICommand
        public ICommand loadShadowMaskIC { get; set; }
        public ICommand openWindowAddIngredientIC { get; set; }
        public ICommand loadIngredientListIC { get; set; }
        public ICommand loadUnitListIC { get; set; }
        public ICommand addIngredientToImportIC { get; set; }
        public ICommand openWindowEditIngredientIC { get; set; }
        public ICommand deleteIngredientIC { get; set; }
        public ICommand openBillImportWindowIC { get; set; }
        public ICommand searchDetailIngredientIC { get; set; }
        public ICommand searchDetailImportIC { get; set; }
        #endregion

        public IngredientViewModel()
        {
            loadShadowMaskIC = new RelayCommand<Grid>((p) => { return true; }, (p) =>
            {
                MaskName = p;
            });

            loadIngredientListIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                loadIngredientList();
            });

            loadUnitListIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                loadUnitList();
            });

            openWindowAddIngredientIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                HeaderOperation = (string)Application.Current.Resources["AddIngredient"];

                MaskName.Visibility = Visibility.Visible;
                resetIngredient();
                TypeOperation = 1;
                OperationIngredientWindow w = new OperationIngredientWindow();
                w.ShowDialog();
                MaskName.Visibility = Visibility.Collapsed;
            });

            openBillImportWindowIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                openBillImportWindow();
            });

            closeOperationIngredientWindowIC = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                p.Close();
            });

            addIngredientToImportIC = new RelayCommand<ProductDTO>((p) => { return true; }, (p) =>
            {
                addIngredientToImport();
            });

            openWindowEditIngredientIC = new RelayCommand<ProductDTO>((p) => { return true; }, (p) =>
            {
                openWindowEditIngredient();
            });

            deleteIngredientIC = new RelayCommand<ProductDTO>((p) => { return true; }, (p) =>
            {
                deleteIngredient();
            });

            #region
            confirmOperationIngredientIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                confirmOperationIngredient();
            });
            #endregion

            #region import
            confirmImportIC = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                confirmImport(p);
            });

            closeBillImportWindowIC = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                p.Close();
            });

            searchDetailImportIC = new RelayCommand<TextBox>((p) => { return true; }, (p) =>
            {
                searchDetailImport(p.Text);
            });
            searchDetailIngredientIC = new RelayCommand<TextBox>((p) => { return true; }, (p) =>
            {
                searchDetailIngredient(p.Text);
            });

            removeIngredientIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                removeIngredient();
            });
            #endregion
        }

        /// <summary>
        /// Load list nguyên liệu
        /// </summary>
        public async void loadIngredientList()
        {
            MaskName.Visibility = Visibility.Visible;
            IsLoading = true;

            (string label, List<IngredientDTO> ingredients) = await IngredientService.Ins.getListIngredient();

            if (ingredients != null)
            {
                IngredientList = new ObservableCollection<IngredientDTO>(ingredients);
                __IngredientList = new List<IngredientDTO>(ingredients);
            }
            else
            {
                IngredientList = new ObservableCollection<IngredientDTO>();
                __IngredientList = new List<IngredientDTO>();
            }

            MaskName.Visibility = Visibility.Collapsed;
            IsLoading = false;
        }

        /// <summary>
        /// load list đơn vị
        /// </summary>
        public async void loadUnitList()
        {
            (string label, List<UnitDTO> units) = await UnitService.Ins.getAllUnit();

            if (units != null)
                UnitList = new ObservableCollection<UnitDTO>(units);
        }
        /// <summary>
        /// Xóa nguyên liệu 
        /// </summary>
        public async void deleteIngredient()
        {
            MessageBoxCF ms = new MessageBoxCF("Xác nhận xoá nguyên liệu?", MessageType.Waitting, MessageButtons.YesNo);

            if (ms.ShowDialog() == true)
            {
                (string label, bool isDeleteIngredientList) = await IngredientService.Ins.DeleteIngredient(SelectedIngredient);

                if (isDeleteIngredientList)
                {
                    MessageBoxCF msn = new MessageBoxCF(label, MessageType.Accept, MessageButtons.OK);
                    loadIngredientList();
                    msn.ShowDialog();
                }
                else
                {
                    MessageBoxCF msn = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                    msn.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Sửa nguyên liệu 
        /// </summary>
        public void openWindowEditIngredient()
        {
            HeaderOperation = (string)Application.Current.Resources["EditIngredient"];

            MaskName.Visibility = Visibility.Visible;
            OperationIngredientWindow w = new OperationIngredientWindow();
            TypeOperation = 2;
            loadIngredient(SelectedIngredient);
            w.ShowDialog();
            MaskName.Visibility = Visibility.Collapsed;
        }
        private void loadIngredient(IngredientDTO ingredient)
        {
            IngredientName = ingredient.TenNguyenLieu;
            SelectedUnitName = ingredient.TenDonVi;
        }

        /// <summary>
        /// Thêm nguyên liệu vào phiếu nhập kho
        /// </summary>
        public void addIngredientToImport()
        {
            // Kiểm tra nguyên liệu đã tồn tại chưa
            // Đã tồn tại thì số lượng +1
            foreach (var detailImport in DetailImportList)
            {
                if (detailImport.MaNguyenLieu == SelectedIngredient.MaNguyenLieu)
                {
                    detailImport.SoLuong += 1;
                    DetailImportList = new ObservableCollection<DetailImportDTO>(DetailImportList);
                    return;
                }
            }

            // Chưa tồn tại thì thêm vào
            DetailImportList.Add(new DetailImportDTO
            {
                MaNguyenLieu = SelectedIngredient.MaNguyenLieu,
                TenNguyenLieu = SelectedIngredient.TenNguyenLieu,
                MaDonVi = SelectedIngredient.MaDonVi,
                TenDonVi = SelectedIngredient.TenDonVi,
                SoLuong = 1,
                Gia = 1
            });
        }

        /// <summary>
        /// Reset dữ liệu nguyện liệu trên cửa sổ thao tác
        /// </summary>
        private void resetIngredient()
        {
            IngredientName = "";
        }
        private void openBillImportWindow()
        {
            // Chưa có sản phẩm để nhập thì không mở
                if (DetailImportList.Count <= 0)
            {
                MessageBoxCF ms = new MessageBoxCF("Không có sản phẩm để nhập vào kho", MessageType.Error, MessageButtons.OK);
                ms.ShowDialog();
                return;
            }

            MaskName.Visibility = Visibility.Visible;

            // Cập nhật dữ liệu
            EmployeeName = Memory.user.HoTen;
            InvoiceDate = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");

            InvoiceValue = 0;
            foreach (var detailImport in DetailImportList)
            {
                InvoiceValue += detailImport.Gia;
            }

            BillImportWindow w = new BillImportWindow();
            w.ShowDialog();

            MaskName.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// Tìm kiếm nguyên liệu
        /// </summary>
        public void searchDetailIngredient(string text)
        {
            if (text != null)
            {
                // rỗng
                if (string.IsNullOrEmpty(text))
                {
                    isSearchImport = true;
                    IngredientList = new ObservableCollection<IngredientDTO>(__IngredientList);
                }
                else
                {
                    if (!isSearchImport)
                        __IngredientList = new List<IngredientDTO>(IngredientList);

                    isSearchImport = true;
                    IngredientList = new ObservableCollection<IngredientDTO>(__IngredientList.FindAll(x => x.TenNguyenLieu.ToLower().Contains(text.ToLower())));
                }
            }
        }

        /// <summary>
        /// Kiểm tra chỉ được nhập số
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
