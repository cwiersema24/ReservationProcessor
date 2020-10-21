using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReservationProcessor.Utils
{
    public class ConsumerWrapper
    {

        private readonly string _topic;
        private readonly ConsumerConfig _config;
        private readonly IConsumer<Null, string> _consumer;

        public ConsumerWrapper(string topic, ConsumerConfig config)
        {
            _topic = topic;
            _config = config;
            _consumer = new ConsumerBuilder<Null, string>(_config).Build();
            _consumer.Subscribe(_topic);
        }

        public T ReadMessage<T>()
        {
            var response = _consumer.Consume();
            var options = new JsonSerializerOptions();
            options.IgnoreNullValues = true;
            options.Converters.Add(new JsonStringEnumConverter());
            return JsonSerializer.Deserialize<T>(response.Message.Value);
        }
    }
}
