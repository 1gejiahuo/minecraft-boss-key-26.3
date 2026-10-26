package dev.bosskey;

import net.fabricmc.api.ClientModInitializer;
import net.fabricmc.loader.api.FabricLoader;

import java.io.IOException;
import java.io.InputStream;
import java.lang.management.ManagementFactory;
import java.nio.file.Files;
import java.nio.file.Path;

public final class UniversalBossKey implements ClientModInitializer {
    @Override
    public void onInitializeClient() {
        if (!System.getProperty("os.name", "").toLowerCase().contains("win")) {
            System.err.println("Boss Key Universal requires Windows.");
            return;
        }
        try {
            String pid = ManagementFactory.getRuntimeMXBean().getName().split("@")[0];
            Integer.parseInt(pid);
            Path folder = FabricLoader.getInstance().getConfigDir().resolve("bosskey-universal-0.3.0");
            Files.createDirectories(folder);
            Path program = folder.resolve("BossKey-Minecraft-Java.exe");
            Path config = folder.resolve("BossKey.ini");
            if (Files.notExists(program)) copyResource("BossKey-Minecraft-Java.exe", program);
            if (Files.notExists(config)) copyResource("BossKey.ini", config);
            new ProcessBuilder(program.toString(), "--pid=" + pid)
                    .directory(folder.toFile()).start();
        } catch (Exception error) {
            System.err.println("Boss Key Universal could not start: " + error);
        }
    }

    private static void copyResource(String name, Path destination) throws IOException {
        InputStream input = UniversalBossKey.class.getResourceAsStream("/bosskey/" + name);
        if (input == null) throw new IOException("Missing resource: " + name);
        try {
            Files.copy(input, destination);
        } finally {
            input.close();
        }
    }
}

