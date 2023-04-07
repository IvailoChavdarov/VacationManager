using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationManager.Models
{
    public class Team
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public string LeaderId { get; set; }

        public int? ProjectId { get; set; }

        public AppUser Leader { get; set; }

        public IEnumerable<AppUser> Members { get; set;}

        public Project Project { get; set; }
    }
}
