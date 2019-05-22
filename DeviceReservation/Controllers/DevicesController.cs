using DeviceReservation.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceReservation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/devices")]
    public class DevicesController : ControllerBase
    {
        readonly DataContext context;

        public DevicesController(DataContext context) =>
            this.context = context;

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var device = await context.Devices.FindAsync(id);
            if (device is null)
            {
                return NotFound();
            }

            context.Devices.Remove(device);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [EnableQuery]
        [HttpGet]
        public IQueryable<Device> Get() => context.Devices;

        [EnableQuery]
        [HttpGet("{id}")]
        public SingleResult<Device> Get(int id)
        {
            var queryable = context.Devices.Where(d => d.Id == id);
            return SingleResult.Create(queryable);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Device device)
        {
            if (await context.Devices.AnyAsync(d => d.SerialNumber == device.SerialNumber))
            {
                ModelState.AddModelError("", "The specified device already exists.");
                return Conflict(ModelState);
            }

            device.Reservations.Clear();

            context.Devices.Add(device);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = device.Id }, device);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, Device device)
        {
            var original = await context.Devices.FindAsync(id);
            if (original is null)
            {
                return NotFound();
            }

            if (await context.Devices.AnyAsync(d => d.Id != id && d.SerialNumber == device.SerialNumber))
            {
                ModelState.AddModelError("", "The specified device already exists.");
                return Conflict(ModelState);
            }

            original.Brand = device.Brand;
            original.Model = device.Model;
            original.SerialNumber = device.SerialNumber;
            original.Type = device.Type;

            context.Devices.Update(original);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}