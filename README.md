# sign-hmac-base64-generation-tool

Simple Avalonia desktop tool to generate **HMAC-SHA256 (Base64)** for strict JSON payloads.

## CI / Releases

- Pull Requests targeting `master` run CI (restore + build).
- Pushes to `master` run CI again (ensures nothing was skipped).
- Pushing a tag like `v1.0.0` creates a GitHub Release and uploads a **single win-x64 EXE** named `Sign-HmacBase64-GenerationTool-v1.0.0.exe`.

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

### Publish via GitHub Release tag

```bash
git tag v1.0.0
git push origin v1.0.0
```

Then download the EXE from the GitHub Release page.

## CLI mode (optional)

```bash
dotnet run -- --cli
```
