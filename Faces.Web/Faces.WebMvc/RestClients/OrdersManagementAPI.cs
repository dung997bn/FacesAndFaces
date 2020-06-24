using Faces.WebMvc.Models;
using Microsoft.Extensions.Configuration;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Faces.WebMvc.RestClients
{
    public class OrdersManagementAPI : IOrdersManagementAPI
    {
        private IOrdersManagementAPI _restClient;
        public OrdersManagementAPI(IConfiguration config, HttpClient httpClient)
        {
            string apiHostAndPort = config.GetSection("ApiServiceLocation").GetValue<string>("OrdersApiLocation");
            httpClient.BaseAddress = new Uri($"http://{apiHostAndPort}/api");
            _restClient = RestService.For<IOrdersManagementAPI>(httpClient);
        }
        public async Task<OrderViewModel> GetOrderById(string id)
        {
            try
            {
                return await _restClient.GetOrderById(id);
            }
            catch (ApiException ex)
            {

                if (ex.StatusCode == HttpStatusCode.NotFound)
                    return null;
                else
                    throw ex;

            }
        }

        public async Task<List<OrderViewModel>> GetOrders()
        {
            return await _restClient.GetOrders();
        }
    }
}
