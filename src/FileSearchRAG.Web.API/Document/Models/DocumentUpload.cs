namespace FileSearchRAG.Web.API.Document.Models
{
    public class DocumentUpload
    {
        public DocumentUpload(string fileName, byte[] fileBytes)
        {
            FileName = fileName;
            FileBytes = fileBytes;
        }

        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
    }
}
