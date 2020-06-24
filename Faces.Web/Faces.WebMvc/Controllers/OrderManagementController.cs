using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Faces.WebMvc.RestClients;
using Microsoft.AspNetCore.Mvc;

namespace Faces.WebMvc.Controllers
{
    public class OrderManagementController : Controller
    {
        private readonly IOrdersManagementAPI _ordersManagementAPI;

        public OrderManagementController(IOrdersManagementAPI ordersManagementAPI)
        {
            _ordersManagementAPI = ordersManagementAPI;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _ordersManagementAPI.GetOrders();
            foreach (var order in orders)
            {
                order.ImageString = ConvertAndFormatToString(order.ImageData);
            }

            return View(orders);
        }
        private string ConvertAndFormatToString(byte[] imageData)
        {
            string imageBase64Data = Convert.ToBase64String(imageData);
            return string.Format("data:image/png;base64, {0}", imageBase64Data);
        }



        [Route("/Details/{orderId}")]
        public async Task<IActionResult> Details(string orderId)
        {
            var order = await _ordersManagementAPI.GetOrderById(orderId);
            order.ImageString = ConvertAndFormatToString(order.ImageData);
            foreach (var detail in order.OrderDetails)
            {
                detail.ImageString = ConvertAndFormatToString(detail.FaceData);
            }
            return View(order);

        }
    }
}
