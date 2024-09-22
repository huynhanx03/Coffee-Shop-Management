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
    public class ProductTypeService
    {
        private static ProductTypeService _ins;
        public static ProductTypeService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new ProductTypeService();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        ///     Trả về danh sách loại sản phẩm
        /// </returns>
        public async Task<(string, List<ProductTypeDTO>)> getAllProductType()
        {
            return await ProductTypeAPI.Ins.GetProductTypes();
        }
    }
}
