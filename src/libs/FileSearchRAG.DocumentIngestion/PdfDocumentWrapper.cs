using Microsoft.SemanticKernel.Text;
using Spire.Doc;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using static System.Net.Mime.MediaTypeNames;

namespace FileSearchRAG.DocumentIngestion
{
    public class WordDocumentUploadWrapper
    {
        public List<string> GetChunks(Stream wordStream)
        {
            List<string> pages = new List<string>();

            Document document = new Document(wordStream);

            foreach (Section section in document.Sections)
            {
                Document singlePageDoc = new Document();
                singlePageDoc.Sections.Add(section.Clone());

                string text = string.Empty;

                using (MemoryStream pdfStream = new MemoryStream())
                {
                    singlePageDoc.SaveToStream(pdfStream, FileFormat.PDF);
                    pdfStream.Position = 0;

                    using (PdfDocument pdf = PdfDocument.Open(pdfStream))
                    {
                        foreach (Page page in pdf.GetPages())
                        {
                            text = ContentOrderTextExtractor.GetText(page);

                            pages.Add(text);
                        }
                    }                    
                }
            }

            return TextChunker.SplitMarkdownParagraphs(pages, maxTokensPerParagraph: 150, overlapTokens: 15);
        }
    }

    public class PdfDocumentUploadWrapper
    {
        public List<string> GetChunks(byte[] fileBytes)
        {
            List<string> pages = GetPdfLines(fileBytes);

            return TextChunker.SplitMarkdownParagraphs(pages, maxTokensPerParagraph: 150, overlapTokens: 15);
        }

        private List<string> GetPdfLines(byte[] fileBytes)
        {
            string text = string.Empty;
            List<string> pages = new List<string>();

            using (PdfDocument pdf = PdfDocument.Open(fileBytes))
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

    public class PdfDocumentWrapper
    {
        public DocumentIngestedResponse GetChunks(string path)
        {
            List<string> pages = GetPdfLines(path);            

            List<string> chunks = TextChunker.SplitMarkdownParagraphs(pages, maxTokensPerParagraph: 150, overlapTokens: 15);

            string fileName = Path.GetFileName(path);

            return new DocumentIngestedResponse(fileName, chunks);
        }

        private List<string> GetPdfLines(string path)
        {
            string text = string.Empty;
            List<string> pages = new List<string>();

            using (PdfDocument pdf = PdfDocument.Open(path))
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
