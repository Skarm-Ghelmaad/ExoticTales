using System;
using Kingmaker.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.Utility;
using ExoticTales.Config;
using ExoticTales.Extensions;
using ExoticTales.Utilities;
using HarmonyLib;
using Kingmaker.Blueprints.Classes;
using Kingmaker.UnitLogic.ActivatableAbilities.Restrictions;
using Kingmaker.UI.UnitSettings.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.ResourceLinks;
using ExoticTales.NewComponents;
using ExoticTales.NewComponents.AreaEffects;
using HlP = ExoticTales.Utilities.Helpers;
using ExH = ExoticTales.Utilities.ExpandedHelpers;

namespace ExoticTales.NewContent.Features
{
    class LowLightVision
    {

        public static void AddLowLightVision()
        {


            var iconLLV = AssetLoader.LoadInternal("Features", "Icon_LowLightVision.png");

        // This is the passive buff that applies the +2 perception bonus.

        var LowLightVisionPassiveBuff = Helpers.CreateBuff("LowLightVisionPassiveBuff", bp => {

            bp.SetName("Low-Light Vision"); //Note: This buff is the primary buff set by the aura component, it is used by NPCs and does not show as description.
            bp.SetDescription("This grants you a +2 circumstance bonus to perception in dim light.");
            bp.m_Icon = iconLLV;
            bp.SetBuffFlags(BlueprintBuff.Flags.HiddenInUi);
            bp.AddComponent(Helpers.Create<AddStatBonus>(c => {
                c.Stat = StatType.SkillPerception;
                c.Descriptor = ModifierDescriptor.Circumstance;
                c.Value = 2;
            }));
            bp.AddComponent<SpellDescriptorComponent>(c => {
                c.Descriptor = SpellDescriptor.SightBased;
            });
        });

            var LowLightVisionFeature = Helpers.CreateBlueprint<BlueprintFeature>("LowLightVisionFeature", bp => {
                bp.SetName("Low-Light Vision");
                bp.SetDescription("You can see twice as far as humans in conditions of dim light. \n When in dimly-lit environment or in darkness, however, you gain a +2 circumstance bonus to Perception.");
                bp.m_Icon = iconLLV;
                bp.Groups = new FeatureGroup[] { FeatureGroup.Racial };
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.AddComponent(Helpers.Create<NocturnalAuraFeatureComponent>(c => {

                    c.m_EffectBuff = LowLightVisionPassiveBuff.ToReference<BlueprintBuffReference>();
                    c.exactingCheck = false;
                    c.weatherCheck = false;
                    c.checkCaster = false;
                    c.triggeringBuffs = false;
                    c.triggeringFacts = false;
                    c.suppressingBuffs = false;
                    c.suppressingFacts = false;

                }));

            });




        }

    }
}
