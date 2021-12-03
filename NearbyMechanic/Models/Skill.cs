using System;
using System.Collections.Generic;
using System.Text;

namespace NearbyMechanic.Models
{
    public class Skill
    {
        public string Id { get; set; }
        public string CarModel { get; set; }
        public string CarMake { get; set; }
        public bool Status { get; set; }
    }
}
