using System;

internal static class WindowMatcher
{
    public static bool Matches(string process, string title, bool isForeground)
    {
        if (String.IsNullOrWhiteSpace(title) || title.IndexOf("Launcher", StringComparison.OrdinalIgnoreCase) >= 0)
            return false;
        if (!HotkeyConfig.ProcessNames.Contains(process)) return false;
        if (isForeground && HotkeyConfig.AllowFocusedProcess) return true;
        foreach (string keyword in HotkeyConfig.TitleKeywords)
            if (title.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) return true;
        return false;
    }
}

