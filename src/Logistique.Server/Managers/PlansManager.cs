using BlazorBoilerplate.Server.Middleware.Wrappers;
using BlazorBoilerplate.Shared;
using BlazorBoilerplate.Shared.AuthorizationDefinitions;
using BlazorBoilerplate.Shared.DataModels;
using BlazorBoilerplate.Shared.Dto.Logistique;
using BlazorBoilerplate.Storage;
using Logistique.Shared.AuthorizationDefinitions;
using Logistique.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace BlazorBoilerplate.Server.Managers
{
    public class PlansManager
    {
        private readonly ApplicationDbContext _applicationDb;
        private readonly LogistiqueDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PlansManager(ApplicationDbContext applicationDb, LogistiqueDbContext context, IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDb = applicationDb;
            _context = context;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse> GetPlans()
        {
            ClaimsPrincipal User = _httpContextAccessor.HttpContext.User;
            if (User.IsInRole(Roles.Manager))
            {
                return new ApiResponse(Status200OK, "GetPlans: Success", await _context.Plans.Select(p => new Plan { Id = p.Id, Date = p.Date, Name = p.Name }).ToListAsync());
            }
            else
            {
                return new ApiResponse(Status200OK, "GetPlans: Success", await _context.Plans.Where(p => p.Orders.Any(o => _applicationDb.Users.Find(o.DriverId).UserName == User.Identity.Name)).Select(p => new Plan { Id = p.Id, Date = p.Date, Name = p.Name }).ToListAsync());
            }
        }

        public async Task<ApiResponse> GetPlan(int id)
        {
            ClaimsPrincipal User = _httpContextAccessor.HttpContext.User;
            if (User.IsInRole(Roles.Manager))
            {
                Plan plan = _context.Plans
                    .Include(p => p.Orders).ThenInclude(o => o.Location).ThenInclude(l => l._LocationType)
                    .Single(p => p.Id == id);
                PlanDto planDto = JsonConvert.DeserializeObject<PlanDto>(JsonConvert.SerializeObject(plan));
                foreach (Order order in plan.Orders)
                {
                    planDto.Drivers.Add(new DriverDto(_applicationDb.Users.Find(order
                        .DriverId)));
                }
                return new ApiResponse(200, "GetPlan: Success", plan);
            }
            else
            {
                Plan plan = _context.Plans
                    .Where(p => p.Orders.Any(o => _applicationDb.Users.Find(o.DriverId).UserName == User.Identity.Name))
                    .Include(p => p.Orders).ThenInclude(o => o.Location).ThenInclude(l => l._LocationType)
                    .Single(p => p.Id == id);
                return new ApiResponse(200, "GetPlan: Success", plan);
            }
        }

        //TEMPO
        internal class DriverData
        {
            public string type { get; set; } = "A";
            public string start_location_id { get; set; }
            public string end_location_id { get; set; }
            public string start_location_coord { get; set; }
            public string end_location_coord { get; set; }
            public string start_time { get; set; } = "0";
            public string end_time { get; set; } = "100000";
            public string returntodepot { get; set; } = "1";
            /*
            "type": "A",
			"start_location_id": "49",
			"end_location_id": "49",
			"start_location_coord": "35.783866,51.446011",
			"end_location_coord": "35.783866,51.446011",
			"start_time": "0",
			"end_time": "100000",
			"returntodepot": "1"
             */
        }

        internal class request
        {
            public string location { get; set; }
            public string capacity { get; set; } = "1";
            public string timewindow_start { get; set; } = "0";
            public string timewindow_end { get; set; } = "100000";
            public string duration { get; set; } = "0";
        }

        public async Task<ApiResponse> CreatePlan(PlanDto plan)
        {
            try
            {
                Dictionary<string, DriverData> drivers = new Dictionary<string, DriverData>();
                Dictionary<string, request> requests = new Dictionary<string, request>();

                foreach (DriverDto v in plan.Drivers)
                {
                    drivers.Add(v.UserName, new DriverData
                    {
                        start_location_id = plan.Origin.Id.ToString(),
                        start_location_coord = plan.Origin.Latitude.ToString() + "," + plan.Origin.Longitude.ToString(),
                        end_location_id = plan.Origin.Id.ToString(),
                        end_location_coord = plan.Origin.Latitude.ToString() + "," + plan.Origin.Longitude.ToString()
                    });
                }
                foreach (LocationDto l in plan.Locations)
                {
                    requests.Add(l.Id.ToString(), new request
                    {
                        location = l.Latitude.ToString() + "," + l.Longitude.ToString()
                    });
                }
                Dictionary<string, object> problem = new Dictionary<string, object>
                {
                    { "vehicle", drivers },
                    { "request", requests }
                };
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                string json = JsonConvert.SerializeObject(problem, settings);
                Plan newPlan = new Plan
                {
                    Name = plan.Name,
                    Date = plan.DateTime,
                    PlanJson = json
                };
                _context.Plans.Add(newPlan);
                await _context.SaveChangesAsync();

                return new ApiResponse(Status200OK, "CreatePlan: Success", newPlan);
            }
            catch (Exception e)
            {
                return new ApiResponse(500, $"{((e.InnerException != null) ? e.InnerException.Message : e.Message)}");
            }
        }

        public async Task<ApiResponse> SendToSolver(int id)
        {
            Plan plan = _context.Plans.Find(id);
            if (string.IsNullOrEmpty(plan.RouteJson))
            {
                string result;
                try
                {
                    using (Py.GIL())
                    {
                        dynamic rpc = PythonEngine.ModuleFromString("rpc", File.ReadAllText("rpc.py"));
                        dynamic Route = rpc.solver(plan.PlanJson);
                        result = Convert.ToString(Route);
                        plan.RouteJson = result;
                    }
                }
                catch (Exception e)
                {
                    return new ApiResponse(Status503ServiceUnavailable, $"SendToSolver:{((e.InnerException != null) ? e.InnerException.Message : e.Message)}");
                }
                await PutPlan(plan.Id, plan);
            }

            JToken Response = JToken.Parse(plan.RouteJson);
            List<JToken> tokens = Response.ToList();
            foreach (JToken tok in tokens)
            {
                ApplicationUser driver = _applicationDb.Users.Single(u => u.UserName == ((JProperty)tok).Name);
                int orderValue = 0;
                foreach (JValue loc in tok.First)
                {
                    Order order = new Order
                    {
                        Value = orderValue,
                        PlanId = plan.Id,
                        DriverId = driver.Id,
                        LocationId = Convert.ToInt32(loc.Value)
                    };
                    _context.Orders.Add(order);
                    orderValue++;
                }
            }

            await _context.SaveChangesAsync();
            return new ApiResponse(Status200OK, "SendToSolver: Success");
        }

        public async Task<ApiResponse> PutPlan(int id, Plan plan)
        {
            if (id != plan.Id)
            {
                return new ApiResponse(400, "PutPlan: Bad Request");
            }

            _context.Entry(plan).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlanExists(id))
                {
                    return new ApiResponse(404, "PutPlan: Not Found");
                }
                else
                {
                    throw;
                }
            }
            return new ApiResponse(200, "PutPlan: Success", await _context.Plans.FindAsync(id));
        }

        public async Task<ApiResponse> DeletePlan(int id)
        {
            Plan plan = await _context.Plans.FindAsync(id);
            if (plan == null)
            {
                return new ApiResponse(404, "PutPlan: Not Found");
            }

            _context.Plans.Remove(plan);
            await _context.SaveChangesAsync();

            return new ApiResponse(200, "DeletePlan: Success", plan);
        }

        private bool PlanExists(int id) => _context.Plans.Any(e => e.Id == id);
    }
}