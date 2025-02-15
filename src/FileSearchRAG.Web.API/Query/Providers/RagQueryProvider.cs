using FileSearchRAG.OpenAi;
using FileSearchRAG.Pinecone;
using FileSearchRAG.Web.API.Query.Models;
using OpenAI.Embeddings;

namespace FileSearchRAG.Web.API.Query.Providers
{
    public class RagQueryProvider : IRagQueryProvider
    {
        private readonly ILogger<RagQueryProvider> _logger;
        private readonly IOpenAiWrapper _openAiClient;
        private readonly IPineconeClientWrapper _pineconeClient;

        public RagQueryProvider(ILogger<RagQueryProvider> logger, IOpenAiWrapper openAiClient, IPineconeClientWrapper pineconeClient)
        {
            _logger = logger;
            _openAiClient = openAiClient;
            _pineconeClient = pineconeClient;
        }

        public async Task<QueryResponse> QueryAsync(string query, string customerId)
        {
            EmbeddingsResponse embeddingResponse = await _openAiClient.GetEmbeddings(new List<string>() { query });

            VectorMatch responseContent = await _pineconeClient.QueryAsync(
                embeddingResponse.Data[0].Embedding.Select(e => (float)e).ToArray(),
                customerId);

            string answer = await _openAiClient.QueryAsync(query, responseContent.Context);

            string filesAnswer = responseContent.Sources.Count > 0
                ? $"The answer came from the following file/s:\n{string.Join("\n", responseContent.Sources)}"
                : "The answer came from knowledge outside of your private documents";

            return new QueryResponse(answer, filesAnswer);
        }
    }
}
