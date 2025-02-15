import React, { useState, useEffect, useRef } from "react";

interface ConversationEntry {
  type: "user" | "bot";
  answer: string;
  filesAnswer: string;
  timestamp: string;
}

function QueryConversation() {
  const url = process.env.REACT_APP_API_BASE_URL ?? "";
  const [query, setQuery] = useState<string>("");
  const [systemPrompt, setSystemPrompt] =
    useState<string>(`Use the given context to answer the question
If you don't know the answer, say you don't know.
Use three sentence maximum and keep the answer concise.`);
  const [conversation, setConversation] = useState<ConversationEntry[]>([]);
  const [loadingQuery, setLoadingQuery] = useState<boolean>(false);
  const [customerIdQuery, setCustomerIdQuery] = useState<string>("");
  const conversationEndRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (conversationEndRef.current) {
      conversationEndRef.current.scrollIntoView({ behavior: "smooth" });
    }
  }, [conversation]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoadingQuery(true);
    try {
      const apiResponse = await fetch(`${url}/query`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          query,
          systemPrompt,
          customerId: customerIdQuery,
        }),
      });

      const data = await apiResponse.json();

      setConversation((prevConversation) => [
        ...prevConversation,
        {
          type: "user",
          answer: query,
          filesAnswer: data.filesAnswer,
          timestamp: new Date().toLocaleTimeString(),
        },
        {
          type: "bot",
          answer: data.answer,
          filesAnswer: "",
          timestamp: new Date().toLocaleTimeString(),
        },
      ]);
    } catch (error) {
      console.error("Error fetching data:", error);
      setConversation((prevConversation) => [
        ...prevConversation,
        {
          type: "user",
          answer: query,
          filesAnswer: "",
          timestamp: new Date().toLocaleTimeString(),
        },
        {
          type: "bot",
          answer: "Failed to fetch response. Please try again.",
          filesAnswer: "",
          timestamp: new Date().toLocaleTimeString(),
        },
      ]);
    } finally {
      setLoadingQuery(false);
    }
  };

  return (
    <div className="w-full md:w-1/2 flex flex-col gap-4">
      <div className="bg-white p-6 rounded-lg shadow-lg flex-1 flex flex-col">
        <div className="text-center mb-6">
          <h1 className="text-3xl font-bold text-gray-800">
            File Search with RAG
          </h1>
          <p className="text-gray-600 mt-2">
            Ask me something about the private files you upload
          </p>
        </div>

        <div
          className="flex-1 overflow-y-auto mb-4 p-4 bg-gray-50 rounded-lg"
          style={{ maxHeight: "calc(100vh - 400px)" }}
        >
          <div className="space-y-4">
            {conversation.map((entry, index) => (
              <div
                key={index}
                className={`flex ${
                  entry.type === "user" ? "justify-end" : "justify-start"
                }`}
              >
                <div
                  className={`max-w-2/3 p-3 rounded-lg ${
                    entry.type === "user"
                      ? "bg-blue-500 text-white"
                      : "bg-gray-200 text-gray-800"
                  }`}
                >
                  <p className="text-sm">{entry.answer}</p>
                  <p className="text-sm pt-1">{entry.filesAnswer}</p>
                  <p
                    className={`text-xs mt-2 ${
                      entry.type === "user" ? "text-gray-100" : "text-gray-700"
                    }`}
                  >
                    {entry.timestamp}
                  </p>
                </div>
              </div>
            ))}
          </div>
          <div ref={conversationEndRef} />
        </div>
      </div>

      <form
        onSubmit={handleSubmit}
        className="bg-white p-6 rounded-lg shadow-lg"
      >
        <div className="space-y-4">
          <input
            type="text"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            placeholder="Enter your search query..."
            className="w-full p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            disabled={loadingQuery}
          />
          <textarea
            value={systemPrompt}
            onChange={(e) => setSystemPrompt(e.target.value)}
            placeholder="Enter system prompt..."
            className="w-full p-3 h-32 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            disabled={loadingQuery}
            style={{ whiteSpace: "pre-wrap" }}
          />
          <input
            type="text"
            value={customerIdQuery}
            onChange={(e) => setCustomerIdQuery(e.target.value)}
            placeholder="Enter Customer ID for Query"
            className="w-full p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            disabled={loadingQuery}
          />
          <button
            type="submit"
            className={`w-full sm:w-1/3 bg-blue-500 text-white p-3 rounded-lg transition duration-300 ${
              loadingQuery || !customerIdQuery || !query || !systemPrompt
                ? "cursor-not-allowed opacity-50"
                : "hover:bg-blue-600"
            }`}
            disabled={
              loadingQuery || !customerIdQuery || !query || !systemPrompt
            }
          >
            {loadingQuery ? "Loading..." : "Submit"}
          </button>
        </div>
      </form>
    </div>
  );
}

export default QueryConversation;
