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
        private readonly WordDocumentUploadWrapper _wordDocumentUploadWrapper;

        public DocumentProvider(
            ILogger<DocumentProvider> logger, 
            IOpenAiWrapper openAiClient, 
            IPineconeClientWrapper pineconeClient, 
            PdfDocumentUploadWrapper pdfDocumentUploadWrapper, 
            WordDocumentUploadWrapper wordDocumentUploadWrapper)
        {
            _logger = logger;
            _openAiClient = openAiClient;
            _pineconeClient = pineconeClient;
            _pdfDocumentUploadWrapper = pdfDocumentUploadWrapper;
            _wordDocumentUploadWrapper = wordDocumentUploadWrapper;
        }

        public Task ClearAllAsync() =>
            _pineconeClient.ClearAllAsync();

        public async Task IngestAsync(DocumentUpload fileInfo, string customerId)
        {
            List<string> chunks = new List<string>();

            string fileExtension = Path.GetExtension(fileInfo.FileName);
            if (fileExtension == ".pdf")
                chunks = _pdfDocumentUploadWrapper.GetChunks(fileInfo.FileBytes);
            else if (fileExtension == ".doc" || fileExtension == ".docx")
                chunks = _wordDocumentUploadWrapper.GetChunks(fileInfo.FileStream);

            EmbeddingsResponse insertEmbeddings = await _openAiClient.GetEmbeddings(chunks);

            List<Insert> inserts = _openAiClient.CreateInserts(chunks, insertEmbeddings);

            await _pineconeClient.InsertVectors(inserts, fileInfo.FileName, customerId);
        }
    }
}
