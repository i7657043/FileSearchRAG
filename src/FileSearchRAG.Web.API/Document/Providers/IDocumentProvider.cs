using FileSearchRAG.Web.API.Document.Models;

namespace FileSearchRAG.Web.API.Document.Providers
{
    public interface IDocumentProvider
    {
        Task IngestAsync(DocumentUpload fileInfo, string customerId);
    }
}
