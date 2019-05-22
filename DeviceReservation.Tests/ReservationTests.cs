using DeviceReservation.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DeviceReservation.Tests
{
    public class ReservationTests : IClassFixture<SutFactory>
    {
        readonly HttpClient client;

        public ReservationTests(SutFactory factory) =>
            client = factory.CreateClient();

        [Fact]
        public async Task Delete_NoContent()
        {
            var response = await PostDeviceAsync();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var location = response.Headers.Location;

            response = await Post1Async(int.Parse(location.Segments.Last()));
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            response = await client.DeleteAsync(response.Headers.Location);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            response = await client.DeleteAsync(location);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Delete_NotFound()
        {
            var uri = new Uri(client.BaseAddress, "api/reservations/0");
            var response = await client.DeleteAsync(uri);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Get_OK()
        {
            var uri = new Uri(client.BaseAddress, "api/reservations");
            var response = await client.GetAsync(uri);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_NotFound()
        {
            var uri = new Uri(client.BaseAddress, "api/reservations/0");
            var response = await client.GetAsync(uri);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetById_OK()
        {
            var response = await PostDeviceAsync();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var location1 = response.Headers.Location;

            response = await Post1Async(int.Parse(location1.Segments.Last()));
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var location2 = response.Headers.Location;

            response = await client.GetAsync(location2);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            response = await client.DeleteAsync(location2);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            response = await client.DeleteAsync(location1);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Post_BadRequest()
        {
            var uri = new Uri(client.BaseAddress, "api/reservations");
            var response = await client.PostAsJsonAsync(uri, new Reservation());
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Post_Conflict_Device()
        {
            var response = await Post1Async(int.MaxValue);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Post_Conflict_Overlaping()
        {
            var response = await PostDeviceAsync();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var location1 = response.Headers.Location;
            var deviceId = int.Parse(location1.Segments.Last());

            response = await Post1Async(deviceId);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var location2 = response.Headers.Location;

            response = await Post1Async(deviceId);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

            response = await client.DeleteAsync(location2);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            response = await client.DeleteAsync(location1);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Post_Created()
        {
            var response = await PostDeviceAsync();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var location = response.Headers.Location;

            response = await Post1Async(int.Parse(location.Segments.Last()));
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            response = await client.DeleteAsync(response.Headers.Location);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            response = await client.DeleteAsync(location);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Put_BadRequest()
        {
            var uri = new Uri(client.BaseAddress, "api/reservations/0");
            var response = await client.PutAsJsonAsync(uri, new Reservation());
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Put_Conflict_Device()
        {
            var response = await PostDeviceAsync();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var location1 = response.Headers.Location;

            response = await Post1Async(int.Parse(location1.Segments.Last()));
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var location2 = response.Headers.Location;

            response = await Put1Async(location2, int.MaxValue);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

            response = await client.DeleteAsync(location2);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            response = await client.DeleteAsync(location1);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Put_Conflict_Overlaping()
        {
            var response = await PostDeviceAsync();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var location1 = response.Headers.Location;
            var deviceId = int.Parse(location1.Segments.Last());

            response = await Post1Async(deviceId);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var location2 = response.Headers.Location;

            response = await Post2Async(deviceId);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var location3 = response.Headers.Location;

            response = await Put2Async(location2, deviceId);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

            response = await client.DeleteAsync(location3);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            response = await client.DeleteAsync(location2);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            response = await client.DeleteAsync(location1);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Put_NoContent()
        {
            var response = await PostDeviceAsync();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var location1 = response.Headers.Location;
            var deviceId = int.Parse(location1.Segments.Last());

            response = await Post1Async(deviceId);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var location2 = response.Headers.Location;

            response = await Put2Async(location2, deviceId);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            response = await client.DeleteAsync(location2);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            response = await client.DeleteAsync(location1);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Put_NotFound()
        {
            var uri = new Uri(client.BaseAddress, "api/reservations/0");
            var response = await Put1Async(uri, int.MaxValue);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        Task<HttpResponseMessage> PostDeviceAsync()
        {
            var device = new Device
            {
                Brand = "Brand",
                Model = "Model",
                SerialNumber = Guid.NewGuid().ToString(),
                Type = "Type"
            };

            var uri = new Uri(client.BaseAddress, "api/devices");
            return client.PostAsJsonAsync(uri, device);
        }

        Task<HttpResponseMessage> Post1Async(int deviceId)
        {
            var reservation = new Reservation
            {
                DeviceId = deviceId,
                From = new DateTimeOffset(1, 1, 1, 0, 0, 0, TimeSpan.Zero),
                ReserverId = "ReserverId",
                ReserverName = "ReserverName",
                To = new DateTimeOffset(1, 1, 1, 1, 0, 0, TimeSpan.Zero)
            };

            var uri = new Uri(client.BaseAddress, "api/reservations");
            return client.PostAsJsonAsync(uri, reservation);
        }

        Task<HttpResponseMessage> Post2Async(int deviceId)
        {
            var reservation = new Reservation
            {
                DeviceId = deviceId,
                From = new DateTimeOffset(1, 1, 1, 1, 0, 0, TimeSpan.Zero),
                ReserverId = "ReserverId",
                ReserverName = "ReserverName",
                To = new DateTimeOffset(1, 1, 1, 2, 0, 0, TimeSpan.Zero)
            };

            var uri = new Uri(client.BaseAddress, "api/reservations");
            return client.PostAsJsonAsync(uri, reservation);
        }

        Task<HttpResponseMessage> Put1Async(Uri location, int deviceId)
        {
            var reservation = new Reservation
            {
                DeviceId = deviceId,
                From = new DateTimeOffset(1, 1, 1, 0, 0, 0, TimeSpan.Zero),
                ReserverId = "ReserverId",
                ReserverName = "ReserverName",
                To = new DateTimeOffset(1, 1, 1, 1, 0, 0, TimeSpan.Zero)
            };

            return client.PutAsJsonAsync(location, reservation);
        }

        Task<HttpResponseMessage> Put2Async(Uri location, int deviceId)
        {
            var reservation = new Reservation
            {
                DeviceId = deviceId,
                From = new DateTimeOffset(1, 1, 1, 1, 0, 0, TimeSpan.Zero),
                ReserverId = "ReserverId",
                ReserverName = "ReserverName",
                To = new DateTimeOffset(1, 1, 1, 2, 0, 0, TimeSpan.Zero)
            };

            return client.PutAsJsonAsync(location, reservation);
        }
    }
}