using BlazorBoilerplate.Shared.DataInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlazorBoilerplate.Shared.DataModels
{
    public class SmsInvitation
    {
        [Key]
        public int Id { get; set; }

        public DateTime SentAt { get; set; } = DateTime.Now;

        public string PhoneNumber { get; set; }

        public string VerificationCode { get; set; }
    }
}