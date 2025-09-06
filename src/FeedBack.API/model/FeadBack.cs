using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeadBack.Models
{
    public class FeadBack
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; }

        [Required]
        public string VinAuto { get; set; } = null!;

        [Required]
        public string NameClient { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime DateTime { get; set; } = DateTime.UtcNow;

        public int feed { get; set; }
    }
}
