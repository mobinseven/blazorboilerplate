using BlazorBoilerplate.Shared.DataInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorBoilerplate.Shared.DataModels
{
    public class Plan
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string PlanJson { get; set; }

        public string RouteJson { get; set; }

        [NotMapped]
        private DateTime _Date;

        [Column("Date")]
        public DateTime Date
        {
            get
            {
                return _Date;
            }
            set
            {
                _Date = value.Date;
            }
        }

        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }
}