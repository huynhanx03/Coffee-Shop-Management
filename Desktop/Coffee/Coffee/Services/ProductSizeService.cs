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
    public class ProductSizeService
    {
        private static ProductSizeDAL _ins;
        public static ProductSizeDAL Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new ProductSizeDAL();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        ///     Trả về danh sách kích thước sản phẩm
        /// </returns>
        public async Task<(string, List<ProductSizeDTO>)> getAllProductSzie()
        {
            return await ProductSizeDAL.Ins.getAllProductSzie();
        }


    }
}
