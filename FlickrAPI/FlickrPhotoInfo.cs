using Newtonsoft.Json;

namespace FlickrAPI
{
    public partial class FlickrPhotoInfo
    {
        [JsonProperty("photo")]
        public Photo Photo { get; set; }

        [JsonProperty("stat")]
        public string Stat { get; set; }
    }
}
