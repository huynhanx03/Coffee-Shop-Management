using Coffee.DTOs;
using Coffee.Models;
using Coffee.Services;
using Coffee.Utils;
using Coffee.Views.MessageBox;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Coffee.ViewModel.AdminVM.Store
{
    public partial class StoreViewModel
    {
        #region variable
        public string _Image { get; set; }
        public string Image
        {
            get { return _Image; }
            set { _Image = value; OnPropertyChanged(); }
        }

        private ObservableCollection<BannerModel> _BannerList;

        public ObservableCollection<BannerModel> BannerList
        {
            get { return _BannerList; }
            set { _BannerList = value; OnPropertyChanged(); }
        }
        #endregion

        #region ICommand
        public ICommand uploadImageIC { get; set; }
        public ICommand createBannerIC { get; set; }
        public ICommand loadBannerIC { get; set; }
        public ICommand deleteBannerIC { get; set; }

        #endregion

        #region function
        private void uploadImage()
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
                    MessageBoxCF ms = new MessageBoxCF("Tải ảnh lên thất bại", MessageType.Error, MessageButtons.OK);
                    ms.ShowDialog();
                }
            }
        }

        private async void createBanner()
        {
            MaskName.Visibility = Visibility.Visible;
            IsLoading = true;

            string newImage = await CloudService.Ins.UploadImage(Image);

            BannerModel banner = new BannerModel
            {
                HinhAnh = newImage
            };

            (string label, BannerModel bannerNew) = await BannerService.Ins.createBanner(banner);

            if (bannerNew != null) 
            {
                MessageBoxCF ms = new MessageBoxCF(label, MessageType.Accept, MessageButtons.OK);
                loadBanner();
                ms.ShowDialog();
            }
            else
            {
                MessageBoxCF ms = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                ms.ShowDialog();
            }

            MaskName.Visibility = Visibility.Collapsed;
            IsLoading = false;
        }

        /// <summary>
        /// Load danh sách banner
        /// </summary>
        private async void loadBanner()
        {
            (string label, List<BannerModel> bannerList) = await BannerService.Ins.getListBanner();

            if (bannerList != null)
            {
                BannerList = new ObservableCollection<BannerModel>(bannerList);
            }
            else
            {
                BannerList = new ObservableCollection<BannerModel>();
            }
        }

        private async void deleteBanner(BannerModel banner)
        {
            MaskName.Visibility = Visibility.Visible;

            MessageBoxCF msDelete = new MessageBoxCF("Xác nhận xoá banner này", MessageType.Waitting, MessageButtons.YesNo);

            if (msDelete.ShowDialog() == true)
            {
                IsLoading = true;

                (string label, bool isDelete) = await BannerService.Ins.DeleteBanner(banner);

                if (isDelete)
                {
                    MessageBoxCF ms = new MessageBoxCF(label, MessageType.Accept, MessageButtons.OK);
                    loadBanner();
                    ms.ShowDialog();
                }
                else
                {
                    MessageBoxCF ms = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                    ms.ShowDialog();
                }
            }

            MaskName.Visibility = Visibility.Collapsed;
            IsLoading = false;
        }

        #endregion
    }
}
