using Coffee.DTOs;
using Coffee.Services;
using Coffee.Utils;
using Coffee.Views.Admin.EmployeePage;
using Coffee.Views.MessageBox;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OfficeOpenXml;
using Coffee.Views.Admin.CustomerPage;

namespace Coffee.ViewModel.AdminVM.Employee
{
    public partial class EmployeeViewModel: BaseViewModel, IConstraintViewModel
    {
        #region variable
        public Grid MaskName { get; set; }

        private ObservableCollection<EmployeeDTO> _EmployeeList;

        public ObservableCollection<EmployeeDTO> EmployeeList
        {
            get { return _EmployeeList; }
            set { _EmployeeList = value; OnPropertyChanged(); }
        }

        private List<EmployeeDTO> __employeeList;
        private List<EmployeeDTO> employeeSearch;
        private List<EmployeeDTO> employeePosition;

        private ObservableCollection<PositionDTO> _EmployeePositionList;

        public ObservableCollection<PositionDTO> EmployeePositionList
        {
            get { return _EmployeePositionList; }
            set { _EmployeePositionList = value; OnPropertyChanged(); }
        }

        private PositionDTO _selectedPosition;
        public PositionDTO selectedPosition
        {
            get { return _selectedPosition; }
            set
            {
                _selectedPosition = value;
                OnPropertyChanged();
            }
        }

        private bool _IsLoading;

        public bool IsLoading
        {
            get { return _IsLoading; }
            set { _IsLoading = value; OnPropertyChanged(); }
        }

        public string _HeaderOperation { get; set; }
        public string HeaderOperation
        {
            get { return _HeaderOperation; }
            set { _HeaderOperation = value; OnPropertyChanged(); }
        }

        #endregion

        #region ICommand
        public ICommand loadShadowMaskIC { get; set; }
        public ICommand openWindowAddEmployeeIC { get; set; }
        public ICommand loadEmployeeListIC { get; set; }
        public ICommand searchEmployeeIC { get; set; }
        public ICommand openWindowEditEmployeeIC { get; set; }
        public ICommand deleteEmployeeIC { get; set; }
        public ICommand exportExcelIC { get; set; }
        public ICommand selectedEmployeePositionIC { get; set; }
        public ICommand loadEmployeePositionListIC { get; set; }

        #endregion

        public EmployeeViewModel()
        {
            resetEmployee();

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

            loadEmployeeListIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                loadEmployeeList();
            });

            loadEmployeePositionListIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                loadEmployeePositionList();
            });

            openWindowEditEmployeeIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                openWindowEditEmployee();
            });

            deleteEmployeeIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                deleteEmployee();
            });

            exportExcelIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                exportExcel();
            });

            selectedEmployeePositionIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                selectedEmployeePosition(selectedPosition);
            });

            openWindowAddEmployeeIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                HeaderOperation = (string)Application.Current.Resources["AddEmployee"];

                MaskName.Visibility = Visibility.Visible;
                resetEmployee();
                loadPosition();
                WorkingDay = DateTime.Now;
                Birthday = DateTime.Now;
                OperationEmployeeWindow w = new OperationEmployeeWindow();
                TypeOperation = 1; // Add employee
                w.ShowDialog();
                MaskName.Visibility = Visibility.Collapsed;
            });

            searchEmployeeIC = new RelayCommand<TextBox>(null, (p) =>
            {
                searchEmployee(p.Text);
            });

            #region operation
            confirmOperationEmployeeIC = new RelayCommand<object>((p) =>
            {
                return !(string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(Image)
                    || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(NumberPhone)
                    || string.IsNullOrEmpty(IDCard) || string.IsNullOrEmpty(Address)
                    || string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password));
            }, 
            (p) =>
            {
                confirmOperationEmployee();
            }); 

            closeOperationEmployeeWindowIC = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                p.Close();
            });

            uploadImageIC = new RelayCommand<object>((p) => { return true; }, (p) =>
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
            });

            
            #endregion
        }

        /// <summary>
        /// Load danh sách nhân viên
        /// </summary>
        private async void loadEmployeeList()
        {
            MaskName.Visibility = Visibility.Visible;
            IsLoading = true;

            (string label, List<EmployeeDTO> employees) = await EmployeeService.Ins.getListEmployee();

            if (employees != null)
            {
                EmployeeList = new ObservableCollection<EmployeeDTO>(employees);
                __employeeList = new List<EmployeeDTO>(employees);
                employeePosition = new List<EmployeeDTO>(employees);
                employeeSearch = new List<EmployeeDTO>(employees);
            }
            else
            {
                EmployeeList = new ObservableCollection<EmployeeDTO>();
                __employeeList = new List<EmployeeDTO>();
                employeePosition = new List<EmployeeDTO>();
                employeeSearch = new List<EmployeeDTO>();
            }

            MaskName.Visibility = Visibility.Collapsed;
            IsLoading = false;
        }

        /// <summary>
        /// Mở cửa số sửa nhân viên
        /// </summary>
        public async Task openWindowEditEmployee()
        {
            HeaderOperation = (string)Application.Current.Resources["EditEmployee"];

            MaskName.Visibility = Visibility.Visible;
            await loadPosition();
            OperationEmployeeWindow w = new OperationEmployeeWindow();
            TypeOperation = 2; // Edit employee
            loadEmployee(SelectedEmployee);
            w.ShowDialog();
            MaskName.Visibility = Visibility.Collapsed;
        }


        /// <summary>
        /// Load dữ liệu nhân viên click
        /// </summary>
        /// <param name="employee"></param>
        private void loadEmployee(EmployeeDTO employee)
        {
            //MaskNameOperation.Visibility = Visibility.Visible;
            //IsLoadingOperation = true;

            OriginImage = employee.HinhAnh;
            FullName = employee.HoTen;
            Email = employee.Email;
            IDCard = employee.CCCD_CMND;
            NumberPhone = employee.SoDienThoai;
            Username = employee.TaiKhoan;
            Password = employee.MatKhau;
            Address = employee.DiaChi;
            Image = employee.HinhAnh;
            Wage = employee.Luong;
            Birthday = DateTime.ParseExact(employee.NgaySinh, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            WorkingDay = DateTime.ParseExact(employee.NgayLam, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            SelectedGender = employee.GioiTinh;
            SelectedPositionName = employee.TenChucVu;

            //MaskNameOperation.Visibility = Visibility.Collapsed;
            //IsLoadingOperation = false;
        }

        /// <summary>
        /// Reset dữ liệu nhân viên trên cửa sổ thao tác của nhân viên
        /// </summary>
        private void resetEmployee()
        {
            FullName = "";
            Email = "";
            IDCard = "";
            NumberPhone = "";
            Username = "";
            Password = "";
            Address = "";
            Image = "https://user-images.githubusercontent.com/5709133/50445980-88299a80-0912-11e9-962a-6fd92fd18027.png";
            Wage = 0;
        }

        /// <summary>
        /// Xoá nhân viên
        /// </summary>
        public async void deleteEmployee()
        {
            MaskName.Visibility = Visibility.Visible;
            IsLoading = true;

            MessageBoxCF ms = new MessageBoxCF(Application.Current.Resources["ConfirmDeleteEmployee"] as string, MessageType.Waitting, MessageButtons.YesNo);

            if (ms.ShowDialog() == true)
            {
                (string label, bool isDeleteEmployee) = await EmployeeService.Ins.DeleteEmployee(SelectedEmployee);

                IsLoading = false;

                if (isDeleteEmployee)
                {
                    MessageBoxCF msn = new MessageBoxCF(label, MessageType.Accept, MessageButtons.OK);
                    loadEmployeeList();
                    msn.ShowDialog();
                }
                else
                {
                    MessageBoxCF msn = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                    msn.ShowDialog();
                }
            }

            MaskName.Visibility = Visibility.Collapsed;
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

        /// <summary>
        /// In dữ liệu ra excel
        /// </summary>
        private void exportExcel()
        {
            MaskName.Visibility = Visibility.Visible;
            IsLoading = true;

            System.Windows.Forms.SaveFileDialog sf = new System.Windows.Forms.SaveFileDialog
            {
                FileName = "DanhSachNhanVien",
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
                worksheet.Cells[1, 1].Value = "Mã nhân viên";
                worksheet.Cells[1, 2].Value = "Tên nhân viên";
                worksheet.Cells[1, 3].Value = "CCCD/CMND";
                worksheet.Cells[1, 4].Value = "Số điện thoại";
                worksheet.Cells[1, 5].Value = "Email";
                worksheet.Cells[1, 6].Value = "Giới tính";
                worksheet.Cells[1, 7].Value = "Ngày sinh";
                worksheet.Cells[1, 8].Value = "Ngày làm";
                worksheet.Cells[1, 9].Value = "Tên chức vụ";
                worksheet.Cells[1, 10].Value = "Lương";
                        
                // Dữ liệu
                int count = 2;
                foreach (var item in EmployeeList)
                {
                    worksheet.Cells[count, 1].Value = item.MaNhanVien;
                    worksheet.Cells[count, 2].Value = item.HoTen;
                    worksheet.Cells[count, 3].Value = item.CCCD_CMND;
                    worksheet.Cells[count, 4].Value = item.SoDienThoai;
                    worksheet.Cells[count, 5].Value = item.Email;
                    worksheet.Cells[count, 6].Value = item.GioiTinh;
                    worksheet.Cells[count, 7].Value = item.NgaySinh;
                    worksheet.Cells[count, 8].Value = item.NgayLam;
                    worksheet.Cells[count, 9].Value = item.TenChucVu;
                    worksheet.Cells[count, 10].Value = item.Luong;

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

        private async void loadEmployeePositionList()
        {
            (string label, List<PositionDTO> listPosition) = await PositionService.Ins.getAllPosition();

            if (listPosition != null)
            {
                EmployeePositionList = new ObservableCollection<PositionDTO>(listPosition);
            }
            else
                EmployeePositionList = new ObservableCollection<PositionDTO>();

            EmployeePositionList.Insert(0, new PositionDTO
            {
                MaChucVu = "CD0000",
                TenChucVu = "Toàn bộ",
            });
        }

        private void searchEmployee(string text)
        {
            if (text != null)
            {
                if (__employeeList != null)
                    employeeSearch = new List<EmployeeDTO>(__employeeList.FindAll(x => x.HoTen.ToLower().Contains(text.ToLower())));
            }

            EmployeeList = new ObservableCollection<EmployeeDTO>(employeeSearch.Intersect(employeePosition));
        }

        private void selectedEmployeePosition(PositionDTO position)
        {
            if (position != null)
            {
                if (position.MaChucVu == "CD0000")
                    employeePosition = new List<EmployeeDTO>(__employeeList);
                else
                    employeePosition = new List<EmployeeDTO>(__employeeList.FindAll(p => p.MaChucVu == position.MaChucVu));
            }

            EmployeeList = new ObservableCollection<EmployeeDTO>(employeeSearch.Intersect(employeePosition));
        }
    }
}
