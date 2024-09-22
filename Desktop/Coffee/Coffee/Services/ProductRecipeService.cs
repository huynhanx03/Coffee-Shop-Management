using Coffee.DALs;
using Coffee.DTOs;
using Coffee.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.Services
{
    public class ProductRecipeService
    {
        private static ProductRecipeService _ins;
        public static ProductRecipeService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new ProductRecipeService();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// Tạo công thức sản phẩm
        /// </summary>
        /// <param name="productID"> mã sản phẩm </param>
        /// <param name="listProductRecipe"> list công thức sản phẩm </param>
        /// <returns>
        ///     1. Thông báo
        ///     2. True nếu tạo thành công, False nếu tạo thất bại
        /// </returns>
        public async Task<(string, bool)> createProductRecipe(string productID, List<ProductRecipeDTO> listProductRecipe)
        {
            return await ProductRecipeDAL.Ins.createProductRecipe(productID, listProductRecipe);
        }
    }
}
