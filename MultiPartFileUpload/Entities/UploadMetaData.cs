using System.Collections.Generic;
using Newtonsoft.Json;

namespace MultiPartFileUpload.Entities
{
    [JsonObject]
    public class UploadMetaData
    {
        public string Aa { get; set; }
        public string Bb { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public string Compression { get; set; }
    }
}
