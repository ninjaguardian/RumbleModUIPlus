using HarmonyLib;
using MelonLoader;
using System;
using System.Diagnostics;
using System.Reflection;

[assembly: MelonInfo(typeof(RumbleModUIPlus.RumbleModUIPlusClass), RumbleModUIPlus.BuildInfo.ModName, RumbleModUIPlus.BuildInfo.ModVersion, "ninjaguardian", "https://thunderstore.io/c/rumble/p/ninjaguardian/RumbleModUIPlus")]
[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]

[assembly: MelonColor(255, 0, 160, 230)]
[assembly: MelonAuthorColor(255, 0, 160, 230)]

[assembly: MelonPlatform(MelonPlatformAttribute.CompatiblePlatforms.WINDOWS_X64)]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]
[assembly: VerifyLoaderVersion(RumbleModUIPlus.BuildInfo.MLVersion, true)]

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
        public const string ModVersion = "1.0.0";
        /// <summary>
        /// MelonLoader version.
        /// </summary>
        public const string MLVersion = "0.7.0";
    }
    /// <inheritdoc/>
    public class Mod : RumbleModUI.Mod
    {
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
    }

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
}
