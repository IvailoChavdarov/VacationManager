using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace VacationManager.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        [MaxLength(70)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(70)]
        public string LastName { get; set; }

        public int? TeamId { get; set; }
        public Team Team { get; set; }

        public int? TeamLedId { get; set; }
        public Team TeamLed { get; set; }

        public IEnumerable<Holiday> RequestedHolidays { get; set; } 
    }
}
