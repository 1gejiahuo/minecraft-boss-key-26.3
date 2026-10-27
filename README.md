# Minecraft Boss Key：Java 版 1.0 起

老板或家长突然走到电脑旁？**Ctrl+Shift+C 一键收起 Minecraft**：游戏画面从桌面和任务栏消失，声音同步安静；等查岗结束，再按一次就能回到游戏，不必退出世界或重新进服务器。快捷键是 Windows 全局热键，游戏窗口隐藏后依然可以触发。

项目提供 **Windows 通用独立程序**、**Fabric 通用入口**、**26.3 原生 Fabric Mod**，以及 Forge、NeoForge 的跨版本实验入口。独立程序面向 Windows 上 Java 版 1.0 起的正式版及愚人节版。各加载器入口内置并启动同一 Windows 程序，通过游戏进程 ID 精确识别窗口。任选一种即可，不能同时运行，因为它们会争用同一个全局快捷键。

> 目前为预览版：代码已编译，但尚未逐一在所有正式版和愚人节版中实机验证。不同启动器、Java 运行时和全屏方式可能需要调整配置；见下方兼容说明。

## 查岗时会发生什么

1. **有人靠近：**按 Ctrl+Shift+C，立刻隐藏 Minecraft 窗口，并处理游戏声音。
2. **查看任务栏：**被隐藏的游戏窗口不再显示在任务栏；独立程序本身也没有任务栏或托盘图标。
3. **人走之后：**再按 Ctrl+Shift+C，重新显示游戏窗口并恢复声音；多人连接保持原状。

这不是系统级“隐身”：任务管理器仍能看到进程，启动器窗口也不会自动隐藏；游戏世界、服务器连接及后台活动仍会继续。

## 选择版本

| 版本 | 安装方式 | 隐藏时的声音 | 适用范围 |
| --- | --- | --- | --- |
| Windows 独立程序 | 运行 `BossKey-Minecraft-Java.exe`；无需 Mod | 将游戏音频会话静音，恢复时还原原状态 | Windows + Java 版 1.0 起正式版及愚人节版；识别游戏所在的 Java 窗口 |
| Fabric 通用入口 | 将 `boss-key-fabric-universal-0.3.0-preview.jar` 放入 `mods` | 内置的 Windows 程序静音游戏音频会话 | Fabric 官方支持的 Java 版 1.14 起正式版，以及可安装 Fabric 的愚人节版；Windows 64 位 + .NET Framework 4.8 |
| 26.3 原生 Fabric Mod | 将 `boss-key-26.3-0.1.0.jar` 放入 `mods` | 暂停游戏音效，恢复时继续 | Minecraft Java 26.3 + Fabric Loader + Fabric API + Java 25 |
| NeoForge 26.3 | 将 `boss-key-neoforge-26.3-0.4.0-preview.jar` 放入 `mods` | 内置的 Windows 程序静音游戏音频会话 | Minecraft Java 26.3 + NeoForge 26.3.0.99 起 + Windows 64 位 + .NET Framework 4.8 |
| Forge 1.12.2 | 将 `boss-key-forge-1.12.2-0.4.0-preview.jar` 放入 `mods` | 内置的 Windows 程序静音游戏音频会话 | Minecraft Java 1.12.2 + Forge + Windows 64 位 + .NET Framework 4.8 |
| Forge 1.20.1 | 将 `boss-key-forge-1.20.1-0.5.0-preview.jar` 放入 `mods` | 内置的 Windows 程序静音游戏音频会话 | Minecraft Java 1.20.1 + Forge 47.x + Windows 64 位 + .NET Framework 4.8 |
| NeoForge 1.21.1 | 将 `boss-key-neoforge-1.21.1-0.5.0-preview.jar` 放入 `mods` | 内置的 Windows 程序静音游戏音频会话 | Minecraft Java 1.21.1 + NeoForge 21.1.x + Windows 64 位 + .NET Framework 4.8 |
| Forge 旧版实验入口 | `boss-key-forge-1.7-0.6.0-preview.jar` 或 `boss-key-forge-legacy-0.6.0-preview.jar` | 同上 | 分别面向 Forge 1.7.x、Forge 1.8–1.12；实际支持取决于该游戏版本是否有 Forge |
| Forge 新版实验入口 | `boss-key-forge-modern-0.6.0-preview.jar` | 同上 | 面向 Forge 1.13+；需对应游戏版本的 Forge |
| NeoForge 实验入口 | `boss-key-neoforge-early-0.6.0-preview.jar` 或 `boss-key-neoforge-modern-0.6.0-preview.jar` | 同上 | 分别面向 NeoForge 1.20.2–1.20.4、1.20.5+；需对应游戏版本的 NeoForge |

