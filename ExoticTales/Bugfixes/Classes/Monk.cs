using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.ElementsSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.CasterCheckers;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using System.Collections.Generic;
using System.Linq;
using ExoticTales.Config;
using ExoticTales.Extensions;
using ExoticTales.Utilities;



namespace ExoticTales.Bugfixes.Classes
{
    class Monk
    {

        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch
        {
            static bool Initialized;

            static void Postfix()
            {
                if (Initialized) return;
                Initialized = true;
                Main.LogHeader("Patching Monk");

                PatchBase();
                PatchScaledFist();
            }

            static void PatchBase()
            {

                if (ModSettings.Fixes.Monk.Base.IsDisabled("AddKiPoolBaseAbility")) { return; }

                var KiPowerFeature = Resources.GetBlueprint<BlueprintFeature>("e9590244effb4be4f830b1e3fffced13");

                var ScaledFistPowerFeature = Resources.GetBlueprint<BlueprintFeature>("ae98ab7bda409ef4bb39149a212d6732");

                string redoneMonkKiPowerName = KiPowerFeature.Name + " {g|Encyclopedia:Wisdom}Wisdom{/g}";
                string redoneScaledFistPowerName = ScaledFistPowerFeature.Name + " {g|Encyclopedia:Charisma}Charisma{/g}";

                KiPowerFeature.SetName(redoneMonkKiPowerName);
                ScaledFistPowerFeature.SetName(redoneScaledFistPowerName);



            }

            static void PatchScaledFist()
            {
                PatchScaledFistPower();

                void PatchScaledFistPower()
                {

                    if (ModSettings.Fixes.Monk.Archetypes["ScaledFist"].IsDisabled("KiPoolDescriptionFix")) { return; }

                    var ScaledFistPowerFeature = Resources.GetBlueprint<BlueprintFeature>("ae98ab7bda409ef4bb39149a212d6732");

                    string fixedScaledFistPowerDescription = ScaledFistPowerFeature.Description.Replace("Wisdom", "Charisma");

                    ScaledFistPowerFeature.SetDescription(fixedScaledFistPowerDescription);

                }

            }

        }
    }
}
