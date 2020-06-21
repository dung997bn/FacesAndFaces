using MassTransit;
using MessagingInterfacesContants.Commands;
using OrdersApi.Models;
using OrdersApi.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersApi.Messages.Consumers
{
    public class RegisterOrderCommandConsumer : IConsumer<IRegisterOderCommand>
    {
        private IOrderRepository _orderRepository;

        public RegisterOrderCommandConsumer(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public Task Consume(ConsumeContext<IRegisterOderCommand> context)
        {
            var result = context.Message;
            if(result.OrderId != null && result.PictureUrl !=null 
                && result.UserEmail !=null && result.ImageData != null)
            {
                SaveOrder(result);
            }
            return Task.FromResult(true);
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
