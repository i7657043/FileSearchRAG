using FileSearchRAG.DocumentIngestion;
using FileSearchRAG.OpenAi;
using FileSearchRAG.Pinecone;
using FileSearchRAG.Web.API.Document.Models;
using OpenAI.Embeddings;
using Pinecone;
using static FileSearchRAG.Web.API.Document.Providers.DocumentProvider;

namespace FileSearchRAG.Web.API.Document.Providers
{
    public partial class DocumentProvider : IDocumentProvider
    {
        private readonly ILogger<DocumentProvider> _logger;
        private readonly IOpenAiWrapper _openAiClient;
        private readonly IPineconeClientWrapper _pineconeClient;
        private readonly PdfDocumentUploadWrapper _pdfDocumentUploadWrapper;

        public DocumentProvider(ILogger<DocumentProvider> logger, IOpenAiWrapper openAiClient, IPineconeClientWrapper pineconeClient, PdfDocumentUploadWrapper pdfDocumentUploadWrapper)
        {
            _logger = logger;
            _openAiClient = openAiClient;
            _pineconeClient = pineconeClient;
            _pdfDocumentUploadWrapper = pdfDocumentUploadWrapper;
        }

        public async Task IngestAsync(DocumentUpload fileInfo, string customerId)
        {
            List<string> chunks = _pdfDocumentUploadWrapper.GetChunks(fileInfo.FileBytes);

            EmbeddingsResponse insertEmbeddings = await _openAiClient.GetEmbeddings(chunks);

            List<Insert> inserts = _openAiClient.CreateInserts(chunks, insertEmbeddings);

            await _pineconeClient.InsertVectors(inserts, fileInfo.FileName, customerId);
        }
    }
}
