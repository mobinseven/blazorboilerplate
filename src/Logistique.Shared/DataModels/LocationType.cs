using BlazorBoilerplate.Shared.DataInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BlazorBoilerplate.Shared.DataModels
{
    public class LocationType
    {
        [Key]
        public int Id { get; set; }

        [Column("Value")]
        public string Value { get; set; }

        [Column("IconName")]
        public string IconName { get; set; }
    }
}