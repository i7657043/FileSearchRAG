using OpenAI.Embeddings;

namespace FileSearchRAG.OpenAi
{
    public interface IOpenAiWrapper
    {
        Task<string> QueryAsync(string query, string systemPrompt, string context);
        Task<EmbeddingsResponse> GetEmbeddings(List<string> chunks);
        List<Insert> CreateInserts(List<string> chunks, EmbeddingsResponse insertEmbeddings);
    }
}
