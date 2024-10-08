using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using magicvilla_Web.Models;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services
{
    public class BaseService(IHttpClientFactory httpClient) : IBaseService
    {
        public APIResponse ResponseModel = new();
        public IHttpClientFactory HttpClient = httpClient;

        public Task<T> SendAsync<T>(APIRequest apiRequest)
        {
            try
            {

                var client = httpClient.CreateClient("MagicAPI");
                HttpRequestMessage message = new();

                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url);

                if (apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.Se);
                }
            }
            catch ()
            {

            }
        }
    }
}