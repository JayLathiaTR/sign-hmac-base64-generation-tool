# Publishes SignHmacTutorial as a single self-contained Windows .exe
# Output: .\publish\win-x64\SignHmacTutorial.exe

$ErrorActionPreference = 'Stop'

Push-Location $PSScriptRoot
try {
    $outDir = Join-Path $PSScriptRoot 'publish\win-x64'
    $project = Join-Path $PSScriptRoot 'SignHmacTutorial.csproj'

    $args = @(
        'publish',
        $project,
        '-c', 'Release',
        '-r', 'win-x64',
        '--self-contained', 'true',
        '-o', $outDir,
        '/p:PublishSingleFile=true',
        '/p:IncludeNativeLibrariesForSelfExtract=true',
        '/p:PublishTrimmed=false',
        '/p:PublishReadyToRun=true',
        '/p:DebugType=None',
        '/p:DebugSymbols=false'
    )

    dotnet @args

    $exe = Join-Path $outDir 'SignHmacTutorial.exe'
    if (Test-Path $exe) {
        Write-Host "\nPublished: $exe" -ForegroundColor Green
    } else {
        Write-Host "\nPublish finished, but exe not found at: $exe" -ForegroundColor Yellow
        Write-Host "Check output folder: $outDir"
    }
}
finally {
    Pop-Location
}
