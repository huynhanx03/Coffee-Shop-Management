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
    public class ProductRecipeDAL
    {
        private static ProductRecipeDAL _ins;
        public static ProductRecipeDAL Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new ProductRecipeDAL();
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
            try
            {
                using (var context = new Firebase())
                {
                    foreach (var productRecipe in listProductRecipe)
                    {
                        await context.Client.SetTaskAsync("SanPham/" + productID + "/CongThuc/" + productRecipe.MaNguyenLieu, productRecipe);
                    }

                    return ("Thêm công thức sản phẩm thành công", true);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, false);
            }
        }
    }
}
