using FileSearchRAG.DocumentIngestion;
using FileSearchRAG.OpenAi;
using FileSearchRAG.Pinecone;
using FileSearchRAG.Web.API.Document.Providers;
using FileSearchRAG.Web.API.Query.Providers;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<IOpenAiWrapper>(new OpenAiWrapper(builder.Configuration.GetValue<string>("apiKeys:openAi")!));
        builder.Services.AddSingleton<IPineconeClientWrapper>(new PineconeClientWrapper(builder.Configuration.GetValue<string>("apiKeys:pinecone")!));
        builder.Services.AddSingleton<PdfDocumentUploadWrapper>();
        builder.Services.AddSingleton<IDocumentProvider, DocumentProvider>();
        builder.Services.AddSingleton<IRagQueryProvider, RagQueryProvider>();

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}