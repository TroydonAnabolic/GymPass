using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymPass.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UsernameChangeLimit { get; set; } = 10;
        public byte[] ProfilePicture { get; set; }

        // Facility Details
        public bool IsInsideGym { get; set; }
        public bool IsUsingWeightsRoom { get; set; }
        public bool IsUsingCardioRoom{ get; set; }
        public bool IsUsingStretchRoom { get; set; }
        public bool OpenDoorRequest { get; set; }
        public bool ExitGymRequest { get; set; }
    }
}
