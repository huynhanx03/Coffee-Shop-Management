using Coffee.API;
using Coffee.DTOs;
using Coffee.Services;
using Coffee.Utils;
using Coffee.Utils.Helper;
using Coffee.Views.Admin;
using Coffee.Views.Admin.EmployeePage;
using Coffee.Views.Login;
using Coffee.Views.MessageBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Coffee.ViewModel.LoginVM
{
    public class LoginViewModel: BaseViewModel
    {
        #region variable
        private string _UsernameLogin;

        public string UsernameLogin
        {
            get { return _UsernameLogin; }
            set { _UsernameLogin = value; OnPropertyChanged(); }
        }

        private string _PasswordLogin;

        public string PasswordLogin
        {
            get { return _PasswordLogin; }
            set { _PasswordLogin = value; OnPropertyChanged(); }
        }

        public Grid mask { get; set; }
        public bool _IsLogin;
        public bool IsLogin
        {
            get { return _IsLogin; }
            set { _IsLogin = value; OnPropertyChanged(); }
        }

        private bool isCheckLogin = true;

        #endregion

        #region ICommend
        public ICommand loadLoginPageIC { get; set; }
        public ICommand savePasswordChangedCF { get; set; }
        public ICommand loginIC { get; set; }
        public ICommand loadMaskIC { get; set; }
        public ICommand firstLoadIC { get; set; }

        #endregion

        public LoginViewModel()
        {
            firstLoadIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (isCheckLogin) checkLogin();
            });

            loadMaskIC = new RelayCommand<Grid>((p) => { return true; }, (p) =>
            {
                mask = p;
            });

            loadLoginPageIC = new RelayCommand<Frame>((p) => { return true; }, (p) =>
            {
                p.Content = new LoginPage();
            });

            savePasswordChangedCF = new RelayCommand<PasswordBox>((p) => { return true; }, (p) =>
            {
                PasswordLogin = p.Password;
            });

            loginIC = new RelayCommand<object>((p) => 
            { 
                return !(string.IsNullOrEmpty(UsernameLogin) || string.IsNullOrEmpty(PasswordLogin)); 
            }, 
            async (p) =>
            {
                IsLogin = true;
                mask.Visibility = Visibility.Visible;

                (string label, UserDTO user) = await UserService.Ins.findUser(UsernameLogin, PasswordLogin);
                
                // Tìm thấy được tài khoản
                if (user != null)
                {
                    // Lưu user
                    Memory.user = user;

                    // Mở cửa sổ
                    MainAdminWindow w = new MainAdminWindow();
                    w.Show();

                    // Đóng của sổ login
                    LoginWindow wLogin = Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault();
                    wLogin.Close();
                }
                else // Không tìm thấy tài khoản - đăng nhập thất bại
                {
                    MessageBoxCF ms = new MessageBoxCF("Tài khoản hoặc mật khẩu không chính xác", MessageType.Error, MessageButtons.OK);
                    ms.ShowDialog();
                }

                PasswordLogin = "";
                IsLogin = false;
                mask.Visibility = Visibility.Collapsed;
            });
        }

        public async void checkLogin()
        {
            isCheckLogin = false;

            IsLogin = true;
            mask.Visibility = Visibility.Visible;

            (string label, bool isCheck) = await UserService.Ins.checkToken();

            if (isCheck)
            {
                (string labelGetUser, UserDTO user) = await UserService.Ins.GetUser(Helper.getUserID());

                if (user != null)
                {
                    // Lưu user
                    Memory.user = user;

                    // Mở cửa sổ
                    MainAdminWindow w = new MainAdminWindow();
                    w.Show();

                    // Đóng của sổ login
                    LoginWindow wLogin = Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault();
                    wLogin.Close();
                }
            }

            IsLogin = false;
            mask.Visibility = Visibility.Collapsed;
        }
    }
}
