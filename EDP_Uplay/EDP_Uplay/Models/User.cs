using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EDP_Uplay.Models
{
    public enum UserRole
    {
        User,
        Admin
    }
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(100), JsonIgnore]
        public string PasswordHash { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }

        [JsonIgnore]
        public List<Ticket>? Tickets { get; set; }
        [MaxLength(20)]
        public string? ImageFile { get; set; }

        [Required]
        public UserRole Role { get; set; } = UserRole.User;
        public int earnPoints { get; set; }
        public int remainingPoints { get; set; }
        public int integral { get; set; }
        public string state { get; set; }
    }
}