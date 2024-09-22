using Coffee.API;
using Coffee.DALs;
using Coffee.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.Services
{
    public class DiscountProductService
    {
        private static DiscountProductService _ins;
        public static DiscountProductService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new DiscountProductService();
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
        public async Task<(string, bool)> createDiscountProductDTO(DiscountProductDTO discountProduct)
        {
            return await ProductAPI.Ins.updateDiscountProduct(discountProduct.MaSanPham, discountProduct.PhanTramGiam);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<(string, List<DiscountProductDTO>)> getListDiscountProduct()
        {
            return await ProductAPI.Ins.getDiscountProducts();
        }

        /// <summary>
        /// Xoá tất cả sản phẩm giảm giá
        /// </summary>
        /// <returns>
        ///     1. Thông báo
        ///     2. True/False
        /// </returns>
        public async Task<(string, bool)> DeleteDiscountProductToday(List<ProductDTO> products)
        {
            foreach (var product  in products)
            {
                await ProductAPI.Ins.updateDiscountProduct(product.MaSanPham, 0);
            }

            return ("Xoá sản phẩm giảm giá thành công", true);
        }
    }
}
