# Compatibility

The Windows app uses Windows window and audio APIs instead of Minecraft internals. Its intended scope is Minecraft: Java Edition on Windows, from the 1.0 release onward, including official releases and April Fools builds. The 26.3 Fabric mod has a separate, version-specific scope.

## How the Windows app finds a game

1. If the foreground window belongs to an allowed process (default: `java` or `javaw`), it can be selected regardless of the title. This helps old and April Fools builds with unusual titles.
2. Otherwise, a visible window from an allowed process is selected when its title contains a configured keyword (default: `Minecraft`).
3. Windows whose title contains `Launcher` are excluded.

Edit `BossKey.ini` and restart to change `processNames`, `titleKeywords`, or `allowFocusedProcess`. The process name is the executable name without `.exe`.

## Verification status

The app compiles, and the matching logic passes a smoke check for a release title, an unusual foreground title, the launcher exclusion, and an unrelated process. Hiding, restoration, and audio muting have not been checked on every game version. Fullscreen modes and custom launchers can behave differently. Test with a disposable world before relying on the shortcut.

This scope concerns Java Edition. It does not imply support for Bedrock Edition or Minecraft versions before 1.0.

