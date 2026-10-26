package dev.bosskey;

import com.sun.jna.platform.win32.User32;
import com.sun.jna.platform.win32.WinUser;
import net.fabricmc.api.ClientModInitializer;
import net.fabricmc.fabric.api.client.event.lifecycle.v1.ClientLifecycleEvents;
import net.minecraft.client.Minecraft;
import org.lwjgl.sdl.SDLVideo;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public final class BossKeyClient implements ClientModInitializer {
    private static final Logger LOGGER = LoggerFactory.getLogger("Boss Key");
    private WindowsHotkey hotkey;
    private boolean hidden;

    @Override
    public void onInitializeClient() {
        if (!System.getProperty("os.name", "").toLowerCase().contains("win")) {
            LOGGER.warn("Boss Key supports Windows only");
            return;
        }
        hotkey = new WindowsHotkey(() -> Minecraft.getInstance().execute(this::toggle));
        hotkey.start();
        ClientLifecycleEvents.CLIENT_STOPPING.register(client -> hotkey.stop());
    }

    private void toggle() {
        Minecraft client = Minecraft.getInstance();
        if (client.getWindow() == null) return;
        long window = client.getWindow().handle();
        if (window == 0L) return;
        if (hidden) {
            if (SDLVideo.SDL_ShowWindow(window)) {
                SDLVideo.SDL_RaiseWindow(window);
                hidden = false;
                client.getSoundManager().resume();
            } else {
                LOGGER.error("Could not show Minecraft window");
            }
        } else {
            client.getSoundManager().pauseAllExcept();
            if (SDLVideo.SDL_HideWindow(window)) {
                hidden = true;
            } else {
                client.getSoundManager().resume();
                LOGGER.error("Could not hide Minecraft window");
            }
        }
    }
}
