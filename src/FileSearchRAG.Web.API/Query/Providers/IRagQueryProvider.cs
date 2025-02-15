using FileSearchRAG.Web.API.Query.Models;

namespace FileSearchRAG.Web.API.Query.Providers
{
    public interface IRagQueryProvider
    {
        Task<QueryResponse> QueryAsync(string query, string customerId);
    }
}
