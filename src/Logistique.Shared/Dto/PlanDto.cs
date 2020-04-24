using System;
using System.Collections.Generic;

namespace BlazorBoilerplate.Shared.Dto.Logistique
{
    public class PlanDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;

        public LocationDto Origin { get; set; } //List<LocationDto>

        public List<LocationDto> Locations { get; set; }

        public List<DriverDto> Drivers { get; set; }
    }
}