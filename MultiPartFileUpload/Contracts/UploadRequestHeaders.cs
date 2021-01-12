using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace MultiPartFileUpload.Contracts
{
    public class UploadRequestHeaders
    {
        public long ContentLength { get; set; }
        public int UploadOffset { get; set; }
        public bool IsFinal { get; set; }
    }
}
