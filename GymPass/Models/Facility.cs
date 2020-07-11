using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymPass.Models
{
    public class Facility
    {
        public int FacilityID { get; set; }
        public string FacilityName { get; set; }
        public int NumberOfClientsInGym { get; set; }
        public int NumberOfClientsUsingWeightRoom { get; set; }
        public int NumberOfClientsUsingCardioRoom { get; set; }
        public int NumberOfClientsUsingStretchRoom{ get; set; }
        public bool IsOpenDoorRequested { get; set; } = false;
        public bool DoorOpened { get; set; } = false;
        public TimeSpan DoorCloseTimer { get; set; } = TimeSpan.FromSeconds(5);

        //public bool IsDeepLensRequested { get; set; } = false;
        //public bool IsWithin10m { get; set; } = false;
        //public bool IsAlexaRequested { get; set; } = false;

    }
}
