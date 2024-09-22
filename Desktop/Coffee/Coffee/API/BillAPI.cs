using Coffee.DTOs;
using Coffee.Models;
using Coffee.Utils;
using Coffee.Utils.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.API
{
    public class BillAPI
    {
        private static BillAPI _ins;
        public static BillAPI Ins
        {
            get
            {
                if (_ins == null)
                    _ins = new BillAPI();
                return _ins;
            }
            private set
            {
                _ins = value;
            }
        }

        public string beginUrl = "/bill-sell";

        public async Task<(string, List<BillDTO>)> GetBillSells(DateTime fromDate, DateTime toDate)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage resp = await client.GetAsync(Constants.API.IP + beginUrl + "/bill-sells/" + "?fromDate=" + fromDate.ToString("yyyy-MM-ddTHH:mm:ss") + "&toDate=" + toDate.ToString("yyyy-MM-ddTHH:mm:ss"));
                    string responseContent = await resp.Content.ReadAsStringAsync();

                    // Parse the JSON
                    var jsonObj = JObject.Parse(responseContent);

                    if (resp.IsSuccessStatusCode)
                    {
                        // Extract the data portion
                        var data = jsonObj["data"];

                        var billsells = JsonConvert.DeserializeObject<List<BillDTO>>(data.ToString());
                        return ("Lấy danh sách hóa đơn thành công", billsells);
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

        public async Task<(string, bool)> CreateBillSell(BillModel billSell)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    string json = JsonConvert.SerializeObject(billSell);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(Constants.API.IP + beginUrl + "/bill-sell", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return ("Thêm hóa đơn thành công", true);
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

        public async Task<(string, BillModel)> UpdateBillSell(BillModel billSell)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    string json = JsonConvert.SerializeObject(billSell);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync(Constants.API.IP + beginUrl + $"/bill-sell/{billSell.MaHoaDon}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return ("Cập nhật hoá đơn thành công", billSell);
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

        public async Task<(string, bool)> updateTableBooking(string tableIDBooking, string tableIDFree)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var container = new
                    {
                        tableID1 = tableIDBooking,
                        tableID2 = tableIDFree
                    };

                    string json = JsonConvert.SerializeObject(container);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync(Constants.API.IP + beginUrl + "/bill-sell-table-booking", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return ("Cập nhật hoá đơn thành công", true);
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

        public async Task<(string, BillDTO)> getBillSellUnpaid(string tableID, string status)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var container = new
                    {
                        MaBan = tableID,
                        TrangThai = status
                    };

                    string json = JsonConvert.SerializeObject(container);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Send a GET request to the specified URL
                    HttpResponseMessage resp = await client.GetAsync(Constants.API.IP + beginUrl + $"/bill-sell-unpaid?status={status}&tableID={tableID}");

                    string responseContent = resp.Content.ReadAsStringAsync().Result;

                    // Parse the JSON
                    var jsonObj = JObject.Parse(responseContent);

                    if (resp.IsSuccessStatusCode)
                    {
                        // Extract the data portion
                        var data = jsonObj["data"];

                        var bill = JsonConvert.DeserializeObject<BillDTO>(data.ToString());

                        return ("Lấy hoá đơn thành công", bill);
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

        public async Task<(string, bool)> DeleteBillSell(string billSellID)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage response = await client.DeleteAsync(Constants.API.IP + beginUrl + $"/bill-sell/{billSellID}");

                    if (response.IsSuccessStatusCode)
                    {
                        return ("Xóa hóa đơn thành công", true);
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

        public async Task<(string, bool)> mergeBillOfTables(string _tableID1, string _tableID2)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var container = new
                    {
                        tableID1 = _tableID1,
                        tableID2 = _tableID2
                    };

                    string json = JsonConvert.SerializeObject(container);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync(Constants.API.IP + beginUrl + "/merge-bill", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return ("Cập nhật hoá đơn thành công", true);
                    }
                    else
                    {
                        return ("Cập nhật hoá đơn thất bại", false);
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
