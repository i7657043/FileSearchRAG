namespace FileSearchRAG.Web.API.Document.Models
{
    public class DocumentUpload
    {
        public DocumentUpload(string fileName, byte[] fileBytes, int chunkSize, int chunkOverlap, Stream fileStream)
        {
            FileName = fileName;
            FileBytes = fileBytes;
            ChunkSize = chunkSize;
            ChunkOverlap = chunkOverlap;
            FileStream = fileStream;
        }

        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
        public int ChunkSize { get; set; }
        public int ChunkOverlap { get; set; }
        public Stream FileStream { get; set; }
    }
}