这些版本都不创建托盘图标。隐藏游戏窗口后，任务栏中不应再显示该游戏窗口；程序或游戏进程仍在任务管理器中运行。隐藏不会暂停多人服务器，也不会断开连接。

## 下载与使用

从本仓库的 [downloads](downloads/) 目录下载所需文件。整合包 `minecraft-boss-key-java-1.0-plus-preview.zip` 内含所有程序文件、快捷键配置和说明。每次只选择一个适合当前加载器的 JAR；不用加载器时使用 `BossKey-Minecraft-Java.exe` 与 `BossKey.ini`。

### Windows 独立程序

1. 启动 Minecraft。
2. 运行 `BossKey-Minecraft-Java.exe`。程序本身没有控制台窗口、任务栏按钮或托盘图标。
3. 按默认 **Ctrl+Shift+C** 隐藏游戏，再按一次恢复。
4. 按 **Ctrl+Shift+X** 退出独立程序；退出前会尝试恢复游戏窗口和声音。

若热键被其他软件占用，程序会提示并退出。错误日志位于 `%LOCALAPPDATA%\MinecraftBossKey\bosskey.log`。这个程序需要 Windows 的 .NET Framework 4.8。默认会识别当前前台的 `java.exe` 或 `javaw.exe` 游戏窗口；即使愚人节版窗口标题不是 Minecraft，也能通过前台窗口操作。若游戏不在前台，则按窗口标题里的 `Minecraft` 查找。

### Fabric 通用入口

1. 在支持的游戏版本中安装 Fabric Loader。
2. 将 `boss-key-fabric-universal-0.3.0-preview.jar` 放入实例的 `mods` 文件夹。无需 Fabric API。
3. 启动游戏。Mod 会把内置 Windows 程序释放到实例的 `config/bosskey-universal-0.3.0/`，并传入游戏进程 ID 启动它。
4. 按 **Ctrl+Shift+C** 隐藏或恢复。关闭游戏后，辅助程序会检测进程退出并自行结束。

Fabric 官方支持的正式版从 1.14 起；1.0–1.13 请使用 Windows 独立程序。愚人节版需要该版本本身能安装 Fabric Loader。通用入口目前只在 26.3 构建环境下完成编译，尚未逐版本实机验证。

Quilt Loader 通常可以加载 Fabric Mod；可以尝试同一个 Fabric 通用 JAR，但本项目尚未在 Quilt 游戏实例中实测。

### Forge 和 NeoForge

将对应游戏版本的 JAR 放入实例的 `mods` 文件夹。启动后，入口会把内置程序释放到 `config/bosskey-neoforge-0.4.0/`、`config/bosskey-forge-0.4.0/`、`config/bosskey-neoforge-0.5.0/` 或 `config/bosskey-forge-0.5.0/`，随后可用 **Ctrl+Shift+C** 隐藏或恢复。另有 0.6.0 跨版本实验入口，见上表和 [兼容性说明](COMPATIBILITY.md)。这些入口使用官方加载器依赖编译并检查 JAR 内容，尚未逐版本进游戏验证。无法安装对应加载器的版本请使用 Windows 独立程序。

### 26.3 原生 Fabric Mod

1. 安装与 Minecraft Java 26.3 对应的 Fabric Loader 和 Fabric API。
2. 将 `boss-key-26.3-0.1.0.jar` 放入该游戏实例的 `mods` 文件夹。
3. 进入游戏后按 **Ctrl+Shift+C** 隐藏，再按一次恢复。

Mod 仅安装在客户端；服务器无需安装。需要 Java 25。

## 修改快捷键

用记事本修改配置文件，保存后**重启独立程序或 Minecraft** 即可生效。无需重新编译。

- **Windows 独立程序：**编辑与 `BossKey-Minecraft-Java.exe` 同目录的 `BossKey.ini`。下载目录已附此文件；如果缺失，首次运行会尝试自动生成。
- **Fabric 通用入口：**编辑游戏实例目录里的 `config/bosskey-universal-0.3.0/BossKey.ini`。
- **NeoForge / Forge 适配入口：**编辑实例目录 `config/bosskey-neoforge-0.4.0/`、`config/bosskey-forge-0.4.0/`、`config/bosskey-neoforge-0.5.0/` 、`config/bosskey-forge-0.5.0/` 或对应 0.6.0 入口生成的 `config/bosskey-forge-1.7-0.6.0/`、`config/bosskey-forge-legacy-0.6.0/`、`config/bosskey-forge-modern-0.6.0/`、`config/bosskey-neoforge-early-0.6.0/`、`config/bosskey-neoforge-modern-0.6.0/` 中的 `BossKey.ini`。
- **26.3 原生 Fabric Mod：**首次启动后编辑游戏实例目录里的 `config/bosskey.properties`。

独立版示例：

