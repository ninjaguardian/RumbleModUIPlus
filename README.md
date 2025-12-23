# RumbleModUIPlus
![Photo](https://raw.githubusercontent.com/ninjaguardian/RumbleModUIPlus/master/icon.png)

[![Github](https://cdn.jsdelivr.net/npm/@intergrav/devins-badges@3.2.0/assets/cozy/available/github_vector.svg)](https://github.com/ninjaguardian/RumbleModUIPlus)
[![Changelog](https://cdn.jsdelivr.net/npm/@intergrav/devins-badges@3.2.0/assets/cozy/documentation/changelog_vector.svg)](https://thunderstore.io/c/rumble/p/ninjaguardian/RumbleModUIPlus/changelog)
[![Thunderstore](https://cdn.jsdelivr.net/npm/@intergrav/devins-badges@3.2.0/assets/cozy/documentation/website_vector.svg)](https://thunderstore.io/c/rumble/p/ninjaguardian/RumbleModUIPlus)

## What is this?
Adds stuff for devs to RumbleModUI (also some bug fixes).
One example is the ability to add folders to your mod settings!

## Instructions
1. Install [MelonLoader](https://github.com/LavaGang/MelonLoader)
2. Run RUMBLE without mods
3. Drop Mods from .zip into RUMBLE's installation folder
4. Install [RumbleModUI](https://thunderstore.io/c/rumble/p/Baumritter/RumbleModUI)
5. Play RUMBLE!

## Bug fixes and tweaks
<details>
  <summary>v2.0.0</summary>

  - All of `RumbleModUI.Mod`'s calls to `MelonLogger.Msg` are replaced with `MelonLogger.Warning` for visibility (because all warnings and errors were previously just sent as normal messages).
  - `RumbleModUI.Mod.GetFromFile` optimized with some issues fixed.
    - No longer loads stuff that is marked as `DoNotSave`.
    - Fixed the system that finds which setting is on a line.
    - (Note for devs) This uses a prefix that returns false. This may mess with other patches.
</details>

## For devs
<details>
  <summary>v2.0.0</summary>
  
  - You can change your Settings.txt to use a `ModFormatVersion` instead of `ModVersion`. This makes it so that the user's settings are not deleted every update. To do this, reference RumbleModUIPlus.dll and replace your call to `new RumbleModUI.Mod` with `new RumbleModUIPlus.Mod`. Next, where you specify `ModVersion`, also specify `ModFormatVersion` (i.e. `mod.ModFormatVersion = "1.0.0"`). Changing the `ModFormatVersion` will act like `ModVersion` did and delete the user's settings (will be changed eventually).
  - `Mod.AddToListAtStart`, `Mod.AddToListAtIndex`, `Mod.AddDescriptionAtStart`, and `Mod.AddDescriptionAtIndex` are available.
  - You can add folders with `Mod.AddFolder`. The order in the UI is the order in which they are stored **in the mod**. This means `AddToListAtStart` and `AddToListAtIndex` can still be used to set order.
      - Add an item to the folder with `ModSettingFolder.AddSetting`. For example:
      ```c#
      Mod mod = new() {...};
      mod.AddFolder("name", "optional description")
        .AddSetting(mod.AddToList(...)) // Chaining is supported
        .AddSetting(mod.AddToList(...)) // You can also just save the result of AddFolder to a variable
        .AddSetting(mod.AddToListAtStart(...)) // This will be second in the folder
        .AddSetting(mod.AddToListAtStart(...)); // This will be first in the folder
      ```
      - Subfolders work :>
      - Settings in different folders **cannot** have the same name (will be changed eventually).
  - `ModSettingFolder.RemoveSetting`, `ModSettingFolder.RemoveAllSettings`, and `ModSettingFolder.RemoveFolder` are available.
    - `RemoveSettingC(ModSetting)` is `RemoveSetting` with chaining.
    - `RemoveSettingC(ModSetting, out bool)` is `RemoveSetting` with chaining, and the result of `RemoveSetting` is sent in `out bool`.
  - You can use `RumbleModUIPlus.RumbleModUIPlusClass.GetSelectedMod()` to get the currently selected mod in the UI instance.
  - You can use `RumbleModUIPlus.RumbleModUIPlusClass.GetSelectedModSetting()` to get the currently selected setting of the currently selected mod in the UI instance.
  - `RumbleModUIPlus.Tags` is the same as `RumbleModUI.Tags`, but it contains `bool InFolder` (you do not need to manually change this).
</details>
<details>
  <summary>v1.0.2</summary>
  
  - You can change your Settings.txt to use a ModFormatVersion instead of ModVersion. This makes it so that the user's settings are not deleted every update. To do this, reference RumbleModUIPlus.dll and replace your call to `new RumbleModUI.Mod` with `new RumbleModUIPlus.Mod`. Next, where you specify ModVersion, also specify ModFormatVersion (i.e. `mod.ModFormatVersion = "1.0.0"`).
  - `AddToListAtStart`, `AddToListAtIndex`, `AddDescriptionAtStart`, and `AddDescriptionAtIndex` are available.
</details>
<details>
  <summary>v1.0.0-v1.0.1</summary>
  
  - In these versions, this mod only allows you to change your Settings.txt to use a ModFormatVersion instead of ModVersion. This makes it so that the user's settings are not deleted every update. To do this, reference RumbleModUIPlus.dll and replace your call to `new RumbleModUI.Mod` with `new RumbleModUIPlus.Mod`. Next, where you specify ModVersion, also specify ModFormatVersion (i.e. `mod.ModFormatVersion = "1.0.0"`).
</details>

## Help And Other Resources
Get help and find other resources in the [Modding Discord](https://discord.gg/fsbcnZgzfa)

[![CC0-1.0 License](https://img.shields.io/badge/License-CC0_1.0_Universal-green.svg)](https://github.com/ninjaguardian/RumbleModUIPlus?tab=CC0-1.0-1-ov-file)