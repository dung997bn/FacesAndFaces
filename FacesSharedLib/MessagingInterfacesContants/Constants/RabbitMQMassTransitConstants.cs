using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingInterfacesContants.Constants
{
    public class RabbitMQMassTransitConstants
    {
        public const string RabbitMqUri = "rabbitmq://localhost/";
        public const string UserName = "guest";
        public const string Password = "guest";
        public const string RegisterOrderCommandQueue = "register.orders.command";

        public const string NotificationServiceQueue = "notification.service.queue";
        public const string OrderDispatchedServiceQueue = "order.dispatch.service.queue";
    }
}
