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
    public class ProductTypeDAL
    {
        private static ProductTypeDAL _ins;
        public static ProductTypeDAL Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new ProductTypeDAL();
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
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("LoaiSanPham");

                    if (response.Body != null && response.Body != "null")
                    {
                        Dictionary<string, ProductTypeDTO> data = response.ResultAs<Dictionary<string, ProductTypeDTO>>();

                        // Chuyển đổi từ điển thành danh sách
                        List<ProductTypeDTO> ListProductType = data.Values.ToList();

                        return ("Lấy danh sách loại sản phẩm thành công", ListProductType);
                    }
                    else
                    {
                        return ("Lấy danh sách loại sản phẩm thành công", new List<ProductTypeDTO>());
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
