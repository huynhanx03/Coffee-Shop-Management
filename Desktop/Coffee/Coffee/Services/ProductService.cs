using Coffee.API;
using Coffee.DALs;
using Coffee.DTOs;
using Coffee.Models;
using Coffee.Utils;
using Coffee.Utils.Helper;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.Services
{
    public class ProductService
    {
        private static ProductService _ins;
        public static ProductService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new ProductService();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// Thêm sản phẩm
        /// </summary>
        /// <param name="product"> Sản phẩm </param>
        /// <returns>
        ///     1: Lỗi khi thêm dữ liệu
        ///     2: Sản phẩm
        /// </returns>
        public async Task<(string, ProductModel)> createProduct(ProductModel product)
        {
            return await ProductAPI.Ins.createproduct(product);

            // ProductDTO product, List<ProductSizeDetailDTO> listProductSizeDetail, List<ProductRecipeDTO> listProductRecipe

            //// Kiểm tra tên sản phẩm
            //(string label, ProductDTO productFind) = await ProductDAL.Ins.findProductByName(product.TenSanPham, product.MaSanPham);

            //if (productFind != null)
            //    return ("Tên sản phẩm đã tồn tại", null);

            //// Tìm mã sản phẩm lớn nhất
            //string maSanPhamMax = await this.getMaxMaSanPham();
            //string maSanPhamNew = Helper.nextID(maSanPhamMax, "SP");

            //// Tạo sản phẩm
            //product.MaSanPham = maSanPhamNew;

            //(string labelCreateProduct, ProductDTO productNew) = await ProductDAL.Ins.createProduct(product);

            //if (productNew != null)
            //{
            //    // Thêm chi tiết kích thước sản phẩm
            //    (string labelCreateProductSizeDetail, bool isCreateProductSizeDetail) = await ProductSizeDetailService.Ins.createProductSizeDetail(productNew.MaSanPham, listProductSizeDetail); ;

            //    // Thêm công thức sản phẩm 
            //    (string labelCreateProductRecipe, bool isCreateProductRecipe) = await ProductRecipeService.Ins.createProductRecipe(productNew.MaSanPham, listProductRecipe);

            //    if (!isCreateProductSizeDetail)
            //        return (labelCreateProductSizeDetail, null);

            //    if (!isCreateProductRecipe)
            //        return (labelCreateProductRecipe, null);

            //    return (labelCreateProduct, productNew);
            //}
            //else
            //    return (labelCreateProduct, null);
        }

        /// <summary>
        /// Cập nhật sản phẩm
        /// </summary>
        /// <param name="product"> Sản phẩm </param>
        /// <returns>
        ///     1: Thông báo
        ///     2: Sản phẩm
        /// </returns>
        public async Task<(string, ProductModel)> updateProduct(ProductModel product)
        {
            return await ProductAPI.Ins.updateProduct(product);
            //ProductDTO product, List< ProductSizeDetailDTO > listProductSizeDetail, List<ProductRecipeDTO> listProductRecipe
            //// Kiểm tra tên sản phẩm
            //(string label, ProductDTO productFind) = await ProductDAL.Ins.findProductByName(product.TenSanPham, product.MaSanPham);

            //if (productFind != null)
            //    return ("Tên sản phẩm đã tồn tại", null);

            //(string labelUpdate, ProductDTO productUpdate) = await ProductDAL.Ins.updateProduct(product);

            //if (productUpdate == null)
            //    return (labelUpdate, null);

            //// Thêm chi tiết kích thước sản phẩm
            //(string labelCreateProductSizeDetail, bool isCreateProductSizeDetail) = await ProductSizeDetailService.Ins.createProductSizeDetail(product.MaSanPham, listProductSizeDetail); ;

            //// Thêm công thức sản phẩm 
            //(string labelCreateProductRecipe, bool isCreateProductRecipe) = await ProductRecipeService.Ins.createProductRecipe(product.MaSanPham, listProductRecipe);

            //if (!isCreateProductSizeDetail)
            //    return (labelCreateProductSizeDetail, null);

            //if (!isCreateProductRecipe)
            //    return (labelCreateProductRecipe, null);

            //return (labelUpdate, productUpdate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        ///     Mã sản phẩm lớn nhất
        /// </returns>
        public async Task<string> getMaxMaSanPham()
        {
            return await ProductDAL.Ins.getMaxMaSanPham();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        ///     Lấy tên sản phâm
        /// </returns>
        public async Task<ProductDTO> getNameProduct(string id)
        {
            return await ProductDAL.Ins.getNameProduct(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        ///     Danh sách sản phẩm
        /// </returns>
        public async Task<(string, List<ProductDTO>)> getListProduct()
        {
            return await ProductAPI.Ins.getProducts();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        ///     Danh sách sản phẩm
        /// </returns>
        public async Task<(string, List<ProductRecommendDTO>)> getListProductRecommend()
        {
            return await ProductDAL.Ins.getListProductRecommend();
        }

        /// <summary>
        /// Xoá sản phẩm
        /// </summary>
        /// <param name="product"> Sản phẩm </param>
        /// <returns>
        ///     1: Thông báo
        ///     2: True nếu xoá thành công, False xoá thất bại
        /// </returns>
        public async Task<(string, bool)> DeleteProduct(ProductDTO product)
        {
            await CloudService.Ins.DeleteImage(product.HinhAnh);

            return await ProductAPI.Ins.deleteProduct(product.MaSanPham);
        }

        /// <summary>
        /// Tăng số lượng sản phẩm
        /// </summary>
        /// <param name="productID"> mã sản phẩm </param>
        /// <param name="quantity"> số lượng </param>
        /// <returns>\
        ///     1. Thông báo
        ///     2. True nếu xoá thành công, False xoá thất bại
        /// </returns>
        public async Task<(string, bool)> increaseQuantityProduct(ProductDTO product, int quantity)
        {
            if (product != null)
            {
                product.SoLuong += quantity;
                return await ProductAPI.Ins.updateQuantityProduct(product.MaSanPham, product.SoLuong);
            }
            else
                return ("Lỗi khi cập nhật số lượng sản phẩm", false);
        }

        /// <summary>
        /// Tăng số lượng sản phẩm
        /// </summary>
        /// <param name="productID"> mã sản phẩm </param>
        /// <param name="quantity"> số lượng </param>
        /// <returns>\
        ///     1. Thông báo
        ///     2. True nếu xoá thành công, False xoá thất bại
        /// </returns>
        public async Task<(string, bool)> reduceQuantityProduct(ProductDTO product, int quantity)
        {
            //(string label, ProductDTO product) = await ProductDAL.Ins.findProduct(productID);

            product.SoLuong -= quantity;
            return await ProductAPI.Ins.updateQuantityProduct(product.MaSanPham, product.SoLuong);
        }

        public async Task<(string, bool)> reduceQuantityProduct(string productID, int quantity)
        {
            (string label, ProductDTO product) = await ProductAPI.Ins.getProduct(productID);

            if (product != null)
            {
                product.SoLuong -= quantity;
                return await ProductAPI.Ins.updateQuantityProduct(productID, product.SoLuong);
            }
            else
                return (label, false);
        }
    }
}
