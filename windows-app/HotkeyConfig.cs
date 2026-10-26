using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

internal struct Hotkey
{
    public uint Modifiers;
    public uint Key;
    public string Label;
}

internal static class HotkeyConfig
{
    public static HashSet<string> ProcessNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "java", "javaw" };
    public static string[] TitleKeywords = { "Minecraft" };
    public static bool AllowFocusedProcess = true;

    public static void Load(out Hotkey toggle, out Hotkey quit)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BossKey.ini");
        if (!File.Exists(path))
        {
            try { File.WriteAllText(path, "# Edit settings, then restart BossKey-Minecraft-Java.exe" + Environment.NewLine
                + "hotkey=CTRL+SHIFT+C" + Environment.NewLine
                + "exitHotkey=CTRL+SHIFT+X" + Environment.NewLine
                + "processNames=java,javaw" + Environment.NewLine
                + "titleKeywords=Minecraft" + Environment.NewLine
                + "allowFocusedProcess=true" + Environment.NewLine, Encoding.UTF8); }
            catch { }
        }

        string toggleText = "CTRL+SHIFT+C";
        string quitText = "CTRL+SHIFT+X";
        string processText = "java,javaw";
        string titleText = "Minecraft";
        string focusedText = "true";
        if (File.Exists(path))
        {
            foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
            {
                string trimmed = line.Trim();
                if (trimmed.Length == 0 || trimmed.StartsWith("#") || trimmed.StartsWith(";")) continue;
                int equals = trimmed.IndexOf('=');
                if (equals < 0) throw new FormatException("配置行缺少 =：" + trimmed);
                string name = trimmed.Substring(0, equals).Trim();
                string value = trimmed.Substring(equals + 1).Trim();
                if (name.Equals("hotkey", StringComparison.OrdinalIgnoreCase)) toggleText = value;
                else if (name.Equals("exitHotkey", StringComparison.OrdinalIgnoreCase)) quitText = value;
                else if (name.Equals("processNames", StringComparison.OrdinalIgnoreCase)) processText = value;
                else if (name.Equals("titleKeywords", StringComparison.OrdinalIgnoreCase)) titleText = value;
                else if (name.Equals("allowFocusedProcess", StringComparison.OrdinalIgnoreCase)) focusedText = value;
            }
        }

        toggle = Parse(toggleText);
        quit = Parse(quitText);
        if (toggle.Modifiers == quit.Modifiers && toggle.Key == quit.Key)
            throw new FormatException("隐藏键和退出键不能相同。");
        ProcessNames = ParseList(processText);
        TitleKeywords = ParseList(titleText).ToArray();
        if (ProcessNames.Count == 0 || TitleKeywords.Length == 0)
            throw new FormatException("processNames 和 titleKeywords 不能为空。");
        if (!Boolean.TryParse(focusedText, out AllowFocusedProcess))
            throw new FormatException("allowFocusedProcess 只能是 true 或 false。");
    }

    private static HashSet<string> ParseList(string value)
    {
        HashSet<string> result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (string item in value.Split(','))
            if (item.Trim().Length != 0) result.Add(item.Trim());
        return result;
    }

    private static Hotkey Parse(string value)
    {
        uint modifiers = 0;
        uint key = 0;
        foreach (string part in value.ToUpperInvariant().Split('+'))
        {
            string token = part.Trim();
            if (token == "CTRL" || token == "CONTROL") modifiers |= 0x0002;
            else if (token == "SHIFT") modifiers |= 0x0004;
            else if (token == "ALT") modifiers |= 0x0001;
            else if (token.Length == 1 && ((token[0] >= 'A' && token[0] <= 'Z') ||
                                             (token[0] >= '0' && token[0] <= '9')))
            {
                if (key != 0) throw new FormatException("只能指定一个主按键：" + value);
                key = token[0];
            }
            else if (token.Length >= 2 && token[0] == 'F')
            {
                int number;
                if (!Int32.TryParse(token.Substring(1), out number) || number < 1 || number > 11 || key != 0)
                    throw new FormatException("只支持 F1 到 F11：" + value);
                key = (uint)(0x70 + number - 1);
            }
            else throw new FormatException("不支持的按键：" + token);
        }
        if (modifiers == 0 || key == 0) throw new FormatException("组合键需要 Ctrl、Shift 或 Alt，再加一个字母、数字或 F1-F11：" + value);
        return new Hotkey { Modifiers = modifiers, Key = key, Label = value };
    }
}

