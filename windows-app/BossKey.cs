using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

internal static class BossKey
{
    private const int ToggleId = 0x424B;
    private const int QuitId = 0x424C;
    private const uint ModNoRepeat = 0x4000;
    private const int WmHotkey = 0x0312;
    private const int WmTimer = 0x0113;
    private const int SwHide = 0;
    private const int SwShow = 5;

    private static IntPtr hiddenWindow = IntPtr.Zero;
    private static int hiddenProcessId;
    private static readonly Dictionary<string, bool> oldMuteStates = new Dictionary<string, bool>();

    [STAThread]
    private static void Main()
    {
        Hotkey toggleKey;
        Hotkey quitKey;
        try { HotkeyConfig.Load(out toggleKey, out quitKey); }
        catch (Exception error) { MessageBox.Show("按键配置错误：" + error.Message, "Minecraft 老板键", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

        if (!RegisterHotKey(IntPtr.Zero, ToggleId, toggleKey.Modifiers | ModNoRepeat, toggleKey.Key))
        {
            MessageBox.Show("组合键 " + toggleKey.Label + " 已被其他程序占用，老板键无法启动。", "Minecraft 老板键",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        if (!RegisterHotKey(IntPtr.Zero, QuitId, quitKey.Modifiers | ModNoRepeat, quitKey.Key))
        {
            UnregisterHotKey(IntPtr.Zero, ToggleId);
            MessageBox.Show("组合键 " + quitKey.Label + " 已被其他程序占用，老板键无法启动。", "Minecraft 老板键",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        UIntPtr timer = SetTimer(IntPtr.Zero, UIntPtr.Zero, 1000, IntPtr.Zero);
        try
        {
            NativeMessage message;
            while (GetMessage(out message, IntPtr.Zero, 0, 0) > 0)
            {
                if (message.message == WmHotkey)
                {
                    if (message.wParam.ToInt64() == ToggleId) Toggle();
                    else if (message.wParam.ToInt64() == QuitId) { Restore(); if (hiddenWindow == IntPtr.Zero) break; }
                }
                else if (message.message == WmTimer && hiddenWindow != IntPtr.Zero)
                {
                    if (!IsWindow(hiddenWindow))
                    {
                        hiddenWindow = IntPtr.Zero;
                        hiddenProcessId = 0;
                        oldMuteStates.Clear();
                    }
                    else TryMuteNewSessions();
                }
            }
        }
        finally
        {
            Restore();
            if (timer != UIntPtr.Zero) KillTimer(IntPtr.Zero, timer);
            UnregisterHotKey(IntPtr.Zero, ToggleId);
            UnregisterHotKey(IntPtr.Zero, QuitId);
        }
    }

    private static void Toggle()
    {
        if (hiddenWindow != IntPtr.Zero)
        {
            Restore();
            return;
        }

        IntPtr gameWindow = FindMinecraftWindow();
        if (gameWindow == IntPtr.Zero) return;

        uint pid;
        GetWindowThreadProcessId(gameWindow, out pid);
        hiddenWindow = gameWindow;
        hiddenProcessId = unchecked((int)pid);
        try { AudioSessions.SetMute(hiddenProcessId, true, oldMuteStates, true); }
        catch (Exception error) { Log("Mute failed: " + error); Restore(); MessageBox.Show("无法静音 Minecraft，已取消隐藏。详情见日志。", "Minecraft 老板键", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

        ShowWindow(gameWindow, SwHide);
        if (IsWindowVisible(gameWindow))
        {
            Log("Could not hide Minecraft window");
            Restore();
        }
    }

    private static void Restore()
    {
        IntPtr gameWindow = hiddenWindow;
        if (gameWindow == IntPtr.Zero) return;

        if (IsWindow(gameWindow))
        {
            ShowWindow(gameWindow, SwShow);
            if (!IsWindowVisible(gameWindow)) SetWindowPos(gameWindow, IntPtr.Zero, 0, 0, 0, 0, 0x0047);
            if (!IsWindowVisible(gameWindow)) { Log("Could not restore Minecraft window"); return; }
            SetForegroundWindow(gameWindow);
        }

        try { AudioSessions.SetMute(hiddenProcessId, false, oldMuteStates, false); }
        catch (Exception error) { Log("Unmute failed: " + error); }

        hiddenWindow = IntPtr.Zero;
        hiddenProcessId = 0;
        oldMuteStates.Clear();
    }

    private static void TryMuteNewSessions()
    {
        try { AudioSessions.SetMute(hiddenProcessId, true, oldMuteStates, true); }
        catch (Exception error) { Log("Periodic mute failed: " + error); }
    }

    private static IntPtr FindMinecraftWindow()
    {
        IntPtr foreground = GetForegroundWindow();
        if (IsMinecraftWindow(foreground, true)) return foreground;

        IntPtr found = IntPtr.Zero;
        EnumWindows(delegate(IntPtr window, IntPtr ignored)
        {
            if (IsMinecraftWindow(window, false))
            {
                found = window;
                return false;
            }
            return true;
        }, IntPtr.Zero);
        return found;
    }

    private static bool IsMinecraftWindow(IntPtr window, bool isForeground)
    {
        if (window == IntPtr.Zero || !IsWindowVisible(window)) return false;
        int titleLength = GetWindowTextLength(window);
        if (titleLength == 0) return false;
        StringBuilder title = new StringBuilder(titleLength + 1);
        GetWindowText(window, title, title.Capacity);
        string name = title.ToString();
        uint pid;
        GetWindowThreadProcessId(window, out pid);
        try
        {
            string executable = Process.GetProcessById(unchecked((int)pid)).ProcessName;
            return WindowMatcher.Matches(executable, name, isForeground);
        }
        catch { return false; }
    }

    private static void Log(string line)
    {
        try
        {
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MinecraftBossKey");
            Directory.CreateDirectory(folder);
            File.AppendAllText(Path.Combine(folder, "bosskey.log"), DateTime.Now.ToString("s") + " " + line + Environment.NewLine);
        }
        catch { }
    }

    private delegate bool EnumWindowsCallback(IntPtr window, IntPtr parameter);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeMessage
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public int pointX;
        public int pointY;
        public uint privateData;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr window, int id, uint modifiers, uint key);
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr window, int id);
    [DllImport("user32.dll")]
    private static extern int GetMessage(out NativeMessage message, IntPtr window, uint minFilter, uint maxFilter);
    [DllImport("user32.dll")]
    private static extern UIntPtr SetTimer(IntPtr window, UIntPtr id, uint milliseconds, IntPtr callback);
    [DllImport("user32.dll")]
    private static extern bool KillTimer(IntPtr window, UIntPtr id);
    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsCallback callback, IntPtr parameter);
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")]
    private static extern int GetWindowTextLength(IntPtr window);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr window, StringBuilder text, int maxLength);
    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr window, out uint processId);
    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr window);
    [DllImport("user32.dll")]
    private static extern bool IsWindow(IntPtr window);
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr window, int command);
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr window);
    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr window, IntPtr insertAfter, int x, int y, int width, int height, uint flags);
}





