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
    public class TableTypeService
    {
        private static TableTypeService _ins;
        public static TableTypeService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new TableTypeService();
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
            return await TableAPI.Ins.getTableTypes();
        }
    }
}
