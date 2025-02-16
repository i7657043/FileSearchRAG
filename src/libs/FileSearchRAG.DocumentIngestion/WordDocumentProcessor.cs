using Microsoft.SemanticKernel.Text;
using Spire.Doc;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace FileSearchRAG.DocumentIngestion
{
    public class WordDocumentProcessor
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
}