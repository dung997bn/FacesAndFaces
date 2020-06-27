using EmailService;
using MassTransit;
using MessagingInterfacesContants.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Comsumers
{
    public class OrderProcessedEventConsumer : IConsumer<IOrderProcessedEvent>
    {
        private readonly IEmailSender _emailSender;

        public OrderProcessedEventConsumer(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task Consume(ConsumeContext<IOrderProcessedEvent> context)
        {
            var rootFolder = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin"));
            var result = context.Message;
            var facesData = result.Faces;
            if (facesData.Count < 1)
            {
                await Console.Out.WriteLineAsync($"No faces detected");
            }
            else
            {
                int j = 0;
                foreach (var face in facesData)
                {
                    MemoryStream ms = new MemoryStream(face);
                    var image = Image.FromStream(ms);
                    image.Save(rootFolder + "/Images/face" + j + ".jpg", ImageFormat.Jpeg);
                    j++;
                }
            }
            //Here is add Email Sending code
            List<string> mailAddress = new List<string>();
            mailAddress.Add(result.UserEmail);
            await _emailSender.SendEmailAsync(new Message(mailAddress, "your order" + result.OrderId,
                  "From FacesAndFaces", facesData));

            await context.Publish<IOrderDispatchedEvent>(new
            {
                OrderId = context.Message.OrderId,
                DispatchDateTime = DateTime.UtcNow
            });
        }
    }
}
