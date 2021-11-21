using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Blueprints;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Parts;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Craft;
using Kingmaker.ElementsSystem;
using static Kingmaker.ElementsSystem.Condition;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.UnitLogic.Abilities.Components.TargetCheckers;
using Kingmaker.UnitLogic.Abilities.Components.CasterCheckers;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.Utility;
using ExoticTales.Config;
using ExoticTales.Extensions;
using ExoticTales.Utilities;
using HarmonyLib;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Blueprints.Facts;
using UnityEngine;
using UnityEngine.Serialization;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.Visual.Animation.Kingmaker.Actions;
using ExoticTales.NewComponents.OwlcatReplacements;
using Kingmaker.Blueprints.Area;
using Kingmaker.Blueprints.Validation;
using Kingmaker.Controllers;
using Kingmaker.PubSubSystem;
using Kingmaker.Visual.Particles;
using Kingmaker.UnitLogic.ActivatableAbilities.Restrictions;
using Kingmaker.UI.UnitSettings.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.ResourceLinks;
using ExoticTales.NewComponents;
using ExoticTales.NewComponents.AreaEffects;
using static ExoticTales.NewUnitParts.CustomStatTypes;
using HlP = ExoticTales.Utilities.Helpers;
using ExH = ExoticTales.Utilities.ExpandedHelpers;





namespace ExoticTales.NewContent.Features
{
    class DarkVision
    {



