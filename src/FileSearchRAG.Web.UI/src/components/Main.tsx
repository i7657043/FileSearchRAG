import React from "react";
import QueryConversation from "./QueryConversation";
import FileUploadDelete from "./FileUploadDelete";

const Main: React.FC = () => {
  return (
    <div className="min-h-screen bg-gray-100 flex flex-col md:flex-row p-4 md:p-8 gap-4 md:gap-8">
      <QueryConversation />
      <FileUploadDelete />
    </div>
  );
};

export default Main;
