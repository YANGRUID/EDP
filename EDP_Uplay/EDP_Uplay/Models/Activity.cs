using System.ComponentModel.DataAnnotations;

namespace EDP_Uplay.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Price { get; set; }
        public string Type { get; set; }
        public string AvailableDates { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [MaxLength(20)]
        public string? ImageFile { get; set; }
    }
}
