# RumbleModUIPlus
![Photo](https://raw.githubusercontent.com/ninjaguardian/RumbleModUIPlus/master/icon.png)

[![Github](https://cdn.jsdelivr.net/npm/@intergrav/devins-badges@3.2.0/assets/cozy/available/github_vector.svg)](https://github.com/ninjaguardian/RumbleModUIPlus)
[![Changelog](https://cdn.jsdelivr.net/npm/@intergrav/devins-badges@3.2.0/assets/cozy/documentation/changelog_vector.svg)](https://thunderstore.io/c/rumble/p/ninjaguardian/RumbleModUIPlus/changelog)
[![Thunderstore](https://cdn.jsdelivr.net/npm/@intergrav/devins-badges@3.2.0/assets/cozy/documentation/website_vector.svg)](https://thunderstore.io/c/rumble/p/ninjaguardian/RumbleModUIPlus)

## What is this?
Adds stuff for devs to RumbleModUI.

## Instructions
1. Install [MelonLoader](https://github.com/LavaGang/MelonLoader)
2. Run RUMBLE without mods
3. Drop Mods from .zip into RUMBLE's installation folder
4. Install [RumbleModUI](https://thunderstore.io/c/rumble/p/Baumritter/RumbleModUI)
5. Play RUMBLE!

## For devs
<details>
  <summary>v1.0.0</summary>
  
  As of v1.0.0, this mod only allows you to change your Settings.txt to use a ModFormatVersion instead of ModVersion. This makes it so the user's settings are not deleted every update. To do this, refrence RumbleModUIPlus.dll and replace your call to `new RumbleModUI.Mod` with `new RumbleModUIPlus.Mod`. Next, where you specify ModVersion, also specify ModFormatVersion (i.e. `mod.ModFormatVersion = "1.0.0"`).
</details>

## Help And Other Resources
Get help and find other resources in the [Modding Discord](https://discord.gg/fsbcnZgzfa)

[![CC0-1.0 License](https://img.shields.io/badge/License-CC0_1.0_Universal-green.svg)](https://github.com/ninjaguardian/RumbleModUIPlus?tab=CC0-1.0-1-ov-file)