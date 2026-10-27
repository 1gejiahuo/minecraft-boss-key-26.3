package dev.bosskey;

import net.neoforged.fml.common.Mod;

import java.io.IOException;
import java.io.InputStream;
import java.lang.management.ManagementFactory;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;

@Mod("bosskeyneoforge")
public final class NeoForgeBossKey {
    public NeoForgeBossKey() {
        try {
            Class<?> environment = Class.forName("net.neoforged.fml.loading.FMLEnvironment");
            Object side;
            try {
                side = environment.getMethod("getDist").invoke(null);
            } catch (NoSuchMethodException oldLoader) {
                side = environment.getField("dist").get(null);
            }
            if (!"CLIENT".equals(String.valueOf(side))) return;
            if (!System.getProperty("os.name", "").toLowerCase().contains("win")) return;
            String pid = ManagementFactory.getRuntimeMXBean().getName().split("@")[0];
            Integer.parseInt(pid);
            Path folder = Paths.get(System.getProperty("user.dir"), "config", "bosskey-neoforge-modern-0.6.0");
            Files.createDirectories(folder);
            Path program = folder.resolve("BossKey-Minecraft-Java.exe");
            Path config = folder.resolve("BossKey.ini");
            if (Files.notExists(program)) copyResource("BossKey-Minecraft-Java.exe", program);
            if (Files.notExists(config)) copyResource("BossKey.ini", config);
            new ProcessBuilder(program.toString(), "--pid=" + pid)
                    .directory(folder.toFile()).start();
        } catch (Exception error) {
            System.err.println("Boss Key NeoForge could not start: " + error);
        }
    }

    private static void copyResource(String name, Path destination) throws IOException {
        InputStream input = NeoForgeBossKey.class.getResourceAsStream("/bosskey/" + name);
        if (input == null) throw new IOException("Missing resource: " + name);
        try {
            Files.copy(input, destination);
        } finally {
            input.close();
        }
    }
}
