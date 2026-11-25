using HerosPath.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HerosPath.Models
{
    public class LevelProgress
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
        [ForeignKey("Level")]
        public int LevelId { get; set; }
        public Level Level { get; set; }
        public int Stars { get; set; } = 0;
        public int BestMoves { get; set; } = 0;
        public bool IsCompleted { get; set; } = false;
        public bool IsUnlocked { get; set; } = false;
        public DateTime CompletedAt { get; set; }
    }
}