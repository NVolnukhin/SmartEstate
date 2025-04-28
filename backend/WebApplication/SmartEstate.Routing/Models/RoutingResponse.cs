using System.Collections.Generic;
using Newtonsoft.Json;

namespace SmartEstate.Routing.Models
{
    public class RoutingResponse
    {
        [JsonProperty("paths")]
        public List<Path> Paths { get; set; }

        public class Path
        {
            [JsonProperty("distance")]
            public double Distance { get; set; } // в метрах
            
            [JsonProperty("time")]
            public long Time { get; set; } // в миллисекундах
        }
    }
}