
Start-Process powershell -WorkingDirectory .\FileSearchRAG.Web.API -ArgumentList "dotnet run"
Start-Sleep -Seconds 3
Start-Process powershell -WorkingDirectory .\FileSearchRAG.Web.UI -ArgumentList "yarn start"
