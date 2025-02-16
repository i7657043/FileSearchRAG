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

        builder.Services.AddSingleton<IOpenAiWrapper>(
            new OpenAiWrapper(builder.Configuration.GetValue<string>("apiKeys:openAi")!));
        builder.Services.AddSingleton<IPineconeClientWrapper>(
            new PineconeClientWrapper(builder.Configuration.GetValue<string>("apiKeys:pinecone")!, 
            builder.Configuration.GetValue<string>("pinecone:index")!));
        builder.Services.AddSingleton<PdfDocumentUploadWrapper>();
        builder.Services.AddSingleton<WordDocumentUploadWrapper>();        
        builder.Services.AddSingleton<IDocumentProvider, DocumentProvider>();
        builder.Services.AddSingleton<IRagQueryProvider, RagQueryProvider>();

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder => builder.AllowAnyOrigin()
                                  .AllowAnyMethod()
                                  .AllowAnyHeader());
        });

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }        

        app.UseAuthorization();

        app.UseCors("AllowAllOrigins");

        app.MapControllers();

        app.Run();
    }
}