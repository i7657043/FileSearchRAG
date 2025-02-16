namespace FileSearchRAG.Web.API.Document.Models
{
    public class DocumentUpload
    {
        public DocumentUpload(string fileName, int chunkSize, int chunkOverlap, Stream fileStream)
        {
            FileName = fileName;
            ChunkSize = chunkSize;
            ChunkOverlap = chunkOverlap;
            FileStream = fileStream;
        }

        public string FileName { get; set; }
        public int ChunkSize { get; set; }
        public int ChunkOverlap { get; set; }
        public Stream FileStream { get; set; }
    }
}
