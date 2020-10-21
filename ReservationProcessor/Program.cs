using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ReservationProcessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var consumerConfig = new ConsumerConfig();
                    hostContext.Configuration.Bind("kafka", consumerConfig);
                    services.AddHttpClient<ReservationHttpService>();
                    services.AddSingleton<ConsumerConfig>(consumerConfig);
                    services.AddHostedService<Worker>();
                });
    }
}
