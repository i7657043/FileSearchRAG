﻿using Microsoft.SemanticKernel.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using static System.Net.Mime.MediaTypeNames;

namespace FileSearchRAG.DocumentIngestion
{
    public class PdfDocumentBasicProcessor
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
