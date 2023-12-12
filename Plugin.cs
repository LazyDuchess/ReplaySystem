using BepInEx;
using BepInEx.Logging;
using Reptile;
using HarmonyLib;

namespace ReplaySystem
{
    [BepInPlugin(GUID, Name, Version)]
    internal class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;
        public ManualLogSource Log => Logger;
        private const string GUID = "com.LazyDuchess.BRC.ReplaySystem";
        private const string Name = "ReplaySystem";
        private const string Version = "1.0.0";

        private void Awake()
        {
            Instance = this;
            var harmony = new Harmony(GUID);
            harmony.PatchAll();
            Logger.LogInfo($"{Name} {Version} loaded!");
            StageManager.OnStagePostInitialization += StageManager_OnStagePostInitialization;
        }

        private void StageManager_OnStagePostInitialization()
        {
            ReplayManager.Create();
        }
    }
}
