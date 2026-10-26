# Minecraft Boss Key 26.3

老板或家长突然走到电脑旁？**Ctrl+Shift+C 一键收起 Minecraft**：游戏画面从桌面和任务栏消失，声音同步安静；等查岗结束，再按一次就能回到游戏，不必退出世界或重新进服务器。快捷键是 Windows 全局热键，游戏窗口隐藏后依然可以触发。

项目提供 **Windows 独立程序** 与 **Fabric 客户端 Mod** 两种实现；任选一种即可，不能同时运行，因为它们会争用同一个全局快捷键。

> 目前为预览版：两种实现均已编译，但尚未在真实 Minecraft 26.3 游戏窗口中完成隐藏、恢复和独占全屏测试。

## 查岗时会发生什么

1. **有人靠近：**按 Ctrl+Shift+C，立刻隐藏 Minecraft 窗口，并处理游戏声音。
2. **查看任务栏：**被隐藏的游戏窗口不再显示在任务栏；独立程序本身也没有任务栏或托盘图标。
3. **人走之后：**再按 Ctrl+Shift+C，重新显示游戏窗口并恢复声音；多人连接保持原状。

这不是系统级“隐身”：任务管理器仍能看到进程，启动器窗口也不会自动隐藏；游戏世界、服务器连接及后台活动仍会继续。

## 选择版本

| 版本 | 安装方式 | 隐藏时的声音 | 适用范围 |
| --- | --- | --- | --- |
| Windows 独立程序 | 运行 `BossKey-26.3.exe`；无需 Mod | 将 Minecraft 音频会话静音，恢复时还原原状态 | Windows + Minecraft Java 26.3；识别标题含 Minecraft 的 java/javaw 窗口 |
| Fabric Mod | 将 `boss-key-26.3-0.1.0.jar` 放入 `mods` | 暂停游戏音效，恢复时继续 | Minecraft Java 26.3 + Fabric Loader + Fabric API + Java 25 |

两个版本都不创建托盘图标。隐藏游戏窗口后，任务栏中不应再显示该游戏窗口；程序或游戏进程仍在任务管理器中运行。隐藏不会暂停多人服务器，也不会断开连接。

## 下载与使用

从本仓库的 **Releases** 下载所需文件。

### Windows 独立程序

1. 启动 Minecraft。
2. 运行 `BossKey-26.3.exe`。程序本身没有控制台窗口、任务栏按钮或托盘图标。
3. 按默认 **Ctrl+Shift+C** 隐藏游戏，再按一次恢复。
4. 按 **Ctrl+Shift+X** 退出独立程序；退出前会尝试恢复游戏窗口和声音。

若热键被其他软件占用，程序会提示并退出。错误日志位于 `%LOCALAPPDATA%\MinecraftBossKey\bosskey.log`。这个程序需要 Windows 的 .NET Framework 4.8。

### Fabric Mod

1. 安装与 Minecraft Java 26.3 对应的 Fabric Loader 和 Fabric API。
2. 将 `boss-key-26.3-0.1.0.jar` 放入该游戏实例的 `mods` 文件夹。
3. 进入游戏后按 **Ctrl+Shift+C** 隐藏，再按一次恢复。

Mod 仅安装在客户端；服务器无需安装。需要 Java 25。

## 修改快捷键

用记事本修改配置文件，保存后**重启独立程序或 Minecraft** 即可生效。无需重新编译。

- **Windows 独立程序：**编辑与 `BossKey-26.3.exe` 同目录的 `BossKey.ini`。下载包已附此文件；如果缺失，首次运行会尝试自动生成。
- **Fabric Mod：**首次启动后编辑游戏实例目录里的 `config/bosskey.properties`。

独立版示例：

```ini
hotkey=CTRL+ALT+H
exitHotkey=CTRL+SHIFT+X
```

Mod 示例：

```properties
hotkey=CTRL+ALT+H
```

可用 `CTRL`、`SHIFT`、`ALT` 搭配一个字母、数字或 `F1`–`F11`；至少要有一个修饰键。独立版的隐藏键和退出键不能相同，也不要选择已被其他程序占用的组合。

## 构建

### Fabric Mod

在 `fabric-mod` 目录使用 JDK 25 运行：

```powershell
.\gradlew.bat build
```

构建产物在 `fabric-mod/build/libs/`。

### Windows 独立程序

在 `windows-app` 目录运行：

```powershell
.\build.ps1
```

需要 Windows 的 .NET Framework 4.8 C# 编译器，无需 NuGet 包。产物为 `windows-app/BossKey-26.3.exe`。

## 已知限制

- 独立程序通过窗口标题与进程名查找游戏；若启动器或 Mod 修改了标题，可能无法识别。
- 目前没有可视化设置界面，快捷键通过文本配置修改。
- 独占全屏模式尚未实测；首次使用建议先在窗口化或无边框全屏模式验证。
- 如果独立程序异常退出，Windows 可能保留游戏音频会话的静音状态，可在音量混合器中手动解除。
- 隐藏不等于退出，世界和多人连接会继续运行。

## 源码

- `fabric-mod/`：Fabric 26.3 客户端 Mod。
- `windows-app/`：不依赖 Minecraft Mod 的 Windows 后台程序。

基于 [Fabric 官方示例项目](https://github.com/FabricMC/fabric-example-mod)搭建 Mod 构建结构。欢迎通过 Issues 提交 26.3 实机测试结果和问题复现步骤。

## 许可

MIT License，见 [LICENSE](LICENSE)。

---

## English

A boss key for surprise workplace or parent check-ins: press Ctrl+Shift+C to hide the Minecraft window, remove its taskbar button, and quiet the game; press it again to return. Choose either the standalone Windows application or the Fabric client mod. **Ctrl+Shift+C** hides and restores the game window. The standalone app uses **Ctrl+Shift+X** to exit safely. Edit `BossKey.ini` for the standalone app or `config/bosskey.properties` for the mod, then restart to change the shortcut. Both builds compile, but in-game behavior, especially exclusive fullscreen, has not yet been verified. See the Chinese sections above for installation, build commands, and limitations.



