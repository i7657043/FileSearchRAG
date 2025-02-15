variable "webApiAppName" {
  type      = string
  sensitive = true
}

variable "image" {
  type      = string
  sensitive = true
}

variable "acrpassword" {
  type      = string
  sensitive = true
}

variable "pineconeApiKey" {
  type      = string
  sensitive = true
}

variable "openAiApiKey" {
  type      = string
  sensitive = true
}
