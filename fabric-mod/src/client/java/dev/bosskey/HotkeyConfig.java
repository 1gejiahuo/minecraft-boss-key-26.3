package dev.bosskey;

import net.fabricmc.loader.api.FabricLoader;

import java.io.IOException;
import java.io.Reader;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.StandardOpenOption;
import java.util.Locale;
import java.util.Properties;

record HotkeyConfig(int modifiers, int virtualKey, String label) {
    private static final String DEFAULT = "# Edit the key combination, then restart Minecraft\n"
            + "hotkey=CTRL+SHIFT+C\n";

    static HotkeyConfig load() throws IOException {
        Path file = FabricLoader.getInstance().getConfigDir().resolve("bosskey.properties");
        if (Files.notExists(file)) {
            Files.createDirectories(file.getParent());
            try {
                Files.writeString(file, DEFAULT, StandardCharsets.UTF_8, StandardOpenOption.CREATE_NEW);
            } catch (java.nio.file.FileAlreadyExistsException ignored) {
                // Another startup created it.
            }
        }
        Properties properties = new Properties();
        try (Reader reader = Files.newBufferedReader(file, StandardCharsets.UTF_8)) {
            properties.load(reader);
        }
        return parse(properties.getProperty("hotkey", "CTRL+SHIFT+C"));
    }

    static HotkeyConfig parse(String value) {
        int modifiers = 0;
        int virtualKey = 0;
        for (String part : value.toUpperCase(Locale.ROOT).split("\\+")) {
            String token = part.trim();
            switch (token) {
                case "CTRL", "CONTROL" -> modifiers |= 0x0002;
                case "SHIFT" -> modifiers |= 0x0004;
                case "ALT" -> modifiers |= 0x0001;
                default -> {
                    if (token.length() == 1 &&
                            ((token.charAt(0) >= 'A' && token.charAt(0) <= 'Z') ||
                             (token.charAt(0) >= '0' && token.charAt(0) <= '9'))) {
                        if (virtualKey != 0) throw new IllegalArgumentException("Only one main key is allowed: " + value);
                        virtualKey = token.charAt(0);
                    } else if (token.matches("F(?:[1-9]|1[01])")) {
                        if (virtualKey != 0) throw new IllegalArgumentException("Only one main key is allowed: " + value);
                        virtualKey = 0x70 + Integer.parseInt(token.substring(1)) - 1;
                    } else {
                        throw new IllegalArgumentException("Unsupported key: " + token);
                    }
                }
            }
        }
        if (modifiers == 0 || virtualKey == 0) {
            throw new IllegalArgumentException("Use Ctrl, Shift or Alt plus one letter, digit or F1-F11: " + value);
        }
        return new HotkeyConfig(modifiers, virtualKey, value);
    }
}
