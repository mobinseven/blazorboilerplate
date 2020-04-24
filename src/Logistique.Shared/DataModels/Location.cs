using BlazorBoilerplate.Shared.DataInterfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorBoilerplate.Shared.DataModels
{
    public class Location
    {
        public Location()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int LocationTypeId { get; set; }

        public virtual LocationType _LocationType { get; set; }

        [NotMapped]
        public virtual string LocationType
        {
            get
            {
                if (_LocationType != null)
                    return _LocationType.Value;
                else
                    return string.Empty;
            }
        }

        public ICollection<Order> Orders { get; set; }
    }
}