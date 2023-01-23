using HarmonyLib;
using Kitchen;
using Kitchen.Modules;
using KitchenLib;
using KitchenMods;
using System.Reflection;
using UnityEngine;

namespace LevelViewer {

    public class LevelViewerMod : BaseMod {

        public const string MOD_ID = "blargle.LevelViewer";
        public const string MOD_NAME = "Level Viewer";
        public const string MOD_AUTHOR = "blargle";
        public const string MOD_VERSION = "0.0.1";

        public LevelViewerMod() : base(MOD_ID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, ">=1.1.3", Assembly.GetExecutingAssembly()) { }
    }

    public class LevelCollectorSystem : FranchiseFirstFrameSystem, IModSystem {

        public static bool loaded = false;
        public static SPlayerLevel levelSingleton;

        protected override void OnUpdate() {
            loaded = true;
            levelSingleton = GetOrDefault<SPlayerLevel>();
        }
    }

    [HarmonyPatch(typeof(PlayerElement), "SetPlayer")]
    class PlayerElement_SetPlayer_Patch {

        public static void Postfix(int id, PlayerElement __instance) {
            PlayerInfo info = Players.Main.Get(id);
            if (!info.IsLocalUser || info.IsJoining || !LevelCollectorSystem.loaded) {
                return;
            }

            __instance.SetName(info.Profile.Name, $"{info.Username} - Level {LevelCollectorSystem.levelSingleton.Level + 1}");
            __instance.SetJoinProgress(ProgressionHelpers.GetProgressPercent(LevelCollectorSystem.levelSingleton));
            var color = info.Profile.Colour;
            __instance.JoinBar.Color = new Color(color.r, color.g, color.b, 0.5f);
            __instance.JoinBar.gameObject.SetActive(true);
        }
    }
}