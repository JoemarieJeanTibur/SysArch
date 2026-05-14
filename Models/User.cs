using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tibur_LabAct1.Models
{
    public class User
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string IdNumber { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        [Required]
        public int CourseLvl { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string RepeatPassword { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Course { get; set; }
        [Required]
        public string Address { get; set; }

        public string? ProfilePicture { get; set; }

        public int RemainingSession { get; set; } = 30;

    }
}
