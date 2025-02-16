using FileSearchRAG.DocumentIngestion;
using FileSearchRAG.OpenAi;
using FileSearchRAG.Pinecone;
using FileSearchRAG.Web.API.Document.Models;
using FileSearchRAG.Web.API.Extensions;
using OpenAI.Embeddings;

namespace FileSearchRAG.Web.API.Document.Providers
{
    public class DocumentProvider : IDocumentProvider
    {
        private readonly ILogger<DocumentProvider> _logger;
        private readonly IOpenAiWrapper _openAiClient;
        private readonly IPineconeClientWrapper _pineconeClient;
        private readonly List<IDocumentProcessor> _docProcessors;

        public DocumentProvider(
            ILogger<DocumentProvider> logger,
            IOpenAiWrapper openAiClient,
            IPineconeClientWrapper pineconeClient,
            List<IDocumentProcessor> docProcessors)
        {
            _logger = logger;
            _openAiClient = openAiClient;
            _pineconeClient = pineconeClient;
            _docProcessors = docProcessors;
        }

        public Task ClearAllAsync() =>
            _pineconeClient.ClearAllAsync();

        public async Task IngestAsync(DocumentUpload fileInfo, string customerId)
        {
            List<string> chunks = new List<string>();

            string fileExtension = Path.GetExtension(fileInfo.FileUpload.FileName);
            MemoryStream stream = fileInfo.FileUpload.GetFileStream();

            chunks = _docProcessors.First(processor => processor.DocumentType == GetDocumentType(fileExtension))
                .GetChunks(stream, fileInfo.ChunkSize, fileInfo.ChunkOverlap);

            EmbeddingsResponse insertEmbeddings = await _openAiClient.GetEmbeddings(chunks);

            List<Insert> inserts = _openAiClient.CreateInserts(chunks, insertEmbeddings);

            await _pineconeClient.InsertVectors(inserts, fileInfo.FileUpload.FileName, customerId);
        }

        private DocumentType GetDocumentType(string fileExtension) =>
            fileExtension.ToLower() switch
            {
                ".pdf" => DocumentType.PDF,
                ".doc" or ".docx" => DocumentType.DOC,
                ".txt" => DocumentType.TXT,
                _ => throw new NotSupportedException($"Unsupported file type: {fileExtension}")
            };
    }
}
