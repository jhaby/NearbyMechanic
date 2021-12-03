using System;
using System.Collections.Generic;
using System.Text;

namespace NearbyMechanic.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Fullname { get; set; }
        public string PhotoUrl { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public bool IsMechanic { get; set; }
        public bool IsDriver { get; set; }
        public bool IsSupplier { get; set; }
        public string CarModel { get; set; }
        public string CarMake { get; set; }
        public string CarPlateNo { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<string> Skills { get; set; }
        public string Progress { get; set; }
    }
}
