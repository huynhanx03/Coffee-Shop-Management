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
    public class PositionDAL
    {
        private static PositionDAL _ins;
        public static PositionDAL Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new PositionDAL();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        ///     Trả về danh sách chức vụ
        /// </returns>
        public async Task<(string, List<PositionDTO>)> getAllPosition()
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("ChucDanh");

                    if (response.Body != null && response.Body != "null")
                    {
                        Dictionary<string, PositionDTO> data = response.ResultAs<Dictionary<string, PositionDTO>>();

                        // Chuyển đổi từ điển thành danh sách
                        List<PositionDTO> ListPosition = data.Values.ToList();

                        return ("Lấy danh sách chức vụ thành công", ListPosition);
                    }
                    else
                    {
                        return ("Lấy danh sách chức vụ thành công", new List<PositionDTO>());
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
