using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeviceReservation.Models
{
    public class Device
    {
        [Required]
        [StringLength(256)]
        public string Brand { get; set; } = "";

        [Range(0, 0)]
        public int Id { get; set; } = 0;

        [Required]
        [StringLength(256)]
        public string Model { get; set; } = "";

        [ValidateNever]
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();

        [Required]
        [StringLength(256)]
        public string SerialNumber { get; set; } = "";

        [Required]
        [StringLength(256)]
        public string Type { get; set; } = "";
    }
}