using BlazorBoilerplate.Server.Managers;
using BlazorBoilerplate.Server.Middleware.Wrappers;
using BlazorBoilerplate.Shared.AuthorizationDefinitions;
using BlazorBoilerplate.Shared.Dto.Logistique;
using BlazorBoilerplate.Storage;
using Logistique.Shared.AuthorizationDefinitions;
using Logistique.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BlazorBoilerplate.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlansController : ControllerBase
    {
        private readonly LogistiqueDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly PlansManager _plansManager;

        public PlansController(LogistiqueDbContext context, IAuthorizationService authorizationService, PlansManager plansManager)
        {
            _context = context;
            _authorizationService = authorizationService;
            _plansManager = plansManager;
        }

        // GET: api/Plans
        [HttpGet]
        public async Task<ApiResponse> GetPlans() => await _plansManager.GetPlans();

        // GET: api/Plans
        [HttpGet("WithOrders")]
        public async Task<ApiResponse> GetPlansWithOrders() => new ApiResponse(200, "GetPlans: Success", await _context.Plans.Include(p => p.Orders).ToListAsync());

        // GET: api/Plans/5
        [HttpGet("{id}")]
        public async Task<ApiResponse> GetPlan(int id) => await _plansManager.GetPlan(id);

        // POST: api/Plans
        [HttpPost]
        [Authorize(Roles = Roles.Manager)]
        public async Task<ApiResponse> CreatePlan(PlanDto plan) => await _plansManager.CreatePlan(plan);

        [HttpPut("SendToSolver/{id}")]
        public async Task<ApiResponse> SendToSolver(int id) => await _plansManager.SendToSolver(id);

        // DELETE: api/Plans/5
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Manager)]
        public async Task<ApiResponse> DeletePlan(int id) => await _plansManager.DeletePlan(id);
    }
}