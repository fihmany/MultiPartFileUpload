using System.IO;
using System.IO.Compression;

namespace MultiPartFileUpload.Compressions
{
    public class DeflateCompression : ICompression
    {
        public string Name { get; } = "deflate";
        public Stream Decompress(Stream compressedStream)
        {
            return new DeflateStream(compressedStream, CompressionMode.Decompress);
        }

        public Stream Compress(Stream decompressedStream)
        {
            return new DeflateStream(decompressedStream, CompressionMode.Compress);
        }
    }
}
