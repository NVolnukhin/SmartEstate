namespace SmartEstate.Routing.Models
{
    public class GraphHopperOptions
    {
        public string ApiKey { get; set; }
        public string BaseUrl { get; set; } = "https://graphhopper.com/api/1/";
        public string RouteEndpoint { get; set; } = "route";
    }
}