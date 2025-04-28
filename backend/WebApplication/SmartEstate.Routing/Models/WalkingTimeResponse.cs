namespace SmartEstate.Routing.Models
{
    public class WalkingTimeResponse
    {
        public double DistanceMeters { get; set; }
        public double WalkingTimeSeconds { get; set; }
        public double WalkingTimeMinutes { get; set; }
        
        public static WalkingTimeResponse FromPath(RoutingResponse.Path path)
        {
            return new WalkingTimeResponse
            {
                DistanceMeters = path.Distance,
                WalkingTimeSeconds = path.Time / 1000,
                WalkingTimeMinutes = path.Time / 60000
            };
        }
    }
}