using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TimeSync.Core.model
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public required string Email { get; set; }

        [Required]
        [Column(TypeName = "varchar(255)")]
        public required string Password { get; set; }
    }
}
