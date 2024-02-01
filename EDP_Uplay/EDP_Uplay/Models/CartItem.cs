using System.Text.Json.Serialization;

namespace EDP_Uplay.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ActivityId { get; set; }
        public int Quantity { get; set; }

        [JsonIgnore]
        public Cart Cart { get; set; }
        public Activity Activity { get; set; }
    }
}
