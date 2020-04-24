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
    public class LocationsController : ControllerBase
    {
        private readonly LogistiqueDbContext _context;

        public LocationsController(LogistiqueDbContext context)
        {
            _context = context;
        }

        // GET: api/Locations
        [HttpGet]
        public async Task<ApiResponse> GetLocations()
        {
            return new ApiResponse(200, "Retrieved Locations", await _context.Locations.Include(lo => lo._LocationType).ToListAsync());
        }

        // GET: api/Locations/5
        [HttpGet("{id}")]
        public async Task<ApiResponse> GetLocation(int id)
        {
            return new ApiResponse(200, "Retrieved Location", await _context.Locations.FindAsync(id));
        }

        // PUT: api/Locations/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        [Authorize(Roles = Roles.Manager)]
        public async Task<ApiResponse> PutLocation(int id, Location location)
        {
            if (id != location.Id)
            {
                return new ApiResponse(400, "PutLocation: Bad Request");
            }

            _context.Entry(location).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExists(id))
                {
                    return new ApiResponse(404, "PutLocation: Not found");
                }
                else
                {
                    throw;
                }
            }
            Location EditedLocation = await _context.Locations.Include(l => l._LocationType).SingleAsync(l => l.Id == location.Id);
            return new ApiResponse(200, "PutLocation: Successful", EditedLocation);
        }

        // POST: api/Locations
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [Authorize(Roles = Roles.Manager)]
        public async Task<ApiResponse> PostLocation(Location location)
        {
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            Location AddedLocation = await _context.Locations.Include(l => l._LocationType).SingleAsync(l => l.Id == location.Id);
            return new ApiResponse(200, "PostLocation: Successful", AddedLocation);
        }

        // DELETE: api/Locations/5
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Manager)]
        public async Task<ApiResponse> DeleteLocation(int id)
        {
            Location location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return new ApiResponse(404, "DeleteLocation: Not found");
            }

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();

            return new ApiResponse(201, "DeleteLocation: Successful", location);
        }

        private bool LocationExists(int id)
        {
            return _context.Locations.Any(e => e.Id == id);
        }
    }
}