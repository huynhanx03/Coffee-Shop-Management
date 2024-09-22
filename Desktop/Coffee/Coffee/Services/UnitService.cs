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
    public class UnitService
    {
        private static UnitService _ins;
        public static UnitService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new UnitService();
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
            return await IngredientAPI.Ins.getUnitIngredient();
        }
    }
}
