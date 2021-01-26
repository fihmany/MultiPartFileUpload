namespace MultiPartFileUpload.Contracts
{
    public class UploadRequestHeaders
    {
        public int ContentLength { get; set; }
        public int UncompressedLength { get; set; }
        public int UploadOffset { get; set; }
        public int? UploadLen { get; set; }
        public bool IsFinal { get; set; }
    }
}
