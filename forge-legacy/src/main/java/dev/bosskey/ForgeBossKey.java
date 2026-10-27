package dev.bosskey;

import net.minecraftforge.fml.common.Mod;

import java.io.IOException;
import java.io.InputStream;
import java.lang.management.ManagementFactory;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;

@Mod(modid = "bosskeyforge", name = "Boss Key Forge", version = "0.6.0-preview",
        clientSideOnly = true)
public final class ForgeBossKey {
    public ForgeBossKey() {
        if (!System.getProperty("os.name", "").toLowerCase().contains("win")) return;
        try {
            String pid = ManagementFactory.getRuntimeMXBean().getName().split("@")[0];
            Integer.parseInt(pid);
            Path folder = Paths.get(System.getProperty("user.dir"), "config", "bosskey-forge-legacy-0.6.0");
            Files.createDirectories(folder);
            Path program = folder.resolve("BossKey-Minecraft-Java.exe");
            Path config = folder.resolve("BossKey.ini");
            if (Files.notExists(program)) copyResource("BossKey-Minecraft-Java.exe", program);
            if (Files.notExists(config)) copyResource("BossKey.ini", config);
            new ProcessBuilder(program.toString(), "--pid=" + pid)
                    .directory(folder.toFile()).start();
        } catch (Exception error) {
            System.err.println("Boss Key Forge could not start: " + error);
        }
    }

    private static void copyResource(String name, Path destination) throws IOException {
        InputStream input = ForgeBossKey.class.getResourceAsStream("/bosskey/" + name);
        if (input == null) throw new IOException("Missing resource: " + name);
        try {
            Files.copy(input, destination);
        } finally {
            input.close();
        }
    }
}
