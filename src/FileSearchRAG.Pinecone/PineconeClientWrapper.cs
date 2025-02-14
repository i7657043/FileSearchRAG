using FileSearchRAG.OpenAi;
using OpenAI.Embeddings;
using Pinecone;

namespace FileSearchRAG.Pinecone
{
    public class PineconeClientWrapper
    {
        private readonly IndexClient _pineconeVectorStore;
        public PineconeClientWrapper(string apiKey)
        {
            PineconeClient pinecone = new PineconeClient(apiKey);

            _pineconeVectorStore = pinecone.Index("search-index-2");

            DescribeIndexStatsResponse indexStatsResponse = _pineconeVectorStore.DescribeIndexStatsAsync(new DescribeIndexStatsRequest()).Result;

            Console.WriteLine(indexStatsResponse.ToString());            
        }

        public async Task<string> QueryAsync(float[] queryVectors, string customerId)
        {           
            QueryResponse response = await _pineconeVectorStore.QueryAsync(new QueryRequest()
            {
                TopK = 3,
                Vector = queryVectors,
                IncludeMetadata = true,
                Filter = new Metadata
                {
                    ["customer_id"] = customerId
                }
            });

            string context = string.Join("\n", response.Matches?.Select(match => match.Metadata["text"])!);

            return context;
        }

        public async Task InsertVectors(List<Insert> inserts, string fileName, string customerId)
        {
            List<Vector> vectors = new List<Vector>();

            foreach (Insert insert in inserts)
            {
                vectors.Add(new Vector()
                {
                    Id = Guid.NewGuid().ToString(),
                    Metadata = new Metadata()
                    {
                        ["source"] = fileName,
                        ["customer_id"] = customerId,
                        ["text"] = insert.Text
                    },
                    Values = insert.Values
                });
            }

            await _pineconeVectorStore.UpsertAsync(new UpsertRequest() { Vectors = vectors });
        }
    }

    public class VectorWrapper
    {
        public string Id { get; set; } = string.Empty;
        public List<float> Values { get; set; } = new List<float>();
        public List<Metadata> Metadata { get; set; } = new List<Metadata>();
    }
}
