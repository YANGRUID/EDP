using Microsoft.AspNetCore.Mvc;
using EDP_Uplay.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace EDP_Uplay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActivityController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _configuration;
        public ActivityController(MyDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpPost]
        public IActionResult AddActivity(Activity activity)
        {
            var now = DateTime.Now;
            var myActivity = new Activity()
            {
                Title = activity.Title.Trim(),
                Description = activity.Description.Trim(),
                Price = activity.Price,
                Type = activity.Type,
                AvailableDates = activity.AvailableDates,
                Capacity = activity.Capacity,
                CreatedAt = now,
                UpdatedAt = now,
                ImageFile = activity.ImageFile
            };
            _context.Activities.Add(myActivity);
            _context.SaveChanges();
            return Ok(myActivity);
        }

        [HttpGet]
        public IActionResult GetAll(string? search)
        {
            IQueryable<Activity> query = _context.Activities;

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => a.Title.Contains(search)
                                      || a.Description.Contains(search)
                                      || a.Type.Contains(search));
            }

            var activities = query.OrderByDescending(a => a.CreatedAt).ToList();
            return Ok(activities);
        }
        [HttpGet("{id}")]
        public IActionResult GetActivity(int id)
        {
            var activity = _context.Activities.Find(id);
            if (activity == null)
            {
                return NotFound();
            }
            return Ok(activity);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateActivity(int id, [FromBody] Activity activityUpdate)
        {
            var activity = _context.Activities.Find(id);
            if (activity == null)
            {
                return NotFound();
            }

            // Update the properties
            activity.Title = activityUpdate.Title.Trim();
            activity.Description = activityUpdate.Description.Trim();
            activity.Price = activityUpdate.Price;
            activity.Type = activityUpdate.Type;
            activity.AvailableDates = activityUpdate.AvailableDates;
            activity.Capacity = activityUpdate.Capacity;
            activity.UpdatedAt = DateTime.Now;
            activity.ImageFile = activityUpdate.ImageFile;

            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteActivity(int id)
        {
            var activity = _context.Activities.Find(id);
            if (activity == null)
            {
                return NotFound();
            }

            _context.Activities.Remove(activity);
            _context.SaveChanges();
            return Ok();
        }

    }
}