using Newtonsoft.Json;

namespace MAS.Hackathon.BackEnd.Models
{
    public class RequestPredictionModel
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
