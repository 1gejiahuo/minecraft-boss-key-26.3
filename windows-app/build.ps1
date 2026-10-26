$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) { throw '.NET Framework 4.8 C# compiler not found' }
Push-Location $PSScriptRoot
try {
    & $compiler /nologo /target:winexe /platform:x64 /optimize+ /r:System.Windows.Forms.dll /out:BossKey-26.3.exe BossKey.cs AudioSessions.cs HotkeyConfig.cs
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    Write-Output 'Built BossKey-26.3.exe'
} finally {
    Pop-Location
}