```ini
hotkey=CTRL+ALT+H
exitHotkey=CTRL+SHIFT+X
processNames=java,javaw
titleKeywords=Minecraft
allowFocusedProcess=true
```

Mod 示例：

```properties
hotkey=CTRL+ALT+H
```

可用 `CTRL`、`SHIFT`、`ALT` 搭配一个字母、数字或 `F1`–`F11`；至少要有一个修饰键。独立版的隐藏键和退出键不能相同，也不要选择已被其他程序占用的组合。

### 旧版、愚人节版或第三方启动器识别不到窗口

优先切到游戏窗口再按隐藏键。`allowFocusedProcess=true` 允许隐藏当前前台的 Java 窗口，不依赖窗口标题；因此仅在游戏位于前台时按键。若启动器使用自定义进程名，将该名称加入 `processNames`，用英文逗号分隔。若窗口标题里不含 Minecraft，且希望游戏不在前台时也能找到它，将标题中稳定的关键词加入 `titleKeywords`。修改后重启程序。可以把 `allowFocusedProcess` 设为 `false`，要求标题必须匹配关键词。

示例：`processNames=java,javaw,customjava` 与 `titleKeywords=Minecraft,Minecraft 2.0`。不要填宽泛的关键词，否则可能匹配其他 Java 窗口。

## 构建

### Fabric 通用入口

在 `fabric-universal` 目录使用 JDK 25 运行 `./gradlew.bat build`。源码编译为 Java 8 字节码，但构建工具使用 26.3 开发环境。产物在 `fabric-universal/build/libs/`。

### 26.3 原生 Fabric Mod

在 `fabric-mod` 目录使用 JDK 25 运行：

```powershell
.\gradlew.bat build
```

构建产物在 `fabric-mod/build/libs/`。

### NeoForge / Forge

在相应目录运行 `build.ps1`。NeoForge 26.3 需要 JDK 25；NeoForge 1.21.1 生成 Java 21 字节码；Forge 1.20.1 生成 Java 17 字节码；Forge 1.12.2 生成 Java 8 字节码。脚本会下载相应加载器的官方编译依赖，产物在各自的 `out/` 目录。

### Windows 独立程序

在 `windows-app` 目录运行：

```powershell
.\build.ps1
```

需要 Windows 的 .NET Framework 4.8 C# 编译器，无需 NuGet 包。产物为 `windows-app/BossKey-Minecraft-Java.exe`。

## 已知限制

- 独立程序通过前台 Java 窗口，或进程名加窗口标题查找游戏。自定义 Java 启动器可通过 `BossKey.ini` 调整识别规则。
- Fabric 通用入口依赖内置 Windows 程序与 .NET Framework 4.8；它不是对所有 Minecraft 版本分别编译的原生游戏钩子。旧版本及愚人节版是否能安装 Fabric Loader，取决于该版本的加载器支持。
- 这是一套跨版本的窗口操作方案，不代表每个正式版和愚人节版都已经实机测试。部分特殊启动方式或全屏模式可能无法隐藏、恢复或静音。
- 目前没有可视化设置界面，快捷键通过文本配置修改。
- 独占全屏模式尚未实测；首次使用建议先在窗口化或无边框全屏模式验证。
- 如果独立程序异常退出，Windows 可能保留游戏音频会话的静音状态，可在音量混合器中手动解除。
- 隐藏不等于退出，世界和多人连接会继续运行。

## 源码

- `fabric-mod/`：Fabric 26.3 客户端 Mod。
- `fabric-universal/`：Java 8 字节码的 Fabric 通用入口及内置 Windows 程序。
- `neoforge-26.3/`、`neoforge-1.21.1/`、`forge-1.12.2/` 与 `forge-1.20.1/`：加载器适配入口。
- `windows-app/`：面向 Java 版 1.0 起正式版及愚人节版的 Windows 后台程序。

基于 [Fabric 官方示例项目](https://github.com/FabricMC/fabric-example-mod)搭建 Mod 构建结构。欢迎通过 Issues 提交 26.3 实机测试结果和问题复现步骤。

## 许可

MIT License，见 [LICENSE](LICENSE)。

---

## English

A boss key for surprise workplace or parent check-ins: press Ctrl+Shift+C to hide the Minecraft window, remove its taskbar button, and quiet the game; press it again to return. The Windows app targets Java Edition 1.0 onward. The universal Fabric entrypoint bundles that Windows app and targets Fabric-supported versions from 1.14 onward, including April Fools builds when a loader is available. Quilt may load that Fabric jar. Separate adapters have been built for Forge 1.12.2 and 1.20.1, and NeoForge 1.21.1 and 26.3; the native Fabric mod targets 26.3. Edit `BossKey.ini` or `config/bosskey.properties` and restart to change shortcuts. Game runtime compatibility has not been verified across these versions.


