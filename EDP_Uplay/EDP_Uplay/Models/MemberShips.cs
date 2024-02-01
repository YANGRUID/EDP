using System;//RUIDONG PAGE
namespace EDP_Uplay.Models
{
	public class MemberShips
	{


        public int Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int needPoints { get; set; }
        public string color { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }
	
}

