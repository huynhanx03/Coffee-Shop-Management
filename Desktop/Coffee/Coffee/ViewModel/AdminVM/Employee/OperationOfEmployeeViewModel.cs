using Coffee.DTOs;
using Coffee.Services;
using Coffee.Utils;
using Coffee.Utils.Helper;
using Coffee.Views.MessageBox;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Coffee.ViewModel.AdminVM.Employee
{
    public partial class EmployeeViewModel: BaseViewModel
    {
        #region variable
        public string _FullName { get; set; }
        public string FullName
        {
            get { return _FullName; }
            set { _FullName = value; OnPropertyChanged(); }
        }

        public string _Email { get; set; }
        public string Email
        {
            get { return _Email; }
            set { _Email = value; OnPropertyChanged(); }
        }

        public string _NumberPhone { get; set; }
        public string NumberPhone
        {
            get { return _NumberPhone; }
            set { _NumberPhone = value; OnPropertyChanged(); }
        }

        public string _IDCard { get; set; }
        public string IDCard
        {
            get { return _IDCard; }
            set { _IDCard = value; OnPropertyChanged(); }
        }

        public string _Address { get; set; }
        public string Address
        {
            get { return _Address; }
            set { _Address = value; OnPropertyChanged(); }
        }

        public DateTime _Birthday { get; set; }
        public DateTime Birthday
        {
            get { return _Birthday; }
            set { _Birthday = value; OnPropertyChanged(); }
        }

        public DateTime _WorkingDay { get; set; }
        public DateTime WorkingDay
        {
            get { return _WorkingDay; }
            set { _WorkingDay = value; OnPropertyChanged(); }
        }

        public string _Position { get; set; }
        public string Position
        {
            get { return _Position; }
            set { _Position = value; OnPropertyChanged(); }
        }

        public decimal _Wage { get; set; }
        public decimal Wage
        {
            get { return _Wage; }
            set { _Wage = value; OnPropertyChanged(); }
        }

        public ObservableCollection<PositionDTO> _ListPosition { get; set; }
        public ObservableCollection<PositionDTO> ListPosition
        {
            get { return _ListPosition; }
            set { _ListPosition = value; OnPropertyChanged(); }
        }

        public string _SelectedPositionName { get; set; }
        public string SelectedPositionName
        {
            get { return _SelectedPositionName; }
            set { _SelectedPositionName = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> _ListGender { get; set; }
        public ObservableCollection<string> ListGender
        {
            get { return _ListGender; }
            set { _ListGender = value; OnPropertyChanged(); }
        }

        public string _SelectedGender { get; set; }
        public string SelectedGender
        {
            get { return _SelectedGender; }
            set { _SelectedGender = value; OnPropertyChanged(); }
        }
        
        public string _Image { get; set; }
        public string Image
        {
            get { return _Image; }
            set { _Image = value; OnPropertyChanged(); }
        }

        public string _Username { get; set; }
        public string Username
        {
            get { return _Username; }
            set { _Username = value; OnPropertyChanged(); }
        }

        public string _Password { get; set; }
        public string Password
        {
            get { return _Password; }
            set { _Password = value; OnPropertyChanged(); }
        }

        public int TypeOperation { get; set; }
        public string OriginImage { get; set; }

        public EmployeeDTO _SelectedEmployee { get; set; }
        public EmployeeDTO SelectedEmployee
        {
            get { return _SelectedEmployee; }
            set { _SelectedEmployee = value; OnPropertyChanged(); }
        }

        private bool _IsLoadingOperation;

        public bool IsLoadingOperation
        {
            get { return _IsLoadingOperation; }
            set { _IsLoadingOperation = value; OnPropertyChanged(); }
        }

        public Grid MaskNameOperation { get; set; }

        #endregion

        #region ICommand
        public ICommand confirmOperationEmployeeIC { get; set; }
        public ICommand closeOperationEmployeeWindowIC { get; set; }
        public ICommand uploadImageIC { get; set; }
        public ICommand loadShadowMaskOperationIC { get; set; }
        

        #endregion

        #region function
        /// <summary>
        /// Lấy danh sách chức vị
        /// </summary>
        public async Task loadPosition()
        {
            (string label, List<PositionDTO> listPosition) = await PositionService.Ins.getAllPosition();

            if (listPosition != null)
            {
                ListPosition = new ObservableCollection<PositionDTO>(listPosition);
            }
        }

        /// <summary>
        /// Xác nhận thao tác nhân viên
        /// </summary>
        public async void confirmOperationEmployee()
        {
            MaskNameOperation.Visibility = Visibility.Visible;
            IsLoadingOperation = true;

            if (!Helper.checkCardID(IDCard))
            {
                MessageBoxCF ms = new MessageBoxCF("CCCD/CMND không hợp lệ", MessageType.Error, MessageButtons.OK);
                ms.ShowDialog();

                MaskNameOperation.Visibility = Visibility.Collapsed;
                IsLoadingOperation = false;
                return;
            }

            if (!Helper.checkEmail(Email))
            {
                MessageBoxCF ms = new MessageBoxCF("Email không hợp lệ", MessageType.Error, MessageButtons.OK);
                ms.ShowDialog();

                MaskNameOperation.Visibility = Visibility.Collapsed;
                IsLoadingOperation = false;
                return;
            }

            if (!Helper.checkPhone(NumberPhone))
            {
                MessageBoxCF ms = new MessageBoxCF("Số điện thoại không hợp lệ", MessageType.Error, MessageButtons.OK);
                ms.ShowDialog();

                MaskNameOperation.Visibility = Visibility.Collapsed;
                IsLoadingOperation = false;
                return;
            }

            PositionDTO positionDTO = (ListPosition.First(p => p.TenChucVu == SelectedPositionName) as PositionDTO);

            string newImage = Image;

            if (OriginImage != Image)
                newImage = await CloudService.Ins.UploadImage(Image);

            EmployeeDTO employee = new EmployeeDTO
            {
                HoTen = FullName.Trim(),
                CCCD_CMND = IDCard.Trim(),
                Email = Email.Trim(),
                SoDienThoai = NumberPhone.Trim(),
                DiaChi = Address.Trim(),
                GioiTinh = SelectedGender,
                MaChucVu = positionDTO.MaChucVu,
                Luong = Wage,
                NgayLam = WorkingDay.ToString("dd/MM/yyyy"),
                NgaySinh = Birthday.ToString("dd/MM/yyyy"),
                HinhAnh = newImage,
                TaiKhoan = Username.Trim(),
                MatKhau = Password.Trim(),
                TenChucVu = SelectedPositionName
            };

            switch (TypeOperation)
            {
                case 1:
                    (string label, EmployeeDTO NewEmployee) = await EmployeeService.Ins.createEmpoloyee(employee);

                    if (NewEmployee != null)
                    {
                        MessageBoxCF ms = new MessageBoxCF(label, MessageType.Accept, MessageButtons.OK);
                        ms.ShowDialog();
                        resetEmployee();

                        loadEmployeeList();
                    }
                    else
                    {
                        // Xoá ảnh
                        string labelClound = await CloudService.Ins.DeleteImage(newImage);

                        // Xoá user


                        // Xoá employee

                        MessageBoxCF ms = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                        ms.ShowDialog();
                    }
                    break;
                case 2:
                    employee.MaNhanVien = SelectedEmployee.MaNhanVien;

                    (string labelEdit, EmployeeDTO NewEmployeeEdit) = await EmployeeService.Ins.updateEmpoloyee(employee);

                    if (NewEmployeeEdit != null)
                    {
                        await CloudService.Ins.DeleteImage(OriginImage);

                        MessageBoxCF ms = new MessageBoxCF(labelEdit, MessageType.Accept, MessageButtons.OK);
                        ms.ShowDialog();
                        loadEmployeeList();
                    }
                    else
                    {
                        // Xoá ảnh
                        if (OriginImage != Image) 
                            await CloudService.Ins.DeleteImage(newImage);

                        // Xoá user


                        // Xoá employee

                        MessageBoxCF ms = new MessageBoxCF(labelEdit, MessageType.Error, MessageButtons.OK);
                        ms.ShowDialog();
                    }

                    break;
                default:
                    break;
            }

            MaskNameOperation.Visibility = Visibility.Collapsed;
            IsLoadingOperation = false;

        }
        #endregion
    }
}
