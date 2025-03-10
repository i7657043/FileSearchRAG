﻿using FileSearchRAG.OpenAi;
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

        public async Task<RagQueryResponse> QueryAsync(RagQueryRequest request)
        {
            EmbeddingsResponse embeddingResponse = await _openAiClient.GetEmbeddings(new List<string>() { request.Query });

            VectorMatch responseContent = await _pineconeClient.QueryAsync(
                embeddingResponse.Data[0].Embedding.Select(e => (float)e).ToArray(),
                request.CustomerId);

            string answer = await _openAiClient.QueryAsync(request.Query, request.SystemPrompt, responseContent.Context);

            string filesAnswer = responseContent.Sources.Count > 0
                ? $"The following documents contributed to this answer: {string.Join(",", responseContent.Sources)}"
                : "Your private documents were not used in generating this answer";

            return new RagQueryResponse(answer, filesAnswer);
        }
    }
}
