using System;//RUIDONG
namespace EDP_Uplay.Models
{
    public class Redemptions
    {
        public int Id { get; set; }
        public string title { get; set; }
        public string image { get; set; }
        public int needPoints { get; set; }
        public string kind { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }

    public class RedemptionsRequest
    {
        public int Id { get; set; }
        public string title { get; set; }
        public int needPoints { get; set; }
        public string kind { get; set; }
        public IFormFile file { get; set; }
    }
}

