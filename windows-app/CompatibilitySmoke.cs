using System;

internal static class CompatibilitySmoke
{
    private static void Check(bool result, string caseName)
    {
        if (!result) throw new Exception("Window matching failed: " + caseName);
    }

    private static void Main()
    {
        Check(WindowMatcher.Matches("javaw", "Minecraft 1.0", false), "release title");
        Check(WindowMatcher.Matches("javaw", "Love and Hugs", true), "focused April Fools title");
        Check(!WindowMatcher.Matches("javaw", "Love and Hugs", false), "unfocused unrelated title");
        Check(!WindowMatcher.Matches("javaw", "Minecraft Launcher", true), "launcher");
        Check(!WindowMatcher.Matches("unrelated", "Minecraft 1.0", true), "unrelated process");
        Console.WriteLine("Window matching checks passed");
    }
}

