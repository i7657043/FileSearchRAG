using Microsoft.SemanticKernel.Text;
using System.Text;

namespace FileSearchRAG.DocumentIngestion
{
    public class TextDocumentProcessor: IDocumentProcessor
    {
        public DocumentType DocumentType { get => DocumentType.TXT; }

        public List<string> GetChunks(Stream textStream, int chunkSize, int chunkOverlap)
        {
            List<string> chunks = new List<string>();

            using (StreamReader reader = new StreamReader(textStream))
            {
                string documentText = reader.ReadToEnd();

                var paragraphs = TextChunker.SplitPlainTextParagraphs(new string [] { documentText }, chunkSize, chunkOverlap);

                foreach (var paragraph in paragraphs)
                    chunks.Add(paragraph);
            }

            return chunks;
        }
    }
}
