$ErrorActionPreference = 'Stop'
$javaHome = $env:JAVA_HOME
if (-not $javaHome) { throw 'Set JAVA_HOME to a JDK 25 installation.' }
$javac = Join-Path $javaHome 'bin\javac.exe'
$jar = Join-Path $javaHome 'bin\jar.exe'
if (-not (Test-Path -LiteralPath $javac)) { throw 'JDK 25 javac.exe not found.' }
$root = $PSScriptRoot
$deps = Join-Path $root '.deps'
$out = Join-Path $root 'out'
$classes = Join-Path $out 'classes'
New-Item -ItemType Directory -Force $deps,$classes | Out-Null
$loader = Join-Path $deps 'fml-loader-12.0.12.jar'
if (-not (Test-Path -LiteralPath $loader)) {
    Invoke-WebRequest 'https://maven.neoforged.net/releases/net/neoforged/fancymodloader/loader/12.0.12/loader-12.0.12.jar' -OutFile $loader
}
$stubSource = Join-Path $deps 'compile-stubs\net\neoforged\api\distmarker\Dist.java'
$stubClasses = Join-Path $deps 'stub-classes'
New-Item -ItemType Directory -Force (Split-Path $stubSource),$stubClasses | Out-Null
[IO.File]::WriteAllText($stubSource, 'package net.neoforged.api.distmarker; public enum Dist { CLIENT, DEDICATED_SERVER }')
& $javac --release 25 -d $stubClasses $stubSource
if ($LASTEXITCODE -ne 0) { throw 'Could not compile the compile-only Dist stub.' }
& $javac --release 25 -cp "$loader;$stubClasses" -d $classes (Join-Path $root 'src\main\java\dev\bosskey\NeoForgeBossKey.java')
if ($LASTEXITCODE -ne 0) { throw 'Could not compile the NeoForge entrypoint.' }
Copy-Item -LiteralPath (Join-Path $root 'src\main\resources\META-INF') -Destination $classes -Recurse -Force
Copy-Item -LiteralPath (Join-Path $root 'src\main\resources\bosskey') -Destination $classes -Recurse -Force
$artifact = Join-Path $out 'boss-key-neoforge-26.3-0.4.0-preview.jar'
Push-Location $classes
try { & $jar --create --file $artifact .; if ($LASTEXITCODE -ne 0) { throw 'Could not package the NeoForge jar.' } }
finally { Pop-Location }
Write-Output "Built $artifact"

