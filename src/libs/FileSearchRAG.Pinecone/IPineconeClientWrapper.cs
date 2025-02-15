﻿using FileSearchRAG.OpenAi;

namespace FileSearchRAG.Pinecone
{
    public interface IPineconeClientWrapper
    {
        Task<VectorMatch> QueryAsync(float[] queryVectors, string customerId);
        Task InsertVectors(List<Insert> inserts, string fileName, string customerId);
    }
}
