using Coffee.DTOs;
using Coffee.Services;
using Coffee.Utils;
using Coffee.Views.Admin.TablePage;
using Coffee.Views.MessageBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Coffee.ViewModel.AdminVM.Table
{
    public partial class MainTableViewModel: BaseViewModel
    {
        #region variable
        private ObservableCollection<ProductDTO> _ProductList;
        public ObservableCollection<ProductDTO> ProductList
        {
            get { return _ProductList; }
            set { _ProductList = value; OnPropertyChanged(); }
        }

        private List<ProductDTO> __ProductList;
        private List<ProductDTO> __ProductSearchList;

        private ProductDTO _SelectedProduct;
        public ProductDTO SelectedProduct
        {
            get { return _SelectedProduct; }
            set { _SelectedProduct = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ProductTypeDTO> _ProductTypeList;
        public ObservableCollection<ProductTypeDTO> ProductTypeList
        {
            get { return _ProductTypeList; }
            set { _ProductTypeList = value; OnPropertyChanged(); }
        }

        private ProductTypeDTO _SelectedProductType;
        public ProductTypeDTO SelectedProductType
        {
            get { return _SelectedProductType; }
            set { _SelectedProductType = value; OnPropertyChanged(); }
        }

        #endregion

        #region ICommend
        public ICommand loadMenuListIC { get; set; }
        public ICommand selectedTypeProductIC { get; set; }
        public ICommand loadProductTypeListIC { get; set; }
        public ICommand searchProductIC { get; set; }
        public ICommand addProductToBillIC { get; set; }

        #endregion

        /// <summary>
        /// load danh sách menu
        /// </summary>
        private async void loadMenuList()
        {
            MaskName.Visibility = Visibility.Visible;
            IsLoading = true;

            (string label, List<ProductDTO> listProduct) = await ProductService.Ins.getListProduct();

            if (listProduct != null)
            {
                __ProductSearchList = new List<ProductDTO>(listProduct);
                __ProductList = new List<ProductDTO>(listProduct);
                ProductList = new ObservableCollection<ProductDTO>(listProduct);
            }
            else
            {
                __ProductSearchList = new List<ProductDTO>();
                __ProductList = new List<ProductDTO>();
                ProductList = new ObservableCollection<ProductDTO>();
            }

            MaskName.Visibility = Visibility.Collapsed;
            IsLoading = false;
        }

        /// <summary>
        /// Chọn loại sản phẩm
        /// </summary>
        private void selectedTypeProduct()
        {
            try
            {
                if (SelectedProductType != null)
                    if (SelectedProductType.MaLoaiSanPham == "LS0000")
                        ProductList = new ObservableCollection<ProductDTO>(__ProductSearchList);
                    else
                        ProductList = new ObservableCollection<ProductDTO>(__ProductSearchList.FindAll(p => p.MaLoaiSanPham == SelectedProductType.MaLoaiSanPham));
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// load loại sản phẩm
        /// </summary>
        private async void loadProductTypeList()
        {
            (string label, List<ProductTypeDTO> listProductType) = await ProductTypeService.Ins.getAllProductType();

            if (listProductType != null)
            {
                ProductTypeList = new ObservableCollection<ProductTypeDTO>(listProductType);
                ProductTypeList.Insert(0, new ProductTypeDTO
                {
                    MaLoaiSanPham = "LS0000",
                    LoaiSanPham = "Toàn bộ",
                });
            }
        }

        /// <summary>
        /// Thêm sản phẩm vào hoá đơn
        /// </summary>
        private async void addProductToBill()
        {
            List<DetailBillDTO> listFind = DetailBillList.Where(x => x.MaSanPham == SelectedProduct.MaSanPham).ToList();

            int totalQuantity = listFind.Sum(x => x.SoLuong);

            string productID = SelectedProduct.MaSanPham;

            // Đã hết số lượng
            if (totalQuantity == SelectedProduct.SoLuong)
            {
                MessageBoxCF ms = new MessageBoxCF("Sản phẩm đã hết hàng\nGợi ý sản phẩm?", MessageType.Waitting, MessageButtons.YesNo);
                
                MaskName.Visibility = Visibility.Visible;

                if (ms.ShowDialog() == true)
                {
                    List<ProductRecommendDTO> listProductRecommend = await RecommendSystemService.Ins.getRecommend(productID);

                    if (listProductRecommend != null)
                    {
                        ProductRecommendList = new ObservableCollection<ProductRecommendDTO>(listProductRecommend);
                        RecommendProductWindow w = new RecommendProductWindow();

                        w.ShowDialog();
                    }
                    else
                    {
                        MessageBoxCF msEr = new MessageBoxCF("Lỗi", MessageType.Error, MessageButtons.OK);
                        msEr.ShowDialog();
                    }
                }

                MaskName.Visibility = Visibility.Collapsed;
                return;
            }

            if (listFind.Count > 0)
            {
                // List size trước đó
                List<ProductSizeDetailDTO> listProductSizeDetail = listFind.Select(item => item.SelectedProductSize).ToList();

                // Add size khác
                List<ProductSizeDetailDTO> listSize = SelectedProduct.DanhSachChiTietKichThuocSanPham.Except(listProductSizeDetail).ToList();
            
                if (listSize.Count > 0)
                {
                    DetailBillList.Add(new DetailBillDTO
                    {
                        MaSanPham = SelectedProduct.MaSanPham,
                        TenSanPham = SelectedProduct.TenSanPham,
                        SoLuong = 1,
                        DanhSachChiTietKichThuocSanPham = new ObservableCollection<ProductSizeDetailDTO>(SelectedProduct.DanhSachChiTietKichThuocSanPham),
                        SelectedProductSize = listSize[0],
                        ThanhTien = listSize[0].Gia
                    });
                }
            }
            else
            {
                // Thêm vào danh sách bill mặc định sẽ là size[0]

                DetailBillList.Add(new DetailBillDTO
                {
                    MaSanPham = SelectedProduct.MaSanPham,
                    TenSanPham = SelectedProduct.TenSanPham,
                    SoLuong = 1,
                    DanhSachChiTietKichThuocSanPham = new ObservableCollection<ProductSizeDetailDTO>(SelectedProduct.DanhSachChiTietKichThuocSanPham),
                    SelectedProductSize = SelectedProduct.DanhSachChiTietKichThuocSanPham[0],
                    ThanhTien = SelectedProduct.DanhSachChiTietKichThuocSanPham[0].Gia
                });
            }

            CalculateTotalBill();
        }
    }
}
