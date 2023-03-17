using System.ComponentModel.DataAnnotations;

namespace VacationManager.Models
{
    public class Team
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public string LeaderId { get; set; }
        public AppUser Leader { get; set; }
        public IEnumerable<AppUser> Developers { get; set;}

    }
}
