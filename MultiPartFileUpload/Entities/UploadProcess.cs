using System;
using System.IO;
using System.Collections.Generic;

namespace MultiPartFileUpload.Entities
{
    public class UploadProcess
    {
        public string Id;
        public int Size;
        public bool Finished;
        public string Compression;
        public FileStream UploadedFile;
        public Dictionary<int, Part> Parts;
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
            return Id == other.Id && Size == other.Size && Finished == other.Finished && Compression == other.Compression;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Size, Finished, Compression);
        }

        public override string ToString()
        {
            return $"{this.Id} {this.Compression} {this.Finished} {this.Size}";
        }
    }
}