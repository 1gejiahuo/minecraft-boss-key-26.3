$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) { throw '.NET Framework 4.8 C# compiler not found' }
Push-Location $PSScriptRoot
try {
    & $compiler /nologo /target:winexe /platform:x64 /optimize+ /r:System.Windows.Forms.dll /out:BossKey-Minecraft-Java.exe BossKey.cs AudioSessions.cs HotkeyConfig.cs WindowMatcher.cs
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    Write-Output 'Built BossKey-Minecraft-Java.exe'
} finally {
    Pop-Location
}


