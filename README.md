# sign-hmac-base64-generation-tool

Simple Avalonia desktop tool to generate **HMAC-SHA256 (Base64)** for strict JSON payloads.

## Run (dev)

```bash
dotnet run
```

## Publish single EXE (Windows x64)

```powershell
pwsh -File .\publish-win-x64.ps1
```

Output:

- `publish\win-x64\SignHmacTutorial.exe`

## CLI mode (optional)

```bash
dotnet run -- --cli
```
