using Faces.WebMvc.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Faces.WebMvc.RestClients
{
    public interface IOrdersManagementAPI
    {
        [Get("/orders")]
        Task<List<OrderViewModel>> GetOrders();
        [Get("/orders/{orderId}")]
        Task<OrderViewModel> GetOrderById(string orderId);
    }
}
