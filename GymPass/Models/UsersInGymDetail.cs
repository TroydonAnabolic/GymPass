using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GymPass.Models
{
    // this table server to identify a list of users currently in the gym, and the approximate number
    public class UsersInGymDetail
    {
        public int UsersInGymDetailID { get; set; }
        public int FacilityID { get; set; }
        public Facility Facility { get; set; }
        public string FirstName { get; set; }
        public DateTime TimeAccessGranted { get; set; }
        public TimeSpan? EstimatedTrainingTime { get; set; } = TimeSpan.FromMinutes(45); // default training time is 45 mins, incase user skips
        public string UniqueEntryID { get; set; } // used to identify which entry to remove when the user leaves
    }
}
