using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VacationManager.Models
{
    public class Holiday
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime DateOfRequest { get; set; }

        [Required]
        public bool IsHalfDay { get; set; }

        [Required]
        public bool IsApproved { get;set; }

        [Required]
        public string RequesterId { get; set; }

        public AppUser Requester { get; set; }
    }
}
