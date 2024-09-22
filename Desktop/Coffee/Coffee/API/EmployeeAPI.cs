using Coffee.DTOs;
using Coffee.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Coffee.Utils;
using Coffee.Utils.Helper;

namespace Coffee.API
{
    internal class EmployeeAPI
    {
        private static EmployeeAPI _ins;

        public static EmployeeAPI Ins
        {
            get
            {
                if (_ins == null)
                    _ins = new EmployeeAPI();
                return _ins;
            }
            private set
            {
                _ins = value;
            }
        }

        public string beginUrl = "/employee";

        //// <summary>
        /// 
        /// </summary>
        /// <returns>
        ///     Danh sách nhân viên
        /// </returns>
        public async Task<(string, List<EmployeeDTO>)> getEmployees()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    // Send a GET request to the specified URL
                    HttpResponseMessage resp = await client.GetAsync(Constants.API.IP + beginUrl + "/employees");

                    string responseContent = resp.Content.ReadAsStringAsync().Result;

                    // Parse the JSON
                    var jsonObj = JObject.Parse(responseContent);

                    if (resp.IsSuccessStatusCode)
                    {
                        // Extract the data portion
                        var data = jsonObj["data"];

                        // Deserialize the data portion into a list
                        var emloyees = JsonConvert.DeserializeObject<List<EmployeeDTO>>(data.ToString());

                        return ("Lấy danh sách nhân viên thành công", emloyees);
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
        ///     Danh sách chức vụ nhân viên
        /// </returns>
        public async Task<(string, List<PositionDTO>)> getPositionEmployee()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    // Send a GET request to the specified URL
                    HttpResponseMessage resp = await client.GetAsync(Constants.API.IP + beginUrl + "/positions");

                    string responseContent = resp.Content.ReadAsStringAsync().Result;

                    // Parse the JSON
                    var jsonObj = JObject.Parse(responseContent);

                    if (resp.IsSuccessStatusCode)
                    {
                        // Extract the data portion
                        var data = jsonObj["data"];

                        // Deserialize the data portion into a list
                        var position = JsonConvert.DeserializeObject<List<PositionDTO>>(data.ToString());

                        return ("Lấy danh sách chức vụ nhân viên thành công", position);
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
        /// Thêm nhân viên
        /// INPUT: EmployeeDTO: nhân viên
        /// </summary>
        /// <param name="employee"></param>
        /// <returns>
        ///     1: Lỗi khi thêm dữ liệu
        ///     2: nhân viên
        /// </returns>
        public async Task<(string, EmployeeDTO)> createEmployee(EmployeeDTO employee)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    string json = JsonConvert.SerializeObject(employee);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(Constants.API.IP + beginUrl + "/employee", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return ("Thêm nhân viên thành công", employee);
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
        /// Cập nhật nhân viên
        /// INPUT: Customer: nhân viên
        /// </summary>
        /// <param name="employee"></param>
        /// <returns>
        ///     1: Thông báo
        ///     2: nhân viên
        /// </returns>
        public async Task<(string, EmployeeDTO)> updateEmployee(EmployeeDTO employee)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    string json = JsonConvert.SerializeObject(employee);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync(Constants.API.IP + beginUrl + "/employee/" + employee.MaNhanVien, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return ("Cập nhật nhân viên thành công", employee);
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
        /// Xoá nhân viên
        /// INPUT:
        ///     employeeID: mã nhân viên
        /// </summary>
        /// <param name="employeeID"></param>
        /// <returns>
        ///     1: Thông báo
        ///     2: True nếu xoá thành công, False xoá thất bại
        /// </returns>
        public async Task<(string, bool)> DeleteEmployee(string employeeID)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage response = await client.DeleteAsync(Constants.API.IP + beginUrl + $"/employee/{employeeID}");

                    if (response.IsSuccessStatusCode)
                    {
                        return ("Xoá nhân viên thành công", true);
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
