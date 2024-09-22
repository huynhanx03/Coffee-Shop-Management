using Coffee.DALs;
using Coffee.Models;
using Coffee.Utils;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.Services
{
    public class RankService
    {
        private static RankService _ins;
        public static RankService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new RankService();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        ///     Trả về danh sách mức độ thân thiết
        /// </returns>
        public async Task<(string, List<RankModel>)> getAllRank()
        {
            return await RankDAL.Ins.getAllRank();
        }

        /// <summary>
        /// Lấy mức độ thân thiết theo mã mức độ thân thiết
        /// </summary>
        /// <param name="rankID"></param>
        /// <returns></returns>
        public async Task<(string, RankModel)> getRank(string rankID)
        {
            return await RankDAL.Ins.getRank(rankID);
        }
    }
}
