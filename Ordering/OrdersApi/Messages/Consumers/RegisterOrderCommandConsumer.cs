using MassTransit;
using MessagingInterfacesContants.Commands;
using MessagingInterfacesContants.Events;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using OrdersApi.Hubs;
using OrdersApi.Models;
using OrdersApi.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OrdersApi.Messages.Consumers
{
    public class RegisterOrderCommandConsumer : IConsumer<IRegisterOderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHubContext<OrderHub> _hubContext;

        public RegisterOrderCommandConsumer(IOrderRepository orderRepository, IHttpClientFactory clientFactory, IHubContext<OrderHub> hubContext)
        {
            _orderRepository = orderRepository;
            _clientFactory = clientFactory;
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<IRegisterOderCommand> context)
        {
            var result = context.Message;
            if (result.OrderId != null && result.PictureUrl != null
                && result.UserEmail != null && result.ImageData != null)
            {
                SaveOrder(result);
                await _hubContext.Clients.All.SendAsync("UpdateOrders", "New Order Created", result.OrderId);

                var client = _clientFactory.CreateClient();
                Tuple<List<byte[]>, Guid> orderDetailData = await GetFacesFromFaceApiAsync(client, result.ImageData, result.OrderId);
                List<byte[]> faces = orderDetailData.Item1;
                Guid orderId = orderDetailData.Item2;
                SaveOrderDetails(orderId, faces);

                await _hubContext.Clients.All.SendAsync("UpdateOrders", "Order processed", result.OrderId);

                await context.Publish<IOrderProcessedEvent>(new
                {
                    OrderId = orderId,
                    result.UserEmail,
                    Faces = faces,
                    result.PictureUrl
                });
            }
        }

        private async Task<Tuple<List<byte[]>, Guid>> GetFacesFromFaceApiAsync(HttpClient client, byte[] imageData, Guid orderId)
        {
            var byteContent = new ByteArrayContent(imageData);
            Tuple<List<byte[]>, Guid> orderDetailData = null;
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            using (var response = await client.PostAsync("http://localhost:6000/api/faces?orderId=" + orderId, byteContent))
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                try
                {
                    orderDetailData = JsonConvert.DeserializeObject<Tuple<List<byte[]>, Guid>>(apiResponse);
                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }
            return orderDetailData;
        }

        private void SaveOrderDetails(Guid orderId, List<byte[]> faces)
        {
            var order = _orderRepository.GetOrderAsync(orderId).Result;
            if (order != null)
            {
                order.Status = Status.Processed;
                foreach (var face in faces)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = orderId,
                        FaceData = face
                    };
                    order.OrderDetails.Add(orderDetail);
                }
                _orderRepository.UpdateOrder(order);
            }
        }

        private void SaveOrder(IRegisterOderCommand result)
        {
            Order order = new Order
            {
                OrderId = result.OrderId,
                UserEmail = result.UserEmail,
                Status = Status.Registered,
                PictureUrl = result.PictureUrl,
                ImageData = result.ImageData
            };
            _orderRepository.RegisterOrder(order);
        }
    }
}
