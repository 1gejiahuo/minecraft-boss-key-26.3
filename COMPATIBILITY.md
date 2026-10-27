# Compatibility

The Windows app uses Windows window and audio APIs instead of Minecraft internals. Its intended scope is Minecraft: Java Edition on Windows, from the 1.0 release onward, including official releases and April Fools builds. The universal Fabric entrypoint bundles this Windows app and passes the game process ID to it. The 26.3 native Fabric mod has a separate, version-specific scope.

Fabric officially supports releases from 1.14 onward and snapshots from 18w43b onward; Minecraft 1.0–1.13 cannot be covered by an officially supported Fabric mod. The universal entrypoint targets supported Fabric versions and builds with Java 8 bytecode, but has not been run against each game version. April Fools versions require a working Fabric Loader installation. The standalone Windows app is the option for older versions.

## How the Windows app finds a game

1. If the foreground window belongs to an allowed process (default: `java` or `javaw`), it can be selected regardless of the title. This helps old and April Fools builds with unusual titles.
2. Otherwise, a visible window from an allowed process is selected when its title contains a configured keyword (default: `Minecraft`).
3. Windows whose title contains `Launcher` are excluded.

Edit `BossKey.ini` and restart to change `processNames`, `titleKeywords`, or `allowFocusedProcess`. The process name is the executable name without `.exe`.

## Verification status

The app compiles, and the matching logic passes a smoke check for a release title, an unusual foreground title, the launcher exclusion, and an unrelated process. Hiding, restoration, and audio muting have not been checked on every game version. Fullscreen modes and custom launchers can behave differently. Test with a disposable world before relying on the shortcut.

This scope concerns Java Edition. It does not imply support for Bedrock Edition or Minecraft versions before 1.0.

## Other loaders

Quilt Loader generally accepts Fabric mods. This jar uses Fabric Loader APIs only, so it is a candidate for Quilt; Quilt runtime testing is still pending. Separate entrypoint JARs have been built for Forge 1.12.2 and 1.20.1, and NeoForge 1.21.1 and 26.3 using official loader compile dependencies. They bundle the same Windows helper and pass the game process ID. Their JAR structure has been checked, but no game runtime test has been performed. Other Forge/NeoForge versions can use the standalone Windows app until an adapter for that loader version is built and tested.


## Cross-version loader entrypoints (0.6.0 preview)

These are *experimental family builds*, compiled against one official loader artifact per family. They have not been launched in every Minecraft version. A version needs a working installation of its corresponding loader; unusual April Fools builds may have no loader at all. The Windows standalone EXE is the version-neutral option for those builds.

| Loader family | JAR | Intended range | Build check |
| --- | --- | --- | --- |
| Forge 1.7 | `boss-key-forge-1.7-0.6.0-preview.jar` | 1.7.x | Forge 1.7.10 API, Java 7 bytecode |
| Forge legacy | `boss-key-forge-legacy-0.6.0-preview.jar` | 1.8–1.12 | Forge 1.8.9 API, Java 7 bytecode |
| Forge modern | `boss-key-forge-modern-0.6.0-preview.jar` | 1.13+ | Forge 1.20.1 API, Java 8 bytecode |
| NeoForge early | `boss-key-neoforge-early-0.6.0-preview.jar` | 1.20.2–1.20.4 | NeoForge FML 2.0.17 API, Java 17 bytecode |
| NeoForge modern | `boss-key-neoforge-modern-0.6.0-preview.jar` | 1.20.5+ | NeoForge FML 4.0.45 API, Java 21 bytecode |

Forge 1.0–1.6 is not covered by these mod JARs. The app covers these game versions without a loader. For 1.6.x, API inspection suggests the Forge 1.7 entrypoint could be adapted, but that range needs its own packaging and test before it is claimed. For each April Fools build, use a loader JAR only if that build can actually run its corresponding loader. Do not install two Boss Key entrypoints together.
