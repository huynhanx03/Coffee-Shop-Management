using Coffee.DTOs;
using Coffee.Services;
using Coffee.Utils.Helper;
using Coffee.Utils;
using Coffee.Views.MessageBox;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using Coffee.API;
using Coffee.Models;

namespace Coffee.ViewModel.AdminVM.Menu
{
    public partial class MenuViewModel: BaseViewModel
    {
        #region Variable
        private int TypeOperation { get; set; }

        private ObservableCollection<ProductSizeDTO> _ListProductSize;

        public ObservableCollection<ProductSizeDTO> ListProductSize
        {
            get { return _ListProductSize; }
            set { _ListProductSize = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ProductSizeDetailDTO> _ListProductSizeDetail;

        public ObservableCollection<ProductSizeDetailDTO> ListProductSizeDetail
        {
            get { return _ListProductSizeDetail; }
            set { _ListProductSizeDetail = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ProductTypeDTO> _ListProductType;

        public ObservableCollection<ProductTypeDTO> ListProductType
        {
            get { return _ListProductType; }
            set { _ListProductType = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ProductRecipeDTO> _ProductRecipeList = new ObservableCollection<ProductRecipeDTO>();

        public ObservableCollection<ProductRecipeDTO> ProductRecipeList
        {
            get { return _ProductRecipeList; }
            set { _ProductRecipeList = value; OnPropertyChanged(); }
        }

        private ProductRecipeDTO _SelectProductRecipe;
        public ProductRecipeDTO SelectProductRecipe
        {
            get { return _SelectProductRecipe; }
            set { _SelectProductRecipe = value; OnPropertyChanged(); }
        }

        private IngredientDTO _SelectedIngredient;

        public IngredientDTO SelectedIngredient
        {
            get { return _SelectedIngredient; }
            set { _SelectedIngredient = value; OnPropertyChanged(); }
        }


        private string _ProductName;

        public string ProductName
        {
            get { return _ProductName; }
            set { _ProductName = value; OnPropertyChanged(); }
        }

        private string _ProductType;

        public string ProductType
        {
            get { return _ProductType; }
            set { _ProductType = value; OnPropertyChanged(); }
        }

        private int _Quantity;

        public int Quantity
        {
            get { return _Quantity; }
            set { _Quantity = value; OnPropertyChanged(); }
        }

        private decimal _Price;

        public decimal Price
        {
            get { return _Price; }
            set { _Price = value; OnPropertyChanged(); }
        }

        private string OriginImage;
        private string _Image;

        public string Image
        {
            get { return _Image; }
            set { _Image = value; OnPropertyChanged(); }
        }

        private string _Description;

        public string Description
        {
            get { return _Description; }
            set { _Description = value; OnPropertyChanged(); }
        }

        private ObservableCollection<IngredientDTO> _IngredientList;

        public ObservableCollection<IngredientDTO> IngredientList
        {
            get { return _IngredientList; }
            set { _IngredientList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<UnitDTO> _UnitList;

        public ObservableCollection<UnitDTO> UnitList
        {
            get { return _UnitList; }
            set { _UnitList = value; OnPropertyChanged(); }
        }

        private string _SelectedProdcutTypeName;

        public string SelectedProdcutTypeName
        {
            get { return _SelectedProdcutTypeName; }
            set { _SelectedProdcutTypeName = value; OnPropertyChanged(); }
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
        public ICommand confirmOperationProductIC { get; set; }
        public ICommand closeOperationProductWindowIC { get; set; }
        public ICommand loadShadowMaskOperationIC { get; set; }


        #endregion

        #region function
        /// <summary>
        /// Lấy danh sách kích thước sản phẩm
        /// </summary>
        public async void loadProductSize()
        {
            (string label, List<ProductSizeDTO> listProductSize) = await ProductSizeService.Ins.getAllProductSzie();

            if (listProductSize != null)
            {
                ListProductSize = new ObservableCollection<ProductSizeDTO>(listProductSize);
            }
            else
            {
                MessageBoxCF ms = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                ms.ShowDialog();
            }
        }

        /// <summary>
        /// Lấy danh sách loại sản phẩm
        /// </summary>
        public async void loadProductType()
        {
            (string label, List<ProductTypeDTO> listProductType) = await ProductTypeService.Ins.getAllProductType();

            if (listProductType != null)
            {
                ListProductType = new ObservableCollection<ProductTypeDTO>(listProductType);
            }
            else
            {
                MessageBoxCF ms = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                ms.ShowDialog();
            }
        }

        /// <summary>
        /// Lấy danh sách chi tiết kích thước sản phẩm
        /// </summary>
        public async void loadProductSizeDetail()
        {
            (string label, List<ProductSizeDetailDTO> listProductSizeDetail) = await ProductSizeDetailService.Ins.getAllProductSzieDetail();

            if (listProductSizeDetail != null)
            {
                ListProductSizeDetail = new ObservableCollection<ProductSizeDetailDTO>(listProductSizeDetail);
            }
            else
            {
                MessageBoxCF ms = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                ms.ShowDialog();
            }
        }

        /// <summary>
        /// Xác nhận thao tác sản phẩm
        /// </summary>
        public async void confirmOperationProduct()
        {
            MaskNameOperation.Visibility = Visibility.Visible;
            IsLoadingOperation = true;

            ProductTypeDTO productType = (ListProductType.First(p => p.LoaiSanPham == SelectedProdcutTypeName) as ProductTypeDTO);
            string newImage = Image;

            if (OriginImage != Image)
                newImage = await CloudService.Ins.UploadImage(Image);

            ProductModel product = new ProductModel
            {
                TenSanPham = ProductName.Trim(),
                MaLoaiSanPham = productType.MaLoaiSanPham,
                HinhAnh = newImage,
                Mota = Description.Trim(),
                CongThuc = ProductRecipeList.ToDictionary(recipe => recipe.MaNguyenLieu),
                ChiTietKichThuocSanPham = ListProductSizeDetail.ToDictionary(size => size.MaKichThuoc)
            };

            switch (TypeOperation)
            {
                case 1:
                    (string label, ProductModel NewProduct) = await ProductService.Ins.createProduct(product);

                    if (NewProduct != null)
                    {
                        MessageBoxCF ms = new MessageBoxCF(label, MessageType.Accept, MessageButtons.OK);
                        ms.ShowDialog();
                        resetProduct();

                        loadProductList();
                    }
                    else
                    {
                        // Xoá ảnh
                        string labelClound = await CloudService.Ins.DeleteImage(newImage);

                        MessageBoxCF ms = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                        ms.ShowDialog();
                    }
                    break;
                case 2:
                    product.MaSanPham = SelectedProduct.MaSanPham;
                    product.SoLuong = Quantity;
                    product.PhanTramGiam = SelectedProduct.PhanTramGiam;

                    (string labelEdit, ProductModel NewProductEdit) = await ProductService.Ins.updateProduct(product);

                    if (NewProductEdit != null)
                    {
                        MessageBoxCF ms = new MessageBoxCF(labelEdit, MessageType.Accept, MessageButtons.OK);
                        ms.ShowDialog();
                        loadProductList();
                    }
                    else
                    {
                        // Xoá ảnh
                        if (OriginImage != Image)
                            await CloudService.Ins.DeleteImage(newImage);

                        // Xoá product

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

        /// <summary>
        /// Load list nguyên liệu
        /// </summary>
        public async void loadIngredientList()
        {
            (string label, List<IngredientDTO> ingredients) = await IngredientService.Ins.getListIngredient();

            if (ingredients != null)
            {
                IngredientList = new ObservableCollection<IngredientDTO>(ingredients);
            }
        }

        /// <summary>
        /// load list đơn vị
        /// </summary>
        public async void loadUnitList()
        {
            (string label, List<UnitDTO> units) = await UnitService.Ins.getAllUnit();

            if (units != null)
                UnitList = new ObservableCollection<UnitDTO>(units);
        }

        /// <summary>
        /// Thêm nguyên liệu vào công thức 
        /// </summary>
        public void addIngredientToRecipe()
        {
            // Kiểm tra nguyên liệu đã tồn tại chưa
            // Đã tồn tại thì số lượng +1
            foreach (var productRecipe in ProductRecipeList)
            {
                if (productRecipe.MaNguyenLieu == SelectedIngredient.MaNguyenLieu)
                {
                    productRecipe.SoLuong += 1;
                    ProductRecipeList = new ObservableCollection<ProductRecipeDTO>(ProductRecipeList);
                    return;
                }
            }

            // Chưa tồn tại thì thêm vào
            ProductRecipeList.Add(new ProductRecipeDTO
            {
                MaNguyenLieu = SelectedIngredient.MaNguyenLieu,
                TenNguyenLieu = SelectedIngredient.TenNguyenLieu,
                MaDonVi = SelectedIngredient.MaDonVi,
                TenDonVi = SelectedIngredient.TenDonVi,
                SoLuong = 1,
            });
        }

        /// <summary>
        /// Xoá nguyên liệu khỏi công thức
        /// </summary>
        public void removeRecipe()
        {
            ProductRecipeList.Remove(SelectProductRecipe);
            ProductRecipeList = new ObservableCollection<ProductRecipeDTO>(ProductRecipeList);
        }
    }
}
