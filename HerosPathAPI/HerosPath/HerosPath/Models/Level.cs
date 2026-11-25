using System.ComponentModel.DataAnnotations;

namespace HerosPath.Models
{
    public class Level
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int LevelNumber { get; set; }
        public int OrderIndex { get; set; }
    }
}