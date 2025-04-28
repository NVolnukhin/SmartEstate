using System.ComponentModel.DataAnnotations;

namespace SmartEstate.Routing.Models
{
    public class RoutingRequest
    {
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public double OriginLatitude { get; set; }
    
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public double OriginLongitude { get; set; }
    
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public double DestinationLatitude { get; set; }
    
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public double DestinationLongitude { get; set; }
    
        public string Vehicle { get; set; } = "foot";
        public bool Elevation { get; set; } = false;
        public string Locale { get; set; } = "en";
    }
}