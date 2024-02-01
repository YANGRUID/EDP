namespace EDP_Uplay.Models
{
    public class Cart
    {
   
            public int Id { get; set; }
            public int UserId { get; set; } 

            public User User { get; set; }
            public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

            public DateTime LastUpdated { get; set; }
            // Other fields like TotalPrice can be calculated properties based on CartItems
        
    }
}
