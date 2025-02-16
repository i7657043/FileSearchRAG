import React from "react";
import "./App.css";
import FileUploadDelete from "./components/FileUploadDelete";
import QueryConversation from "./components/QueryConversation";

function App() {
  return (
    <div className="min-h-screen bg-gray-900 ">
      <div className="flex flex-col md:flex-row">
        <QueryConversation />
        <FileUploadDelete />
      </div>
    </div>
  );
}

export default App;
