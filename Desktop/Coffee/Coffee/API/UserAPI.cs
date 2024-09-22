using Coffee.DTOs;
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
    internal class UserAPI
    {
        private static UserAPI _ins;

        public static UserAPI Ins
        {
            get
            {
                if (_ins == null)
                    _ins = new UserAPI();
                return _ins;
            }
            private set
            {
                _ins = value;
            }
        }

        public string beginUrl = "/user";

        public async Task<(string, UserDTO)> getUserByNumberPhone(string numberPhone)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    // Send a GET request to the specified URL
                    HttpResponseMessage resp = await client.GetAsync(Constants.API.IP + beginUrl + $"/number-phone/{numberPhone}");

                    string responseContent = resp.Content.ReadAsStringAsync().Result;

                    // Parse the JSON
                    var jsonObj = JObject.Parse(responseContent);

                    if (resp.IsSuccessStatusCode)
                    {
                        // Extract the data portion
                        var data = jsonObj["data"];

                        // Since data contains a nested object, we need to get the first property value
                        var userToken = data.First().First();

                        // Deserialize the nested object to UserDTO
                        var user = userToken.ToObject<UserDTO>();

                        return ("Lấy thông tin người dùng thành công", user);
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

        public async Task<(string, bool)> checkToken()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    // Send a GET request to the specified URL
                    HttpResponseMessage resp = await client.GetAsync(Constants.API.IP + beginUrl + $"/token/check-token");

                    string responseContent = resp.Content.ReadAsStringAsync().Result;

                    // Parse the JSON
                    var jsonObj = JObject.Parse(responseContent);
                    var message = jsonObj["message"].ToString();

                    if (resp.IsSuccessStatusCode)
                    {
                        return (message, true);
                    }
                    else
                    {
                        return (message, false);
                    }
                }
                catch (HttpRequestException e)
                {
                    return (e.Message, false);
                }
            }
        }

        public async Task<(string, UserDTO)> updateUser(UserDTO user)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    string json = JsonConvert.SerializeObject(user);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync(Constants.API.IP + beginUrl + "/user/" + user.MaNguoiDung, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return ("Cập nhật người dùng thành công", user);
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

        public async Task<(string, UserDTO)> login(string _username, string _password)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var data = new
                    {
                        username = _username,
                        password = _password
                    };

                    string json = JsonConvert.SerializeObject(data);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(Constants.API.IP + beginUrl + "/login-desktop", content);

                    string responseContent = response.Content.ReadAsStringAsync().Result;

                    // Parse the JSON
                    var jsonObj = JObject.Parse(responseContent);

                    if (response.IsSuccessStatusCode)
                    {
                        // Assuming the token is under the 'token' key
                        string token = jsonObj["token"].ToString();

                        // Extract the data portion
                        var dataObj = jsonObj["data"];

                        var user = JsonConvert.DeserializeObject<UserDTO>(dataObj.ToString());

                        Properties.Settings.Default.Token = token;
                        Properties.Settings.Default.UserID = user.MaNguoiDung;
                        Properties.Settings.Default.Save();

                        return ("Đăng nhập thành công", user);
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

        public async Task<(string, UserDTO)> GetUser(string userID)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    // Send GET request
                    HttpResponseMessage response = await client.GetAsync(Constants.API.IP + beginUrl + "/user/" + userID);

                    // Read the response and parse it into a UserDTO object
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var jsonObj = JObject.Parse(responseContent);

                    if (response.IsSuccessStatusCode)
                    {
                        UserDTO user = JsonConvert.DeserializeObject<UserDTO>(jsonObj["data"].ToString());

                        return ("Lấy thông tin người dùng thành công", user);
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

    }
}
