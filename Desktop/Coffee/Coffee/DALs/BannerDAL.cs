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
    public class BannerDAL
    {
        private static BannerDAL _ins;
        public static BannerDAL Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new BannerDAL();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// Tạo bannner mới
        /// </summary>
        /// <param name="banner"></param>
        /// <returns></returns>
        public async Task<(string, BannerModel)> createBanner(BannerModel banner)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.SetTaskAsync("Banner/" + banner.MaBanner, banner);

                    return ("Thêm banner thành công", banner);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<(string, List<BannerModel>)> getListBanner()
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse bannerResponse = await context.Client.GetTaskAsync("Banner");
                    Dictionary<string, BannerModel> bannerData = bannerResponse.ResultAs<Dictionary<string, BannerModel>>();

                    return ("Lấy danh sách sản phẩm giảm giá thành công", bannerData.Values.ToList());
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        /// <summary>
        /// Lấy mã banner lớn nhất
        /// </summary>
        /// <returns></returns>
        public async Task<string> getMaxMaBanner()
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("Banner");

                    if (response.Body != null && response.Body != "null")
                    {
                        Dictionary<string, BannerModel> data = response.ResultAs<Dictionary<string, BannerModel>>();

                        string MaxMaBanner = data.Values.Select(p => p.MaBanner).Max();

                        return MaxMaBanner;
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Xoá banner
        /// </summary>
        /// <returns>
        ///     1. Thông báo
        ///     2. True/False
        /// </returns>
        public async Task<(string, bool)> DeleteBanner(string bannerID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.DeleteTaskAsync("Banner/" + bannerID);
                    return ("Xoá banner thành công", true);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, false);
            }
        }
    }
}
