namespace FileSearchRAG.Web.API.Document.Models
{
    public class DocumentUpload
    {
        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
        public int ChunkSize { get; set; }
        public int ChunkOverlap { get; set; }

        public DocumentUpload(string fileName, byte[] fileBytes, int chunkSize, int chunkOverlap)
        {
            FileName = fileName;
            FileBytes = fileBytes;
            ChunkSize = chunkSize;
            ChunkOverlap = chunkOverlap;
        }
    }
}
