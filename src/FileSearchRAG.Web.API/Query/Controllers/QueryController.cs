using FileSearchRAG.Web.API.Query.Models;
using FileSearchRAG.Web.API.Query.Providers;
using Microsoft.AspNetCore.Mvc;

namespace FileSearchRAG.Web.API.Query.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QueryController : ControllerBase
    {
        private readonly ILogger<QueryController> _logger;
        private readonly IRagQueryProvider _queryProvider;

        public QueryController(ILogger<QueryController> logger, IRagQueryProvider queryProvider)
        {
            _logger = logger;
            _queryProvider = queryProvider;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> QueryAsync([FromBody] QueryRequest request)
        {
            string customerId = "001";

            QueryResponse response = await _queryProvider.QueryAsync(request.Query, customerId);

            return Ok(response);
        }
    }
}
