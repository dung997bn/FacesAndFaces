using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Faces.WebMvc.Models;
using MassTransit;
using System.IO;
using MessagingInterfacesContants.Constants;
using MessagingInterfacesContants.Commands;

namespace Faces.WebMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBusControl _busControl;

        public HomeController(ILogger<HomeController> logger, IBusControl busControl)
        {
            _logger = logger;
            _busControl = busControl;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RegisterOrder()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterOrder(OrderViewModel model)
        {
            MemoryStream memory = new MemoryStream();
            using (var uploadedFile = model.File.OpenReadStream())
            {
                await uploadedFile.CopyToAsync(memory);
            }
            model.ImageData = memory.ToArray();
            model.PictureUrl = model.File.FileName;
            model.OrderId = Guid.NewGuid();
            var sendToUri = new Uri($"{RabbitMQMassTransitConstants.RabbitMqUri }" +

                $"{RabbitMQMassTransitConstants.RegisterOrderCommandQueue}");

            var endPoint = await _busControl.GetSendEndpoint(sendToUri);

            await endPoint.Send<IRegisterOderCommand>(new
            {
                model.OrderId,
                model.UserEmail,
                model.ImageData,
                model.PictureUrl
            });
            ViewData["OrderId"] = model.OrderId;
            return View("Thanks");
        }
    }
}
