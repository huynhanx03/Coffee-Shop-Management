using Coffee.DALs;
using Coffee.DTOs;
using Coffee.Models;
using Coffee.Services;
using Coffee.Views.Admin.CustomerPage;
using Coffee.Views.MessageBox;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Coffee.ViewModel.AdminVM.Customer
{
    public partial class CustomerViewModel: BaseViewModel, IConstraintViewModel
    {
        #region variable
        public Grid MaskName { get; set; }

        private ObservableCollection<CustomerDTO> _CustomerList;

        public ObservableCollection<CustomerDTO> CustomerList
        {
            get { return _CustomerList; }
            set { _CustomerList = value; OnPropertyChanged(); }
        }

        private List<CustomerDTO> __CustomerList;

        private ObservableCollection<AddressModel> _AddressList;

        public ObservableCollection<AddressModel> AddressList
        {
            get { return _AddressList; }
            set { _AddressList = value; OnPropertyChanged(); }
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
        public ICommand openWindowAddCustomerIC { get; set; }
        public ICommand loadCustomerListIC { get; set; }
        public ICommand searchCustomerIC { get; set; }
        public ICommand openWindowEditCustomerIC { get; set; }
        public ICommand exportExcelIC { get; set; }
        public ICommand deleteCustomerIC { get; set; }
        public ICommand closeViewAddressCustomerWindowIC { get; set; }
        public ICommand viewAddressCustomerIC { get; set; }

        #endregion

        public CustomerViewModel()
        {
            ListGender = new ObservableCollection<string>{
                (string)Application.Current.Resources["Male"],
                (string)Application.Current.Resources["Female"],
                (string)Application.Current.Resources["Other"],
            };

            loadShadowMaskIC = new RelayCommand<Grid>((p) => { return true; }, (p) =>
            {
                MaskName = p;
            });

            loadShadowMaskOperationIC = new RelayCommand<Grid>((p) => { return true; }, (p) =>
            {
                MaskNameOperation = p;
            });

            loadCustomerListIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                loadCustomerList();
            });

            openWindowEditCustomerIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                openWindowEditCustomer();
            });

            exportExcelIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                exportExcel();
            });

            deleteCustomerIC = new RelayCommand<CustomerDTO>((p) => { return true; }, (p) =>
            {
                deleteCustomer(p);
            });

            openWindowAddCustomerIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                openWindowAddCustomer();
            });

            searchCustomerIC = new RelayCommand<TextBox>(null, (p) =>
            {
                if (p.Text != null)
                {
                    if (__CustomerList != null)
                        CustomerList = new ObservableCollection<CustomerDTO>(__CustomerList.FindAll(x => x.HoTen.ToLower().Contains(p.Text.ToLower())));
                }
            });

            viewAddressCustomerIC = new RelayCommand<CustomerDTO>((p) => { return true; }, (p) =>
            {
                viewAddressCustomer(p);
            });

            closeViewAddressCustomerWindowIC = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                p.Close();
            });

            #region operation
            confirmOperationCustomerIC = new RelayCommand<object>((p) =>
            {
                return !(string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(Image)
                     || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(NumberPhone)
                     || string.IsNullOrEmpty(IDCard)
                     || string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password));
            },
            (p) =>
            {
                confirmOperationCustomer();
            });

            closeOperationCustomerWindowIC = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                p.Close();
            });

            uploadImageIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                uploadImage();
            });

            
            #endregion


        }

        #region function
        /// <summary>
        /// Load danh sách khách hàng
        /// </summary>
        private async void loadCustomerList()
        {
            MaskName.Visibility = Visibility.Visible;
            IsLoading = true;

            (string label, List<CustomerDTO> Customers) = await CustomerService.Ins.getListCustomer();

            if (Customers != null)
            {
                CustomerList = new ObservableCollection<CustomerDTO>(Customers);
                __CustomerList = new List<CustomerDTO>(Customers);
            }
            else
            {
                CustomerList = new ObservableCollection<CustomerDTO>();
                __CustomerList = new List<CustomerDTO>();
            }

            MaskName.Visibility = Visibility.Collapsed;
            IsLoading = false;
        }

        /// <summary>
        /// Mở cửa số sửa khách hàng
        /// </summary>
        public async Task openWindowEditCustomer()
        {
            HeaderOperation = (string)Application.Current.Resources["EditCustomer"];

            MaskName.Visibility = Visibility.Visible;
            OperationCustomerWindow w = new OperationCustomerWindow();
            TypeOperation = 2; // Edit Customer
            loadCustomer(SelectedCustomer);
            w.ShowDialog();
            MaskName.Visibility = Visibility.Collapsed;
        }


        /// <summary>
        /// Load dữ liệu khách hàng click
        /// </summary>
        /// <param name="Customer"></param>
        private void loadCustomer(CustomerDTO Customer)
        {
            //MaskNameOperation.Visibility = Visibility.Visible;
            //IsLoadingOperation = true;

            OriginImage = Customer.HinhAnh;
            FullName = Customer.HoTen;
            Email = Customer.Email;
            IDCard = Customer.CCCD_CMND;
            NumberPhone = Customer.SoDienThoai;
            Username = Customer.TaiKhoan;
            Password = Customer.MatKhau;
            Address = Customer.DiaChi;
            Image = Customer.HinhAnh;
            Birthday = DateTime.ParseExact(Customer.NgaySinh, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            SelectedGender = Customer.GioiTinh;

            //MaskNameOperation.Visibility = Visibility.Collapsed;
            //IsLoadingOperation = false;
        }

        /// <summary>
        /// Reset dữ liệu khách hàng trên cửa sổ thao tác của khách hàng
        /// </summary>
        private void resetCustomer()
        {
            FullName = "";
            Email = "";
            IDCard = "";
            NumberPhone = "";
            Username = "";
            Password = "";
            Address = "";
            Image = "";
        }

        /// <summary>
        /// Xoá khách hàng
        /// </summary>
        public async void deleteCustomer(CustomerDTO customer)
        {
            MaskName.Visibility = Visibility.Visible;

            MessageBoxCF ms = new MessageBoxCF(Application.Current.Resources["ConfirmDeleteCustomer"] as string, MessageType.Waitting, MessageButtons.YesNo);

            if (ms.ShowDialog() == true)
            {
                IsLoading = true;

                (string label, bool isDeleteCustomer) = await CustomerService.Ins.DeleteCustomer(customer);

                if (isDeleteCustomer)
                {
                    MessageBoxCF msn = new MessageBoxCF(label, MessageType.Accept, MessageButtons.OK);
                    loadCustomerList();
                    msn.ShowDialog();
                }
                else
                {
                    MessageBoxCF msn = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                    msn.ShowDialog();
                }
            }

            MaskName.Visibility = Visibility.Collapsed;
            IsLoading = false;

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

        public void openWindowAddCustomer()
        {
            HeaderOperation = (string)Application.Current.Resources["AddCustomer"];

            MaskName.Visibility = Visibility.Visible;
            resetCustomer();
            Birthday = DateTime.Now;
            OperationCustomerWindow w = new OperationCustomerWindow();
            TypeOperation = 1; // Add Customer
            w.ShowDialog();
            MaskName.Visibility = Visibility.Collapsed;
        }
        
        public void uploadImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.png;*.jpeg;*.webp;*.gif|All Files|*.*";
            if (openFileDialog.ShowDialog() == true)
            {

                Image = openFileDialog.FileName;
                if (Image != null)
                {
                    // Image was uploaded successfully.                        
                }
                else
                {
                    MessageBoxCF ms = new MessageBoxCF(Application.Current.Resources["UploadImageFailed"] as string, MessageType.Error, MessageButtons.OK);
                    ms.ShowDialog();
                }
            }
        }


        /// <summary>
        /// In dữ liệu ra excel
        /// </summary>
        private void exportExcel()
        {
            MaskName.Visibility = Visibility.Visible;
            IsLoading = true;

            System.Windows.Forms.SaveFileDialog sf = new System.Windows.Forms.SaveFileDialog
            {
                FileName = "DanhSachKhachHang",
                Filter = "Excel |*.xlsx",
                ValidateNames = true
            };

            if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                // Tạo một đối tượng ExcelPackage
                ExcelPackage package = new ExcelPackage();

                // Tạo một đối tượng ExcelWorksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sf.FileName);

                // Tiêu đề cột
                worksheet.Cells[1, 1].Value = "Mã khách hàng";
                worksheet.Cells[1, 2].Value = "Tên khách hàng";
                worksheet.Cells[1, 3].Value = "CCCD/CMND";
                worksheet.Cells[1, 4].Value = "Số điện thoại";
                worksheet.Cells[1, 5].Value = "Email";
                worksheet.Cells[1, 6].Value = "Giới tính";
                worksheet.Cells[1, 7].Value = "Ngày sinh";
                worksheet.Cells[1, 8].Value = "Ngày tạo";
                worksheet.Cells[1, 9].Value = "Điểm tích luỹ";

                // Dữ liệu
                int count = 2;
                foreach (var item in CustomerList)
                {
                    worksheet.Cells[count, 1].Value = item.MaKhachHang;
                    worksheet.Cells[count, 2].Value = item.HoTen;
                    worksheet.Cells[count, 3].Value = item.CCCD_CMND;
                    worksheet.Cells[count, 4].Value = item.SoDienThoai;
                    worksheet.Cells[count, 5].Value = item.Email;
                    worksheet.Cells[count, 6].Value = item.GioiTinh;
                    worksheet.Cells[count, 7].Value = item.NgaySinh;
                    worksheet.Cells[count, 8].Value = item.NgayTao;
                    worksheet.Cells[count, 9].Value = item.DiemTichLuy;

                    count++;
                }

                // Lưu file Excel
                FileInfo fileInfo = new FileInfo(sf.FileName);
                package.SaveAs(fileInfo);

                Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                MessageBoxCF mb = new MessageBoxCF(Application.Current.Resources["ExportSuccess"] as string, MessageType.Accept, MessageButtons.OK);
                mb.ShowDialog();
            }

            MaskName.Visibility = Visibility.Collapsed;
            IsLoading = false;
        }

        private async void viewAddressCustomer(CustomerDTO customer)
        {
            MaskName.Visibility = Visibility.Visible;
            IsLoading = true;

            // Load các vị trí
            (string label, List<AddressModel> addressList) = await CustomerService.Ins.getListAddressCustomer(customer.MaKhachHang);
            
            IsLoading = false;

            if (addressList != null)
            {
                AddressList = new ObservableCollection<AddressModel>(addressList);

                // Hiển thị view

                ViewAddressWindow w = new ViewAddressWindow();
                w.ShowDialog();
            }
            else
            {
                MessageBoxCF ms = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                ms.ShowDialog();
            }

            MaskName.Visibility = Visibility.Collapsed;
        }
        #endregion
    }
}
