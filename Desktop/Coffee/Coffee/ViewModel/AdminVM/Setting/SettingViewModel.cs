using Coffee.DTOs;
using Coffee.Services;
using Coffee.Utils;
using Coffee.Utils.Helper;
using Coffee.Views.MessageBox;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Coffee.ViewModel.AdminVM.Setting
{
    public partial class SettingViewModel : BaseViewModel, IConstraintViewModel
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
        public string OriginImage { get; set; }

        public string _Password { get; set; }
        public string Password
        {
            get { return _Password; }
            set { _Password = value; OnPropertyChanged(); }
        }

        private bool _IsLoading;

        public bool IsLoading
        {
            get { return _IsLoading; }
            set { _IsLoading = value; }
        }

        private Grid Mask {  get; set; }
        #endregion

        #region Icommand
        public ICommand uploadImageIC {  get; set; }
        public ICommand confirmUserIC {  get; set; }
        public ICommand loadShadowMaskIC {  get; set; }
        public ICommand loadDataIC {  get; set; }
        #endregion

        #region

        #endregion
        public SettingViewModel()
        {
            ListGender = new ObservableCollection<string>{
                (string)Application.Current.Resources["Male"],
                (string)Application.Current.Resources["Female"],
                (string)Application.Current.Resources["Other"],
            };

            uploadImageIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                uploadImage();
            });

            loadDataIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                LoadThongTin();
            });
            confirmUserIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                confirmUser();
            });

            loadShadowMaskIC = new RelayCommand<Grid>((p) => { return true; }, (p) =>
            {
                Mask = p;
            });
        }


        public void LoadThongTin()
        {
            Mask.Visibility = Visibility.Visible;
            IsLoading = true;

            UserDTO user = Memory.user;
            OriginImage = user.HinhAnh;
            FullName = user.HoTen;
            Email = user.Email;
            SelectedGender = user.GioiTinh;
            Username = user.TaiKhoan;
            Password = user.MatKhau;
            NumberPhone = user.SoDienThoai;
            IDCard = user.CCCD_CMND;
            Address = user.DiaChi;
            Birthday = Convert.ToDateTime(user.NgaySinh);
            WorkingDay = Convert.ToDateTime(user.NgayTao);
            Image = user.HinhAnh;

            Mask.Visibility = Visibility.Collapsed;
            IsLoading = false;
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

        public async void confirmUser()
        {
            Mask.Visibility = Visibility.Visible;
            IsLoading = true;

            if (!Helper.checkCardID(IDCard))
            {
                MessageBoxCF ms = new MessageBoxCF(Application.Current.Resources["CCCDCMNDInvalid"] as string, MessageType.Error, MessageButtons.OK);
                ms.ShowDialog();

                Mask.Visibility = Visibility.Collapsed;
                IsLoading = false;
                return;
            }

            if (!Helper.checkEmail(Email))
            {
                MessageBoxCF ms = new MessageBoxCF(Application.Current.Resources["EmailInvalid"] as string, MessageType.Error, MessageButtons.OK);
                ms.ShowDialog();

                Mask.Visibility = Visibility.Collapsed;
                IsLoading = false;
                return;
            }

            if (!Helper.checkPhone(NumberPhone))
            {
                MessageBoxCF ms = new MessageBoxCF(Application.Current.Resources["NumberPhoneInvalid"] as string, MessageType.Error, MessageButtons.OK);
                ms.ShowDialog();

                Mask.Visibility = Visibility.Collapsed;
                IsLoading = false;
                return;
            }


            string newImage = Image;

            if (OriginImage != Image)
                newImage = await CloudService.Ins.UploadImage(Image);

            UserDTO user = new UserDTO
            {
                MaNguoiDung = Memory.user.MaNguoiDung,
                HoTen = FullName.Trim(),
                CCCD_CMND = IDCard.Trim(),
                Email = Email.Trim(),
                SoDienThoai = NumberPhone.Trim(),
                DiaChi = Address.Trim(),
                GioiTinh = SelectedGender,
                NgayTao = WorkingDay.ToString("dd/MM/yyyy"),
                NgaySinh = Birthday.ToString("dd/MM/yyyy"),
                HinhAnh = newImage,
                TaiKhoan = Username.Trim(),
                MatKhau = Password.Trim(),
            };

            (string labelEdit, UserDTO userEdit) = await UserService.Ins.updateUser(user);
            
            IsLoading = false;

            if (userEdit != null)
            {
                MessageBoxCF msa = new MessageBoxCF(Application.Current.Resources["UpdateInformationSuccess"] as string, MessageType.Accept, MessageButtons.OK);
                msa.ShowDialog();

                await CloudService.Ins.DeleteImage(OriginImage);

                Memory.user = user;
            }
            else
            {
                MessageBoxCF msa = new MessageBoxCF(labelEdit, MessageType.Error, MessageButtons.OK);
                msa.ShowDialog();
            }

            Mask.Visibility = Visibility.Collapsed;
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
