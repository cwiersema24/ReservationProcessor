using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReservationProcessor.Utils;

namespace ReservationProcessor
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ConsumerConfig _config;
        private readonly ReservationHttpService _httpService;

        public Worker(ILogger<Worker> logger, ConsumerConfig config, ReservationHttpService httpService)
        {
            _logger = logger;
            _config = config;
            _httpService = httpService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumerHelper = new ConsumerWrapper("libraryreservations", _config);
            _logger.LogInformation("This Service is running and waiting for reservations");
            while (!stoppingToken.IsCancellationRequested)
            {
                var order = consumerHelper.ReadMessage<ReservationMessage>();
                _logger.LogInformation($"Got a reservation fo {order.For} for the items {order.Items}");

                var numberOfItems = order.Items.Split(',').Count();
                await Task.Delay(1000 * numberOfItems);
                if(numberOfItems % 2 == 0)
                {
                    await _httpService.MarkReservationAccepted(order);
                    _logger.LogInformation("\tApproved that order.");
                }
                else
                {
                    await _httpService.MarkReservationRejected(order);
                    _logger.LogInformation("\tRejected that one!");
                }
            }
            
        }
    }

    public class ReservationMessage
    {
        public int Id { get; set; }
        public string For { get; set; }
        public string Items { get; set; }
        public string Status { get; set; }
    }

}
