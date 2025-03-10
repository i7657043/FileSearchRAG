﻿using FileSearchRAG.OpenAi;
using Pinecone;
using System.Text;

namespace FileSearchRAG.Pinecone
{
    public class PineconeClientWrapper : IPineconeClientWrapper
    {
        private const string _pinconeNamespace = "cust";
        private readonly IndexClient _pineconeVectorStore;

        public PineconeClientWrapper(string apiKey, string indexName)
        {
            PineconeClient pinecone = new PineconeClient(apiKey);

            _pineconeVectorStore = pinecone.Index(indexName);

            DescribeIndexStatsResponse indexStatsResponse = _pineconeVectorStore.DescribeIndexStatsAsync(new DescribeIndexStatsRequest()).Result;

            Console.WriteLine(indexStatsResponse.ToString());
        }

        public async Task ClearAllAsync()
        {
            await _pineconeVectorStore.DeleteAsync(new DeleteRequest()
            {
                Namespace = _pinconeNamespace,
                DeleteAll = true              
            });            
        }

        public async Task<VectorMatch> QueryAsync(float[] queryVectors, string customerId)
        {
            QueryResponse response = await _pineconeVectorStore.QueryAsync(new QueryRequest()
            {
                TopK = 3,
                Vector = queryVectors,
                IncludeMetadata = true,
                Filter = new Metadata
                {
                    ["customer_id"] = customerId
                },
                Namespace = _pinconeNamespace
            });

            VectorMatch vectorMatch = new VectorMatch();
            StringBuilder sb = new StringBuilder();
            foreach (var match in response.Matches?.Where(match => match.Score > 0.75)!)
            {
                if (match?.Metadata == null)
                    continue;

                string source = match.Metadata["source"].Value.ToString();
                if (!vectorMatch.Sources.Any(matchSource => matchSource == source))
                    vectorMatch.Sources.Add(source);

                sb.Append($"{match.Metadata["text"].ToString()}\n");
            }
            vectorMatch.Context = sb.ToString();

            return vectorMatch;
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

            UpsertResponse response = await _pineconeVectorStore.UpsertAsync(new UpsertRequest() 
            { 
                Vectors = vectors, 
                Namespace = _pinconeNamespace 
            });
        }
    }
}
