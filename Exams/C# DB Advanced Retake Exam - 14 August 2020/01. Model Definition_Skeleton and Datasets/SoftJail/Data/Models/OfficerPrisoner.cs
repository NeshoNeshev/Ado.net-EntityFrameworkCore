
using System.ComponentModel.DataAnnotations;

namespace SoftJail.Data.Models
{
    public class OfficerPrisoner
    {
        [Key]
        public int 	PrisonerId  { get; set; }
        [Required]
        public Prisoner Prisoner { get; set; }
        [Key]
        public int	OfficerId  { get; set; }
        [Required]
        public Officer Officer { get; set; }
    }
}
