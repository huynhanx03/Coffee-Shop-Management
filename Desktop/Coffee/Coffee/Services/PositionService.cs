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
    public class PositionService
    {
        private static PositionService _ins;
        public static PositionService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new PositionService();
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
            return await EmployeeAPI.Ins.getPositionEmployee();
        }
    }
}
