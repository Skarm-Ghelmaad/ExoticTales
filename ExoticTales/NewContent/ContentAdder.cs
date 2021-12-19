using HarmonyLib;
using Kingmaker.Blueprints.JsonSystem;

namespace ExoticTales.NewContent {
    class ContentAdder {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch {
            static bool Initialized;

            [HarmonyPriority(Priority.First)]
            static void Postfix() {
                if (Initialized) return;
                Initialized = true;
                Main.LogHeader("Loading New Content");


                Feats.QuickDraw.AddQuickDraw();
                Features.DarkVision.AddDarkVision();
                Features.LowLightVision.AddLowLightVision();
                Features.UniversalKi.AddUniversalKi();

            }
        }
    }
}
