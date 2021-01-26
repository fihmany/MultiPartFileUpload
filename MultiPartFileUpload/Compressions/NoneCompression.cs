using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MultiPartFileUpload.Compressions
{
    public class NoneCompression : ICompression
    {
        public string Name { get; } = "None";
        public Stream Decompress(Stream compressedStream)
        {
            return compressedStream;
        }

        public Stream Compress(Stream decompressedStream)
        {
            return decompressedStream;
        }
    }
}
