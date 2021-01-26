using System.IO;

namespace MultiPartFileUpload.Compressions
{
    public interface ICompression
    {
        public string Name { get; }
        public Stream Decompress(Stream compressedStream);
        public Stream Compress(Stream decompressedStream);
    }
}
