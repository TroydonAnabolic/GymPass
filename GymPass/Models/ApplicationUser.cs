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
        public int DefaultGym { get; set; }


        // Facility Details
        public bool AccessGrantedToFacility { get; set; }
        public bool IsCameraScanSuccessful { get; set; }
        public bool IsWithin10m { get; set; }
        public bool IsInsideGym { get; set; }
        public bool OpenDoorRequest { get; set; }
        public bool ExitGymRequest { get; set; }
        public DateTime TimeAccessDenied { get; set; }
        public DateTime TimeAccessGranted { get; set; }
        public TimeSpan IntendedTrainingDuration { get; set; }
        public bool WillUseWeightsRoom { get; set; } 
        public bool WillUseCardioRoom { get; set; }
        public bool WillUseStretchRoom { get; set; } 
        public bool HasLoggedWorkoutToday { get; set; }
        public DateTime TimeLoggedWorkout { get; internal set; }
    }
}
