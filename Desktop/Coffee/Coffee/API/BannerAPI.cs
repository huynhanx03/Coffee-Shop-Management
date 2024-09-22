using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Coffee.Models;
using Coffee.Utils;
using Coffee.Utils.Helper;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Coffee.API
{
    public class BannerAPI
    {
        private static BannerAPI _ins;

        public static BannerAPI Ins
        {
            get
            {
                if (_ins == null)
                    _ins = new BannerAPI();
                return _ins;
            }
            private set
            {
                _ins = value;
            }
        }

        public string beginUrl = "/banner";

        /// <summary>
        /// Lấy danh sách banner
        /// </summary>
        /// <returns>
        ///     1. Thông báo
        ///     2. Danh sách banner
        /// </returns>
        public async Task<(string, List<BannerModel>)> getBanners()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Send a GET request to the specified URL
                    HttpResponseMessage resp = await client.GetAsync(Constants.API.IP + beginUrl + "/banners");

                    string responseContent = resp.Content.ReadAsStringAsync().Result;

                    // Parse the JSON
                    var jsonObj = JObject.Parse(responseContent);

                    if (resp.IsSuccessStatusCode)
                    {
                        // Extract the data portion
                        var data = jsonObj["data"];

                        // Deserialize the data portion into a list
                        var banners = JsonConvert.DeserializeObject<List<BannerModel>>(data.ToString());

                        return ("Lấy danh sách banner thành công", banners);
                    }
                    else
                    {
                        return (jsonObj["message"].ToString(), null);
                    }
                }
                catch (HttpRequestException e)
                {
                    return (e.Message, null);
                }
            }
        }

        /// <summary>
        /// Tạo bannner mới
        /// </summary>
        /// <param name="banner"></param>
        /// <returns></returns>
        public async Task<(string, BannerModel)> AddBanner(BannerModel banner)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string json = JsonConvert.SerializeObject(banner);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage response = await client.PostAsync(Constants.API.IP + beginUrl + "/banner", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return ("Thêm banner thành công", banner);
                    }
                    else
                    {
                        string responseContent = response.Content.ReadAsStringAsync().Result;

                        // Parse the JSON
                        var jsonObj = JObject.Parse(responseContent);

                        return (jsonObj["message"].ToString(), null);
                    }
                }
                catch (HttpRequestException e)
                {
                    return (e.Message, null);
                }
            }
        }


        /// <summary>
        /// Lấy mã banner lớn nhất
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetMaxMaBanner()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(Constants.API.IP + beginUrl + "/banners/maxBannerId");

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        JObject jsonObj = JObject.Parse(json);
                        string maxBannerId = (string)jsonObj["maxBannerId"];
                        return maxBannerId;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (HttpRequestException e)
                {
                    return null;
                }
            }
        }


        /// <summary>
        /// Xoá banner
        /// </summary>
        /// <returns>
        ///     1. Thông báo
        ///     2. True/False
        /// </returns>
        public async Task<(string, bool)> DeleteBanner(string bannerId)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage response = await client.DeleteAsync(Constants.API.IP + beginUrl + $"/banners/{bannerId}");

                    if (response.IsSuccessStatusCode)
                    {
                        return ("Xoá banner thành công", true);
                    }
                    else
                    {
                        return ("Xoá banner thất bại", false);
                    }
                }
                catch (HttpRequestException e)
                {
                    return (e.Message, false);
                }
            }
        }

    }
}
