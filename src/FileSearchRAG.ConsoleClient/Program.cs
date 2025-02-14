using FileSearchRAG.DocumentIngestion;
using FileSearchRAG.OpenAi;
using FileSearchRAG.Pinecone;
using Microsoft.Extensions.Configuration;
using OpenAI.Embeddings;
using System.Reflection;

internal class Program
{
    private static async Task Main(string[] args)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets(Assembly.GetExecutingAssembly())
            .Build();

        string customerId = "001";
        string query = "Who is XXX Construction apprentice scheme part funded by?";

        Console.WriteLine("Getting pinecone client...");
        OpenAiWrapper openAiWrapper = new OpenAiWrapper(config.GetValue<string>("apiKeys:openAi")!);
        PineconeClientWrapper pineconeClient = new PineconeClientWrapper(config.GetValue<string>("apiKeys:pinecone")!);

        //Console.WriteLine("Chunking data...");
        //Documents documents = new Documents();
        //DocumentIngestedResponse response = documents.GetChunks("Moray Council.pdf");
        //Console.WriteLine(String.Join("\n", response.Chunks) + "\n");

        //Console.WriteLine("Inserting vectors into pinecone db...");
        //EmbeddingsResponse insertEmbeddings = await openAiWrapper.GetEmbeddings(response.Chunks);
        //List<Insert> inserts = openAiWrapper.CreateInserts(response.Chunks, insertEmbeddings);
        //await pineconeClient.InsertVectors(inserts, response.FileName, customerId);        

        Console.WriteLine($"Querying LLM: \"{query}\" ...");
        EmbeddingsResponse embeddingResponse = await openAiWrapper.GetEmbeddings(new List<string>() { query });
        string responseContent = await pineconeClient.QueryAsync(
            embeddingResponse.Data[0].Embedding.Select(e => (float)e).ToArray(),
            customerId);
        string answer = await openAiWrapper.QueryAsync(query, responseContent);

        Console.WriteLine($"LLM answer: \"{answer}\" ...");
    }
}