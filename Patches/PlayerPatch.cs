using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using HarmonyLib;

namespace ReplaySystem.Patches
{
    [HarmonyPatch(typeof(Player))]
    internal class PlayerPatch
    {
        
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.UpdateRotation))]
        private static void UpdateRotation_Prefix(Player __instance)
        {
            
            var replayManager = ReplayManager.Instance;
            if (replayManager == null)
                return;
            if (WorldHandler.instance.GetCurrentPlayer() != __instance)
                return;
            replayManager.OnFixedUpdate();
        }
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.Jump))]
        private static void Jump_Prefix(Player __instance)
        {

            var replayManager = ReplayManager.Instance;
            if (replayManager == null)
                return;
            if (WorldHandler.instance.GetCurrentPlayer() != __instance)
                return;
            if (replayManager.CurrentReplayState is ReplayRecorder recorder)
            {
                var frame = recorder.LastFrame;
                if (frame != null)
                {
                    frame.Jump = true;
                    frame.JumpRequested = false;
                    frame.JumpButtonHeld = false;
                    frame.JumpButtonNew = false;
                }
            }
        }
    }
}
