using FileSearchRAG.DocumentIngestion;
using FileSearchRAG.OpenAi;
using FileSearchRAG.Pinecone;
using Microsoft.Extensions.Configuration;
using OpenAI.Embeddings;
using Spectre.Console;
using System.Reflection;

internal class Program
{
    private static readonly string queryCommand = "q";
    private static readonly string loadCommand = "l";

    private static async Task Main(string[] args)
    {
        Console.WriteLine("Welcome");

        IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets(Assembly.GetExecutingAssembly())
            .Build();

        string customerId = "001";
        Console.WriteLine("Getting pinecone client...");
        OpenAiWrapper openAiWrapper = new OpenAiWrapper(config.GetValue<string>("apiKeys:openAi")!);
        PineconeClientWrapper pineconeClient = new PineconeClientWrapper(config.GetValue<string>("apiKeys:pinecone")!, "search-index-2");

        string command = GetCommandFromUserInput();
        if (command == queryCommand)
        {
            while (true)
            {
                string query = GetQueryFromUserInput();
                if (query == "exit")
                    break;

                AnsiConsole.MarkupLine($"\n[blue]Querying LLM: {query}[/]");
                EmbeddingsResponse embeddingResponse = await openAiWrapper.GetEmbeddings(new List<string>() { query });
                VectorMatch responseContent = await pineconeClient.QueryAsync(
                    embeddingResponse.Data[0].Embedding.Select(e => (float)e).ToArray(),
                    customerId);
                string systemPrompt = "Use the given context to answer the question.\n" +
                    "If you don't know the answer, say you don't know.\n" +
                    "Use three sentence maximum and keep the answer concise.\n";
                string answer = await openAiWrapper.QueryAsync(query, systemPrompt, responseContent.Context);
                string fileAnswer = responseContent.Sources.Count > 0
                    ? $"The answer came from the following file/s:\n{String.Join("\n", responseContent.Sources)}"
                    : "The answer came from knowledge outside of your documents";
                AnsiConsole.MarkupLine($"\n[green]LLM answer: {answer}\n[/]");

                Console.WriteLine(fileAnswer);
            }
        }
        else
        {
            Console.WriteLine("Chunking data...");
            PdfDocumentWrapper documents = new PdfDocumentWrapper();
            DocumentIngestedResponse response = documents.GetChunks("Moray Council.pdf");
            Console.WriteLine(String.Join("\n", response.Chunks) + "\n");

            Console.WriteLine("Inserting vectors into pinecone db...");
            EmbeddingsResponse insertEmbeddings = await openAiWrapper.GetEmbeddings(response.Chunks);
            List<Insert> inserts = openAiWrapper.CreateInserts(response.Chunks, insertEmbeddings);
            await pineconeClient.InsertVectors(inserts, response.FileName, customerId);
        }

        Console.WriteLine($"\nDone. Press any key to quit ...\n");

        Console.ReadKey();
    }

    private static string GetCommandFromUserInput()
    {
        string command = string.Empty;

        while (true)
        {
            Console.WriteLine("\nEnter key for command: query (q) or load (l)");
            command = Console.ReadLine()!;
            if (command == loadCommand || command == queryCommand)
            {
                break;
            }
        }

        return command;
    }

    private static string GetQueryFromUserInput()
    {
        string query = string.Empty;

        while (true)
        {
            Console.WriteLine("\nEnter query: ");
            query = Console.ReadLine()!;
            if (!string.IsNullOrEmpty(query))
            {
                break;
            }
        }

        return query;
    }
}