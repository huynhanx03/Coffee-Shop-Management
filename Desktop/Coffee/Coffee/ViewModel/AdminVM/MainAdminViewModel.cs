using Coffee.DesignPattern.Mediator;
using Coffee.Properties;
using Coffee.Utils;
using Coffee.Views.Admin;
using Coffee.Views.Admin.ChatPage;
using Coffee.Views.Admin.CustomerPage;
using Coffee.Views.Admin.EmployeePage;
using Coffee.Views.Admin.EvaluatePage;
using Coffee.Views.Admin.IngredientPage;
using Coffee.Views.Admin.MenuPage;
using Coffee.Views.Admin.OrderPage;
using Coffee.Views.Admin.SettingPage;
using Coffee.Views.Admin.StatisticPage;
using Coffee.Views.Admin.StorePage;
using Coffee.Views.Admin.TablePage;
using Coffee.Views.Admin.VoucherPage;
using Coffee.Views.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Coffee.ViewModel.AdminVM
{
    public class MainAdminViewModel: BaseViewModel
    {
        public int _role { get; set; }
        public int role 
        {
            get { return _role; }
            set { _role = value; OnPropertyChanged(); } 
        }

        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; OnPropertyChanged(); }
        }

        private string _Image;
        public string Image
        {
            get { return _Image; }
            set { _Image = value; OnPropertyChanged(); }
        }


        private int _orderCount;
        public int orderCount
        {
            get { return _orderCount; }
            set { _orderCount = value; OnPropertyChanged(); }
        }

        private string _optionName { get; set; }
        public string optionName
        {
            get { return _optionName; }
            set { _optionName = value; OnPropertyChanged(); }
        }

        private RadioButton settingBtn {  get; set; }

        public ICommand loadTablesPageIC { get; set; }
        public ICommand loadMenuPageIC { get; set; }
        public ICommand loadIngredientsPageIC { get; set; }
        public ICommand loadStatisticPageIC { get; set; }
        public ICommand loadEmployeePageIC { get; set; }
        public ICommand loadSettingPageIC { get; set; }
        public ICommand loadChatPageIC { get; set; }
        public ICommand loadRoleIC { get; set; }
        public ICommand loadVoucherPageIC { get; set; }
        public ICommand loadEvaluatePageIC { get; set; }
        public ICommand loadStorePageIC { get; set; }
        public ICommand loadCustomerPageIC { get; set; }
        public ICommand loadOrderPageIC { get; set; }
        public ICommand changeLanguageIC { get; set; }
        public ICommand loguoutIC { get; set; }
        public ICommand loadSettingButtonIC { get; set; }
        public ICommand clickAvatarIC { get; set; }
        public ICommand loadBeginningIC { get; set; }


        public MainAdminViewModel()
        {
            if (ConcreteMediator.Ins.mainAdminViewModel == null) ConcreteMediator.Ins.mainAdminViewModel = this;

            Image = Memory.user.HinhAnh;
            Name = Memory.user.HoTen;

            loadBeginningIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                loadBeginning();
            });

            loadRoleIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                role = 1;
            });
            
            clickAvatarIC = new RelayCommand<Frame>((p) => { return true; }, (p) =>
            {
                settingBtn.IsChecked = true;
                p.Content = new MainSettingPage();
                optionName = "Cài đặt";
            });

            loadSettingButtonIC = new RelayCommand<RadioButton>((p) => { return true; }, (p) =>
            {
                settingBtn = p;
            });

            loadTablesPageIC = new RelayCommand<Frame>((p) => { return true; }, (p) =>
            {
                p.Content = new MainTablePage();
                optionName = "Trang chủ";
            });

            loadMenuPageIC = new RelayCommand<Frame>((p) => { return true; }, (p) =>
            {
                p.Content = new MainMenuPage();
                optionName = "Quản lý thực đơn";
            });

            loadIngredientsPageIC = new RelayCommand<Frame>((p) => { return true; }, (p) =>
            {
                p.Content = new MainIngredientPage();
                optionName = "Quản lý nguyên liệu";
            });

            loadStatisticPageIC = new RelayCommand<Frame>((p) => { return true; }, (p) =>
            {
                p.Content = new MainStatisticPage();
                optionName = "Thống kê";
            });

            loadEmployeePageIC = new RelayCommand<Frame>((p) => { return true; }, (p) =>
            {
                p.Content = new MainEmployeePage();
                optionName = "Quản lý nhân viên";
            });

            loadSettingPageIC = new RelayCommand<Frame>((p) => { return true; }, (p) =>
            {
                p.Content = new MainSettingPage();
                optionName = "Cài đặt";
                //optionName = (string)Application.Current.Resources["Setting"];
            });

            loadChatPageIC = new RelayCommand<Frame>((p) => { return true; }, (p) =>
            {
                p.Content = new MainChatPage();
                optionName = "Nhắn tin";
            });

            loadVoucherPageIC = new RelayCommand<Frame>((p) => { return true; }, (p) =>
            {
                p.Content = new MainVoucherPage();
                optionName = "Quản lý phiếu giảm giá";
            });

            loadEvaluatePageIC = new RelayCommand<Frame>((p) => { return true; }, (p) =>
            {
                p.Content = new MainEvaluatePage();
                optionName = "Quản lý đánh giá";
            });

            loadStorePageIC = new RelayCommand<Frame>((p) => { return true; }, (p) =>
            {
                p.Content = new MainStorePage();
                optionName = "Quản lý thông tin quán";
            });
            
            loadCustomerPageIC = new RelayCommand<Frame>((p) => { return true; }, (p) =>
            {
                p.Content = new MainCustomerPage();
                optionName = "Quản lý khách hàng";
            });

            loadOrderPageIC = new RelayCommand<Frame>((p) => { return true; }, async (p) =>
            {
                p.Content = new MainOrderPage();
                optionName = "Quản lý đơn hàng";

                await ConcreteMediator.Ins.Notify(this, "LoadOrderList");
            });

            loguoutIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                logout();
            });

            changeLanguageIC = new RelayCommand<Button>((p) => { return true; }, (p) =>
            {
                ResourceDictionary dic = new ResourceDictionary();

                switch (p.Name)
                {
                    case "btnVi":
                        dic.Source = new Uri("..\\ResourcesXAML\\Languages\\LanguageVi.xaml", UriKind.Relative);
                        break;

                    case "btnEn":
                        dic.Source = new Uri("..\\ResourcesXAML\\Languages\\LanguageEn.xaml", UriKind.Relative);
                        break;

                    default:
                        dic.Source = new Uri("..\\ResourcesXAML\\Languages\\LanguageVi.xaml", UriKind.Relative);
                        break;
                }

                Application.Current.Resources.MergedDictionaries.Add(dic);
            });
        }

        private void logout()
        {
            MainAdminWindow wAdmin = Application.Current.Windows.OfType<MainAdminWindow>().FirstOrDefault();
            wAdmin.Visibility = Visibility.Collapsed;

            // Hiện lại cửa sở login
            LoginWindow wLogin = new LoginWindow();
            wLogin.Show();

            // Đóng của sổ hiện tại
            wAdmin.Close();
        }

        private async void loadBeginning()
        {
        }

        public void UpdateOrderCount(int amount)
        {
            orderCount = amount;
        }
    }
}
