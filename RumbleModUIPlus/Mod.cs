using HarmonyLib;
using Il2CppTMPro;
using MelonLoader;
using RumbleModUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using static RumbleModUI.ModSetting;

#region Assemblies
[assembly: MelonInfo(typeof(RumbleModUIPlus.RumbleModUIPlusClass), RumbleModUIPlus.BuildInfo.ModName, RumbleModUIPlus.BuildInfo.ModVersion, RumbleModUIPlus.BuildInfo.Author, RumbleModUIPlus.BuildInfo.DownloadLink)]
[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]

[assembly: MelonColor(255, 0, 160, 230)]
[assembly: MelonAuthorColor(255, 0, 160, 230)]

[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]
[assembly: VerifyLoaderVersion(RumbleModUIPlus.BuildInfo.MLVersion, true)]
#endregion

namespace RumbleModUIPlus
{
    /// <summary>
    /// Info for RumbleModUIPlus.
    /// </summary>
    public static class BuildInfo
    {
        /// <summary>
        /// Mod name.
        /// </summary>
        public const string ModName = "RumbleModUIPlus";
        /// <summary>
        /// Mod version.
        /// </summary>
        public const string ModVersion = "1.0.2";
        /// <summary>
        /// MelonLoader version.
        /// </summary>
        public const string MLVersion = "0.7.0";
        /// <summary>
        /// Mod author.
        /// </summary>
        public const string Author = "ninjaguardian";
        /// <summary>
        /// Mod download link.
        /// </summary>
        public const string DownloadLink = "https://thunderstore.io/c/rumble/p/ninjaguardian/RumbleModUIPlus";
    }

    /// <summary>
    /// Read <see href="https://github.com/ninjaguardian/rumblemoduiplus?tab=readme-ov-file#for-devs"/>
    /// </summary>
    public class Mod : RumbleModUI.Mod
    {
        #region Vars
        /// <summary>
        /// The Duplicate Error Message.
        /// </summary>
        protected const string DuplicateErrorMsg = "AddToList failed: Name not unique";

        private string _modFormatVersion;
        /// <summary>
        /// The format version of the mod's settings.
        /// </summary>
        public string ModFormatVersion
        {
            get => _modFormatVersion ?? ModVersion;
            set => _modFormatVersion = value;
        }
        #endregion

        #region AddToListAtStart & AddToListAtIndex
        /// <summary>
        /// Creates an instance of ModSetting with the type string. <br/>
        /// The value cannot be modified by the user. <br/>
        /// Unlike AddDescription, this adds it at the top.
        /// </summary>
        public ModSetting<string> AddDescriptionAtStart(string Name, string Value, string Description, RumbleModUI.Tags tags) => AddDescriptionAtIndex(Name, Value, Description, tags, 0);

        /// <summary>
        /// Creates an instance of ModSetting with the type string. <br/>
        /// The value cannot be modified by the user. <br/>
        /// Unlike AddDescription, this lets you specify the index.
        /// </summary>
        public ModSetting<string> AddDescriptionAtIndex(string Name, string Value, string Description, RumbleModUI.Tags tags, int Index)
        {
            if (Settings.Count > 0 && Settings.Exists(x => x.Name == Name))
            {
                MelonLogger.Msg(DuplicateErrorMsg + ": " + Name);
                return null;
            }

            ModSetting<string> InputSetting = new()
            {
                Name = Name,
                Description = Description,
                Value = Value,
                SavedValue = Value,
                LinkGroup = 0,
                ValueType = AvailableTypes.Description,
            };

            tags.DoNotSave = true;

            AddTags(InputSetting, tags);

            if (Index < 0 || Index > Settings.Count)
            {
                MelonLogger.Error($"Index {Index} is out of bounds for list with size {Settings.Count}. Falling back to Settings.Add");
                Settings.Add(InputSetting);
            }
            else
                Settings.Insert(Index, InputSetting);
            return InputSetting;
        }

        /// <summary>
        /// Creates an instance of ModSetting with the type string.
        /// <br/>Unlike AddToList, this adds it at the top.
        /// </summary>
        public ModSetting<string> AddToListAtStart(string Name, string Value, string Description, RumbleModUI.Tags tags) => AddToListAtIndex(Name, Value, Description, tags, 0);

