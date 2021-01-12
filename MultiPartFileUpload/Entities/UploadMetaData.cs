using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MultiPartFileUpload.Entities
{
    public class UploadMetaData
    {
        public string Aa;
        public string Bb;
        public Dictionary<string, string> Data;
        [JsonIgnore]
        public string Compression;
    }
}
