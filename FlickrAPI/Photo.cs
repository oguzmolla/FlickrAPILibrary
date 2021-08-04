using Newtonsoft.Json;

namespace FlickrAPI
{
    public partial class Photo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("secret")]
        public string Secret { get; set; }

        [JsonProperty("server")]
        public long Server { get; set; }

        [JsonProperty("originalsecret")]
        public string Originalsecret { get; set; }

        [JsonProperty("originalformat")]
        public string Originalformat { get; set; }
    }
}
