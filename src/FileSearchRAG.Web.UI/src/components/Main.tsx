import React, { useState, useEffect, useRef } from "react";

interface ConversationEntry {
  type: "user" | "bot";
  answer: string;
  filesAnswer: string;
  timestamp: string;
}

function Main() {
  const [query, setQuery] = useState<string>("");
  const [systemPrompt, setSystemPrompt] =
    useState<string>(`Use the given context to answer the question
If you don't know the answer, say you don't know.
Use three sentence maximum and keep the answer concise.`); // Initial multiline value
  const [conversation, setConversation] = useState<ConversationEntry[]>([]);
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [lastUploadedFile, setLastUploadedFile] = useState<string | null>(null);
  const [loadingQuery, setLoadingQuery] = useState<boolean>(false);
  const [loadingFile, setLoadingFile] = useState<boolean>(false);
  const [loadingDelete, setLoadingDelete] = useState<boolean>(false);
  const [uploadError, setUploadError] = useState<string | null>(null);
  const [chunkSize, setChunkSize] = useState<number>(150);
  const [chunkOverlap, setChunkOverlap] = useState<number>(15);
  const [uploadSuccessMessage, setUploadSuccessMessage] = useState<
    string | null
  >(null);
  const [deleteSuccessMessage, setDeleteSuccessMessage] = useState<
    string | null
  >(null);
  const [deleteErrorMessage, setDeleteErrorMessage] = useState<string | null>(
    null
  );
  const [customerIdQuery, setCustomerIdQuery] = useState<string>(""); // Customer ID state for query
  const [customerIdUpload, setCustomerIdUpload] = useState<string>(""); // Customer ID state for upload
  const conversationEndRef = useRef<HTMLDivElement>(null);
  const url = process.env.REACT_APP_API_BASE_URL;
  console.log("URL: " + url);

  useEffect(() => {
    if (conversationEndRef.current) {
      conversationEndRef.current.scrollIntoView({ behavior: "smooth" });
    }
  }, [conversation]);

  useEffect(() => {
    if (
      uploadSuccessMessage ||
      deleteSuccessMessage ||
      uploadError ||
      deleteErrorMessage
    ) {
      const timer = setTimeout(() => {
        setUploadSuccessMessage(null);
        setDeleteSuccessMessage(null);
        setUploadError(null);
        setDeleteErrorMessage(null);
      }, 4000);

      return () => clearTimeout(timer);
    }
  }, [
    uploadSuccessMessage,
    deleteSuccessMessage,
    uploadError,
    deleteErrorMessage,
  ]);

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
        }), // Use customerIdQuery
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

  const handleFileUpload = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedFile || !customerIdUpload) return; // Ensure customerIdUpload is provided

    setLoadingFile(true);
    setUploadError(null); // Reset the error before starting the upload
    setUploadSuccessMessage(null); // Reset the success message before starting the upload

    const formData = new FormData();
    formData.append("file", selectedFile);
    formData.append("chunkSize", chunkSize.toString()); // Add chunk size
    formData.append("chunkOverlap", chunkOverlap.toString()); // Add chunk overlap
    formData.append("customerId", customerIdUpload); // Use customerIdUpload

    try {
      const response = await fetch(`${url}/document`, {
        method: "POST",
        body: formData,
      });

      if (response.status === 200) {
        setLastUploadedFile(selectedFile.name);
        setSelectedFile(null);
        setUploadError(null); // Clear any previous errors
        setUploadSuccessMessage(
          `File "${selectedFile.name}" uploaded successfully!`
        ); // Set success message
      } else {
        throw new Error("Failed to upload file");
      }
    } catch (error) {
      console.error("Error uploading file:", error);
      setUploadError("Failed to upload file. Please try again."); // Set error message
    } finally {
      setLoadingFile(false);
    }
  };

  const handleDeleteAll = async () => {
    setLoadingDelete(true); // Set loading to true when starting the delete process
    try {
      const response = await fetch(`${url}/document`, {
        method: "DELETE",
      });

      if (response.status === 200) {
        setDeleteSuccessMessage("All documents deleted successfully."); // Set success message for delete
      } else {
        throw new Error("Failed to delete documents");
      }
    } catch (error) {
      console.error("Error deleting documents:", error);
      setDeleteErrorMessage("Failed to delete documents. Please try again."); // Set error message for delete
    } finally {
      setLoadingDelete(false); // Set loading to false when done
    }
  };

  return (
    <div className="min-h-screen bg-gray-100 flex flex-col md:flex-row">
      <div className="w-full md:w-1/2 p-4 md:p-8 flex flex-col">
        <div className="bg-white p-6 md:p-8 rounded-lg shadow-lg flex-1 flex flex-col">
          <div className="text-center mb-6">
            <h1 className="text-3xl md:text-4xl font-bold text-gray-800">
              File search with RAG
            </h1>
            <p className="text-gray-600 mt-2">
              Ask me something about the private files you upload
            </p>
          </div>

          {/* Adjust the scrollable area for conversation history */}
          <div
            className="flex-1 overflow-y-auto mb-4 p-4 bg-gray-50 rounded-lg"
            style={{ maxHeight: "calc(100vh - 350px)" }} // Adjust max height to make only the conversation area scrollable
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
                        entry.type === "user"
                          ? "text-gray-100"
                          : "text-gray-700"
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
          className="sticky bottom-0 bg-white p-4 md:p-6 rounded-lg shadow-lg mt-4"
        >
          <div className="flex justify-justify mt-4">
            <input
              type="text"
              value={query}
              onChange={(e) => setQuery(e.target.value)}
              placeholder="Enter your search query..."
              className="w-4/5 p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              disabled={loadingQuery}
            />
          </div>
          <div className="flex justify-justify mt-4">
            <textarea
              value={systemPrompt}
              onChange={(e) => setSystemPrompt(e.target.value)}
              className="w-4/5 p-3 h-32 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              disabled={loadingQuery}
              style={{ whiteSpace: "pre-wrap" }}
            />
          </div>

          {/* Add Customer ID Input Field Below the History Section */}
          <div className="flex justify-left mt-4">
            <input
              type="text"
              value={customerIdQuery}
              onChange={(e) => setCustomerIdQuery(e.target.value)}
              placeholder="Enter Customer ID for Query"
              className="sm:w-1/3 p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              disabled={loadingQuery}
            />
          </div>

          <div className="flex justify-center mt-4">
            <button
              id="query-submit"
              type="submit"
              className={`sm:w-1/3 bg-blue-500 text-white p-3 rounded-lg transition duration-300 ${
                loadingQuery || !customerIdQuery || !query || !systemPrompt
                  ? "cursor-not-allowed"
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

      <div className="w-full md:w-1/2 p-4 md:p-8">
        <div className="bg-white p-6 md:p-8 rounded-lg shadow-lg">
          <div>
            <h2 className="text-2xl font-bold text-gray-800 mb-4">
              Upload a File
            </h2>
            <form onSubmit={handleFileUpload} className="space-y-4">
              <input
                type="file"
                onChange={(e) =>
                  setSelectedFile(e.target.files ? e.target.files[0] : null)
                }
                className="w-full p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                disabled={loadingFile}
              />

              {/* Customer ID Input for Upload */}
              <input
                type="text"
                value={customerIdUpload}
                onChange={(e) => setCustomerIdUpload(e.target.value)}
                placeholder="Enter Customer ID for Upload"
                className="sm:w-1/3 p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                disabled={loadingFile}
              />

              <div className="flex space-x-4">
                <div className="w-full">
                  <label className="block text-sm font-semibold text-gray-700">
                    Chunk Size (bytes)
                  </label>
                  <input
                    type="number"
                    value={chunkSize}
                    onChange={(e) => setChunkSize(Number(e.target.value))}
                    className="w-full p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>
                <div className="w-full">
                  <label className="block text-sm font-semibold text-gray-700">
                    Chunk Overlap (bytes)
                  </label>
                  <input
                    type="number"
                    value={chunkOverlap}
                    onChange={(e) => setChunkOverlap(Number(e.target.value))}
                    className="w-full p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>
              </div>
              <button
                type="submit"
                className={`sm:w-1/3 bg-blue-500 text-white p-3 rounded-lg transition duration-300 ${
                  loadingFile || !selectedFile || !customerIdUpload
                    ? "cursor-not-allowed"
                    : "hover:bg-blue-600"
                }`}
                disabled={loadingFile || !selectedFile || !customerIdUpload}
              >
                {loadingFile ? "Uploading..." : "Upload"}
              </button>

              {uploadSuccessMessage && (
                <div className="mt-4 p-3 bg-green-100 text-green-800 rounded-lg">
                  {uploadSuccessMessage}
                </div>
              )}
              {uploadError && (
                <div className="mt-4 p-3 bg-red-100 text-red-800 rounded-lg">
                  {uploadError}
                </div>
              )}
            </form>

            <div className="mt-16">
              <h2 className="text-2xl font-bold text-gray-800 mb-4">
                Delete all Files
              </h2>
              <button
                onClick={handleDeleteAll}
                className="sm:w-1/3 bg-red-500 text-white p-3 rounded-lg hover:bg-red-600 transition duration-300"
                disabled={loadingDelete}
              >
                {loadingDelete ? "Deleting..." : "Delete All Files"}
              </button>
              {deleteSuccessMessage && (
                <div className="mt-4 p-3 bg-green-100 text-green-800 rounded-lg">
                  {deleteSuccessMessage}
                </div>
              )}
              {deleteErrorMessage && (
                <div className="mt-4 p-3 bg-red-100 text-red-800 rounded-lg">
                  {deleteErrorMessage}
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Main;
