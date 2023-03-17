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

        public int TeamId { get; set; }
        public Team Team { get; set; }
    }
}
