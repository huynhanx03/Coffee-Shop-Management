using Coffee.API;
using Coffee.DALs;
using Coffee.Models;
using Coffee.Utils;
using Coffee.Utils.Helper;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.Services
{
    public class BannerService
    {
        private static BannerService _ins;
        public static BannerService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new BannerService();
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
            string MaxMaBanner = await BannerAPI.Ins.GetMaxMaBanner();

            string NewMaBanner = Helper.nextID(MaxMaBanner, "BN");

            banner.MaBanner = NewMaBanner;

            return await BannerAPI.Ins.AddBanner(banner);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<(string, List<BannerModel>)> getListBanner()
        {
            return await BannerAPI.Ins.getBanners();
        }

        /// <summary>
        /// Xoá banner
        /// </summary>
        /// <returns>
        ///     1. Thông báo
        ///     2. True/False
        /// </returns>
        public async Task<(string, bool)> DeleteBanner(BannerModel banner)
        {
            string labelClound = await CloudService.Ins.DeleteImage(banner.HinhAnh);

            return await BannerAPI.Ins.DeleteBanner(banner.MaBanner);
        }
    }
}
