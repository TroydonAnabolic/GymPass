using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymPass.Models
{
    public class UsersOutOfGymDetails
    {
        public int UserOutOfGymDetailsID { get; set; }
        public int FacilityID { get; set; }
        public DateTime EstimatedTimeToCheck { get; set; }
        public string UniqueEntryID { get; set; } // used to identify which entry to remove when the user leaves
    }
}
