using Coffee.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Coffee.DTOs;
using Coffee.Utils;
using Coffee.Utils.Helper;

namespace Coffee.API
{
    public class EvaluateAPI
    {
        private static EvaluateAPI _ins;

        public static EvaluateAPI Ins
        {
            get
            {
                if (_ins == null)
                    _ins = new EvaluateAPI();
                return _ins;
            }
            private set
            {
                _ins = value;
            }
        }

        public string beginUrl = "/evaluate";

        /// <summary>
        /// Lấy danh sách đánh giá
        /// </summary>
        /// <returns>
        /// </returns>
        public async Task<(string, List<EvaluateDTO>)> getListEvaluate()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Send a GET request to the specified URL
                    HttpResponseMessage resp = await client.GetAsync(Constants.API.IP + beginUrl + "/evaluates");

                    string responseContent = resp.Content.ReadAsStringAsync().Result;

                    // Parse the JSON
                    var jsonObj = JObject.Parse(responseContent);

                    if (resp.IsSuccessStatusCode)
                    {
                        // Extract the data portion
                        var data = jsonObj["data"];

                        // Deserialize the data portion into a list
                        var evaluates = JsonConvert.DeserializeObject<List<EvaluateDTO>>(data.ToString());

                        return ("Lấy danh sách đánh giá thành công", evaluates);
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


        //// <summary>
        /// Xoá đánh giá
        /// </summary>
        /// <param name="EvaluateID"></param>
        /// <returns></returns>
        public async Task<(string, bool)> DeleteEvaluate(string EvaluateID)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage response = await client.DeleteAsync(Constants.API.IP + beginUrl + $"/evaluate/{EvaluateID}");

                    if (response.IsSuccessStatusCode)
                    {
                        return ("Xoá đánh giá thành công", true);
                    }
                    else
                    {
                        string responseContent = response.Content.ReadAsStringAsync().Result;

                        // Parse the JSON
                        var jsonObj = JObject.Parse(responseContent);

                        return (jsonObj["message"].ToString(), false);
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
