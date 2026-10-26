$ErrorActionPreference = 'Stop'
$javaHome = $env:JAVA_HOME
if (-not $javaHome) { throw 'Set JAVA_HOME to a JDK installation.' }
$javac = Join-Path $javaHome 'bin\javac.exe'
$jar = Join-Path $javaHome 'bin\jar.exe'
if (-not (Test-Path -LiteralPath $javac)) { throw 'javac.exe not found.' }
$root = $PSScriptRoot
$deps = Join-Path $root '.deps'
$out = Join-Path $root 'out'
$classes = Join-Path $out 'classes'
New-Item -ItemType Directory -Force $deps,$classes | Out-Null
$forge = Join-Path $deps 'forge-1.12.2-14.23.5.2864-universal.jar'
if (-not (Test-Path -LiteralPath $forge)) {
    Invoke-WebRequest 'https://maven.minecraftforge.net/net/minecraftforge/forge/1.12.2-14.23.5.2864/forge-1.12.2-14.23.5.2864-universal.jar' -OutFile $forge
}
& $javac --release 8 -cp $forge -d $classes (Join-Path $root 'src\main\java\dev\bosskey\ForgeBossKey.java')
if ($LASTEXITCODE -ne 0) { throw 'Could not compile the Forge entrypoint.' }
Copy-Item -LiteralPath (Join-Path $root 'src\main\resources\mcmod.info') -Destination $classes -Force
Copy-Item -LiteralPath (Join-Path $root 'src\main\resources\bosskey') -Destination $classes -Recurse -Force
$artifact = Join-Path $out 'boss-key-forge-1.12.2-0.4.0-preview.jar'
Push-Location $classes
try { & $jar --create --file $artifact .; if ($LASTEXITCODE -ne 0) { throw 'Could not package the Forge jar.' } }
finally { Pop-Location }
Write-Output "Built $artifact"

