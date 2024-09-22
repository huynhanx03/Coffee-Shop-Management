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
    public class TableTypeDAL
    {
        private static TableTypeDAL _ins;
        public static TableTypeDAL Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new TableTypeDAL();
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
        public async Task<(string, List<TableTypeDTO>)> getAllTableType()
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("LoaiBan");

                    if (response.Body != null && response.Body != "null")
                    {
                        Dictionary<string, TableTypeDTO> data = response.ResultAs<Dictionary<string, TableTypeDTO>>();

                        // Chuyển đổi từ điển thành danh sách
                        List<TableTypeDTO> ListTableType = data.Values.ToList();

                        return ("Lấy danh sách loại bàn thành công", ListTableType);
                    }
                    else
                    {
                        return ("Lấy danh sách loại bàn thành công", new List<TableTypeDTO>());
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
