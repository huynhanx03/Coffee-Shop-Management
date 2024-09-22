using Coffee.API;
using Coffee.DALs;
using Coffee.DTOs;
using Coffee.Utils;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.Services
{
    public class ProductSizeDetailService
    {
        private static ProductSizeDetailService _ins;
        public static ProductSizeDetailService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new ProductSizeDetailService();
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
            return await ProductAPI.Ins.getSizes();
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
            return await ProductSizeDetailDAL.Ins.createProductSizeDetail(productID, listProductSizeDetail);
        }

        /// <summary>
        /// </summary>
        /// <returns>
        ///     Trả về tên kích thước sản phẩm
        /// </returns>
        public async Task<ProductSizeDetailDTO> getNameSize(string sizeid)
        {
            return await ProductSizeDetailDAL.Ins.getNameSize(sizeid);
        }
    }
}
