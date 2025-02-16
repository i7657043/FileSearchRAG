import React, { useState, useRef } from "react";

function FileUploadDelete() {
  const url = process.env.REACT_APP_API_BASE_URL ?? "";
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [lastUploadedFile, setLastUploadedFile] = useState<string | null>(null);
  const [loadingFile, setLoadingFile] = useState<boolean>(false);
  const [loadingDelete, setLoadingDelete] = useState<boolean>(false);
  const [uploadError, setUploadError] = useState<string | null>(null);
  const [chunkSize, setChunkSize] = useState<string>("150");
  const [chunkOverlap, setChunkOverlap] = useState<string>("15");
  const [uploadSuccessMessage, setUploadSuccessMessage] = useState<
    string | null
  >(null);
  const [deleteSuccessMessage, setDeleteSuccessMessage] = useState<
    string | null
  >(null);
  const [deleteErrorMessage, setDeleteErrorMessage] = useState<string | null>(
    null
  );
  const [customerIdUpload, setCustomerIdUpload] = useState<string>("");
  const fileInputRef = useRef<HTMLInputElement>(null);

  const clearMessage = (
    setter: React.Dispatch<React.SetStateAction<string | null>>
  ) => {
    setTimeout(() => {
      setter(null);
    }, 3000);
  };

  const handleFileUpload = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedFile || !customerIdUpload) return;

    setLoadingFile(true);
    setUploadError(null);
    setUploadSuccessMessage(null);

    const formData = new FormData();
    formData.append("file", selectedFile);
    formData.append("chunkSize", chunkSize);
    formData.append("chunkOverlap", chunkOverlap);
    formData.append("customerId", customerIdUpload);

    try {
      const response = await fetch(`${url}/document`, {
        method: "POST",
        body: formData,
      });

      if (response.status === 200) {
        setLastUploadedFile(selectedFile.name);
        setSelectedFile(null);
        setUploadError(null);
        setUploadSuccessMessage(
          `File "${selectedFile.name}" uploaded successfully!`
        );
        clearMessage(setUploadSuccessMessage); // Clear success message after 4 seconds
        if (fileInputRef.current) {
          fileInputRef.current.value = ""; // Clear the file input
        }
      } else {
        throw new Error("Failed to upload file");
      }
    } catch (error) {
      console.error("Error uploading file:", error);
      setUploadError("Failed to upload file. Please try again.");
      clearMessage(setUploadError); // Clear error message after 4 seconds
    } finally {
      setLoadingFile(false);
    }
  };

  const handleDeleteAll = async () => {
    setLoadingDelete(true);
    try {
      const response = await fetch(`${url}/document`, {
        method: "DELETE",
      });

      if (response.status === 200) {
        setDeleteSuccessMessage("All documents deleted successfully.");
        clearMessage(setDeleteSuccessMessage); // Clear success message after 4 seconds
      } else {
        throw new Error("Failed to delete documents");
      }
    } catch (error) {
      console.error("Error deleting documents:", error);
      setDeleteErrorMessage("Failed to delete documents. Please try again.");
      clearMessage(setDeleteErrorMessage); // Clear error message after 4 seconds
    } finally {
      setLoadingDelete(false);
    }
  };

  return (
    <div className="w-full md:w-1/2 flex flex-col gap-4 bg-black text-white p-4 rounded-lg">
      <div className="bg-gray-900 p-4 rounded-lg shadow-lg">
        <h2 className="text-2xl font-bold text-yellow-400 mb-4">
          Ingest a document
        </h2>

        <form onSubmit={handleFileUpload} className="space-y-4">
          <input
            type="file"
            onChange={(e) =>
              setSelectedFile(e.target.files ? e.target.files[0] : null)
            }
            className="p-2 border text-sm border-gray-700 rounded-lg focus:outline-none focus:ring-2 focus:ring-yellow-400 bg-gray-800 text-white "
            disabled={loadingFile}
            accept=".doc,.docx,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document,.pdf,application/pdf,.txt,text/plain"
            ref={fileInputRef}
          />
          <div className="flex gap-4">
            <div className="w-full">
              <label className="block  font-semibold text-yellow-400 mb-1">
                Customer Id
              </label>
              <input
                type="text"
                value={customerIdUpload}
                onChange={(e) => setCustomerIdUpload(e.target.value)}
                placeholder="Enter Customer ID for Upload"
                className="w-full p-2 border border-gray-700 rounded-lg focus:outline-none focus:ring-2 focus:ring-yellow-400 bg-gray-800 text-white "
                disabled={loadingFile}
              />
            </div>
            <div className="w-full">
              <label className="block  font-semibold text-yellow-400 mb-1">
                Chunk Size (bytes)
              </label>
              <input
                type="text"
                value={chunkSize}
                onChange={(e) => setChunkSize(e.target.value)}
                className="w-full p-2 border border-gray-700 rounded-lg focus:outline-none focus:ring-2 focus:ring-yellow-400 bg-gray-800 text-white "
              />
            </div>
            <div className="w-full">
              <label className="block  font-semibold text-yellow-400 mb-1">
                Chunk Overlap (bytes)
              </label>
              <input
                type="text"
                value={chunkOverlap}
                onChange={(e) => setChunkOverlap(e.target.value)}
                className="w-full p-2 border border-gray-700 rounded-lg focus:outline-none focus:ring-2 focus:ring-yellow-400 bg-gray-800 text-white "
              />
            </div>
          </div>
          <button
            type="submit"
            className={`w-full sm:w-1/3 bg-yellow-400 text-black p-2 rounded-lg transition duration-300  ${
              loadingFile ||
              !selectedFile ||
              !customerIdUpload ||
              !chunkSize ||
              !chunkOverlap
                ? "cursor-not-allowed opacity-50"
                : "hover:bg-yellow-500"
            }`}
            disabled={
              loadingFile ||
              !selectedFile ||
              !customerIdUpload ||
              !chunkSize ||
              !chunkOverlap
            }
          >
            {loadingFile ? "Uploading..." : "Upload"}
          </button>
          {uploadSuccessMessage && (
            <div className="mt-4 p-2 bg-green-900 text-green-400 rounded-lg ">
              {uploadSuccessMessage}
            </div>
          )}
          {uploadError && (
            <div className="mt-4 p-3 bg-red-900 text-red-400 rounded-lg ">
              {uploadError}
            </div>
          )}
        </form>
      </div>

      <div className="bg-gray-900 p-4 rounded-lg shadow-lg mt-5">
        <h2 className="text-2xl font-bold text-yellow-400 mb-4">Delete All</h2>
        <button
          onClick={handleDeleteAll}
          className={`w-full sm:w-1/3 bg-red-500 text-white p-2 rounded-lg transition duration-300  ${
            loadingDelete ? "cursor-not-allowed opacity-50" : "hover:bg-red-600"
          }`}
          disabled={loadingDelete}
        >
          {loadingDelete ? "Deleting..." : "Delete All Vectors"}
        </button>
        {deleteSuccessMessage && (
          <div className="mt-4 p-3 bg-green-900 text-green-400 rounded-lg">
            {deleteSuccessMessage}
          </div>
        )}
        {deleteErrorMessage && (
          <div className="mt-4 p-3 bg-red-900 text-red-400 rounded-lg">
            {deleteErrorMessage}
          </div>
        )}
      </div>
    </div>
  );
}

export default FileUploadDelete;