        /// <summary>
        /// Creates an instance of ModSetting with the type string.
        /// <br/>Unlike AddToList, this lets you specify the index.
        /// </summary>
        public ModSetting<string> AddToListAtIndex(string Name, string Value, string Description, RumbleModUI.Tags tags, int Index)
        {
            if (Settings.Count > 0 && Settings.Exists(x => x.Name == Name))
            {
                MelonLogger.Msg(DuplicateErrorMsg + ": " + Name);
                return null;
            }

            ModSetting<string> InputSetting = new()
            {
                Name = Name,
                Description = Description,
                Value = Value,
                SavedValue = Value,
                LinkGroup = 0,
                ValueType = AvailableTypes.String
            };

            AddTags(InputSetting, tags);

            if (Index < 0 || Index > Settings.Count)
            {
                MelonLogger.Error($"Index {Index} is out of bounds for list with size {Settings.Count}. Falling back to Settings.Add");
                Settings.Add(InputSetting);
            }
            else
                Settings.Insert(Index, InputSetting);
            return InputSetting;
        }

        /// <summary>
        /// Creates an instance of ModSetting with the type bool.
        /// <br/>Unlike AddToList, this adds it at the top.
        /// </summary>
        public ModSetting<bool> AddToListAtStart(string Name, bool Value, int LinkGroup, string Description, RumbleModUI.Tags tags) => AddToListAtIndex(Name, Value, LinkGroup, Description, tags, 0);

        /// <summary>
        /// Creates an instance of ModSetting with the type bool.
        /// <br/>Unlike AddToList, this lets you specify the index.
        /// </summary>
        public ModSetting<bool> AddToListAtIndex(string Name, bool Value, int LinkGroup, string Description, RumbleModUI.Tags tags, int Index)
        {
            if (Settings.Count > 0 && Settings.Exists(x => x.Name == Name))
            {
                MelonLogger.Msg(DuplicateErrorMsg + ": " + Name);
                return null;
            }
            ModSetting<bool> InputSetting = new()
            {
                Name = Name,
                Description = Description,
                Value = Value,
                SavedValue = Value,
                LinkGroup = LinkGroup,
                ValueType = AvailableTypes.Boolean
            };
            if (LinkGroup != 0)
            {
                SetLinkGroup(LinkGroup);
                LinkGroups.Find(x => x.Index == LinkGroup).Settings.Add(InputSetting);
            }

            AddTags(InputSetting, tags);

            if (Index < 0 || Index > Settings.Count)
            {
                MelonLogger.Error($"Index {Index} is out of bounds for list with size {Settings.Count}. Falling back to Settings.Add");
                Settings.Add(InputSetting);
            }
            else
                Settings.Insert(Index, InputSetting);
            return InputSetting;
        }

        /// <summary>
        /// Creates an instance of ModSetting with the type int.
        /// <br/>Unlike AddToList, this adds it at the top.
        /// </summary>
        public ModSetting<int> AddToListAtStart(string Name, int Value, string Description, RumbleModUI.Tags tags) => AddToListAtIndex(Name, Value, Description, tags, 0);

        /// <summary>
        /// Creates an instance of ModSetting with the type int.
        /// <br/>Unlike AddToList, this lets you specify the index.
        /// </summary>
        public ModSetting<int> AddToListAtIndex(string Name, int Value, string Description, RumbleModUI.Tags tags, int Index)
        {
            if (Settings.Count > 0 && Settings.Exists(x => x.Name == Name))
            {
                MelonLogger.Msg(DuplicateErrorMsg + ": " + Name);
                return null;
            }
            ModSetting<int> InputSetting = new()
            {
                Name = Name,
                Description = Description,
                Value = Value,
                SavedValue = Value,
                LinkGroup = 0,
                ValueType = AvailableTypes.Integer
            };

            AddTags(InputSetting, tags);

            if (Index < 0 || Index > Settings.Count)
            {
                MelonLogger.Error($"Index {Index} is out of bounds for list with size {Settings.Count}. Falling back to Settings.Add");
                Settings.Add(InputSetting);
            }
            else
                Settings.Insert(Index, InputSetting);
            return InputSetting;
        }

