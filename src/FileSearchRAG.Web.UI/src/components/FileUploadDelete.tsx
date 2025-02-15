import React, { useState } from "react";

function FileUploadDelete() {
  const url = process.env.REACT_APP_API_BASE_URL ?? "";
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [lastUploadedFile, setLastUploadedFile] = useState<string | null>(null);
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
  const [customerIdUpload, setCustomerIdUpload] = useState<string>("");

  const handleFileUpload = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedFile || !customerIdUpload) return;

    setLoadingFile(true);
    setUploadError(null);
    setUploadSuccessMessage(null);

    const formData = new FormData();
    formData.append("file", selectedFile);
    formData.append("chunkSize", chunkSize.toString());
    formData.append("chunkOverlap", chunkOverlap.toString());
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
      } else {
        throw new Error("Failed to upload file");
      }
    } catch (error) {
      console.error("Error uploading file:", error);
      setUploadError("Failed to upload file. Please try again.");
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
      } else {
        throw new Error("Failed to delete documents");
      }
    } catch (error) {
      console.error("Error deleting documents:", error);
      setDeleteErrorMessage("Failed to delete documents. Please try again.");
    } finally {
      setLoadingDelete(false);
    }
  };

  return (
    <div className="w-full md:w-1/2 flex flex-col gap-4">
      <div className="bg-white p-6 rounded-lg shadow-lg">
        <h2 className="text-2xl font-bold text-gray-800 mb-4">Upload a File</h2>
        <form onSubmit={handleFileUpload} className="space-y-4">
          <input
            type="file"
            onChange={(e) =>
              setSelectedFile(e.target.files ? e.target.files[0] : null)
            }
            className="w-full p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            disabled={loadingFile}
            accept="application/pdf"
          />
          <input
            type="text"
            value={customerIdUpload}
            onChange={(e) => setCustomerIdUpload(e.target.value)}
            placeholder="Enter Customer ID for Upload"
            className="w-full p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            disabled={loadingFile}
          />
          <div className="flex gap-4">
            <div className="w-full">
              <label className="block text-sm font-semibold text-gray-700 mb-1">
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
              <label className="block text-sm font-semibold text-gray-700 mb-1">
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
            className={`w-full sm:w-1/3 bg-blue-500 text-white p-3 rounded-lg transition duration-300 ${
              loadingFile || !selectedFile || !customerIdUpload
                ? "cursor-not-allowed opacity-50"
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
      </div>

      <div className="bg-white p-6 rounded-lg shadow-lg">
        <h2 className="text-2xl font-bold text-gray-800 mb-4">
          Delete All Files
        </h2>
        <button
          onClick={handleDeleteAll}
          className={`w-full sm:w-1/3 bg-red-500 text-white p-3 rounded-lg transition duration-300 ${
            loadingDelete ? "cursor-not-allowed opacity-50" : "hover:bg-red-600"
          }`}
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
  );
}

export default FileUploadDelete;
