namespace FileSearchRAG.Web.API.Query.Models
{
    public class RagQueryRequest
    {
        public string Query { get; set; } = string.Empty;
        public string SystemPrompt { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
    }
}