        /// <summary>
        /// Creates an instance of ModSetting with the type float.
        /// <br/>Unlike AddToList, this adds it at the top.
        /// </summary>
        public ModSetting<float> AddToListAtStart(string Name, float Value, string Description, RumbleModUI.Tags tags) => AddToListAtIndex(Name, Value, Description, tags, 0);

        /// <summary>
        /// Creates an instance of ModSetting with the type float.
        /// <br/>Unlike AddToList, this lets you specify the index.
        /// </summary>
        public ModSetting<float> AddToListAtIndex(string Name, float Value, string Description, RumbleModUI.Tags tags, int Index)
        {
            if (Settings.Count > 0 && Settings.Exists(x => x.Name == Name))
            {
                MelonLogger.Msg(DuplicateErrorMsg + ": " + Name);
                return null;
            }
            ModSetting<float> InputSetting = new()
            {
                Name = Name,
                Description = Description,
                Value = Value,
                SavedValue = Value,
                LinkGroup = 0,
                ValueType = AvailableTypes.Float
            };

            AddTags(InputSetting, tags);

            if (Index < 0 || Index > Settings.Count)
            {
                MelonLogger.Error($"Index {Index} is out of bounds for list with size {Settings.Count}. Falling back to Settings.Add");
                Settings.Add(InputSetting);
            }
            else
                Settings.Insert(Index, InputSetting);
            return InputSetting;
        }

        /// <summary>
        /// Creates an instance of ModSetting with the type double.
        /// <br/>Unlike AddToList, this adds it at the top.
        /// </summary>
        public ModSetting<double> AddToListAtStart(string Name, double Value, string Description, RumbleModUI.Tags tags) => AddToListAtIndex(Name, Value, Description, tags, 0);

        /// <summary>
        /// Creates an instance of ModSetting with the type double.
        /// <br/>Unlike AddToList, this lets you specify the index.
        /// </summary>
        public ModSetting<double> AddToListAtIndex(string Name, double Value, string Description, RumbleModUI.Tags tags, int Index)
        {
            if (Settings.Count > 0 && Settings.Exists(x => x.Name == Name))
            {
                MelonLogger.Msg(DuplicateErrorMsg + ": " + Name);
                return null;
            }
            ModSetting<double> InputSetting = new()
            {
                Name = Name,
                Description = Description,
                Value = Value,
                SavedValue = Value,
                LinkGroup = 0,
                ValueType = AvailableTypes.Double
            };

            AddTags(InputSetting, tags);

            if (Index < 0 || Index > Settings.Count)
            {
                MelonLogger.Error($"Index {Index} is out of bounds for list with size {Settings.Count}. Falling back to Settings.Add");
                Settings.Add(InputSetting);
            }
            else
                Settings.Insert(Index, InputSetting);
            return InputSetting;
        }
        #endregion

        #region AddFolder
        /// <summary>
        /// Adds a folder to the settings list.
        /// </summary>
        /// <param name="name">The name of the folder</param>
        /// <param name="description">The description of the folder</param>
        /// <returns>The folder instance</returns>
        public ModSettingFolder AddFolder(string name, string description = "")
        {
            if (Settings.Count > 0 && Settings.Exists(x => x.Name == name))
            {
                MelonLogger.Msg(DuplicateErrorMsg + ": " + name);
                return null;
            }

            ModSettingFolder inputSetting = new()
            {
                Name = name,
                Description = description,
                Value = "",
                SavedValue = "",
                LinkGroup = 0,
                ValueType = AvailableTypes.Description
            };

            AddTags(inputSetting, new Tags { DoNotSave = true, IsEmpty = true });

            Settings.Add(inputSetting);
            return inputSetting;
        }
        #endregion
    }

