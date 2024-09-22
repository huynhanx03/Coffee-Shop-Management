using Coffee.DTOs;
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
    public class TableAPI
    {
        private static TableAPI _ins;

        public static TableAPI Ins
        {
            get
            {
                if (_ins == null)
                    _ins = new TableAPI();
                return _ins;
            }
            private set
            {
                _ins = value;
            }
        }

        public string beginUrl = "/table";

        /// <summary>
        /// Lấy danh sách bàn
        /// </summary>
        /// <returns>
        ///     Danh sách bàn
        /// </returns>
        public async Task<(string, List<TableDTO>)> getTables()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage resp = await client.GetAsync(Constants.API.IP + beginUrl + "/tables");

                    string responseContent = await resp.Content.ReadAsStringAsync();

                    var jsonObj = JObject.Parse(responseContent);

                    if (resp.IsSuccessStatusCode)
                    {
                        var data = jsonObj["data"];
                        var tables = JsonConvert.DeserializeObject<List<TableDTO>>(data.ToString());
                        return ("Lấy danh sách bàn thành công", tables);
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
        /// Lấy danh sách loại bàn
        /// </summary>
        /// <returns>
        ///     Danh sách loại bàn
        /// </returns>
        public async Task<(string, List<TableTypeDTO>)> getTableTypes()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage resp = await client.GetAsync(Constants.API.IP + beginUrl + "/types");

                    string responseContent = await resp.Content.ReadAsStringAsync();

                    var jsonObj = JObject.Parse(responseContent);

                    if (resp.IsSuccessStatusCode)
                    {
                        var data = jsonObj["data"];
                        var tableTypes = JsonConvert.DeserializeObject<List<TableTypeDTO>>(data.ToString());
                        return ("Lấy danh sách loại bàn thành công", tableTypes);
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
        /// Thêm bàn
        /// INPUT: TableDTO: bàn
        /// </summary>
        /// <param name="table"></param>
        /// <returns>
        ///     1: Lỗi khi thêm dữ liệu
        ///     2: bàn
        /// </returns>
        public async Task<(string, TableDTO)> createTable(TableDTO table)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    string json = JsonConvert.SerializeObject(table);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(Constants.API.IP + beginUrl + "/table", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return ("Thêm bàn thành công", table);
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
        /// Cập nhật bàn
        /// INPUT: TableDTO: bàn
        /// </summary>
        /// <param name="table"></param>
        /// <returns>
        ///     1: Thông báo
        ///     2: bàn
        /// </returns>
        public async Task<(string, TableDTO)> updateTable(TableDTO table)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    string json = JsonConvert.SerializeObject(table);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync(Constants.API.IP + beginUrl + "/table/" + table.MaBan, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return ("Cập nhật bàn thành công", table);
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
        /// Xoá bàn
        /// INPUT:
        ///     tableID: mã bàn
        /// </summary>
        /// <param name="tableID"></param>
        /// <returns>
        ///     1: Thông báo
        ///     2: True nếu xoá thành công, False xoá thất bại
        /// </returns>
        public async Task<(string, bool)> deleteTable(string tableID)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string token = Helper.getToken();

                    // Add Bearer token to Authorization header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage response = await client.DeleteAsync(Constants.API.IP + beginUrl + $"/table/{tableID}");

                    if (response.IsSuccessStatusCode)
                    {
                        return ("Xoá bàn thành công", true);
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

        public async Task<(string, bool)> updateStatusTable(string tableID, string _status)
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
                        status = _status
                    };

                    string json = JsonConvert.SerializeObject(container);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync(Constants.API.IP + beginUrl + "/status- table/" + tableID, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return ("Cập nhật bàn thành công", true);
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
