using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planner.Data;
using Planner.Dtos.Plan;
using Planner.Extensions;

namespace Planner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanApiController : ControllerBase
    {
        private readonly PlannerDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PlanApiController(PlannerDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlanDto>>> GetPlans()
        {
            var user = await _userManager.GetUserAsync(this.User);

            if (_context.Plans == null)
            {
                return NotFound();
            }

            var plans = await _context.Plans.Where(p => p.UserId == user.Id).ToListAsync();

            return Ok(plans.ToPlanDtos());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlanDto>> GetPlan(int id)
        {
            if (_context.Plans == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(this.User);

            var plan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == id);

            if (plan == null)
                return NotFound();

            return Ok(plan.ToPlanDto());
        }

        [HttpPost]
        public async Task<ActionResult<CreatePlanDto>> PostPlan(CreatePlanDto createPlanDto)
        {
            if (_context.Plans == null)
            {
                return Problem("Entity set 'Plans' is null.");
            }

            var plan = new Plan()
            {
                Name = createPlanDto.title,
                Description = createPlanDto.description,
                Color = createPlanDto.color,
                StartDate = createPlanDto.start,
                EndDate = createPlanDto.end
            };

            var user = await _userManager.GetUserAsync(this.User);
            plan.UserId = user.Id;

            _context.Plans.Add(plan);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlan", new { id = plan.Id },
             new PlanDto()
             {
                 id = plan.Id,
                 title = plan.Name,
                 description = plan.Description,
                 start = plan.StartDate,
                 end = plan.EndDate,
                 color = plan.Color
             });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlan(int id, PlanDto updatePlanDto)
        {
            if (id != updatePlanDto.id)
            {
                return BadRequest();
            }

            var plan = await _context.Plans.FindAsync(id);
            var user = await _userManager.GetUserAsync(this.User);

            if (plan == null || (user != null && !user.Id.Equals(plan.UserId)))
            {
                return NotFound();
            }

            plan.Name = updatePlanDto.title;
            plan.Description = updatePlanDto.description;
            plan.Color = updatePlanDto.color;
            plan.StartDate = updatePlanDto.start;
            plan.EndDate = updatePlanDto.end;
            _context.Update(plan);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlanExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePLan(int id)
        {
            if (_context.Plans == null)
            {
                return NotFound();
            }

            var skill = await _context.Plans.FindAsync(id);

            if (skill == null)
            {
                return NotFound();
            }

            _context.Plans.Remove(skill);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlanExists(int id)
        {
            return (_context.Plans?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}