    /// <summary>
    /// Holds the harmony patches and some public methods.
    /// </summary>
    public class RumbleModUIPlusClass : MelonMod
    {
        public override void OnLateInitializeMelon() => UI.instance.UI_Initialized += OnUIInitialized;
        private void OnUIInitialized()
        {
            Mod ModUI = new()
            {
                ModName = "temp",
                ModVersion = "0",
                ModFormatVersion = "0"
            };
            ModUI.SetFolder("delme");
            ModUI.AddDescriptionAtStart("Description", "", "Lets you change the font for other mods.", new Tags { IsSummary = true });
            ModUI.AddFolder("Font Settings", "desc")
                .AddSetting(ModUI.AddToListAtStart("setting", true, 0, "hehe", new RumbleModUI.Tags()))
                .AddSetting(ModUI.AddToListAtStart("setting2", false, 0, "hehe2", new RumbleModUI.Tags()))
                .AddSetting(ModUI.AddFolder("folder?")
                    .AddSetting(ModUI.AddToListAtIndex("setting_in_folder", 0, "hehe_in_folder", new RumbleModUI.Tags(), 3))
                    .AddSetting(ModUI.AddToListAtStart("setting_in_folder2", 0, "hehe_in_folder2", new RumbleModUI.Tags()))
                    .AddSetting(ModUI.AddToList("setting_in_folder3", 0, "hehe_in_folder3", new RumbleModUI.Tags()))
                )
                .AddSetting(ModUI.AddToList("setting3", true, 0, "hehe3", new RumbleModUI.Tags()));
            ModUI.AddToListAtStart("a", false, 0, "broken :<", new RumbleModUI.Tags());
            UI.instance.AddMod(ModUI);
        }

        #region Harmony Patch Helpers
        private static readonly string RumbleModTypeFullName = typeof(RumbleModUI.Mod).FullName;
        private static readonly string[] SpecialNames = { nameof(RumbleModUI.Mod.SaveModData), nameof(RumbleModUI.Mod.GetFromFile) };

        private static bool WasCalledFromSpecial()
        {
            StackTrace st = new(1);
            StackFrame[] frames = st.GetFrames();
            if (frames.Length == 0) return false;

            for (int i = 0; i < Math.Min(frames.Length, 20); i++) // 20 is a safeguard
            {
                if (IsMethodMatchingAny(frames[i]?.GetMethod(), SpecialNames)) return true;
            }

            return false;
        }

        private static bool IsMethodMatchingAny(MethodBase method, string[] targetNames)
        {
            if (method == null) return false;

            Type dt = method.DeclaringType ?? method.ReflectedType;
            if (dt != null && dt.FullName == RumbleModTypeFullName)
            {
                foreach (string targetName in targetNames)
                    if (method.Name == targetName)
                        return true;
            }

            foreach (string targetName in targetNames)
            {
                if (method.Name == $"DMD<{RumbleModTypeFullName}::{targetName}>")
                    return true;
            }

            return false;
        }

        private static FieldInfo Mod_Options = AccessTools.Field(typeof(UI), "Mod_Options");
        private static FieldInfo ModSelection = AccessTools.Field(typeof(UI), "ModSelection");
        private static FieldInfo SettingsSelection = AccessTools.Field(typeof(UI), "SettingsSelection");
        private static FieldInfo UI_DropDown_Settings = AccessTools.Field(typeof(UI), "UI_DropDown_Settings");
        private static FieldInfo SettingsOverride = AccessTools.Field(typeof(UI), "SettingsOverride");
        private static MethodInfo DoOnModSelect = AccessTools.Method(typeof(UI), "DoOnModSelect");
        private static RumbleModUI.Mod getSelectedMod(UI instance) => ((List<RumbleModUI.Mod>)Mod_Options.GetValue(instance))[(int)ModSelection.GetValue(instance)];
        private static int getSelectedModSettingIndex(UI instance) => (int)SettingsSelection.GetValue(instance);
        private static ModSetting getSelectedModSetting(UI instance) => getSelectedMod(instance).Settings[getSelectedModSettingIndex(instance)];
        private static TMP_Dropdown getUI_DropDown_Settings(UI instance) => ((UnityEngine.GameObject)UI_DropDown_Settings.GetValue(instance)).GetComponent<TMP_Dropdown>();
        #endregion

