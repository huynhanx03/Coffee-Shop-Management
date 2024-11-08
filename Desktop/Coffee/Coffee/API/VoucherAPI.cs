using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Coffee.Utils;
using Coffee.DTOs;
using Coffee.Utils.Helper;
using System.Windows;

namespace Coffee.API
{
    public class VoucherAPI
    {
        private static VoucherAPI _ins;

        public static VoucherAPI Ins
        {
            get
            {
                if (_ins == null)
                    _ins = new VoucherAPI();
                return _ins;
            }
            private set
            {
                _ins = value;
            }
        }

        public string beginUrl = "/voucher";


        public async Task<(string, List<VoucherDTO>)> GetVouchers()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage resp = await client.GetAsync(Constants.API.IP + beginUrl + "/vouchers");
                    string responseContent = await resp.Content.ReadAsStringAsync();

                    var jsonObj = JObject.Parse(responseContent);

                    if (resp.IsSuccessStatusCode)
                    {
                        var data = jsonObj["data"];
                        var vouchers = JsonConvert.DeserializeObject<List<VoucherDTO>>(data.ToString());
                        return (Application.Current.Resources["GetListVoucherSuccess"] as string, vouchers);
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

        public async Task<(string, bool)> createVoucher(VoucherDTO voucher)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    string json = JsonConvert.SerializeObject(voucher);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(Constants.API.IP + beginUrl + "/voucher", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return (Application.Current.Resources["AddVoucherSuccess"] as string, true);
                    }
                    else
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();

                        return (JObject.Parse(responseContent)["message"].ToString(), false);
                    }
                }
                catch (HttpRequestException e)
                {
                    return (e.Message, false);
                }
            }
        }

        public async Task<(string, bool)> DeleteVoucher(string voucherId)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage response = await client.DeleteAsync(Constants.API.IP + beginUrl + $"/vouchers/{voucherId}");

                    if (response.IsSuccessStatusCode)
                    {
                        return (Application.Current.Resources["DeleteVoucherSuccess"] as string, true);
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
