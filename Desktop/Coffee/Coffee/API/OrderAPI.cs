using Coffee.DTOs;
using Coffee.Utils.Helper;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Coffee.Utils;

namespace Coffee.API
{
    public class OrderAPI
    {
        private static OrderAPI _ins;
        public static OrderAPI Ins
        {
            get
            {
                if (_ins == null)
                    _ins = new OrderAPI();
                return _ins;
            }
            private set
            {
                _ins = value;
            }
        }

        public string beginUrl = "/order";

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        ///     Danh sách đơn hàng
        /// </returns>
        public async Task<(string, List<OrderDTO>)> getListOrder()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage resp = await client.GetAsync(Constants.API.IP + beginUrl + "/orders");
                    string responseContent = await resp.Content.ReadAsStringAsync();

                    // Parse the JSON
                    var jsonObj = JObject.Parse(responseContent);

                    if (resp.IsSuccessStatusCode)
                    {
                        // Extract the data portion
                        var data = jsonObj["data"];

                        var orders = JsonConvert.DeserializeObject<List<OrderDTO>>(data.ToString());
                        return ("Lấy danh sách đơn hàng thành công", orders);
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

        public async Task<(string, bool)> updateStatusOrder(string orderId, string status)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    string updateUrl = $"{Constants.API.IP}{beginUrl}/orders/{orderId}";

                    // Create the JSON payload
                    var jsonPayload = JsonConvert.SerializeObject(new { status });
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    HttpResponseMessage resp = await client.PutAsync(Constants.API.IP + beginUrl + "/set-status-order/" + orderId, content);

                    string responseContent = await resp.Content.ReadAsStringAsync();

                    // Parse the JSON response
                    var jsonObj = JObject.Parse(responseContent);

                    if (resp.IsSuccessStatusCode)
                    {
                        return ("Cập nhật trạng thái đơn hàng thành công", true);
                    }
                    else
                    {
                        return (jsonObj["message"].ToString(), false);
                    }
                }
                catch (HttpRequestException e)
                {
                    return (e.Message, false);
                }
            }
                
        }

        public async Task<(string, bool)> updateBillIDOrder(string orderId, string billID)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    string updateUrl = $"{Constants.API.IP}{beginUrl}/orders/{orderId}";

                    // Create the JSON payload
                    var jsonPayload = JsonConvert.SerializeObject(new { billID });
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    HttpResponseMessage resp = await client.PutAsync(Constants.API.IP + beginUrl + "/bill/" + orderId, content);

                    string responseContent = await resp.Content.ReadAsStringAsync();

                    // Parse the JSON response
                    var jsonObj = JObject.Parse(responseContent);

                    if (resp.IsSuccessStatusCode)
                    {
                        return ("Cập nhật hoá đơn đơn hàng thành công", true);
                    }
                    else
                    {
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
