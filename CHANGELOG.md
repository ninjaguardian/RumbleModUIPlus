# v2.0.0
- All of RumbleModUI.Mod's calls to MelonLogger.Msg are replaced with MelonLogger.Warning
- `RumbleModUI.Mod.GetFromFile` optimized with some issues fixed.
    - No longer loads stuff that is marked as `DoNotSave`.
    - Fixed the system that finds which setting is on a line.
- Added the folder system
- Added Tags class
- Replaced the old ModFormatVersion system with a prefix and transpiler
- Added public helper methods

# v1.0.2
- Add AddToListAtStart, AddToListAtIndex, AddDescriptionAtStart, and AddDescriptionAtIndex.

# v1.0.1
- Fix windows only warning when used as a reference.

# v1.0.0
- Initial release
