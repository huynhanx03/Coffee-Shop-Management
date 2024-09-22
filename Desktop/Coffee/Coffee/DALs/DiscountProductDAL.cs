using Coffee.DTOs;
using Coffee.Utils;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DALs
{
    public class DiscountProductDAL
    {
        private static DiscountProductDAL _ins;
        public static DiscountProductDAL Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new DiscountProductDAL();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="discountProduct"></param>
        /// <returns></returns>
        public async Task<(string, DiscountProductDTO)> createDiscountProductDTO(DiscountProductDTO discountProduct)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.UpdateTaskAsync("SanPham/" + discountProduct.MaSanPham, new { PhanTramGiam = discountProduct.PhanTramGiam});

                    return ("Thêm sản phẩm giảm giá thành công", discountProduct);
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
        /// <returns></returns>
        public async Task<(string, List<DiscountProductDTO>)> getListDiscountProduct()
        {
            try
            {
                using (var context = new Firebase())
                {
                    // Lấy dữ liệu từ nút "Product" trong Firebase
                    FirebaseResponse productResponse = await context.Client.GetTaskAsync("SanPham");
                    Dictionary<string, ProductDTO> productData = productResponse.ResultAs<Dictionary<string, ProductDTO>>();

                    //FirebaseResponse discountProductResponse = await context.Client.GetTaskAsync("SanPhamGiamGiaHomNay");
                    //Dictionary<string, DiscountProductDTO> discountProductData = discountProductResponse.ResultAs<Dictionary<string, DiscountProductDTO>>();


                    var result = (from product in productData.Values
                                  where product.PhanTramGiam > 0
                                  select new DiscountProductDTO
                                  {
                                      MaSanPham = product.MaSanPham,
                                      TenSanPham = product.TenSanPham,
                                      PhanTramGiam = product.PhanTramGiam
                                  }).ToList();

                    return ("Lấy danh sách sản phẩm giảm giá thành công", result);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        /// <summary>
        /// Xoá tất cả sản phẩm giảm giá
        /// </summary>
        /// <returns>
        ///     1. Thông báo
        ///     2. True/False
        /// </returns>
        public async Task<(string, bool)> DeleteDiscountProductToday(string productID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.UpdateTaskAsync("SanPham/" + productID, new { PhanTramGiam = 0 });

                    return ("Xoá các sản phẩm giảm giá thành công", true);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, false);
            }
        }
    }
}
