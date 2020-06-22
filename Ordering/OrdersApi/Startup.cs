using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using MessagingInterfacesContants.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrdersApi.Messages.Consumers;
using OrdersApi.Persistence;
using OrdersApi.Services;

namespace OrdersApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<OrdersContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("OrdersContextConnection")
                ));

            services.AddHttpClient();

            //Repository
            services.AddTransient<IOrderRepository, OrderRepository>();

            //Masstransit Configuration
            services.AddMassTransit(c =>
            {
                c.AddConsumer<RegisterOrderCommandConsumer>();
            });
            services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(
                cfg =>
                {
                    var host = cfg.Host("localhost", "/", h => { });
                    cfg.ReceiveEndpoint(RabbitMQMassTransitConstants.RegisterOrderCommandQueue, e =>
                    {
                        e.PrefetchCount = 16;
                        e.UseMessageRetry(x => x.Interval(2, TimeSpan.FromSeconds(10)));
                        e.Consumer<RegisterOrderCommandConsumer>(provider);
                    });

                    cfg.ConfigureEndpoints(provider);
                }));
            services.AddSingleton<IHostedService, BusService>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
