using System.ComponentModel.DataAnnotations;

namespace HerosPath.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Session> Sessions { get; set; }
        public ICollection<LevelProgress> LevelProgresses { get; set; }
    }
}