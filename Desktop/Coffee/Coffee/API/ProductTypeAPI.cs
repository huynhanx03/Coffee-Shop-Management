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

namespace Coffee.API
{
    public class ProductTypeAPI
    {
        private static ProductTypeAPI _ins;

        public static ProductTypeAPI Ins
        {
            get
            {
                if (_ins == null)
                    _ins = new ProductTypeAPI();
                return _ins;
            }
            private set
            {
                _ins = value;
            }
        }

        public string beginUrl = "/product";

        /// <summary>
        /// Lấy danh sách loại sản phẩm
        /// </summary>
        /// <returns>
        ///     1. Thông báo
        ///     2. Danh sách loại sản phẩm
        /// </returns>
        public async Task<(string, List<ProductTypeDTO>)> GetProductTypes()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Send a GET request to the specified URL
                    HttpResponseMessage resp = await client.GetAsync(Constants.API.IP + beginUrl + "/categories");

                    string responseContent = resp.Content.ReadAsStringAsync().Result;

                    // Parse the JSON
                    var jsonObj = JObject.Parse(responseContent);

                    if (resp.IsSuccessStatusCode)
                    {
                        // Extract the data portion
                        var data = jsonObj["data"];

                        // Deserialize the data portion into a dictionary
                        var productTypesDict = JsonConvert.DeserializeObject<Dictionary<string, ProductTypeDTO>>(data.ToString());

                        // Convert the dictionary values to a list
                        var productTypes = productTypesDict.Values.ToList();

                        return ("Lấy danh sách loại sản phẩm thành công", productTypes);
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
