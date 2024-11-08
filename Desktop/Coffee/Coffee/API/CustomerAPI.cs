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
using System.Reflection;
using Coffee.Utils.Helper;
using System.Windows;

namespace Coffee.API
{
    public class CustomerAPI
    {
        private static CustomerAPI _ins;

        public static CustomerAPI Ins
        {
            get
            {
                if (_ins == null)
                    _ins = new CustomerAPI();
                return _ins;
            }
            private set
            {
                _ins = value;
            }
        }

        public string beginUrl = "/customer";

        //// <summary>
        /// 
        /// </summary>
        /// <returns>
        ///     Danh sách Khách hàng
        /// </returns>
        public async Task<(string, List<CustomerDTO>)> getCustomers()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    // Send a GET request to the specified URL
                    HttpResponseMessage resp = await client.GetAsync(Constants.API.IP + beginUrl + "/customers");

                    string responseContent = resp.Content.ReadAsStringAsync().Result;

                    // Parse the JSON
                    var jsonObj = JObject.Parse(responseContent);

                    if (resp.IsSuccessStatusCode)
                    {
                        // Extract the data portion
                        var data = jsonObj["data"];

                        // Deserialize the data portion into a list
                        var customers = JsonConvert.DeserializeObject<List<CustomerDTO>>(data.ToString());

                        return (Application.Current.Resources["GetListCustomerSuccess"] as string, customers);
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

        public async Task<(string, List<RankModel>)> getRankCustomers()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    // Send a GET request to the specified URL
                    HttpResponseMessage resp = await client.GetAsync(Constants.API.IP + beginUrl + "/rankCustomers");

                    string responseContent = resp.Content.ReadAsStringAsync().Result;

                    // Parse the JSON
                    var jsonObj = JObject.Parse(responseContent);

                    if (resp.IsSuccessStatusCode)
                    {
                        // Extract the data portion
                        var data = jsonObj["data"];

                        // Deserialize the data portion into a list
                        var customers = JsonConvert.DeserializeObject<List<RankModel>>(data.ToString());

                        return (Application.Current.Resources["GetRankCustomerSuccess"] as string, customers);
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
        /// 
        /// </summary>
        /// <returns>
        ///     Danh sách địa chỉ khách hàng
        /// </returns>
        public async Task<(string, List<AddressModel>)> getAddressCustomer(string customerID)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    // Send a GET request to the specified URL
                    HttpResponseMessage resp = await client.GetAsync(Constants.API.IP + "/address/" + customerID);

                    string responseContent = resp.Content.ReadAsStringAsync().Result;

                    // Parse the JSON
                    var jsonObj = JObject.Parse(responseContent);

                    if (resp.IsSuccessStatusCode)
                    {
                        // Extract the data portion
                        var data = jsonObj["data"];

                        // Deserialize the data portion into a list
                        var addressCustomers = JsonConvert.DeserializeObject<List<AddressModel>>(data.ToString());

                        return (Application.Current.Resources["GetAddressCustomerSuccess"] as string, addressCustomers);
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

        public async Task<(string, bool)> plusPoint(string customerID, double point)
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
                        pointRank = point
                    };

                    string json = JsonConvert.SerializeObject(container);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync(Constants.API.IP + beginUrl + "/point/" + customerID, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return (Application.Current.Resources["UpdatePointSuccess"] as string, true);
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

        /// <summary>
        /// Thêm Khách hàng
        /// INPUT: Customer: Khách hàng
        /// </summary>
        /// <param name="Customer"></param>
        /// <returns>
        ///     1: Lỗi khi thêm dữ liệu
        ///     2: Khách hàng
        /// </returns>
        public async Task<(string, CustomerDTO)> createCustomer(CustomerDTO Customer)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    string json = JsonConvert.SerializeObject(Customer);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(Constants.API.IP + beginUrl + "/customer", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return (Application.Current.Resources["CreateCustomerSuccess"] as string, Customer);
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
        /// Cập nhật Khách hàng
        /// INPUT: Customer: Khách hàng
        /// </summary>
        /// <param name="Customer"></param>
        /// <returns>
        ///     1: Thông báo
        ///     2: Khách hàng
        /// </returns>
        public async Task<(string, CustomerDTO)> updateCustomer(CustomerDTO Customer)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    string json = JsonConvert.SerializeObject(Customer);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync(Constants.API.IP + beginUrl + "/customer/" + Customer.MaKhachHang, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return (Application.Current.Resources["UpdateCustomerSuccess"] as string, Customer);
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
        /// Xoá Khách hàng
        /// INPUT:
        ///     CustomerID: mã Khách hàng
        /// </summary>
        /// <param name="CustomerID"></param>
        /// <returns>
        ///     1: Thông báo
        ///     2: True nếu xoá thành công, False xoá thất bại
        /// </returns>
        public async Task<(string, bool)> DeleteCustomer(string customerID)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage response = await client.DeleteAsync(Constants.API.IP + beginUrl + $"/customer/{customerID}");

                    if (response.IsSuccessStatusCode)
                    {
                        return (Application.Current.Resources["DeleteCustomerSuccess"] as string, true);
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
