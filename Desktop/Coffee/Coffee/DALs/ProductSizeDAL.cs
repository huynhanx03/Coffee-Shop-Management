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
    public class ProductSizeDAL
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
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("KichThuocSanPham");

                    if (response.Body != null && response.Body != "null")
                    {
                        Dictionary<string, ProductSizeDTO> data = response.ResultAs<Dictionary<string, ProductSizeDTO>>();

                        // Chuyển đổi từ điển thành danh sách
                        List<ProductSizeDTO> ListProductSize = data.Values.ToList();

                        return ("Lấy danh sách kích thước sản phẩm thành công", ListProductSize);
                    }
                    else
                    {
                        return ("Lấy danh sách kích thước sản phẩm thành công", new List<ProductSizeDTO>());
                    }
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }
    }
}
