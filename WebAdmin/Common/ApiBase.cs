using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebAdmin.Models.API;

namespace WebApp.Common
{
    public class ApiBase
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ApiBase()
        {
        }

        public static async Task<HttpResponseMessage> GetToken(string url)
        {
            HttpResponseMessage response = null;
            try
            {
                string insideUrl = ConfigurationManager.AppSettings["ApiUri"];
                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(insideUrl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromSeconds(20);
                    response = await Task.FromResult(client.GetAsync(url).Result);
                    log.Debug("Trạng thái của API " + client.BaseAddress + ": " + response.StatusCode);

                    return response;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.NotImplemented,
                    Content = new StringContent(JsonConvert.SerializeObject(new APIResult { message = ex.Message }))
                };
            }
        }

        public static async Task<HttpResponseMessage> PostJsonAsync(string accessToken, string uri, string json)
        {
            try
            {
                string insideUrl = ConfigurationManager.AppSettings["ApiUri"];
                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(insideUrl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await Task.FromResult(client.PostAsync(uri, content).Result);
                    return response;
                }
            }
            catch (Exception ex)
            {
                log.Info("---ex End API");
                log.Error(ex.Message);
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.NotImplemented,
                    Content = new StringContent(JsonConvert.SerializeObject(new APIResult { message = ex.Message }))
                };
            }
        }

        public static async Task<string> GetJsonAsync(string accessToken, string url)
        {
            HttpResponseMessage response = null;
            try
            {
                string insideUrl = ConfigurationManager.AppSettings["ApiUri"];
                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(insideUrl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromSeconds(20);
                    response = await Task.FromResult(client.GetAsync(url).Result);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseString = response.Content.ReadAsStringAsync().Result;
                        return responseString;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return ex.Message;
            }
            return null;
        }

        public static async Task<HttpResponseMessage> GetJsonAsyncResponse(string accessToken, string url)
        {
            HttpResponseMessage response = null;
            try
            {
                string insideUrl = ConfigurationManager.AppSettings["ApiUri"];
                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(insideUrl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromSeconds(20);
                    response = await Task.FromResult(client.GetAsync(url).Result);
                    log.Debug("Trạng thái của API " + client.BaseAddress + ": " + response.StatusCode);

                    return response;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.NotImplemented,
                    Content = new StringContent(JsonConvert.SerializeObject(new APIResult { message = ex.Message }))
                };
            }
        }

        public static async Task<string> GetBase64Async(string accessToken, string url)
        {
            try
            {
                string insideUrl = ConfigurationManager.AppSettings["ApiUri"];
                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(insideUrl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromSeconds(20);
                    var bytes = await Task.FromResult(client.GetByteArrayAsync(url).Result);
                    string base64 = Convert.ToBase64String(bytes);
                    return base64;
                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return ex.Message;
            }
        }

        // CuongHM add
        public static async Task<string> PutJsonAsync(string accessToken, string uri, string json)
        {
            try
            {
                string insideUrl = ConfigurationManager.AppSettings["ApiUri"];
                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(insideUrl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await Task.FromResult(client.PutAsync(uri, content).Result);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseString = response.Content.ReadAsStringAsync().Result;
                        return responseString;
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return ex.Message;
            }
            return null;
        }

        public static async Task<HttpResponseMessage> PutJsonAsyncResponse(string accessToken, string uri, string json)
        {
            try
            {
                string insideUrl = ConfigurationManager.AppSettings["ApiUri"];
                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(insideUrl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromSeconds(20);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await Task.FromResult(client.PutAsync(uri, content).Result);
                    return response;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.NotImplemented,
                    Content = new StringContent(JsonConvert.SerializeObject(new APIResult { message = ex.Message }))
                };
            }
        }
    }
}