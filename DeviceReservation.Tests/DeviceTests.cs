using DeviceReservation.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DeviceReservation.Tests
{
    public class DeviceTests : IClassFixture<SutFactory>
    {
        readonly HttpClient client;

        public DeviceTests(SutFactory factory) =>
            client = factory.CreateClient();

        [Fact]
        public async Task Delete_NoContent()
        {
            var response = await Post1Async();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            response = await client.DeleteAsync(response.Headers.Location);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Delete_NotFound()
        {
            var uri = new Uri(client.BaseAddress, "api/devices/0");
            var response = await client.DeleteAsync(uri);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Get_OK()
        {
            var uri = new Uri(client.BaseAddress, "api/devices");
            var response = await client.GetAsync(uri);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_NotFound()
        {
            var uri = new Uri(client.BaseAddress, "api/devices/0");
            var response = await client.GetAsync(uri);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetById_OK()
        {
            var response = await Post1Async();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var location = response.Headers.Location;

            response = await client.GetAsync(location);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            response = await client.DeleteAsync(location);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Post_BadRequest()
        {
            var uri = new Uri(client.BaseAddress, "api/devices");
            var response = await client.PostAsJsonAsync(uri, new Device());
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Post_Conflict()
        {
            var response = await Post1Async();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var location = response.Headers.Location;

            response = await Post1Async();
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

            response = await client.DeleteAsync(location);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Post_Created()
        {
            var response = await Post1Async();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            response = await client.DeleteAsync(response.Headers.Location);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Put_BadRequest()
        {
            var uri = new Uri(client.BaseAddress, "api/devices/0");
            var response = await client.PutAsJsonAsync(uri, new Device());
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Put_Conflict()
        {
            var response = await Post1Async();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var location1 = response.Headers.Location;

            response = await Post2Async();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var location2 = response.Headers.Location;

            response = await Put2Async(location1);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

            response = await client.DeleteAsync(location2);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            response = await client.DeleteAsync(location1);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Put_NoContent()
        {
            var response = await Post1Async();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var location = response.Headers.Location;

            response = await Put1Async(location);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            response = await client.DeleteAsync(location);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Put_NotFound()
        {
            var uri = new Uri(client.BaseAddress, "api/devices/0");
            var response = await Put1Async(uri);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        Task<HttpResponseMessage> Post1Async()
        {
            var device = new Device
            {
                Brand = "Brand",
                Model = "Model",
                SerialNumber = "1",
                Type = "Type"
            };

            var uri = new Uri(client.BaseAddress, "api/devices");
            return client.PostAsJsonAsync(uri, device);
        }

        Task<HttpResponseMessage> Post2Async()
        {
            var device = new Device
            {
                Brand = "Brand",
                Model = "Model",
                SerialNumber = "2",
                Type = "Type"
            };

            var uri = new Uri(client.BaseAddress, "api/devices");
            return client.PostAsJsonAsync(uri, device);
        }

        Task<HttpResponseMessage> Put1Async(Uri location)
        {
            var device = new Device
            {
                Brand = "Brand",
                Model = "Model",
                SerialNumber = "1",
                Type = "Type"
            };

            return client.PutAsJsonAsync(location, device);
        }

        Task<HttpResponseMessage> Put2Async(Uri location)
        {
            var device = new Device
            {
                Brand = "Brand",
                Model = "Model",
                SerialNumber = "2",
                Type = "Type"
            };

            return client.PutAsJsonAsync(location, device);
        }
    }
}