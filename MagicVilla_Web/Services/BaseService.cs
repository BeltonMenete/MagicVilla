using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using magicvilla_Web.Models;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services
{
    public class BaseService(IHttpClientFactory httpClient) : IBaseService
    {
        public APIResponse ResponseModel = new();
        public IHttpClientFactory httpClientFactory = httpClient;

        public Task<T> SendAsync<T>(APIRequest apiRequest)
        {
            try
            {
                var client = httpClient.CreateClient();
            }
            catch ()
            {

            }
        }
    }
}