        #region Harmony Patches
        [HarmonyPatch(typeof(RumbleModUI.Mod), nameof(RumbleModUI.Mod.ModVersion), MethodType.Getter)]
        private static class ModVersion_Getter_Patch
        {
            private static bool Prefix(RumbleModUI.Mod __instance, ref string __result)
            {
                if (__instance is Mod mod && WasCalledFromSpecial())
                {
                    __result = mod.ModFormatVersion;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(UI), "DoOnSettingsSelect")]
        private static class UI_DoOnSettingsSelect_Patch
        {
            private static void Prefix(UI __instance)
            {
                RumbleModUI.Mod mod = getSelectedMod(__instance);
                int idx = getSelectedModSettingIndex(__instance);
                if (mod.Settings[idx] is not ModSettingFolder modSettingFolder) return;
                UI_OnSettingsSelectionChange_Patch.Empty(__instance);
                Il2CppSystem.Collections.Generic.List<string> list = new();
                list.Add(UI_OnSettingsSelectionChange_Patch.BackText);
                List<LinkGroup> linkGroups = mod.LinkGroups;
                Dictionary<int, object> itemLookup = UI_OnSettingsSelectionChange_Patch.ItemLookup[__instance];
                itemLookup.Add(0, modSettingFolder.Parent);
                int newIdx = -1;

                for (int i = 0, i1 = list.Count; i < mod.Settings.Count; i++)
                {
                    ModSetting setting = mod.Settings[i];
                    if (i == idx)
                    {
                        newIdx = i1;
                        itemLookup.Add(i1++, i);
                        list.Add(setting.Name);
                    }
                    else if (modSettingFolder.Settings.Contains(setting))
                    {
                        itemLookup.Add(i1++, i);
                        if (setting.ValueType == AvailableTypes.Boolean && setting.LinkGroup != 0)
                            list.Add(linkGroups.Find(x => x.Index == setting.LinkGroup).Name + " - " + setting.Name);
                        else
                            list.Add(setting.Name);
                    }
                }

                UI_OnSettingsSelectionChange_Patch.SimplifyItemLookup(__instance, itemLookup);

                TMP_Dropdown UI_DropDown_Settings = getUI_DropDown_Settings(__instance);
                UI_DropDown_Settings.ClearOptions();
                UI_DropDown_Settings.AddOptions(list);
                if (newIdx == -1)
                    MelonLogger.Error("Could not find selected ModSettingFolder");
                else
                    UI_DropDown_Settings.SetValueWithoutNotify(newIdx);
            }

            //private static bool Prefix(UI __instance)
            //{
            //    if (getSelectedModSetting(__instance) is not ModSettingFolder modSettingFolder) return true;

            //    TMP_Dropdown UI_DropDown_Settings = getUI_DropDown_Settings(__instance);
            //    List<ModSetting> settings = getSelectedMod(__instance).Settings;
            //    TMP_Dropdown_Show_Patch.HiddenOptions.Clear();
            //    for (int i = 0; i < UI_DropDown_Settings.options.Count; i++)
            //        if (!modSettingFolder.Settings.Contains(settings[i]))
            //            TMP_Dropdown_Show_Patch.HiddenOptions.Add(UI_DropDown_Settings.options[i]);

            //    return false;
            //}
        }

        [HarmonyPatch(typeof(UI), "DoOnModSelect")]
        private static class UI_DoOnModSelect_Patch
        {
            private static readonly Dictionary<UI, List<ModSetting>> settingsLookup = new();

            private static void Prefix(UI __instance)
            {
                settingsLookup.Add(__instance, getSelectedMod(__instance).Settings);
                UI_OnSettingsSelectionChange_Patch.Empty(__instance);
            }

            private static bool DoContinue(UI instance, ModSetting setting)
            {
                if (setting.Tags is Tags { InFolder: true }) { return true; }

                UI_OnSettingsSelectionChange_Patch.ItemLookup[instance].Add(UI_OnSettingsSelectionChange_Patch.ItemLookup[instance].Count, settingsLookup[instance].IndexOf(setting));
                return false;
            }

