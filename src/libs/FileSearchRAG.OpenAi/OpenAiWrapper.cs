using OpenAI;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI.Models;

namespace FileSearchRAG.OpenAi
{
    public class OpenAiWrapper : IOpenAiWrapper
    {
        private readonly OpenAIClient _openApiClient;

        public OpenAiWrapper(string openAiApiKey)
        {
            _openApiClient = new OpenAIClient(openAiApiKey);
        }

        public async Task<EmbeddingsResponse> GetEmbeddings(List<string> chunks) =>
            await _openApiClient.EmbeddingsEndpoint.CreateEmbeddingAsync(new EmbeddingsRequest(chunks, model: Model.Embedding_Ada_002));
        
        public async Task<string> QueryAsync(string query, string context)
        {
            string systemPrompt = "Use the given context to answer the question.\n" +
                "If you don't know the answer, say you don't know.\n" +
                "Use three sentence maximum and keep the answer concise.\n" +
                "Context:\n" +
                $"{context}";

            List<Message> messages = new List<Message>() {
                new Message(Role.System, systemPrompt),
                new Message(Role.User, query)
            };

            ChatRequest request = new ChatRequest(messages, model: Model.GPT4o, temperature: 0);

            ChatResponse response = await _openApiClient.ChatEndpoint.GetCompletionAsync(request);

            return response.FirstChoice.Message;
        }

        public List<Insert> CreateInserts(List<string> chunks, EmbeddingsResponse insertEmbeddings)
        {
            List<Insert> inserts = new List<Insert>();
            for (int i = 0; i < insertEmbeddings.Data.Count; i++)
            {
                inserts.Add(new Insert()
                {
                    Text = chunks[i],
                    Values = insertEmbeddings.Data[i].Embedding.Select(embedding => (float)embedding).ToArray()
                });
            }

            return inserts;
        }
    }
}
