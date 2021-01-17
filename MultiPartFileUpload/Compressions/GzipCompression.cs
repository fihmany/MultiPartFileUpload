using System.IO;
using System.IO.Compression;

namespace MultiPartFileUpload.Compressions
{
    public class GzipCompression : ICompression
    {
        public string Name { get; } = "GZip";
        public Stream Decompress(Stream compressedStream)
        {
            return new GZipStream(compressedStream, CompressionMode.Decompress);
        }

        public Stream Compress(Stream decompressedStream)
        {
            return new GZipStream(decompressedStream, CompressionMode.Compress);
        }
    }
}
