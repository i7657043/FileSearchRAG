using FileSearchRAG.DocumentIngestion;

namespace FileSearchRAG.Web.API.Extensions
{
    public static class FileExtensions
    {
        public static MemoryStream GetFileStream(this IFormFile file)
        {
            MemoryStream stream = new MemoryStream();
            file.CopyTo(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}
