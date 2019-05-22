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
    [Route("api/reservations")]
    public class ReservationsController : ControllerBase
    {
        readonly DataContext context;

        public ReservationsController(DataContext context) =>
            this.context = context;

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var reservation = await context.Reservations.FindAsync(id);
            if (reservation is null)
            {
                return NotFound();
            }

            context.Reservations.Remove(reservation);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [EnableQuery]
        [HttpGet]
        public IQueryable<Reservation> Get() => context.Reservations;

        [EnableQuery]
        [HttpGet("{id}")]
        public SingleResult<Reservation> Get(int id)
        {
            var queryable = context.Reservations.Where(r => r.Id == id);
            return SingleResult.Create(queryable);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Reservation reservation)
        {
            reservation.Device = await context.Devices.FindAsync(reservation.DeviceId);
            if (reservation.Device is null)
            {
                ModelState.AddModelError(nameof(Reservation.DeviceId), "The specified device does not exist.");
                return Conflict(ModelState);
            }

            if (await context.Reservations.AnyAsync(r => r.DeviceId == reservation.DeviceId && r.From < reservation.To && reservation.From < r.To))
            {
                ModelState.AddModelError("", "The specified reservation already exists.");
                return Conflict(ModelState);
            }

            context.Reservations.Add(reservation);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = reservation.Id }, reservation);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, Reservation reservation)
        {
            var original = await context.Reservations.FindAsync(id);
            if (original is null)
            {
                return NotFound();
            }

            original.Device = await context.Devices.FindAsync(reservation.DeviceId);
            if (original.Device is null)
            {
                ModelState.AddModelError(nameof(Reservation.DeviceId), "The specified device does not exist.");
                return Conflict(ModelState);
            }

            if (await context.Reservations.AnyAsync(r => r.DeviceId == reservation.DeviceId && r.Id != id && r.From < reservation.To && reservation.From < r.To))
            {
                ModelState.AddModelError("", "The specified reservation already exists.");
                return Conflict(ModelState);
            }

            original.From = reservation.From;
            original.ReserverId = reservation.ReserverId;
            original.ReserverName = reservation.ReserverName;
            original.To = reservation.To;

            context.Reservations.Update(original);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}