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
        public async Task<IActionResult> IngestAsync(IFormFile file, [FromForm] string chunkSize, [FromForm] string chunkOverlap, [FromForm] string customerId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            int ingestChunkSize, ingestChunkOverap;

            if (!int.TryParse(chunkSize, out ingestChunkSize) || !int.TryParse(chunkOverlap, out ingestChunkOverap))
                return BadRequest("Chunk size and Chunk overlap must be whole numbers.");            

            await _documentProvider.IngestAsync(new DocumentUpload(file, ingestChunkSize, ingestChunkOverap), customerId);

            return Ok();
        }

        [HttpDelete]
        [Route("")]
        public async Task<IActionResult> ClearAllAsync()
        {
            await _documentProvider.ClearAllAsync();

            return Ok();
        }
    }
}
