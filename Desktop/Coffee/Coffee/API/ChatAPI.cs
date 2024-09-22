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
using Coffee.DTOs;
using Coffee.Utils.Helper;

namespace Coffee.API
{
    internal class ChatAPI
    {
        private static ChatAPI _ins;

        public static ChatAPI Ins
        {
            get
            {
                if (_ins == null)
                    _ins = new ChatAPI();
                return _ins;
            }
            private set
            {
                _ins = value;
            }
        }

        public string beginUrl = "/chat";

        /// <summary>
        /// </summary>
        /// <returns>
        ///     Trả về danh sách liên lạc người dùng
        /// </returns>
        public async Task<(string, List<UserContactDTO>)> getUserContacts()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    // Send a GET request to the specified URL
                    HttpResponseMessage resp = await client.GetAsync(Constants.API.IP + beginUrl + "/userContacts");

                    string responseContent = resp.Content.ReadAsStringAsync().Result;

                    // Parse the JSON
                    var jsonObj = JObject.Parse(responseContent);

                    if (resp.IsSuccessStatusCode)
                    {
                        // Extract the data portion
                        var data = jsonObj["data"];

                        // Deserialize the data portion into a list
                        var userContacts = JsonConvert.DeserializeObject<List<UserContactDTO>>(data.ToString());

                        return ("Lấy danh bạ người dùng thành công", userContacts);
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
        /// Lấy danh sách tin nhắn của người dùng
        /// </summary>
        /// <returns>
        ///     Danh sách tin nhắn
        /// </returns>
        /// 

        public async Task<(string, List<ChatDTO>)> getMessage(string userID, DateTime? datetime)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    string url = Constants.API.IP + beginUrl + "/messages/" + userID;

                    string formattedDateTime = datetime.HasValue ? datetime.Value.ToString("yyyy-MM-ddTHH:mm:ss") : string.Empty;

                    if (!string.IsNullOrEmpty(formattedDateTime))
                    {
                        url += "?datetime=" + formattedDateTime;
                    }

                    // Send a GET request to the specified URL
                    HttpResponseMessage resp = await client.GetAsync(url);

                    string responseContent = resp.Content.ReadAsStringAsync().Result;

                    // Parse the JSON
                    var jsonObj = JObject.Parse(responseContent);

                    if (resp.IsSuccessStatusCode)
                    {
                        // Extract the data portion
                        var data = jsonObj["data"];

                        // Deserialize the data portion into a list
                        var messages = JsonConvert.DeserializeObject<List<ChatDTO>>(data.ToString());

                        return ("Lấy danh sách tin nhắn thành công", messages);
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
        /// Tạo tin nhắn mới
        /// </summary>
        /// <param name="chat">tin nhắn</param>
        /// <param name="userID">mã người dùng</param>
        /// <returns>
        ///     1. Thông báo
        ///     2. True khi tạo thành công
        /// </returns>
        public async Task<(string, bool)> AddMessage(ChatModel chat, string userID)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    string json = JsonConvert.SerializeObject(chat);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(Constants.API.IP + beginUrl + "/messages/" + userID, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return ("Thêm tin nhắn thành công", true);
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
