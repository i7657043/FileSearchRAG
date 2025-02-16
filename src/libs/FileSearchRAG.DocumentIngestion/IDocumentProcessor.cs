
namespace FileSearchRAG.DocumentIngestion
{
    public interface IDocumentProcessor
    {
        List<string> GetChunks(Stream fileStream, int chunkSize, int chunkOverlap);

        DocumentType DocumentType { get; }
    }

    public enum DocumentType
    {
        PDF,
        DOC,
        TXT
    }
}