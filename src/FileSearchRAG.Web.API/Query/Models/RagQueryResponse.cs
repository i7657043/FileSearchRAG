﻿namespace FileSearchRAG.Web.API.Query.Models
{
    public class RagQueryResponse
    {
        public RagQueryResponse(string answer, string filesAnswer)
        {
            Answer = answer;
            FilesAnswer = filesAnswer;
        }

        public string Answer { get; set; } = string.Empty;
        public string FilesAnswer { get; set; } = string.Empty;
    }
}
