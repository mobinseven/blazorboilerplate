using BlazorBoilerplate.Server.Middleware.Wrappers;
using BlazorBoilerplate.Shared.DataModels;
using Logistique.Shared.AuthorizationDefinitions;
using Logistique.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorBoilerplate.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LocationTypesController : ControllerBase
    {
        private readonly LogistiqueDbContext _context;

        public LocationTypesController(LogistiqueDbContext context)
        {
            _context = context;
        }

        // GET: api/LocationTypes
        [HttpGet]
        public async Task<ApiResponse> GetLocationTypes()
        {
            return new ApiResponse(200, "GetLocationTypes: Success", await _context.LocationTypes.ToListAsync());
        }

        // GET: api/LocationTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LocationType>> GetLocationType(int id)
        {
            LocationType locationType = await _context.LocationTypes.FindAsync(id);

            if (locationType == null)
            {
                return NotFound();
            }

            return locationType;
        }

        // PUT: api/LocationTypes/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        [Authorize(Roles = Roles.Manager)]
        public async Task<IActionResult> PutLocationType(int id, LocationType locationType)
        {
            if (id != locationType.Id)
            {
                return BadRequest();
            }

            _context.Entry(locationType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationTypeExists(id))
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

        // POST: api/LocationTypes
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [Authorize(Roles = Roles.Manager)]
        public async Task<ActionResult<LocationType>> PostLocationType(LocationType locationType)
        {
            _context.LocationTypes.Add(locationType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLocationType", new { id = locationType.Id }, locationType);
        }

        // DELETE: api/LocationTypes/5
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Manager)]
        public async Task<ActionResult<LocationType>> DeleteLocationType(int id)
        {
            LocationType locationType = await _context.LocationTypes.FindAsync(id);
            if (locationType == null)
            {
                return NotFound();
            }

            _context.LocationTypes.Remove(locationType);
            await _context.SaveChangesAsync();

            return locationType;
        }

        private bool LocationTypeExists(int id)
        {
            return _context.LocationTypes.Any(e => e.Id == id);
        }
    }
}