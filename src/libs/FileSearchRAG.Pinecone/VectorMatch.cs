namespace FileSearchRAG.Pinecone
{
    public class VectorMatch
    {
        public List<string> Sources { get; set; } = new List<string>();
        public string Context { get; set; } = string.Empty;
    }
}
