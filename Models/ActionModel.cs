using System.Collections.Generic;
using Newtonsoft.Json;

namespace CianPlatform.Models
{
    public class ResultModel
    {
        [JsonProperty("data")]
        public List<Dictionary<string, dynamic>> Data { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
