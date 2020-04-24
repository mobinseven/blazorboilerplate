using Logistique.Shared.DataModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorBoilerplate.Shared.DataModels
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int Value { get; set; }

        public int PlanId { get; set; }

        public bool Done { get; set; }

        public DateTime DoneAt { get; set; }

        public virtual Plan Plan { get; set; }

        public int LocationId { get; set; }

        public virtual Location Location { get; set; }

        [Column("DriverId")]
        public Guid DriverId { get; set; }
    }
}