using HarmonyLib;
using Il2CppTMPro;
using MelonLoader;
using RumbleModUI;
using System;
using System.Collections.Generic;
using System.IO;
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

// TODO: allow same name in different folders (needs to work with saving)
// TODO: format version updater
// TODO: only delete settings on minor/major versions (or not at all if updater)
// TODO: write down that format version should only be changed when breaking changes are made
// TODO: settings names may break with ':' in them
// TODO: string values may break with ':' in them
// TODO: maybe store old Tags instance and forward calls to it

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
        public const string ModVersion = "2.0.0";
        /// <summary>
        /// MelonLoader version.
        /// </summary>
        public const string MLVersion = "0.7.1";
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
                MelonLogger.Warning(DuplicateErrorMsg + ": " + Name);
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
                MelonLogger.Warning(DuplicateErrorMsg + ": " + Name);
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
                MelonLogger.Warning(DuplicateErrorMsg + ": " + Name);
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
                MelonLogger.Warning(DuplicateErrorMsg + ": " + Name);
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
                MelonLogger.Warning(DuplicateErrorMsg + ": " + Name);
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
                MelonLogger.Warning(DuplicateErrorMsg + ": " + Name);
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
                MelonLogger.Warning(DuplicateErrorMsg + ": " + name);
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
    }

    /// <summary>
    /// Holds the harmony patches and some public methods.
    /// </summary>
    public class RumbleModUIPlusClass : MelonMod
    {
        /// <inheritdoc/>
        public override void OnInitializeMelon() => Mod_Log_Patches.PatchAll(HarmonyInstance);

        #if DEBUG
        public override void OnLateInitializeMelon() => UI.instance.UI_Initialized += OnUIInitialized;
        private static void OnUIInitialized()
        {
            Mod ModUI = new()
            {
                ModName = "temp",
                ModVersion = "0",
                ModFormatVersion = "0"
            };
            ModUI.SetFolder("deleteme");
            ModUI.AddDescriptionAtStart("Description", "", "full desc", new Tags { IsSummary = true });
            ModUI.AddFolder("Font Settings", "desc")
                .AddSetting(ModUI.AddToListAtStart("setting", true, 0, "hehe", new RumbleModUI.Tags()))
                .AddSetting(ModUI.AddToListAtStart("setting2", true, 0, "hehe2", new RumbleModUI.Tags()))
                .AddSetting(ModUI.AddFolder("folder?")
                    .AddSetting(ModUI.AddToListAtIndex("setting_in_folder", 0, "hehe_in_folder", new RumbleModUI.Tags(), 3))
                    .AddSetting(ModUI.AddToListAtStart("setting_in_folder2", 0, "hehe_in_folder2", new RumbleModUI.Tags()))
                    .AddSetting(ModUI.AddToList("setting_in_folder3", 0, "hehe_in_folder3", new Tags()))
                )
                .AddSetting(ModUI.AddToList("setting3", true, 0, "hehe3", new RumbleModUI.Tags()));
            ModUI.AddToListAtStart("setting", true, 0, "ttt", new RumbleModUI.Tags());
            ModUI.EnableDebug();
            ModUI.GetFromFile();
            UI.instance.AddMod(ModUI);
        }
        #endif

        #region Harmony Patch Helpers
        private static FieldInfo Mod_Options = AccessTools.Field(typeof(UI), "Mod_Options");
        private static FieldInfo ModSelection = AccessTools.Field(typeof(UI), "ModSelection");
        private static FieldInfo SettingsSelection = AccessTools.Field(typeof(UI), "SettingsSelection");
        private static FieldInfo SettingsOverride = AccessTools.Field(typeof(UI), "SettingsOverride");
        private static FieldInfo UI_DropDown_Settings = AccessTools.Field(typeof(UI), "UI_DropDown_Settings");
        private static MethodInfo DoOnModSelect = AccessTools.Method(typeof(UI), "DoOnModSelect");
        private static FieldInfo Mod_Folders = AccessTools.Field(typeof(RumbleModUI.Mod), "Folders");
        private static FieldInfo Mod_debug = AccessTools.Field(typeof(RumbleModUI.Mod), "debug");
        private static MethodInfo Mod_IsFileLoadedSetter = AccessTools.PropertySetter(typeof(RumbleModUI.Mod), "IsFileLoaded");
        private static MethodInfo Mod_ValueValidation = AccessTools.Method(typeof(RumbleModUI.Mod), "ValueValidation");
        private static RumbleModUI.Mod getSelectedMod(UI instance) => ((List<RumbleModUI.Mod>)Mod_Options.GetValue(instance))[(int)ModSelection.GetValue(instance)];
        private static int getSelectedModSettingIndex(UI instance) => (int)SettingsSelection.GetValue(instance);
        private static TMP_Dropdown getUI_DropDown_Settings(UI instance) => ((UnityEngine.GameObject)UI_DropDown_Settings.GetValue(instance)).GetComponent<TMP_Dropdown>();
        #endregion

        #region Harmony Patches
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
                list.Add("<―― back");
                List<LinkGroup> linkGroups = mod.LinkGroups;
                Dictionary<int, int> itemLookup = UI_OnSettingsSelectionChange_Patch.ItemLookup[__instance];
                itemLookup.Add(0, mod.Settings.IndexOf(modSettingFolder.Parent));
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
        }

        [HarmonyPatch(typeof(UI), "DoOnModSelect")]
        private static class UI_DoOnModSelect_Patch
        {
            private static readonly Dictionary<UI, (int firstSummary, int firstNonFolder)> settingOverride = new();
            private static readonly Dictionary<UI, int> settingIdx = new();

            private static void Prefix(UI __instance)
            {
                if ((int)SettingsOverride.GetValue(__instance) == 0)
                    settingOverride.Add(__instance, (-1, -1));
                settingIdx.Add(__instance, 0);
                UI_OnSettingsSelectionChange_Patch.Empty(__instance);
            }

            private static bool DoContinue(UI instance, ModSetting setting)
            {
                int idx = settingIdx[instance]++;
                if (setting.Tags is Tags { InFolder: true }) { return true; }

                int count = UI_OnSettingsSelectionChange_Patch.ItemLookup[instance].Count;

                if (settingOverride.TryGetValue(instance, out (int firstSummary, int firstNonFolder) value))
                {
                    if (value.firstSummary == -1 && setting.Tags.IsSummary)
                        value.firstSummary = count;
                    else if (value.firstNonFolder == -1 && setting is not ModSettingFolder)
                        value.firstNonFolder = count;
                }

                UI_OnSettingsSelectionChange_Patch.ItemLookup[instance].Add(count, idx);
                return false;
            }

            private static void Finalizer(UI __instance)
            {
                if (__instance != null)
                {
                    settingOverride.Remove(__instance);

                    if (!settingIdx.Remove(__instance))
                        MelonLogger.Error("Removal of UI from settingIdx failed");

                    if (UI_OnSettingsSelectionChange_Patch.ItemLookup.TryGetValue(__instance, out Dictionary<int, int> map))
                        UI_OnSettingsSelectionChange_Patch.SimplifyItemLookup(__instance, map);
                }
            }

            private static void SettingsOverrideHelper(UI instance)
            {
                if (settingOverride.TryGetValue(instance, out (int firstSummary, int firstNonFolder) value))
                {
                    SettingsOverride.SetValue(instance,
                        value.firstSummary != -1
                            ? value.firstSummary
                            : value.firstNonFolder != -1
                                ? value.firstNonFolder
                                : 0
                    );

                    settingOverride.Remove(instance);
                }
            }

            private static int LookupSetting(UI instance, int settingOverride) => UI_OnSettingsSelectionChange_Patch.ItemLookup.TryGetValue(instance, out Dictionary<int, int> dict) && dict.TryGetValue(settingOverride, out int original) ? original : settingOverride;

            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
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
                    .MatchForward(false,
                        new CodeMatch(OpCodes.Ldarg_0),
                        new CodeMatch(OpCodes.Ldfld),
                        new CodeMatch(OpCodes.Ldc_I4_0),
                        new CodeMatch(OpCodes.Cgt_Un),
                        new CodeMatch(OpCodes.Stloc_S)
                    )
                    .ThrowIfInvalid("Could not match: this.SettingsOverride != 0")
                    .InsertAndAdvance(
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UI_DoOnModSelect_Patch), nameof(SettingsOverrideHelper)))
                    )
                    .RemoveInstructions(8)
                    .MatchForward(false,
                        new CodeMatch(OpCodes.Ldarg_0),
                        new CodeMatch(OpCodes.Ldarg_0),
                        new CodeMatch(OpCodes.Ldfld),
                        new CodeMatch(OpCodes.Stfld)
                    )
                    .ThrowIfInvalid("Could not match: this.SettingsSelection = this.SettingsOverride")
                    .Insert(
                        new CodeInstruction(OpCodes.Ldarg_0)
                    )
                    .Advance(4)
                    .Insert(
                        new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UI_DoOnModSelect_Patch), nameof(LookupSetting)))
                    )
                    .MatchForward(false,
                        new CodeMatch(OpCodes.Callvirt, AccessTools.PropertySetter(typeof(TMP_Dropdown), nameof(TMP_Dropdown.value)))
                    )
                    .ThrowIfInvalid("Could not match callvirt TMP_Dropdown.value setter")
                    .SetOperandAndAdvance(
                        AccessTools.Method(typeof(TMP_Dropdown), nameof(TMP_Dropdown.SetValueWithoutNotify), new[] { typeof(int) })
                    )
                    .MatchForward(false,
                        new CodeMatch(OpCodes.Nop),
                        new CodeMatch(OpCodes.Ldarg_0),
                        new CodeMatch(OpCodes.Ldc_I4_0),
                        new CodeMatch(OpCodes.Stfld, AccessTools.Field(typeof(UI), "SettingsSelection")),
                        new CodeMatch(OpCodes.Nop)
                    )
                    .ThrowIfInvalid("Could not match else")
                    .Advance(-1)
                    .RemoveInstructions(6)
                    .InstructionEnumeration();
            }
        }

        [HarmonyPatch(typeof(UI), "OnSettingsSelectionChange")]
        private static class UI_OnSettingsSelectionChange_Patch
        {
            internal static readonly Dictionary<UI, Dictionary<int, int>> ItemLookup = new();

            internal static void SimplifyItemLookup(UI instance, Dictionary<int, int> map)
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

            private static bool Prefix(UI __instance, ref int Input)
            {
                if (ItemLookup.TryGetValue(__instance, out Dictionary<int, int> dict) && dict.TryGetValue(Input, out int original))
                {
                    if (original == -1)
                    {
                        SettingsOverride.SetValue(__instance, 0);
                        DoOnModSelect.Invoke(__instance, null);
                        return false;
                    }
                    Input = original;
                }
                return true;
            }

            internal static void Empty(UI instance)
            {
                if (!ItemLookup.TryAdd(instance, new Dictionary<int, int>()))
                    ItemLookup[instance].Clear();
            }
        }

        private static class Mod_Log_Patches
        {
            internal static void PatchAll(HarmonyLib.Harmony harmony)
            {
                foreach (MethodInfo method in typeof(RumbleModUI.Mod).GetMethods())
                    if (method.DeclaringType == typeof(RumbleModUI.Mod))
                        harmony.Patch(
                            original: method,
                            transpiler: GetTranspiler
                        );
            }

            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) => new CodeMatcher(instructions, generator)
                .Start()
                .MatchForward(false, MelonLoggerMsg)
                .Repeat(match => {
                    match.RemoveInstruction();
                    match.InsertAndAdvance(MelonLoggerWarning);
                })
                .InstructionEnumeration();

            private static readonly CodeMatch MelonLoggerMsg = new(OpCodes.Call, AccessTools.Method(typeof(MelonLogger), nameof(MelonLogger.Msg), new[] { typeof(string) }));
            private static readonly CodeInstruction MelonLoggerWarning = new(OpCodes.Call, AccessTools.Method(typeof(MelonLogger), nameof(MelonLogger.Warning), new[] { typeof(string) }));
            private static HarmonyMethod GetTranspiler => new(typeof(Mod_Log_Patches).GetMethod(nameof(Transpiler), BindingFlags.NonPublic | BindingFlags.Static));
        }

        [HarmonyPatch(typeof(UI), "SaveSettings")]
        private static class UI_SaveSettings_Patch
        {
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                foreach (CodeInstruction instruction in instructions)
                {
                    yield return instruction;
                    if (instruction.opcode == OpCodes.Ldstr && instruction.operand is string s && s.StartsWith("Created by: "))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return Transpilers.EmitDelegate<Func<string, UI, string>>((str, instance) => getSelectedMod(instance) is Mod mod ? str + " and " + BuildInfo.ModName + " " + BuildInfo.ModVersion : str);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(RumbleModUI.Mod), nameof(RumbleModUI.Mod.GetFromFile))]
        private static class Mod_GetFromFile_Patch
        {
            private static bool Prefix(RumbleModUI.Mod __instance)
            {
                Baum_API.Folders Folders = (Baum_API.Folders)Mod_Folders.GetValue(__instance);
                bool debug = (bool)Mod_debug.GetValue(__instance);

                string subFolder = Folders.GetSubFolder(0);
                string Path = (subFolder == null
                    ? Folders.GetFolderString()
                    : Folders.GetFolderString(subFolder))
                    + @"\" + __instance.SettingsFile;

                if (File.Exists(Path))
                {
                    string[] Lines = File.ReadAllLines(Path);
                    if (Lines.Length >= 2 && Lines[0] == __instance.ModName + " " + (__instance is Mod mod ? mod.ModFormatVersion : __instance.ModVersion))
                    {
                        HashSet<ModSetting> processed = new();
                        for (int i = 2; i < Lines.Length; i++)
                        {
                            string line = Lines[i];
                            foreach (ModSetting setting in __instance.Settings)
                            {
                                if (processed.Contains(setting) || setting.Tags.DoNotSave || !line.StartsWith(setting.Name + ": ")) continue;

                                if ((bool)Mod_ValueValidation.Invoke(__instance, new object[] { line[(setting.Name.Length + 2)..], setting }))
                                    setting.SavedValue = setting.Value;
                                else
                                    MelonLogger.Msg(__instance.ModName + " - " + setting.Name + " File Read Error.");

                                if (debug)
                                    MelonLogger.Msg(__instance.ModName + " - " + setting.Name + " " + setting.Value);

                                processed.Add(setting);
                                break;
                            }
                        }
                        Mod_IsFileLoadedSetter.Invoke(__instance, new object[] { true });
                    }
                    else
                    {
                        Mod_IsFileLoadedSetter.Invoke(__instance, new object[] { false });
                        if (Lines.Length < 2)
                            MelonLogger.Error($"Could not load {__instance.ModName}'s settings because there were less than two lines in the settings file.");
                        else if (debug)
                            MelonLogger.Warning(__instance.ModName + "'s settings did not match pattern.");
                    }
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(RumbleModUI.Mod), nameof(RumbleModUI.Mod.SaveModData))]
        private static class Mod_SaveModData
        {
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                MethodInfo get_ModVersion = AccessTools.PropertyGetter(typeof(RumbleModUI.Mod), nameof(RumbleModUI.Mod.ModVersion));
                foreach (CodeInstruction instruction in instructions)
                    if (instruction.opcode == OpCodes.Call && instruction.operand is MethodInfo method && method == get_ModVersion)
                        yield return Transpilers.EmitDelegate<Func<RumbleModUI.Mod, string>>(instance => instance is Mod mod ? mod.ModFormatVersion : instance.ModVersion);
                    else
                        yield return instruction;
            }
        }
        #endregion

        #region Public Methods
        /// <returns>The current selected <see cref="RumbleModUI.Mod"/> in the UI.</returns>
        public static RumbleModUI.Mod GetSelectedMod() => getSelectedMod(UI.instance);
        /// <returns>The current selected <see cref="ModSetting"/> in the UI.</returns>
        public static ModSetting GetSelectedModSetting() => getSelectedMod(UI.instance).Settings[getSelectedModSettingIndex(UI.instance)];
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
        internal ModSettingFolder Parent;

        /// <summary>
        /// Adds a <see cref="ModSetting"/> to the folder and sets <see cref="Tags.InFolder"/> to true.
        /// If needed, converts the setting's <see cref="ModSetting.Tags"/> to <see cref="Tags"/>.
        /// </summary>
        /// <param name="setting">The <see cref="ModSetting"/> to add to the folder</param>
        /// <returns>The <see cref="ModSettingFolder"/> instance (for chaining)</returns>
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
        /// <returns>The <see cref="ModSettingFolder"/> instance (for chaining)</returns>
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
        /// <returns>The <see cref="ModSettingFolder"/> instance (for chaining)</returns>
        public ModSettingFolder RemoveSettingC(ModSetting setting)
        {
            RemoveSetting(setting);
            return this;
        }

        /// <summary>
        /// Removes all the settings from the folder and sets their <see cref="Tags.InFolder"/> to false.
        /// </summary>
        /// <returns>The <see cref="ModSettingFolder"/> instance (for chaining)</returns>
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

        /// <summary>
        /// Removes this folder. All settings in the folder will be put into the mod's root settings list.
        /// </summary>
        /// <param name="mod">The <see cref="RumbleModUI.Mod"/> that has this folder</param>
        /// <returns>Was this removed successfully?</returns>
        public bool RemoveFolder(RumbleModUI.Mod mod) => RemoveFolder(mod.Settings);


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
