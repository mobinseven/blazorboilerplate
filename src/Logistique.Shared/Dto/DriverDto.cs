using BlazorBoilerplate.Shared.DataInterfaces;
using BlazorBoilerplate.Shared.DataInterfaces.Logistique;
using BlazorBoilerplate.Shared.DataModels;
using BlazorBoilerplate.Shared.Dto.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorBoilerplate.Shared.Dto.Logistique
{
    public class DriverDto : ApplicationUser
    {
        public DriverDto() : base()
        { }

        public DriverDto(ApplicationUser driver)
        {
            Id = driver.Id;
            // Orders = driver.Orders;
            PhoneNumber = driver.PhoneNumber;
            UserName = driver.UserName;
            FullName = driver.FullName;
            FirstName = driver.FirstName;
            LastName = driver.LastName;
        }

        public bool Selected { get; set; } = true;
    }
}