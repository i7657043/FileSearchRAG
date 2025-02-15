using FileSearchRAG.Web.API.Document.Providers;
using FileSearchRAG.Web.API.Document.Models;
using Microsoft.AspNetCore.Mvc;

namespace FileSearchRAG.Web.API.Document.Document.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly ILogger<DocumentController> _logger;
        private readonly IDocumentProvider _documentProvider;

        public DocumentController(ILogger<DocumentController> logger, IDocumentProvider documentProvider)
        {
            _logger = logger;
            _documentProvider = documentProvider;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> IngestAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");
            if (file.ContentType != "application/pdf")
                return BadRequest("Only PDF files are supported.");

            byte[] fileBytes;

            using var stream = new MemoryStream();
            file.CopyTo(stream);
            fileBytes = stream.ToArray();

            await _documentProvider.IngestAsync(new DocumentUpload(file.FileName, fileBytes), "001");

            return Ok();
        }
    }
}
