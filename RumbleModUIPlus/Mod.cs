using HarmonyLib;
using MelonLoader;
using RumbleModUI;
using System;
using System.Diagnostics;
using System.Reflection;
using static RumbleModUI.ModSetting;

#region Assemblies
[assembly: MelonInfo(typeof(RumbleModUIPlus.RumbleModUIPlusClass), RumbleModUIPlus.BuildInfo.ModName, RumbleModUIPlus.BuildInfo.ModVersion, "ninjaguardian", "https://thunderstore.io/c/rumble/p/ninjaguardian/RumbleModUIPlus")]
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
    }

    /// <inheritdoc/>
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

        private static readonly string RumbleModTypeFullName = typeof(RumbleModUI.Mod).FullName;
        private static readonly string[] SpecialNames = { nameof(SaveModData), nameof(GetFromFile) };
        #endregion

        #region Harmony Patch Helper Funcs
        internal static bool WasCalledFromSpecial()
        {
            StackTrace st = new(1);
            StackFrame[] frames = st.GetFrames();
            if (frames == null || frames.Length == 0) return false;

            for (int i = 0; i < Math.Min(frames.Length, 20); i++) // 20 is a safeguard
            {
                if (IsMethodMatchingAny(frames[i]?.GetMethod(), SpecialNames)) return true;
            }

            return false;
        }

        internal static bool IsMethodMatchingAny(MethodBase method, string[] targetNames)
        {
            if (method == null) return false;

            Type dt = method.DeclaringType ?? method.ReflectedType;
            if (dt != null && dt.FullName == RumbleModTypeFullName)
            {
                string mName = method.Name;
                for (int i = 0; i < targetNames.Length; i++)
                    if (mName == targetNames[i]) return true;
            }

            string name = method.Name ?? string.Empty;
            for (int i = 0; i < targetNames.Length; i++)
            {
                if (name == $"DMD<{RumbleModTypeFullName}::{targetNames[i]}>")
                    return true;
            }

            return false;
        }
        #endregion

        #region Accessors
        ///// <summary>
        ///// Accessor for Folders
        ///// </summary>
        //public RumbleModUI.Baum_API.Folders Folders
        //{
        //    get => (RumbleModUI.Baum_API.Folders)AccessTools.Field(typeof(RumbleModUI.Mod), "Folders").GetValue(this);
        //    set => AccessTools.Field(typeof(RumbleModUI.Mod), "Folders").SetValue(this, value);
        //}
        ///// <summary>
        ///// Accessor for ModSaved (so you can ?.Invoke())
        ///// </summary>
        //public Action ModSavedAccessor
        //{
        //    get => (Action)AccessTools.Field(typeof(RumbleModUI.Mod), nameof(ModSaved)).GetValue(this);
        //    set => AccessTools.Field(typeof(RumbleModUI.Mod), nameof(ModSaved)).SetValue(this, value);
        //}
        ///// <summary>
        ///// Accessor for IsSaved
        ///// </summary>
        //public bool IsSaved
        //{
        //    get => (bool)AccessTools.Field(typeof(RumbleModUI.Mod), "IsSaved").GetValue(this);
        //    set => AccessTools.Field(typeof(RumbleModUI.Mod), "IsSaved").SetValue(this, value);
        //}
        #endregion

        #region AddToListAtStart & AddToListAtIndex
        /// <summary>
        /// Creates a instance of ModSetting with the type string. <br/>
        /// The value cannot be modified by the user. <br/>
        /// Unlike AddDescription, this adds it at the top.
        /// </summary>
        public ModSetting<string> AddDescriptionAtStart(string Name, string Value, string Description, Tags tags) => AddDescriptionAtIndex(Name, Value, Description, tags, 0);

        /// <summary>
        /// Creates a instance of ModSetting with the type string. <br/>
        /// The value cannot be modified by the user. <br/>
        /// Unlike AddDescription, this lets you specify the index.
        /// </summary>
        public ModSetting<string> AddDescriptionAtIndex(string Name, string Value, string Description, Tags tags, int Index)
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
        /// Creates a instance of ModSetting with the type string.
        /// <br/>Unlike AddToList, this adds it at the top.
        /// </summary>
        public ModSetting<string> AddToListAtStart(string Name, string Value, string Description, Tags tags) => AddToListAtIndex(Name, Value, Description, tags, 0);

        /// <summary>
        /// Creates a instance of ModSetting with the type string.
        /// <br/>Unlike AddToList, this lets you specify the index.
        /// </summary>
        public ModSetting<string> AddToListAtIndex(string Name, string Value, string Description, Tags tags, int Index)
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
        /// Creates a instance of ModSetting with the type bool.
        /// <br/>Unlike AddToList, this adds it at the top.
        /// </summary>
        public ModSetting<bool> AddToListAtStart(string Name, bool Value, int LinkGroup, string Description, Tags tags) => AddToListAtIndex(Name, Value, LinkGroup, Description, tags, 0);

        /// <summary>
        /// Creates a instance of ModSetting with the type bool.
        /// <br/>Unlike AddToList, this lets you specify the index.
        /// </summary>
        public ModSetting<bool> AddToListAtIndex(string Name, bool Value, int LinkGroup, string Description, Tags tags, int Index)
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
        /// Creates a instance of ModSetting with the type int.
        /// <br/>Unlike AddToList, this adds it at the top.
        /// </summary>
        public ModSetting<int> AddToListAtStart(string Name, int Value, string Description, Tags tags) => AddToListAtIndex(Name, Value, Description, tags, 0);

        /// <summary>
        /// Creates a instance of ModSetting with the type int.
        /// <br/>Unlike AddToList, this lets you specify the index.
        /// </summary>
        public ModSetting<int> AddToListAtIndex(string Name, int Value, string Description, Tags tags, int Index)
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
        /// Creates a instance of ModSetting with the type float.
        /// <br/>Unlike AddToList, this adds it at the top.
        /// </summary>
        public ModSetting<float> AddToListAtStart(string Name, float Value, string Description, Tags tags) => AddToListAtIndex(Name, Value, Description, tags, 0);

        /// <summary>
        /// Creates a instance of ModSetting with the type float.
        /// <br/>Unlike AddToList, this lets you specify the index.
        /// </summary>
        public ModSetting<float> AddToListAtIndex(string Name, float Value, string Description, Tags tags, int Index)
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
        /// Creates a instance of ModSetting with the type double.
        /// <br/>Unlike AddToList, this adds it at the top.
        /// </summary>
        public ModSetting<double> AddToListAtStart(string Name, double Value, string Description, Tags tags) => AddToListAtIndex(Name, Value, Description, tags, 0);

        /// <summary>
        /// Creates a instance of ModSetting with the type double.
        /// <br/>Unlike AddToList, this lets you specify the index.
        /// </summary>
        public ModSetting<double> AddToListAtIndex(string Name, double Value, string Description, Tags tags, int Index)
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
    }

    #region Harmony patch
    /// <summary>
    /// Holds the harmony patch.
    /// </summary>
    public class RumbleModUIPlusClass : MelonMod
    {
        [HarmonyPatch(typeof(RumbleModUI.Mod), nameof(RumbleModUI.Mod.ModVersion), MethodType.Getter)]
        static class ModVersion_Getter_Patch
        {
            static bool Prefix(RumbleModUI.Mod __instance, ref string __result)
            {
                if (__instance is Mod mod && Mod.WasCalledFromSpecial())
                {
                    __result = mod.ModFormatVersion;
                    return false;
                }

                return true;
            }
        }
    }
    #endregion
}
