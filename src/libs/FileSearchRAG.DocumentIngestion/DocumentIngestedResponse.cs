namespace FileSearchRAG.DocumentIngestion
{
    public class DocumentIngestedResponse
    {
        public DocumentIngestedResponse(string fileName, List<string> pages)
        {
            FileName = fileName;
            Chunks = pages;
        }

        public string FileName { get; set; } = string.Empty;
        public List<string> Chunks { get; set; } = new List<string>();
    }
}
