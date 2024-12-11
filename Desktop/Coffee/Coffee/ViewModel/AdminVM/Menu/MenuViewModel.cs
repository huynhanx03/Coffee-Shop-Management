using CloudinaryDotNet.Actions;
using Coffee.DTOs;
using Coffee.Services;
using Coffee.Views.Admin.MenuPage;
using Coffee.Views.MessageBox;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Coffee.ViewModel.AdminVM.Menu
{
    public partial class MenuViewModel: BaseViewModel
    {
        #region variable
        public Grid MaskName { get; set; }

        private ObservableCollection<ProductDTO> _ProductList;

        public ObservableCollection<ProductDTO> ProductList
        {
            get { return _ProductList; }
            set { _ProductList = value; OnPropertyChanged(); }
        }

        private List<ProductDTO> __ProductList;
        private ProductDTO _SelectedProduct;
        public ProductDTO SelectedProduct
        {
            get { return _SelectedProduct; }
            set { _SelectedProduct = value; OnPropertyChanged(); }
        }

        private int maxProductQuantity;

        private int _ProductQuantity;

        public int ProductQuantity
        {
            get { return _ProductQuantity; }
            set 
            {
                if (value <= 0)
                    _ProductQuantity = 1;
                else if (value > maxProductQuantity)
                    _ProductQuantity = maxProductQuantity;
                else
                    _ProductQuantity = value;

                OnPropertyChanged();
            }
        }

        public string _HeaderOperation { get; set; }
        public string HeaderOperation
        {
            get { return _HeaderOperation; }
            set { _HeaderOperation = value; OnPropertyChanged(); }
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
        public ICommand openWindowAddProductIC { get; set; }
        public ICommand loadProductListIC { get; set; }
        public ICommand searchItemIC { get; set; }
        public ICommand uploadImageIC { get; set; }
        public ICommand deleteProductIC { get; set; }
        public ICommand editProductIC { get; set; }
        public ICommand addQuantityProductIC { get; set; }
        public ICommand confirmAddQuantityProductIC { get; set; }
        public ICommand openWindowEditProductIC { get; set; }
        #endregion

        public MenuViewModel()
        {
            loadShadowMaskIC = new RelayCommand<Grid>((p) => { return true; }, (p) =>
            {
                MaskName = p;
            });

            loadProductListIC = new RelayCommand<Grid>((p) => { return true; }, (p) =>
            {
                loadProductList();
            });

            loadShadowMaskOperationIC = new RelayCommand<Grid>((p) => { return true; }, (p) =>
            {
                MaskNameOperation = p;
            });

            openWindowAddProductIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                openWindowAddProduct();
            });

            openWindowEditProductIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                openWindowEditProduct();
            });

            deleteProductIC = new RelayCommand<ProductDTO>((p) => { return true; }, (p) =>
            {
                deleteProduct();
            });

            editProductIC = new RelayCommand<ProductDTO>((p) => { return true; }, (p) =>
            {
                editProduct();
            });

            searchItemIC = new RelayCommand<TextBox>(null, (p) =>
            {
                if (p.Text != null)
                {
                    if (__ProductList != null)
                        ProductList = new ObservableCollection<ProductDTO>(__ProductList.FindAll(x => x.TenSanPham.ToLower().Contains(p.Text.ToLower())));
                }
            });

            addQuantityProductIC = new RelayCommand<ProductDTO>((p) => { return true; }, (p) =>
            {
                addQuantityProduct();
            });

            confirmAddQuantityProductIC = new RelayCommand<object>((p) =>
            { 
                return SelectedProduct != null && ProductQuantity > 0; 
            },
            (p) =>
            {
                confirmAddQuantityProduct();
            });

            #region operation
            confirmOperationProductIC = new RelayCommand<object>((p) =>
            {
                return !(string.IsNullOrEmpty(ProductName)
                        || string.IsNullOrEmpty(SelectedProdcutTypeName)
                        || string.IsNullOrEmpty(Image)
                        || string.IsNullOrEmpty(Description));
            },
            (p) =>
            {
                confirmOperationProduct();
            });

            closeOperationProductWindowIC = new RelayCommand<Window>((p) => { return true; }, (p) =>
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
        /// Load danh sách sản phẩm
        /// </summary>
        private async void loadProductList()
        {
            MaskName.Visibility = Visibility.Visible;
            IsLoading = true;

            (string label, List<ProductDTO> pr) = await ProductService.Ins.getListProduct();

            if (pr != null)
            {
                ProductList = new ObservableCollection<ProductDTO>(pr);
                __ProductList = new List<ProductDTO>(pr);
            }
            else
            {
                ProductList = new ObservableCollection<ProductDTO>();
                __ProductList = new List<ProductDTO>();
            }

            MaskName.Visibility = Visibility.Collapsed;
            IsLoading = false;
        }


        /// <summary>
        /// Load dữ liệu sản phẩm click
        /// </summary>
        /// <param name="product"> Sản phẩm </param>
        private void loadProduct(ProductDTO product)
        {
            //MaskName.Visibility = Visibility.Visible;
            //IsLoading = true;

            ProductName = product.TenSanPham;
            SelectedProdcutTypeName = product.LoaiSanPham;
            Quantity = product.SoLuong;
            ListProductSizeDetail = new ObservableCollection<ProductSizeDetailDTO>(product.DanhSachChiTietKichThuocSanPham);
            ProductRecipeList = new ObservableCollection<ProductRecipeDTO>(product.DanhSachCongThuc);
            Description = product.Mota;
            Image = product.HinhAnh;

            //MaskName.Visibility = Visibility.Collapsed;
            //IsLoading = false;
        }

        /// <summary>
        /// Reset dữ liệu sản phâm trên cửa sổ thao tác của sản phẩm
        /// </summary>
        private void resetProduct()
        {
            ProductName = "";
            ProductType = "";
            Quantity = 0;
            loadProductSizeDetail();
            ProductRecipeList = new ObservableCollection<ProductRecipeDTO>();
            Description = "";
            Image = "";
        }

        /// <summary>
        /// Xoá sản phẩm
        /// </summary>
        public async void deleteProduct()
        {
            MaskName.Visibility = Visibility.Visible;
            
            MessageBoxCF ms = new MessageBoxCF(Application.Current.Resources["ConfirmDeleteProduct"] as string, MessageType.Waitting, MessageButtons.YesNo);

            if (ms.ShowDialog() == true)
            {
                IsLoading = true;

                (string label, bool isDeleteProduct) = await ProductService.Ins.DeleteProduct(SelectedProduct);
                
                IsLoading = false;

                if (isDeleteProduct)
                {
                    MessageBoxCF msn = new MessageBoxCF(label, MessageType.Accept, MessageButtons.OK);
                    loadProductList();
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
        /// Mở cửa sổ thêm sản phẩm
        /// </summary>
        public void openWindowAddProduct()
        {
            HeaderOperation = (string)Application.Current.Resources["AddProduct"];

            MaskName.Visibility = Visibility.Visible;
            resetProduct();
            loadProductType();
            loadProductSizeDetail();
            loadIngredientList();
            loadUnitList();
            OperationProductWindow w = new OperationProductWindow();
            TypeOperation = 1; // Add product
            w.ShowDialog();
            MaskName.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Mở cửa sổ thêm sản phẩm
        /// </summary>
        public void openWindowEditProduct()
        {
            HeaderOperation = (string)Application.Current.Resources["EditProduct"];

            MaskName.Visibility = Visibility.Visible;
            loadProduct(SelectedProduct);
            loadProductType();
            loadProductSizeDetail();
            loadIngredientList();
            loadUnitList();
            OperationProductWindow w = new OperationProductWindow();
            TypeOperation = 2; // Edit product
            w.ShowDialog();
            MaskName.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Chỉnh sửa sản phẩm
        /// </summary>
        private void editProduct()
        {

        }

        /// <summary>
        /// Thêm số lượng sản phẩm
        /// </summary>
        private async void addQuantityProduct()
        {
            // Tính giá trị lớn nhất có thể tạo được
            (string label, int maxProductQuantity) = await IngredientService.Ins.getMaxIngredientQuantity(SelectedProduct.DanhSachCongThuc);

            this.maxProductQuantity = maxProductQuantity;
        }

        /// <summary>
        /// Xác nhận thêm số lượng
        /// </summary>
        private async void confirmAddQuantityProduct()
        {
            MaskName.Visibility = Visibility.Visible;
            IsLoading = true;

            ProductDTO currentProduct = SelectedProduct;

            // Giảm bớt số lượng
            await IngredientService.Ins.reduceIngredientQuantity(currentProduct.DanhSachCongThuc, ProductQuantity);

            // Thêm số lượng cho sản phẩm
            (string label, bool isIncrease) = await ProductService.Ins.increaseQuantityProduct(currentProduct, ProductQuantity);
            
            IsLoading = false;

            if (isIncrease)
            {
                ProductQuantity = 0;

                MessageBoxCF ms = new MessageBoxCF(Application.Current.Resources["AddQuatityProductSuccess"] as string, MessageType.Accept, MessageButtons.OK);
                ms.ShowDialog();

                addQuantityProduct();
                loadProductList();
            }
            else
            {
                MessageBoxCF ms = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                ms.ShowDialog();
            }

            MaskName.Visibility = Visibility.Collapsed;
        }
    }
}
