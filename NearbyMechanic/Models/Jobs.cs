using System;
using System.Collections.Generic;
using System.Text;

namespace NearbyMechanic.Models
{
    public class Jobs
    {
        public string ID { get; set; }
        public DateTime DateOfRequest { get; set; }
        public string ClientPhone { get; set; }
        public bool Status { get; set; }
        public string MechanicPhone { get; set; }
        public string Progress { get; set; }
    }
}
