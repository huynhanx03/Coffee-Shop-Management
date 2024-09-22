using Coffee.DTOs;
using Coffee.Utils;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DALs
{
    public class ProductSizeDetailDAL
    {
        private static ProductSizeDetailDAL _ins;
        public static ProductSizeDetailDAL Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new ProductSizeDetailDAL();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        ///     Trả về danh sách chi tiết kích thước sản phẩm
        /// </returns>
        public async Task<(string, List<ProductSizeDetailDTO>)> getAllProductSzieDetail()
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("KichThuocSanPham");

                    if (response.Body != null && response.Body != "null")
                    {
                        Dictionary<string, ProductSizeDetailDTO> data = response.ResultAs<Dictionary<string, ProductSizeDetailDTO>>();

                        // Chuyển đổi từ điển thành danh sách
                        List<ProductSizeDetailDTO> ListProductSizeDetail = data.Values.ToList();

                        return ("Lấy danh sách chi tiết kích thước sản phẩm thành công", ListProductSizeDetail);
                    }
                    else
                    {
                        return ("Lấy danh sách chi tiết kích thước sản phẩm thành công", new List<ProductSizeDetailDTO>());
                    }
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        ///     Lấy tên kích thước
        /// </returns>
        public async Task<ProductSizeDetailDTO> getNameSize(string sizeID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    // Lấy dữ liệu từ nút "KichThuocSanPham" trong Firebase
                    FirebaseResponse sizeResponse = await context.Client.GetTaskAsync("KichThuocSanPham");

                    if (sizeResponse.Body != null && sizeResponse.Body != "null")
                    {
                        Dictionary<string, ProductSizeDetailDTO> sizedata = sizeResponse.ResultAs<Dictionary<string, ProductSizeDetailDTO>>();

                        ProductSizeDetailDTO sizeProduct = (from size in sizedata.Values
                                              where size.MaKichThuoc == sizeID
                                              select new ProductSizeDetailDTO
                                              {
                                                  TenKichThuoc = size.TenKichThuoc
                                              }).FirstOrDefault();
                        return sizeProduct;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                throw new Exception("Có lỗi xảy ra: " + ex.Message);
            }
        }
        /// <summary>
        /// Tạo chi tiết kích thước sản phẩm
        /// </summary>
        /// <param name="productID"> mã sản phẩm </param>
        /// <param name="listProductSizeDetail"> list chi tiết kích thước sản phẩm </param>
        /// <returns>
        ///     1. Thông báo
        ///     2. True nếu tạo thành công, False nếu tạo thất bại
        /// </returns>
        public async Task<(string, bool)> createProductSizeDetail(string productID, List<ProductSizeDetailDTO> listProductSizeDetail)
        {
            try
            {
                using (var context = new Firebase())
                {
                    foreach (var productSizeDetail in listProductSizeDetail)
                    {
                        await context.Client.SetTaskAsync("SanPham/" + productID + "/ChiTietKichThuocSanPham/" + productSizeDetail.MaKichThuoc, productSizeDetail);
                    }

                    return ("Thêm chi tiết kích thước sản phẩm thành công", true);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, false);
            }
        }
    }
}
