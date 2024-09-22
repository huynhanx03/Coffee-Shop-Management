using Coffee.DTOs;
using Coffee.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Coffee.ViewModel.AdminVM.Store
{
    public partial class StoreViewModel: BaseViewModel, IConstraintViewModel
    {
        #region variable

        public Grid MaskName { get; set; }

        private bool _IsLoading;

        public bool IsLoading
        {
            get { return _IsLoading; }
            set { _IsLoading = value; OnPropertyChanged(); }
        }

        #endregion

        #region ICommand
        public ICommand loadShadowMaskIC { get; set; }
        public ICommand loadDataIC { get; set; }
        
        #endregion

        public StoreViewModel()
        {
            loadShadowMaskIC = new RelayCommand<Grid>((p) => { return true; }, (p) =>
            {
                MaskName = p;
            });

            confirmCreateDiscountProductIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                confirmCreateDiscountProduct();
            });

            loadDiscountProductListIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                loadDiscountProductList();
            });

            loadDataIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                loadData();
            });

            uploadImageIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                uploadImage();
            });

            createBannerIC = new RelayCommand<object>((p) => { return !string.IsNullOrEmpty(Image); }, (p) =>
            {
                createBanner();
            });

            loadBannerIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                loadBanner();
            });

            deleteBannerIC = new RelayCommand<BannerModel>((p) => { return true; }, (p) =>
            {
                deleteBanner(p);
            });
        }

        #region function
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

        private void loadData()
        {
            MaskName.Visibility = Visibility.Visible;
            IsLoading = true;

            loadBanner();
            loadDiscountProductList();

            MaskName.Visibility = Visibility.Collapsed;
            IsLoading = false;
        }

        #endregion
    }
}
