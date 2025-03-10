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
  const [systemPrompt, setSystemPrompt] = useState<string>(
    `Use the given context to answer the question
If you don't know the answer, say you don't know.
Use three sentence maximum and keep the answer concise.`
  );
  const [conversation, setConversation] = useState<ConversationEntry[]>([]);
  const [customerIdQuery, setCustomerId] = useState<string>("001");
  const [typingAnswer, setTypingAnswer] = useState<string>("");
  const conversationEndRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (conversationEndRef.current) {
      conversationEndRef.current.scrollIntoView({ behavior: "smooth" });
    }
  }, [conversation, typingAnswer]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Clear typingAnswer when a new query is submitted
    setTypingAnswer("");

    // Add user input to the conversation immediately
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
        answer: "Loading...", // Placeholder for bot's answer
        filesAnswer: "",
        timestamp: new Date().toLocaleTimeString(),
      },
    ]);

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

      // Check for a successful response
      if (!apiResponse.ok) {
        throw new Error(`HTTP error! Status: ${apiResponse.status}`);
      }

      const data = await apiResponse.json();
      console.log(data); // Log the data for debugging

      // Check if the response contains data before starting typing effect
      if (data.answer && data.filesAnswer) {
        startTypingEffect(data.answer, data.filesAnswer);
      } else {
        // If no answer is provided, display an error message
        setConversation((prevConversation) =>
          prevConversation.map((entry, i) =>
            i === prevConversation.length - 1
              ? {
                  ...entry,
                  answer:
                    "No response from the server. Please try again later.",
                }
              : entry
          )
        );
      }
    } catch (error) {
      console.error("Error fetching data:", error);
      // Update the bot's response with an error message
      setConversation((prevConversation) =>
        prevConversation.map((entry, i) =>
          i === prevConversation.length - 1
            ? {
                ...entry,
                answer: "Failed to fetch response. Please try again.",
              }
            : entry
        )
      );
    }
  };

  const startTypingEffect = (fullText: string, filesAnswer: string) => {
    let currentText = "";

    fullText.split("").forEach((char, index) => {
      setTimeout(() => {
        currentText += char;
        setTypingAnswer(currentText);

        // Once typing is done, update the conversation state
        if (index === fullText.length - 1) {
          setConversation((prevConversation) =>
            prevConversation.map((entry, i) =>
              i === prevConversation.length - 1
                ? { ...entry, answer: fullText, filesAnswer }
                : entry
            )
          );
          setTypingAnswer(""); // Clear typingAnswer after the typing effect is complete
        }
      }, index * 20);
    });
  };

  return (
    <div className="w-full md:w-1/2 h-screen flex flex-col bg-black text-white p-4 rounded-lg">
      {/* Conversation Box */}
      <div
        className="flex-1 overflow-y-auto mb-4 bg-gray-800 rounded-lg p-4"
        style={{ minHeight: "250px" }}
      >
        <span className="text-xs text-gray-500">Conversation History</span>
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
                    ? "bg-yellow-400 text-black"
                    : "bg-gray-700 text-white"
                }`}
              >
                <p className="text-sm">
                  {entry.type === "bot" && index === conversation.length - 1
                    ? typingAnswer || entry.answer // Show typingAnswer if it exists, otherwise show the answer
                    : entry.answer}
                </p>
                {entry.type === "bot" && entry.filesAnswer && (
                  <p className="text-sm pt-1 text-gray-400">
                    {entry.filesAnswer}
                  </p>
                )}
                <p
                  className={`text-xs mt-2 ${
                    entry.type === "user" ? "text-gray-800" : "text-gray-300"
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

      {/* Form */}
      <form
        onSubmit={handleSubmit}
        className="bg-gray-900 p-4 rounded-lg shadow-lg"
      >
        <div className="space-y-4">
          <input
            type="text"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            placeholder="Enter your search query..."
            className="w-full text-xs sm:text-base p-2 border border-gray-700 rounded-lg focus:outline-none focus:ring-2 focus:ring-yellow-400 bg-gray-800 text-white "
          />
          <div className="text-xs pl-1">System prompt</div>
          <textarea
            value={systemPrompt}
            onChange={(e) => setSystemPrompt(e.target.value)}
            placeholder="Enter system prompt..."
            className="w-full sm:text-sm text-xs p-2 border border-gray-700 rounded-lg focus:outline-none focus:ring-2 focus:ring-yellow-400 bg-gray-800 text-white "
            style={{ whiteSpace: "pre-wrap", height: "120px" }}
          />
          <input
            type="text"
            value={customerIdQuery}
            onChange={(e) => setCustomerId(e.target.value)}
            placeholder="Enter Customer ID for Query"
            className="w-full text-xs sm:text-base p-2 border border-gray-700 rounded-lg focus:outline-none focus:ring-2 focus:ring-yellow-400 bg-gray-800 text-white "
          />
          <button
            type="submit"
            className={`w-full sm:w-1/3 bg-yellow-400 text-black p-2 rounded-lg transition duration-300 text-sm sm:text-base ${
              !customerIdQuery || !query || !systemPrompt
                ? "cursor-not-allowed opacity-50"
                : "hover:bg-yellow-500"
            }`}
            disabled={!customerIdQuery || !query || !systemPrompt}
          >
            Submit Query
          </button>
        </div>
      </form>
    </div>
  );
}

export default QueryConversation;
