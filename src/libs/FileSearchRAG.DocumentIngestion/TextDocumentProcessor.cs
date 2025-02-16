using Microsoft.SemanticKernel.Text;
using System.Text;

namespace FileSearchRAG.DocumentIngestion
{
    public class TextDocumentProcessor
    {
        public List<string> GetChunks(Stream textStream)
        {
            List<string> chunks = new List<string>();

            using (StreamReader reader = new StreamReader(textStream))
            {
                string documentText = reader.ReadToEnd();

                var paragraphs = TextChunker.SplitPlainTextParagraphs(new string [] { documentText }, 150, 15);

                foreach (var paragraph in paragraphs)
                    chunks.Add(paragraph);
            }

            return chunks;
        }
    }
}
