using Coffee.DALs;
using Coffee.DTOs;
using Coffee.Services;
using Coffee.Views.MessageBox;
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
    public partial class StoreViewModel : BaseViewModel
    {
        #region variable
        private ObservableCollection<DiscountProductDTO> _DiscountProductList;

        public ObservableCollection<DiscountProductDTO> DiscountProductList
        {
            get { return _DiscountProductList; }
            set { _DiscountProductList = value; OnPropertyChanged(); }
        }

        private int _MinimumPercentage = 1;

        public int MinimumPercentage
        {
            get { return _MinimumPercentage; }
            set
            {
                if (value <= 0)
                    value = 1;

                if (value > MaximumPercentage)
                    value = MaximumPercentage;

                _MinimumPercentage = value;
                OnPropertyChanged();
            }
        }

        private int _MaximumPercentage = 1;

        public int MaximumPercentage
        {
            get { return _MaximumPercentage; }
            set
            {
                if (value >= 100)
                    value = 99;

                if (value <= 0)
                    value = 1;

                if (value < MinimumPercentage)
                    value = MinimumPercentage;

                _MaximumPercentage = value;
                OnPropertyChanged();
            }
        }

        private int _Quantity = 1;

        public int Quantity
        {
            get { return _Quantity; }
            set
            {
                if (value > 5)
                    value = 5;

                if (value <= 0)
                    value = 1;

                _Quantity = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region ICommand
        public ICommand loadDiscountProductListIC { get; set; }
        public ICommand confirmCreateDiscountProductIC { get; set; }
        #endregion

        #region function
        // Xác nhận tạo các sản phẩm giảm giá
        private async void confirmCreateDiscountProduct()
        {
            MaskName.Visibility = Visibility.Visible;
            IsLoading = true;

            Random rnd = new Random();

            (string label, List<ProductDTO> products) = await ProductService.Ins.getListProduct();

            (string labelDelete, bool isDelete) = await DiscountProductService.Ins.DeleteDiscountProductToday(products);

            if (products != null)
            {
                int sizeProduct = products.Count;

                for (int count = 0; count < Quantity; ++count)
                {
                    int index = rnd.Next(0, sizeProduct);

                    ProductDTO product = products[index];

                    products.RemoveAt(index);
                    sizeProduct -= 1;

                    DiscountProductDTO discountProduct = new DiscountProductDTO
                    {
                        MaSanPham = product.MaSanPham,
                        PhanTramGiam = rnd.Next(MinimumPercentage, MaximumPercentage + 1)
                    };

                    await DiscountProductService.Ins.createDiscountProductDTO(discountProduct);
                }

                loadDiscountProductList();
                MessageBoxCF ms = new MessageBoxCF(Application.Current.Resources["CreateVoucherSuccess"] as string, MessageType.Accept, MessageButtons.OK);
                ms.ShowDialog();
            }
            else
            {
                MessageBoxCF ms = new MessageBoxCF(Application.Current.Resources["CreateVoucherFailure"] as string, MessageType.Error, MessageButtons.OK);
                ms.ShowDialog();
            }

            MaskName.Visibility = Visibility.Collapsed;
            IsLoading = false;
        }

        /// <summary>
        /// load danh sách sản phẩm giảm giá
        /// </summary>
        private async void loadDiscountProductList()
        {
            (string label, List<DiscountProductDTO> discountProducts) = await DiscountProductService.Ins.getListDiscountProduct();

            if (discountProducts != null)
            {
                DiscountProductList = new ObservableCollection<DiscountProductDTO>(discountProducts);
            }
            else
                DiscountProductList = new ObservableCollection<DiscountProductDTO>();
        }
        #endregion
    }
}
