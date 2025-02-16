using Microsoft.SemanticKernel.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace FileSearchRAG.DocumentIngestion
{
    public class PdfDocumentProcessor : IDocumentProcessor
    {
        public DocumentType DocumentType { get => DocumentType.PDF; }

        public List<string> GetChunks(Stream fileStream, int chunkSize, int chunkOverlap)
        {
            List<string> pages = GetPdfLines(fileStream);

            return TextChunker.SplitMarkdownParagraphs(pages, chunkSize, chunkOverlap);
        }

        private List<string> GetPdfLines(Stream fileStream)
        {
            string text = string.Empty;
            List<string> pages = new List<string>();

            using (PdfDocument pdf = PdfDocument.Open(fileStream))
            {
                foreach (Page page in pdf.GetPages())
                {
                    // Either extract based on order in the underlying document with newlines and spaces.
                    text = ContentOrderTextExtractor.GetText(page);

                    // Or based on grouping letters into words.
                    //string otherText = string.Join(" ", page.GetWords());

                    // Or the raw text of the page's content stream.
                    //string rawText = page.Text;

                    pages.Add(text);
                }
            }

            return pages;
        }
    }
}
