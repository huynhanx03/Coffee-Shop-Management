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
    public class UnitDAL
    {
        private static UnitDAL _ins;
        public static UnitDAL Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new UnitDAL();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        ///     Trả về danh sách đơn vị
        /// </returns>
        public async Task<(string, List<UnitDTO>)> getAllUnit()
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("DonVi");

                    if (response.Body != null && response.Body != "null")
                    {
                        Dictionary<string, UnitDTO> data = response.ResultAs<Dictionary<string, UnitDTO>>();

                        // Chuyển đổi từ điển thành danh sách
                        List<UnitDTO> ListUnit = data.Values.ToList();

                        return ("Lấy danh sách đơn vị thành công", ListUnit);
                    }
                    else
                    {
                        return ("Lấy danh sách đơn vị thành công", new List<UnitDTO>());
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
