$ErrorActionPreference = 'Stop'
$javaHome = $env:JAVA_HOME
if (-not $javaHome) { throw 'Set JAVA_HOME to a JDK 17 or newer installation.' }
$javac = Join-Path $javaHome 'bin\javac.exe'
$jar = Join-Path $javaHome 'bin\jar.exe'
if (-not (Test-Path -LiteralPath $javac)) { throw 'javac.exe not found.' }
$root = $PSScriptRoot
$deps = Join-Path $root '.deps'
$out = Join-Path $root 'out'
$classes = Join-Path $out 'classes'
New-Item -ItemType Directory -Force $deps,$classes | Out-Null
$loader = Join-Path $deps 'javafmllanguage-1.20.1-47.4.28.jar'
if (-not (Test-Path -LiteralPath $loader)) {
    Invoke-WebRequest 'https://maven.minecraftforge.net/net/minecraftforge/javafmllanguage/1.20.1-47.4.28/javafmllanguage-1.20.1-47.4.28.jar' -OutFile $loader
}
& $javac --release 8 -cp $loader -d $classes (Join-Path $root 'src\main\java\dev\bosskey\ForgeBossKey.java')
if ($LASTEXITCODE -ne 0) { throw 'Could not compile the Forge entrypoint.' }
Copy-Item -LiteralPath (Join-Path $root 'src\main\resources\META-INF') -Destination $classes -Recurse -Force
Copy-Item -LiteralPath (Join-Path $root 'src\main\resources\bosskey') -Destination $classes -Recurse -Force
$artifact = Join-Path $out 'boss-key-forge-modern-0.6.0-preview.jar'
Push-Location $classes
try { & $jar --create --file $artifact .; if ($LASTEXITCODE -ne 0) { throw 'Could not package the Forge jar.' } }
finally { Pop-Location }
Write-Output "Built $artifact"
