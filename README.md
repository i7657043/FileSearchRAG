# RagAiAAS

Search your own private files using GPT-4.

### The user can:

#### 1. Upload `.pdf`, `.doc/x`, and `.txt` files through the UI

- The API will:
  - Ingest and chunk the document
  - Generate vectors by encoding the chunks using OpenAI's `text-embedding-ada-002` embedding model
  - Insert the vectors into a Pinecone index

#### 2. Query their documents through the UI

- The API will:
  - Generate vectors by encoding the query using OpenAI's `text-embedding-ada-002` embedding model
  - Retrieve matching records from the Pinecone DB using semantic search
  - Pass those records to OpenAI's API along with the query to generate a more refined response

#### 3. Change configuration through the UI

- Modify size and overlap configuration of document chunking
- Modify the system prompt used for each query
- Test customer data separation and partitioning
