# SPT-Skipper

A BepInEx plugin for SPT-AKI that allows you to skip quest objectives in-game.

## ⚠️ Important Disclaimer

**This is a port of an existing mod to SPT4.** This project is based on the original Skipper mod. I created this port to make it compatible with SPT version 4.x. 

If the original author updates their mod to support SPT4, I will gladly take down this repository. This port is intended as a temporary solution for the community until official support is available.

---

## Features

- Skip quest objectives with a single button click
- Configurable hotkey to show/hide skip buttons
- Optional confirmation dialog before skipping
- Always-visible mode or hotkey-triggered mode

## How to Install

1. Download the latest release from the releases page
2. Extract the zip file contents into your root SPT-AKI folder (where `SPT.Server.exe` and `SPT.Launcher.exe` are located)
3. Your `BepInEx/plugins` folder should now contain a `SkipQuest.dll` file inside
4. Launch the game and configure the mod settings in `BepInEx/config/com.zas.questskipper.cfg` if needed

## Configuration

After running the game once, a configuration file will be created at `BepInEx/config/com.zas.questskipper.cfg`. You can adjust:

- **Enabled**: Global mod toggle
- **Always display Skip button**: If enabled, skip buttons are always visible
- **Display hotkey**: The key to hold to show skip buttons (default: Left Control)
- **Show confirmation button**: If enabled, a confirmation dialog will appear before skipping

## How to Build from Source

1. Download/clone this repository
2. Place the following dependencies in the parent directory of this project:
   - `EscapeFromTarkov_Data/Managed/` - All managed DLL files from your SPT installation
   - `BepInEx/core/` - BepInEx core files (0Harmony.dll, BepInEx.dll)
   - `BepInEx/plugins/spt/` - SPT plugin files (spt-common.dll, spt-reflection.dll)
3. Open the solution in Visual Studio or your preferred IDE
4. Build the project in Debug or Release configuration
5. The compiled `SkipQuest.dll` file will be in the `bin/Debug/netstandard2.1` or `bin/Release/netstandard2.1` folder
6. Copy the DLL to your `BepInEx/plugins` folder

**Note:** The project is configured to automatically copy the built DLL to `BepInEx/plugins` if the directories are set up as described above.

## Credits

Original Skipper mod concept and implementation - credit to the original author, TerkoizLT.
SPT4 port - This repository