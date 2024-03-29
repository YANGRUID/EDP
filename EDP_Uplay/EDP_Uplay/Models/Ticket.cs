﻿using System.ComponentModel.DataAnnotations;

namespace EDP_Uplay.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
        [MaxLength(20)]
        public string? ImageFile { get; set; }
    }
}
