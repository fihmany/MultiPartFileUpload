using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace MultiPartFileUpload.BL
{
    public class MultiPartFileUploadBl
    {
        private readonly ILogger<MultiPartFileUploadBl> _logger;
        public Dictionary<string, UploadProcess> UploadsInProgress;

        public MultiPartFileUploadBl(ILogger<MultiPartFileUploadBl> logger)
        {
            _logger = logger;
        }

        public string GenerateUploadProcess()
        {

        }
    }

    public class UploadProcess
    {
        public string Id;
        public int Size;
        public bool Finished;

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            
            return obj is UploadProcess other && Equals(other);
        }

        protected bool Equals(UploadProcess other)
        {
            return Id == other.Id && Size == other.Size && Finished == other.Finished;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Size, Finished);
        }
    }

}
