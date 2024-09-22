using Coffee.DTOs;
using Coffee.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Coffee.Utils
{
    public class RecommendSystemService
    {
        private static RecommendSystemService _ins;

        public static RecommendSystemService Ins
        {
            get
            {
                if (_ins == null)
                    _ins = new RecommendSystemService();
                return _ins;
            }
            private set
            {
                _ins = value;
            }
        }

        private string baseAPI = "https://huynhnhan2003.pythonanywhere.com";

        public async Task<List<ProductRecommendDTO>> getRecommend(string MaSanPham)
        {
            try
            {
                var data = new
                {
                    MaSanPham = MaSanPham,
                    MaKhachHang = ""
                };

                string json = JsonConvert.SerializeObject(data);

                // Gửi yêu cầu POST đến Colab
                using (var httpClient = new HttpClient())
                {
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await httpClient.PostAsync(baseAPI + "/recommend", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = response.Content.ReadAsStringAsync().Result;

                        return JsonConvert.DeserializeObject<List<ProductRecommendDTO>>(responseContent);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
