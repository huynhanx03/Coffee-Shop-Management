using Coffee.DTOs;
using Coffee.Services;
using Coffee.Utils.Helper;
using Coffee.Utils;
using Coffee.Views.MessageBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;

namespace Coffee.ViewModel.AdminVM.Customer
{
    public partial class CustomerViewModel
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
        
        public string _HeaderOperation { get; set; }
        public string HeaderOperation
        {
            get { return _HeaderOperation; }
            set { _HeaderOperation = value; OnPropertyChanged(); }
        }

        public int TypeOperation { get; set; }
        public string OriginImage { get; set; }

        public CustomerDTO _SelectedCustomer { get; set; }
        public CustomerDTO SelectedCustomer
        {
            get { return _SelectedCustomer; }
            set { _SelectedCustomer = value; OnPropertyChanged(); }
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
        public ICommand confirmOperationCustomerIC { get; set; }
        public ICommand closeOperationCustomerWindowIC { get; set; }
        public ICommand uploadImageIC { get; set; }
        public ICommand loadShadowMaskOperationIC { get; set; }

        #endregion

        #region function
        /// <summary>
        /// Xác nhận thao tác khách hàng
        /// </summary>
        public async void confirmOperationCustomer()
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

            string newImage = Image;

            if (OriginImage != Image)
                newImage = await CloudService.Ins.UploadImage(Image);

            CustomerDTO Customer = new CustomerDTO
            {
                HoTen = FullName.Trim(),
                CCCD_CMND = IDCard.Trim(),
                Email = Email.Trim(),
                SoDienThoai = NumberPhone.Trim(),
                DiaChi = Address.Trim(),
                GioiTinh = SelectedGender,
                NgaySinh = Birthday.ToString("dd/MM/yyyy"),
                HinhAnh = newImage,
                TaiKhoan = Username.Trim(),
                MatKhau = Password.Trim(),
                NgayTao = DateTime.Now.ToString("dd/MM/yyyy")
            };

            switch (TypeOperation)
            {
                case 1:
                    (string label, CustomerDTO NewCustomer) = await CustomerService.Ins.createCustomer(Customer);

                    if (NewCustomer != null)
                    {
                        MessageBoxCF ms = new MessageBoxCF(label, MessageType.Accept, MessageButtons.OK);
                        ms.ShowDialog();
                        resetCustomer();

                        loadCustomerList();
                    }
                    else
                    {
                        // Xoá ảnh
                        string labelClound = await CloudService.Ins.DeleteImage(newImage);

                        // Xoá user


                        // Xoá Customer

                        MessageBoxCF ms = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                        ms.ShowDialog();
                    }
                    break;
                case 2:
                    Customer.MaKhachHang = SelectedCustomer.MaKhachHang;
                    Customer.NgayTao = SelectedCustomer.NgayTao;

                    (string labelEdit, CustomerDTO NewCustomerEdit) = await CustomerService.Ins.updateCustomer(Customer);

                    if (NewCustomerEdit != null)
                    {
                        await CloudService.Ins.DeleteImage(OriginImage);

                        MessageBoxCF ms = new MessageBoxCF(labelEdit, MessageType.Accept, MessageButtons.OK);
                        ms.ShowDialog();
                        loadCustomerList();
                    }
                    else
                    {
                        // Xoá ảnh
                        if (OriginImage != Image)
                            await CloudService.Ins.DeleteImage(newImage);

                        // Xoá user


                        // Xoá Customer

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
