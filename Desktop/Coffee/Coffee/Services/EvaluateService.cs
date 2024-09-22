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
    public class EvaluateService
    {
        private static EvaluateService _ins;
        public static EvaluateService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new EvaluateService();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// Xoá đánh giá
        /// </summary>
        /// <param name="Evaluate"></param>
        /// <returns></returns>
        public async Task<(string, bool)> DeleteEvaluate(EvaluateDTO Evaluate)
        {
            return await EvaluateAPI.Ins.DeleteEvaluate(Evaluate.MaDanhGia);
        }

        /// <summary>
        /// Lấy danh sách đánh giá
        /// </summary>
        /// <returns>
        /// </returns>
        public async Task<(string, List<EvaluateDTO>)> getListEvaluate()
        {
            return await EvaluateAPI.Ins.getListEvaluate();
        }
    }
}
