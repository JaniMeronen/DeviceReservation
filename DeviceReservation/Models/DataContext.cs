using Microsoft.EntityFrameworkCore;

namespace DeviceReservation.Models
{
    public class DataContext : DbContext
    {
        public DbSet<Device> Devices { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        public DataContext(DbContextOptions options) : base(options)
        {
            Devices = Set<Device>();
            Reservations = Set<Reservation>();
        }
    }
}