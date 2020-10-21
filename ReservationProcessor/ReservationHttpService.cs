using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReservationProcessor
{
    public class ReservationHttpService
    {
        private readonly HttpClient _client;

        public ReservationHttpService(HttpClient client, IConfiguration config)
        {
            _client = client;
            _client.BaseAddress = new Uri(config.GetValue<string>("api"));
            _client.DefaultRequestHeaders.Add("accept", "application/json");
            _client.DefaultRequestHeaders.Add("User-Agent", "reservationprocessor");
        }
        public async Task MarkReservationAccepted(ReservationMessage reservation)
        {
            await DoIt(reservation, "accepted");
        }

        public async Task MarkReservationRejected(ReservationMessage reservation)
        {
            await DoIt(reservation, "rejected");
        }

        private async Task DoIt(ReservationMessage reservation, string status)
        {
            var reservationJson = JsonSerializer.Serialize(reservation);
            var content = new StringContent(reservationJson);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await _client.PostAsync($"reservations/{status}", content);
            response.EnsureSuccessStatusCode();
        }

    }
}
