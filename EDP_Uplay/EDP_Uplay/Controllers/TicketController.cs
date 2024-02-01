using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using EDP_Uplay.Models;
using Microsoft.EntityFrameworkCore;

namespace EDP_Uplay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _configuration;
        public TicketController(MyDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        private int GetUserId()
        {
            return Convert.ToInt32(User.Claims
            .Where(c => c.Type == ClaimTypes.NameIdentifier)
            .Select(c => c.Value).SingleOrDefault());
        }
        [HttpPost, Authorize]
        public IActionResult AddTicket(Ticket ticket)
        {
            int userId = GetUserId();
            var now = DateTime.Now;
            var myTicket = new Ticket()
            {
                Title = ticket.Title.Trim(),
                Description = ticket.Description.Trim(),
                CreatedAt = now,
                UpdatedAt = now,
                UserId = userId
            };
            _context.Tickets.Add(myTicket);
            _context.SaveChanges();
            return Ok(myTicket);
        }
        [HttpGet]
        public IActionResult GetAll(string? search)
        {
            IQueryable<Ticket> result = _context.Tickets.Include(t => t.User);
            if (search != null)
            {
                result = result.Where(x => x.Title.Contains(search)
                || x.Description.Contains(search));
            }
            var list = result.OrderByDescending(x => x.CreatedAt).ToList();
            var data = list.Select(t => new
            {
                t.Id,
                t.Title,
                t.Description,
                t.CreatedAt,
                t.UpdatedAt,
                t.UserId,
                User = new
                {
                    t.User?.Username
                }
            });
            return Ok(data);
        }
        [HttpGet("{id}"), Authorize]
        public IActionResult GetTicket(int id)
        {
            var myTicket = _context.Tickets.Find(id);
            if (myTicket == null)
            {
                return NotFound();
            }
            return Ok(myTicket);
        }
        [HttpPut("{id}"), Authorize]
        public IActionResult UpdateTicket(int id, [FromBody] Ticket ticket)
        {
            var myTicket = _context.Tickets.Find(id);
            if (myTicket == null)
            {
                return NotFound();
            }
            int userId = GetUserId();
            if (myTicket.UserId != userId)
            {
                return Forbid();
            }
            myTicket.Title = ticket.Title.Trim();
            myTicket.Description = ticket.Description.Trim();
            myTicket.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return Ok();
        }
        [HttpDelete("{id}"), Authorize]
        public IActionResult DeleteTicket(int id)
        {
            var myTicket = _context.Tickets.Find(id);
            if (myTicket == null)
            {
                return NotFound();
            }
            int userId = GetUserId();
            if (myTicket.UserId != userId)
            {
                return Forbid();
            }
            _context.Tickets.Remove(myTicket);
            _context.SaveChanges();
            return Ok();
        }
        
    }
}