        public static void AddDarkVision()
        {

            // Darkvision is extremely simplified and works in the following way:
            // - DarkvisionRange is a custom stat.
            // - A specific DarkvisionFeature adds adds value to this stat and adds a "checker buff" which applies two buffs when not in daylight:
            // - One grants a +5 circumstance bonus on Perception checks.
            // - The other is only applied if one is a party member and generates an aura buff with a range set according to the DarkvisionRange stat (in steps of 10 feet). 
            // The aura does the following:
            // - Covers all creatures with a blur fx (so it highlights them, but in a "shadowy" way).
            // - Generates a desaturated aura effect.
            // - If the caster is in fog of war, generates a fog of war revealer.
            // - Moreover the Darkvision feature will add an activable ability to suppress darkvision.

            BlueprintBuff ArcanistExploitShadowVeilBuff = Resources.GetBlueprint<BlueprintBuff>("5ceedff361efd4c4eb8e8369c13b03ea"); // Shadow Veil Arcanist Exploit (closest in appearance to my desired effects)
            BlueprintAbilityAreaEffect AuraOfFaithArea = Resources.GetBlueprint<BlueprintAbilityAreaEffect>("7fc8dbff8ba688d4b864b1c1be45fe97"); // Aura Of Faith seems close enough to not require any special adjustment.
            BlueprintBuff AuraOfFaithBuff = Resources.GetBlueprint<BlueprintBuff>("d4aa6633c583f304cbd4b02d0572af28"); // Aura Of Faith seems close enough to not require any special adjustment.


            var iconDAEB = AssetLoader.LoadInternal("Features", "Icon_DarkvisionAuraEffectBuff.png");
            var iconDAESB = AssetLoader.LoadInternal("Features", "Icon_DarkvisionAuraSecondaryEffectBuff.png");
            var iconDaV = AssetLoader.LoadInternal("Features", "Icon_DarkVision.png");
            var iconDaVAb = AssetLoader.LoadInternal("Features", "Icon_DarkVisionActiveBuff.png");

            // The (purely graphic fx) buff to highlight creatures around you.

            var DarkvisionAuraEffectBuff = Helpers.CreateBlueprint<BlueprintBuff>("DarkvisionAuraEffectBuff", bp => {
                bp.SetName("Dark Silhouette");
                bp.SetDescription("You are merely a vague shadowy silhouette in shades of gray when seen by a creature with darkvision.");
                bp.m_Icon = iconDAEB;
                bp.IsClassFeature = true;
                bp.SetBuffFlags(BlueprintBuff.Flags.StayOnDeath| BlueprintBuff.Flags.HiddenInUi);
                bp.FxOnStart = ExH.createPrefabLink("ea8ddc3e798aa25458e2c8a15e484c68"); //Arcanist Exploit Shadow Veil Starting Fx
                bp.FxOnRemove = ExH.createPrefabLink(""); //Create an empty prefab link.
                bp.AddComponent<SpellDescriptorComponent>(c => {
                    c.Descriptor = SpellDescriptor.SightBased;
                });
            });

            var DarkvisionAuraSecondaryEffectBuff = Helpers.CreateBlueprint<BlueprintBuff>("DarkvisionAuraSecondaryEffectBuff", bp => {
                bp.SetName("World in Shades of Grey");
                bp.SetDescription("Darkvision causes the world to be seen in shades of gray.");
                bp.m_Icon = iconDAESB;
                bp.IsClassFeature = true;
                bp.SetBuffFlags(BlueprintBuff.Flags.StayOnDeath | BlueprintBuff.Flags.HiddenInUi);
                bp.FxOnStart = ExH.createPrefabLink("3bf15930463caa643b2706cd1d185f25"); //I am adding the AreshkagalDialogFx to create an "eerie shadow world" effect.
                bp.FxOnRemove = ExH.createPrefabLink(""); //Create an empty prefab link.
                bp.AddComponent<SpellDescriptorComponent>(c => {
                    c.Descriptor = SpellDescriptor.SightBased;
                });
            });

            // The aura area of effect (applying the graphic buff to anyone in range).

            var DarkvisionAuraArea60ft = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>("DarkvisionAuraArea60ft", bp => {
                bp.AggroEnemies = false;
                bp.AffectDead = true;
                bp.Shape = AreaEffectShape.Cylinder;
                bp.Size = new Feet() { m_Value = 63 }; // The paladin's auras are technically 10 feet, but add 3 feet, so I did the same.
                bp.Fx = new PrefabLink();
                bp.AddComponent(Helpers.Create<AbilityAreaEffectBuff>(a => { a.m_Buff = DarkvisionAuraEffectBuff.ToReference<BlueprintBuffReference>(); a.Condition = ExH.CreateConditionsCheckerAnd(ExH.createContextConditionIsCaster(not: true)); }));
                bp.AddComponent<AOEConditionalRevealZone>(aoer =>
                {
                    aoer.m_AreaBlueprint = bp.ToReference<BlueprintAbilityAreaEffectReference>();

                });
            });

            var DarkvisionAuraSecondaryArea60ft = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>("DarkvisionAuraSecondaryArea60ft", bp => {
                bp.AggroEnemies = false;
                bp.AffectDead = true;
                bp.Shape = AreaEffectShape.Cylinder;
                bp.Size = new Feet() { m_Value = 13 };  // Seems that the buff has a radius of about 67 feet, so I gave this aura only to those within 2 squares of the character.
                bp.Fx = new PrefabLink();
                bp.AddComponent(Helpers.Create<AbilityAreaEffectBuff>(a => { a.m_Buff = DarkvisionAuraSecondaryEffectBuff.ToReference<BlueprintBuffReference>(); a.Condition = ExH.CreateConditionsCheckerAnd(null); }));

            });

                        // This is the active (aura-attaching) buff that applies the area of effect to the character.

            var Darkvision60ftActiveBuff = Helpers.CreateBlueprint<BlueprintBuff>("Darkvision60ftActiveBuff", bp => {

                bp.SetName("Darkvision in Use");
                bp.SetDescription("Your sight is adjusted to the current dimly-lit surroundings and you see everything through the gray shades of darkvision." +
                                   "\n This grants you a +5 circumstance bonus to perception and allows you to see any surrounding creature as shadowy silhouettes.");
                bp.IsClassFeature = true;
                bp.m_Icon = iconDaVAb;
                bp.SetBuffFlags(BlueprintBuff.Flags.HiddenInUi);
                bp.AddComponent<AddAreaEffect>(a => {
                    a.m_AreaEffect = DarkvisionAuraArea60ft.ToReference<BlueprintAbilityAreaEffectReference>();
                });
                bp.AddComponent<AddAreaEffect>(a => {
                    a.m_AreaEffect = DarkvisionAuraSecondaryArea60ft.ToReference<BlueprintAbilityAreaEffectReference>();

                });
            });

            // These are the suppressors which is used both to disable the darkvision in daylight and when the activable ability get clicked.

            var iconDaVSb = AssetLoader.LoadInternal("Features", "Icon_DarkVisionSuppressedBuff.png");
            var DarkVisionSuppressedBuffActive = Helpers.CreateBuff("DarkVisionSuppressedBuffActive", bp => {      // This deactivates both the passive bonus to Perception and the graphic fx.
                bp.SetName("Darkvision not in Use");
                bp.SetDescription("Your sight purposefully disadapted your vision to darkness by looking directly at a light source" +
                   "\n  in anticipation for a brighter lit environment or a sudden flash.");
                bp.m_Icon = iconDaVSb;
            });


            var DarkVisionSuppressedBuffNPC = Helpers.CreateBuff("DarkVisionSuppressedBuffNPC", bp => {  // This deactivates the graphic fx AND the active deactivator buff (to avoid problems).
                bp.SetName("Darkvision is Subjective");
                bp.SetDescription("Darkvision is restrited to those who have it.");
                bp.m_Icon = iconDaVSb;
                bp.SetBuffFlags(BlueprintBuff.Flags.HiddenInUi);
            });


            var DarkVisionEnableToggeableBuff = Helpers.CreateBuff("DarkVisionEnableToggeableBuff", bp => {  // This enables the toggeable ability.
                bp.SetName("Darkvision - Toggeable Ability Enabled");
                bp.SetDescription("The toggeable abilty for darkvision is enabled.");
                bp.m_Icon = iconDaVAb;
                bp.SetBuffFlags(BlueprintBuff.Flags.HiddenInUi);
            });

            // This is the passive buff that applies the +5 perception bonus.

            var DarkvisionPassiveBuff = Helpers.CreateBuff("DarkvisionPassiveBuff", bp => {

                bp.SetName("Darkvision - Passive Effect"); //Note: This buff is the primary buff set by the aura component, it is used by NPCs and does not show as description.
                bp.SetDescription("This grants you a +5 circumstance bonus to perception and allows you to see any surrounding creature as shadowy silhouettes.");
                bp.m_Icon = iconDaVAb;
                bp.SetBuffFlags(BlueprintBuff.Flags.HiddenInUi);
                bp.AddComponent(Helpers.Create<AddStatBonus>(c => {
                    c.Stat = StatType.SkillPerception;
                    c.Descriptor = ModifierDescriptor.Circumstance;
                    c.Value = 5;
                }));
                bp.AddComponent<SpellDescriptorComponent>(c => {
                    c.Descriptor = SpellDescriptor.SightBased;
                });
                bp.AddComponent(Helpers.Create<AddBuffOnNotPartyMemberSelected>(ab => { ab.m_EffectBuff = DarkVisionSuppressedBuffNPC.ToReference<BlueprintBuffReference>(); }));

                });


            // ExH.CreateConditionsCheckerOr(ExH.createContextConditionIsSelectedPartyMember(true)), ExH.CreateApplyBuff(DarkVisionSuppressedBuffNPC, null, false, false, false, false, true), null)


            // This is the activable ability which suppressess Darkvision when clicked.

            var DarkVisionSuppressToggleAbility = Helpers.CreateBlueprint<BlueprintActivatableAbility>("DarkVisionSuppressToggleAbility", bp => {
                bp.SetName("Suppress Darkvision");
                bp.SetDescription("You temporarily disadapt your vision to darkness by staring directly at a light source, shifting from darkvision to normal vision.");
                bp.m_Icon = iconDaVSb;
                bp.m_Buff = DarkVisionSuppressedBuffActive.ToReference<BlueprintBuffReference>();
                bp.IsOnByDefault = false;
                bp.DoNotTurnOffOnRest = false;
                bp.AddComponent<ActionPanelLogic>(apl => {
                    apl.AutoFillConditions = ExH.CreateConditionsCheckerAnd(ExH.createContextConditionIsPartyMember());
                });
                bp.DeactivateIfCombatEnded = false;
                bp.DeactivateAfterFirstRound = false;
                bp.DeactivateImmediately = false;
                bp.IsTargeted = false;
                bp.DeactivateIfOwnerDisabled = false;
                bp.DeactivateIfOwnerUnconscious = false;
                bp.OnlyInCombat = false;
                bp.ActivationType = AbilityActivationType.Immediately;
                bp.AddComponent<RestrictionHasFact>( bp => { bp.m_Feature = DarkVisionEnableToggeableBuff.ToReference<BlueprintUnitFactReference>(); });
            });

            // These are the various versions of the Darkvision feature, adding Darkvision for different ranges.

            //OK!!

            var Darkvision60ftFeature = Helpers.CreateBlueprint<BlueprintFeature>("Darkvision60ftFeature", bp => {
                bp.SetName("Darkvision [60 feet]");
                bp.SetDescription("You can see perfectly in the dark up to 60 feet. \n When in dimly-lit environment or in darkness, however, your sight is limited to shades of gray.");
                bp.m_Icon = iconDaV;
                bp.Groups = new FeatureGroup[] { FeatureGroup.Racial };
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.AddComponent(Helpers.Create<NocturnalAuraFeatureComponent>(c => {

                    c.m_EffectBuff = DarkvisionPassiveBuff.ToReference<BlueprintBuffReference>();
                    c.exactingCheck = false;
                    c.weatherCheck = false;
                    c.checkCaster = false;
                    c.triggeringBuffs = false;
                    c.triggeringFacts = false;
                    c.suppressingBuffs = false;
                    c.suppressingFacts = false;

                }));
                bp.AddComponent(Helpers.Create<NocturnalAuraFeatureComponent> (c => {
                    c.m_EffectBuff = Darkvision60ftActiveBuff.ToReference<BlueprintBuffReference>();
                    c.exactingCheck = false;
                    c.weatherCheck = false;
                    c.checkCaster = true;
                    c.triggeringBuffs = false;
                    c.triggeringFacts = false;
                    c.suppressingBuffs = true;
                    c.m_SuppressingBuffs = new BlueprintBuffReference[] { DarkVisionSuppressedBuffNPC.ToReference<BlueprintBuffReference>(), DarkVisionSuppressedBuffActive.ToReference<BlueprintBuffReference>() };

                }));
                bp.AddComponent(Helpers.Create<NocturnalAuraFeatureComponent>(c => {
                    c.m_EffectBuff = DarkVisionEnableToggeableBuff.ToReference<BlueprintBuffReference>();
                    c.exactingCheck = false;
                    c.weatherCheck = false;
                    c.checkCaster = true;
                    c.triggeringBuffs = false;
                    c.triggeringFacts = false;
                    c.suppressingBuffs = true;
                    c.m_SuppressingBuffs = new BlueprintBuffReference[] { DarkVisionSuppressedBuffNPC.ToReference<BlueprintBuffReference>() };

                }));
                bp.AddComponent<AddFacts>(c => {
                    c.m_Facts = new BlueprintUnitFactReference[] {
                            DarkVisionSuppressToggleAbility.ToReference<BlueprintUnitFactReference>(),
                        };
                });

            });


            if (ModSettings.AddedContent.NewSystems.IsDisabled("ShadowAndDarkness")) { return; }


        }
    }
}
