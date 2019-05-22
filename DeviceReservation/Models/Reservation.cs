using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeviceReservation.Models
{
    public class Reservation : IValidatableObject
    {
        [ValidateNever]
        public Device Device { get; set; } = new Device();

        [Range(1, int.MaxValue)]
        public int DeviceId { get; set; } = 0;

        public DateTimeOffset From { get; set; } = DateTimeOffset.MinValue;

        [Range(0, 0)]
        public int Id { get; set; } = 0;

        [Required]
        [StringLength(256)]
        public string ReserverId { get; set; } = "";

        [Required]
        [StringLength(256)]
        public string ReserverName { get; set; } = "";

        public DateTimeOffset To { get; set; } = DateTimeOffset.MinValue;

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (From >= To)
            {
                yield return new ValidationResult("The specified date range is not valid.");
            }
        }
    }
}