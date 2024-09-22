using Coffee.DTOs;
using Coffee.Models;
using Coffee.Utils;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DALs
{
    public class RankDAL
    {
        private static RankDAL _ins;
        public static RankDAL Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new RankDAL();
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
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("MucDoThanThiet");

                    if (response.Body != null && response.Body != "null")
                    {
                        Dictionary<string, RankModel> data = response.ResultAs<Dictionary<string, RankModel>>();

                        // Chuyển đổi từ điển thành danh sách
                        List<RankModel> ListRank = data.Values.ToList();

                        return ("Lấy danh sách mức độ thân thiết thành công", ListRank);
                    }
                    else
                    {
                        return ("Lấy danh sách mức độ thân thiết thành công", new List<RankModel>());
                    }
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        /// <summary>
        /// Lấy mức độ thân thiết theo mã mức độ thân thiết
        /// </summary>
        /// <param name="rankID"></param>
        /// <returns></returns>
        public async Task<(string, RankModel)> getRank(string rankID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("MucDoThanThiet/" + rankID);

                    RankModel rank = response.ResultAs<RankModel>();

                    return ("Lấy mức độ thân thiết thành công", rank);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

    }
}
