using EDP_Uplay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EDP_Uplay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _configuration;
        public CartController(MyDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [Authorize]
        [HttpPost("addItem")]
        public IActionResult AddItemToCart(int activityId, int quantity)
        {
            var userIdString = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized("Need to Login First.");
            }
            var activity = _context.Activities.FirstOrDefault(a => a.Id == activityId);
            if (activity == null)
            {
                return NotFound("Activity not found.");
            }
            if (activity.Capacity < quantity)
            {
                return BadRequest("Insufficient capacity available.");
            }
            var cart = _context.Carts.Include(c => c.CartItems)
                                     .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>(),
                    LastUpdated = DateTime.UtcNow
                };
                _context.Carts.Add(cart);
            }
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ActivityId == activityId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity; // Update quantity if item already exists
            }
            else
            {
                cartItem = new CartItem // Create new CartItem
                {
                    ActivityId = activityId,
                    Quantity = quantity,
                };
                cart.CartItems.Add(cartItem);
            }

            _context.SaveChanges();
            return Ok("Item added to cart.");
        }
        [HttpGet("view")]
        public IActionResult ViewCart()
        {
            var userIdString = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            var cart = _context.Carts.Include(c => c.CartItems)
                                     .ThenInclude(ci => ci.Activity)
                                     .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                // Return an empty cart instead of 404
                return Ok(new Cart { UserId = userId, CartItems = new List<CartItem>() });
            }

            return Ok(cart);
        }
        [Authorize]
        [HttpDelete("removeItem/{cartItemId}")]
        public IActionResult RemoveItemFromCart(int cartItemId)
        {
            var userIdString = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            var cartItem = _context.CartItems
                                   .Include(ci => ci.Cart)
                                   .FirstOrDefault(ci => ci.Id == cartItemId && ci.Cart.UserId == userId);

            if (cartItem == null)
            {
                return NotFound("Cart item not found.");
            }

            _context.CartItems.Remove(cartItem);
            _context.SaveChanges();

            return Ok("Item removed from cart.");
        }
        [Authorize]
        [HttpPut("updateItem/{cartItemId}")]
        public IActionResult UpdateCartItem(int cartItemId, int newQuantity)
        {
            var userIdString = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            var cartItem = _context.CartItems
                                   .Include(ci => ci.Cart)
                                   .Include(ci => ci.Activity)
                                   .FirstOrDefault(ci => ci.Id == cartItemId && ci.Cart.UserId == userId);

            if (cartItem == null)
            {
                return NotFound("Cart item not found.");
            }

            if (newQuantity <= 0 || cartItem.Activity.Capacity < newQuantity)
            {
                return BadRequest("Invalid quantity.");
            }

            cartItem.Quantity = newQuantity;
            _context.SaveChanges();

            return Ok("Cart item quantity updated.");
        }
    }
}
