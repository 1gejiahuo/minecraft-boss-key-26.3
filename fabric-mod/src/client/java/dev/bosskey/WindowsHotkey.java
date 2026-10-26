package dev.bosskey;

import com.sun.jna.platform.win32.Kernel32;
import com.sun.jna.platform.win32.User32;
import com.sun.jna.platform.win32.WinUser;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

final class WindowsHotkey {
    private static final Logger LOGGER = LoggerFactory.getLogger("Boss Key");
    private static final int HOTKEY_ID = 0x424B;
    private static final int MOD_NOREPEAT = 0x4000;
    private static final int WM_QUIT = 0x0012;
    private final Runnable onPress;
    private volatile int threadId;

    WindowsHotkey(Runnable onPress) {
        this.onPress = onPress;
    }

    void start() {
        Thread thread = new Thread(this::run, "boss-key-hotkey");
        thread.setDaemon(true);
        thread.start();
    }

    void stop() {
        int id = threadId;
        if (id != 0) User32.INSTANCE.PostThreadMessage(id, WM_QUIT, null, null);
    }

    private void run() {
        HotkeyConfig config;
        try {
            config = HotkeyConfig.load();
        } catch (Exception error) {
            LOGGER.error("Could not read config/bosskey.properties", error);
            return;
        }
        threadId = Kernel32.INSTANCE.GetCurrentThreadId();
        if (!User32.INSTANCE.RegisterHotKey(null, HOTKEY_ID,
                config.modifiers() | MOD_NOREPEAT, config.virtualKey())) {
            LOGGER.error("Cannot register {}. Another app may be using it. Win32 error: {}",
                    config.label(), Kernel32.INSTANCE.GetLastError());
            threadId = 0;
            return;
        }
        LOGGER.info("{} registered as the global boss key", config.label());
        try {
            WinUser.MSG message = new WinUser.MSG();
            while (User32.INSTANCE.GetMessage(message, null, 0, 0) > 0) {
                if (message.message == WinUser.WM_HOTKEY && message.wParam.intValue() == HOTKEY_ID) {
                    onPress.run();
                }
            }
        } finally {
            User32.INSTANCE.UnregisterHotKey(null, HOTKEY_ID);
            threadId = 0;
        }
    }
}
