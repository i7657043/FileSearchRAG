namespace FileSearchRAG.Web.API.Document.Models
{
    public class DocumentUpload
    {
        public DocumentUpload(IFormFile fileUpload, int chunkSize, int chunkOverlap)
        {
            ChunkSize = chunkSize;
            ChunkOverlap = chunkOverlap;
            FileUpload = fileUpload;
        }

        public IFormFile FileUpload { get; set; }
        public int ChunkSize { get; set; }
        public int ChunkOverlap { get; set; }
    }
}