            private static void Finalizer(UI __instance)
            {
                if (__instance != null && !settingsLookup.Remove(__instance))
                    MelonLogger.Error("Removal of UI failed");

                if (UI_OnSettingsSelectionChange_Patch.ItemLookup.TryGetValue(__instance, out Dictionary<int, object> map))
                    UI_OnSettingsSelectionChange_Patch.SimplifyItemLookup(__instance, map);
            }

            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                // TODO: EmitDelegate? also make the 'special' stuff use EmitDelegate too.
                // TODO: May not work with whatever SettingsOverride is
                return new CodeMatcher(instructions, generator)
                    .Start()
                    .MatchForward(false,
                        new CodeMatch(OpCodes.Ldloca_S),
                        new CodeMatch(OpCodes.Call),
                        new CodeMatch(OpCodes.Brtrue)
                    )
                    .ThrowIfInvalid("Could not match end of foreach in DoOnModSelect")
                    .CreateLabel(out Label label)
                    .Start()
                    .MatchForward(true,
                        new CodeMatch(OpCodes.Ldloc_2),
                        new CodeMatch(OpCodes.Ldloca_S),
                        new CodeMatch(OpCodes.Call),
                        new CodeMatch(OpCodes.Stfld),
                        new CodeMatch(OpCodes.Nop),
                        new CodeMatch(OpCodes.Ldloc_2)
                    )
                    .ThrowIfInvalid("Could not match start of foreach in DoOnModSelect")
                    .Insert(
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldloc_2),
                        new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(
                            typeof(UI).GetNestedType("<>c__DisplayClass92_0", BindingFlags.NonPublic | BindingFlags.Instance),
                            "setting"
                        )),
                        new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UI_DoOnModSelect_Patch), nameof(DoContinue))),
                        new CodeInstruction(OpCodes.Brtrue, label)
                    )
                    .InstructionEnumeration();
            }

            //private static void Postfix(UI __instance)
            //{
            //    TMP_Dropdown UI_DropDown_Settings = getUI_DropDown_Settings(__instance);
            //    List<ModSetting> settings = getSelectedMod(__instance).Settings;
            //    TMP_Dropdown_Show_Patch.HiddenOptions.Clear();
            //    for (int i = 0; i < UI_DropDown_Settings.options.Count; i++)
            //        if (settings[i].Tags is Tags { InFolder: true })
            //            TMP_Dropdown_Show_Patch.HiddenOptions.Add(UI_DropDown_Settings.options[i]);
            //}
        }

        //[HarmonyPatch(typeof(TMP_Dropdown), nameof(TMP_Dropdown.Show))]
        //private static class TMP_Dropdown_Show_Patch
        //{
        //    internal static readonly HashSet<TMP_Dropdown.OptionData> HiddenOptions = new();
        //    private static readonly Dictionary<TMP_Dropdown, Il2CppSystem.Collections.Generic.List<TMP_Dropdown.OptionData>> Originals = new();

        //    private static void Prefix(TMP_Dropdown __instance)
        //    {
        //        Il2CppSystem.Collections.Generic.List<TMP_Dropdown.OptionData> visibleOptions = new();
        //        foreach (TMP_Dropdown.OptionData optionData in __instance.options)
        //            if (!HiddenOptions.Contains(optionData))
        //                visibleOptions.Add(optionData);

        //        if (!Originals.TryAdd(__instance, __instance.options))
        //            MelonLogger.Error("Failed to add TMP_Dropdown");

        //        __instance.options = visibleOptions;
        //    }

        //    private static void Finalizer(TMP_Dropdown __instance)
        //    {
        //        if (__instance != null && Originals.TryGetValue(__instance, out Il2CppSystem.Collections.Generic.List<TMP_Dropdown.OptionData> original))
        //        {
        //            try
        //            {
        //                __instance.options = original;
        //            }
        //            catch (Exception e)
        //            {
        //                MelonLogger.Error("Restoration failed", e);
        //            }
        //            if (!Originals.Remove(__instance))
        //                MelonLogger.Error("Removal of TMP_Dropdown failed");
        //        }
        //    }
        //}

        [HarmonyPatch(typeof(UI), "OnSettingsSelectionChange")]
        private static class UI_OnSettingsSelectionChange_Patch
        {
            internal static readonly Dictionary<UI, Dictionary<int, object>> ItemLookup = new();
            internal const string BackText = "<―― back";

            internal static void SimplifyItemLookup(UI instance, Dictionary<int, object> map)
            {
                foreach (int key in map.Keys.Where(k =>
                    k == 0
                        ? map[0] is 0
                        : (int)map[k] == k
                ).ToList())
                    map.Remove(key);

                if (map.Count == 0)
                    ItemLookup.Remove(instance);
            }

            //private static void Prefix(UI __instance, ref int Input)
            //{
            //    if (ItemLookup.TryGetValue(__instance, out Dictionary<int, object> dict) && dict.TryGetValue(Input, out object original))
            //    {
            //        if (original is not int originalInt)
            //        {
            //            Empty(__instance);
            //            Dictionary<int, object> itemLookup = ItemLookup[__instance];
            //            Il2CppSystem.Collections.Generic.List<string> list = new();
            //            RumbleModUI.Mod mod = getSelectedMod(__instance);
            //            List<LinkGroup> linkGroups = mod.LinkGroups;
            //            List<ModSetting> settings = mod.Settings;
            //            ModSettingFolder parent = (ModSettingFolder) original;
            //            if (parent != null)
            //            {
            //                list.Add(BackText);
            //                itemLookup.Add(0, parent.Parent);
            //            }
            //            for (int i = 0; i < settings.Count; i++)
            //            {
            //                ModSetting setting = settings[i];
            //                if (parent == null
            //                    ? setting.Tags is not Tags { InFolder: true }
            //                    : (parent.Settings.Contains(setting) || setting == parent)
            //                )
            //                {
            //                    itemLookup.Add(itemLookup.Count, i);
            //                    if (setting.ValueType == AvailableTypes.Boolean && setting.LinkGroup != 0)
            //                        list.Add(linkGroups.Find(x => x.Index == setting.LinkGroup).Name + " - " + setting.Name);
            //                    else
            //                        list.Add(setting.Name);
            //                }
            //            }
            //            SimplifyItemLookup(__instance, itemLookup);
            //            TMP_Dropdown UI_DropDown_Settings = getUI_DropDown_Settings(__instance);
            //            UI_DropDown_Settings.ClearOptions();
            //            UI_DropDown_Settings.AddOptions(list);
            //            UI_DropDown_Settings.SetValueWithoutNotify(0);
            //            Input = 0;
            //        }
            //        else
            //            Input = originalInt;
            //    }
            //}

            private static bool Prefix(UI __instance, ref int Input)
            {
                if (ItemLookup.TryGetValue(__instance, out Dictionary<int, object> dict) && dict.TryGetValue(Input, out object original))
                {
                    if (original is not int originalInt)
                    {
                        ModSettingFolder parent = (ModSettingFolder) original;
                        if (parent == null)
                        {
                            SettingsOverride.SetValue(__instance, 0);
                            DoOnModSelect.Invoke(__instance, null);
                            return false;
                        }
                        Input = getSelectedMod(__instance).Settings.IndexOf(parent);
                    }
                    else
                        Input = originalInt;
                }
                return true;
            }

            internal static void Empty(UI instance)
            {
                if (!ItemLookup.TryAdd(instance, new Dictionary<int, object>()))
                    ItemLookup[instance].Clear();
            }
        }
        #endregion

        #region Public Methods
        /// <returns>The current selected <see cref="RumbleModUI.Mod"/> in the UI.</returns>
        public static RumbleModUI.Mod GetSelectedMod() => getSelectedMod(UI.instance);
        /// <returns>The current selected <see cref="ModSetting"/> in the UI.</returns>
        public static ModSetting GetSelectedModSetting() => getSelectedModSetting(UI.instance);
        #endregion
    }

    /// <summary>
    /// Extends <see cref="ModSetting"/> to act as a folder for other settings.
    /// </summary>
    public class ModSettingFolder : ModSetting
    {
        /// <summary>
        /// The settings contained in the folder (order is from <see cref="RumbleModUI.Mod.Settings"/>).
        /// </summary>
        public readonly HashSet<ModSetting> Settings = new();

        /// <summary>
        /// The parent folder of this folder, or null if it's in the root.
        /// </summary>
        internal ModSettingFolder Parent = null;

        /// <summary>
        /// Adds a <see cref="ModSetting"/> to the folder and sets <see cref="Tags.InFolder"/> to true.
        /// If needed, converts the setting's <see cref="ModSetting.Tags"/> to <see cref="Tags"/>.
        /// </summary>
        /// <param name="setting">The <see cref="ModSetting"/> to add to the folder</param>
        /// <returns>The instance (for chaining)</returns>
        public ModSettingFolder AddSetting(ModSetting setting)
        {
            if (setting.Tags is Tags tags)
                tags.InFolder = true;
            else
                setting.Tags = new Tags(setting.Tags) { InFolder = true };
            if (setting is ModSettingFolder folder)
                folder.Parent = this;
            Settings.Add(setting);
            return this;
        }

        /// <summary>
        /// Removes a setting from the folder and sets <see cref="Tags.InFolder"/> to false.
        /// </summary>
        /// <param name="setting">The <see cref="ModSetting"/> that will be removed.</param>
        /// <returns>Was the <see cref="ModSetting"/> removed successfully?</returns>
        public bool RemoveSetting(ModSetting setting)
        {
            ((Tags) setting.Tags).InFolder = false;
            if (setting is ModSettingFolder folder)
                folder.Parent = null;
            return Settings.Remove(setting);
        }

        /// <summary>
        /// Removes a setting from the folder and sets <see cref="Tags.InFolder"/> to false.<br/>
        /// Unlike <see cref="RemoveSetting"/>, this returns the instance for chaining.
        /// </summary>
        /// <param name="setting">The <see cref="ModSetting"/> that will be removed.</param>
        /// <param name="success">Was the <see cref="ModSetting"/> removed successfully?</param>
        /// <returns>The instance (for chaining)</returns>
        public ModSettingFolder RemoveSettingC(ModSetting setting, out bool success)
        {
            success = RemoveSetting(setting);
            return this;
        }

        /// <summary>
        /// Removes a setting from the folder and sets <see cref="Tags.InFolder"/> to false.<br/>
        /// Unlike <see cref="RemoveSetting"/>, this returns the instance for chaining.
        /// </summary>
        /// <param name="setting">The <see cref="ModSetting"/> that will be removed.</param>
        /// <returns>The instance (for chaining)</returns>
        public ModSettingFolder RemoveSettingC(ModSetting setting)
        {
            RemoveSetting(setting);
            return this;
        }

        /// <summary>
        /// Removes this folder. All settings in the folder will be put into the mod's root settings list.
        /// </summary>
        /// <param name="mod">The <see cref="RumbleModUI.Mod"/> that has this folder</param>
        /// <returns>Was this removed successfully?</returns>
        public bool RemoveFolder(RumbleModUI.Mod mod) => RemoveFolder(mod.Settings);

        /// <summary>
        /// Removes all the settings from the folder and sets their <see cref="Tags.InFolder"/> to false.
        /// </summary>
        /// <returns>The instance (for chaining)</returns>
        public ModSettingFolder RemoveAllSettings()
        {
            foreach (ModSetting setting in Settings)
                RemoveSetting(setting);
            return this;
        }

        /// <summary>
        /// Removes this folder. All settings in the folder will be put into the mod's root settings list.
        /// </summary>
        /// <param name="settings">The settings list of <see cref="ModSetting"/> that contains this folder</param>
        /// <returns>Was this removed successfully?</returns>
        public bool RemoveFolder(List<ModSetting> settings)
        {
            RemoveAllSettings();
            return settings.Remove(this);
        }

        /// <inheritdoc/>
        public override string GetValueAsString() => "";
        /// <inheritdoc/>
        public override object Value { get; set; }
        /// <inheritdoc/>
        public override object SavedValue { get; set; }
    }

    /// <summary>
    /// Extends <see cref="RumbleModUI.Tags"/> to add the <see cref="InFolder"/> property.
    /// </summary>
    public class Tags : RumbleModUI.Tags
    {
        /// <summary>
        /// Make a new instance of <see cref="Tags"/> with <see cref="InFolder"/> set to false.
        /// </summary>
        public Tags() => InFolder = false;
        /// <summary>
        /// Convert a <see cref="RumbleModUI.Tags"/> to <see cref="Tags"/>.
        /// </summary>
        /// <param name="tags">The <see cref="RumbleModUI.Tags"/> in which the data will be copied from</param>
        public Tags(RumbleModUI.Tags tags) : this()
        {
            IsSummary = tags.IsSummary;
            IsEmpty = tags.IsEmpty;
            IsCustom = tags.IsCustom;
            CustomString = tags.CustomString;
            IsPassword = tags.IsPassword;
            DoNotSave = tags.DoNotSave;
        }

        /// <summary>
        /// Is the setting in a folder?
        /// </summary>
        public bool InFolder { get; set; }
    }
}
