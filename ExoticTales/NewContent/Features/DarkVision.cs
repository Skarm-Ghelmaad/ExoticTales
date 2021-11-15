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
using ExoticTales.NewComponents.AreaEffects;
using ExoticTales.NewComponents.OwlcatReplacements;
using Kingmaker.Blueprints.Area;
using Kingmaker.Blueprints.Validation;
using Kingmaker.Controllers;
using Kingmaker.PubSubSystem;
using Kingmaker.Visual.Particles;
using Kingmaker.UI.UnitSettings.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
using ExoticTales.NewComponents;
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


            var iconSiD = AssetLoader.LoadInternal("Features", "Icon_ShadeInTheDarkEffect.png");
            var iconDaV = AssetLoader.LoadInternal("Features", "Icon_DarkVision.png");

            // Created the (purely graphic fx) buff to highlight creatures around you.

            BlueprintBuff ShadeInTheDarkEffect = Helpers.CreateCopy<BlueprintBuff>(ArcanistExploitShadowVeilBuff, bp => {
                var setAttackerMissChance = bp.GetComponent<SetAttackerMissChance>();
                var addStatBonus = bp.GetComponent<AddStatBonus>();
                bp.SetName("Dark Silhouette");
                bp.SetDescription("You are merely a vague shadowy silhouette in shades of gray when seen by a creature with darkvision.");
                bp.RemoveComponent(setAttackerMissChance);
                bp.RemoveComponent(addStatBonus);

            });


            // In this highly simplified version of Darkvision, this is the component used if a creature is a party member, which generates an aura around it which:
            // - Highlights all creatures (both living and dead) with a shadowvy blurry fx.
            // - Causes the desaturation of all colors within the area of darkvison.
            // - If the character is in Fog of War, causes the Fog of War to disappear.

            // These are the area of effects.

            var DarkvisionAuraArea10ft = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>("DarkvisionAuraArea10ft", bp => {
                bp.AggroEnemies = false;
                bp.AffectDead = true;
                bp.Shape = AreaEffectShape.Cylinder;
                bp.Size = 10.Feet();
                bp.SpellResistance = false;
                bp.AddComponent<AbilityAreaEffectBuff>(a =>
                {
                    a.m_Buff = ShadeInTheDarkEffect.ToReference<BlueprintBuffReference>();
                    a.Condition = ExH.CreateConditionsCheckerAnd(ExH.createContextConditionIsCaster(not: true));
                });
                bp.AddComponent<AOEConditionalRevealZone>(aoer =>
                {
                    aoer.AreaBlueprint = bp;

                });
                bp.AddComponent<SaturationAuraComponent>(sac => {
                    sac.m_SaturationAuraType = SaturationAuraType.Desaturation;
                    sac.m_Radius = (10 * 0.3048f);
                });
            });

            var DarkvisionAuraArea30ft = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>("DarkvisionAuraArea30ft", bp => {
                bp.AggroEnemies = false;
                bp.AffectDead = true;
                bp.Shape = AreaEffectShape.Cylinder;
                bp.Size = 30.Feet();
                bp.SpellResistance = false;
                bp.AddComponent<AbilityAreaEffectBuff>(a =>
                {
                    a.m_Buff = ShadeInTheDarkEffect.ToReference<BlueprintBuffReference>();
                    a.Condition = ExH.CreateConditionsCheckerAnd(ExH.createContextConditionIsCaster(not: true));
                });
                bp.AddComponent<AOEConditionalRevealZone>(aoer =>
                {
                    aoer.AreaBlueprint = bp;

                });
                bp.AddComponent<SaturationAuraComponent>(sac => {
                    sac.m_SaturationAuraType = SaturationAuraType.Desaturation;
                    sac.m_Radius = (30 * 0.3048f);
                });
            });

            var DarkvisionAuraArea60ft = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>("DarkvisionAuraArea60ft", bp => {
                bp.AggroEnemies = false;
                bp.AffectDead = true;
                bp.Shape = AreaEffectShape.Cylinder;
                bp.Size = 60.Feet();
                bp.SpellResistance = false;
                bp.AddComponent<AbilityAreaEffectBuff>(a =>
                {
                    a.m_Buff = ShadeInTheDarkEffect.ToReference<BlueprintBuffReference>();
                    a.Condition = ExH.CreateConditionsCheckerAnd(ExH.createContextConditionIsCaster(not: true));
                });
                bp.AddComponent<AOEConditionalRevealZone>(aoer =>
                {
                    aoer.AreaBlueprint = bp;

                });
                bp.AddComponent<SaturationAuraComponent>(sac => {
                    sac.m_SaturationAuraType = SaturationAuraType.Desaturation;
                    sac.m_Radius = (60 * 0.3048f);
                });
            });

            var DarkvisionAuraArea90ft = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>("DarkvisionAuraArea90ft", bp => {
                bp.AggroEnemies = false;
                bp.AffectDead = true;
                bp.Shape = AreaEffectShape.Cylinder;
                bp.Size = 90.Feet();
                bp.SpellResistance = false;
                bp.AddComponent<AbilityAreaEffectBuff>(a =>
                {
                    a.m_Buff = ShadeInTheDarkEffect.ToReference<BlueprintBuffReference>();
                    a.Condition = ExH.CreateConditionsCheckerAnd(ExH.createContextConditionIsCaster(not: true));
                });
                bp.AddComponent<AOEConditionalRevealZone>(aoer =>
                {
                    aoer.AreaBlueprint = bp;

                });
                bp.AddComponent<SaturationAuraComponent>(sac => {
                    sac.m_SaturationAuraType = SaturationAuraType.Desaturation;
                    sac.m_Radius = (90 * 0.3048f);
                });
            });

            var DarkvisionAuraArea120ft = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>("DarkvisionAuraArea120ft", bp => {
                bp.AggroEnemies = false;
                bp.AffectDead = true;
                bp.Shape = AreaEffectShape.Cylinder;
                bp.Size = 120.Feet();
                bp.SpellResistance = false;
                bp.AddComponent<AbilityAreaEffectBuff>(a =>
                {
                    a.m_Buff = ShadeInTheDarkEffect.ToReference<BlueprintBuffReference>();
                    a.Condition = ExH.CreateConditionsCheckerAnd(ExH.createContextConditionIsCaster(not: true));
                });
                bp.AddComponent<AOEConditionalRevealZone>(aoer =>
                {
                    aoer.AreaBlueprint = bp;

                });
                bp.AddComponent<SaturationAuraComponent>(sac => {
                    sac.m_SaturationAuraType = SaturationAuraType.Desaturation;
                    sac.m_Radius = (120 * 0.3048f);
                });
            });

            var DarkvisionAuraArea150ft = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>("DarkvisionAuraArea150ft", bp => {
                bp.AggroEnemies = false;
                bp.AffectDead = true;
                bp.Shape = AreaEffectShape.Cylinder;
                bp.Size = 150.Feet();
                bp.SpellResistance = false;
                bp.AddComponent<AbilityAreaEffectBuff>(a =>
                {
                    a.m_Buff = ShadeInTheDarkEffect.ToReference<BlueprintBuffReference>();
                    a.Condition = ExH.CreateConditionsCheckerAnd(ExH.createContextConditionIsCaster(not: true));
                });
                bp.AddComponent<AOEConditionalRevealZone>(aoer =>
                {
                    aoer.AreaBlueprint = bp;

                });
                bp.AddComponent<SaturationAuraComponent>(sac => {
                    sac.m_SaturationAuraType = SaturationAuraType.Desaturation;
                    sac.m_Radius = (150 * 0.3048f);
                });
            });

            var DarkvisionAuraArea180ft = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>("DarkvisionAuraArea180ft", bp => {
                bp.AggroEnemies = false;
                bp.AffectDead = true;
                bp.Shape = AreaEffectShape.Cylinder;
                bp.Size = 180.Feet();
                bp.SpellResistance = false;
                bp.AddComponent<AbilityAreaEffectBuff>(a =>
                {
                    a.m_Buff = ShadeInTheDarkEffect.ToReference<BlueprintBuffReference>();
                    a.Condition = ExH.CreateConditionsCheckerAnd(ExH.createContextConditionIsCaster(not: true));
                });
                bp.AddComponent<AOEConditionalRevealZone>(aoer =>
                {
                    aoer.AreaBlueprint = bp;

                });
                bp.AddComponent<SaturationAuraComponent>(sac => {
                    sac.m_SaturationAuraType = SaturationAuraType.Desaturation;
                    sac.m_Radius = (180 * 0.3048f);
                });
            });

            // AuraApplyingBuff -> Adds +5 circumstance bonus to Perception.
            // Checks FoW: -> Selects correct FoW or NFoW Darkvision range
            // Adds the correct desaturation effect.

            var iconDaVAb = AssetLoader.LoadInternal("Features", "Icon_DarkVisionActiveBuff.png");

            // This is the passive buff that applies the +5 perception bonus.

            var DarkvisionPassiveBuff = Helpers.CreateBuff("DarkvisionPassiveBuff", bp => {

                bp.SetName("Darkvision - Passive Effect");
                bp.SetDescription("This grants you a +5 circumstance bonus to Perception in low-light vision conditions.");
                bp.m_Icon = iconDaVAb;
                bp.AddComponent(Helpers.Create<AddStatBonus>(c => {
                    c.Stat = StatType.SkillPerception;
                    c.Descriptor = ModifierDescriptor.Circumstance;
                    c.Value = 5;
                }));
                bp.AddComponent<SpellDescriptorComponent>(c => {
                    c.Descriptor = SpellDescriptor.SightBased;
                });
            });

            // This is the active buffs meant to be used only by player characters.

            var Darkvision10ftActiveBuff = Helpers.CreateBuff("Darkvision10ftActiveBuff", bp => {

                bp.SetName("Darkvision in Use");
                bp.SetDescription("Your sight is adjusted to the current dimly-lit surroundings and you see everything through the gray shades of darkvision." +
                                   "/n This grants you a +5 circumstance bonus to perception and allows you to see any surrounding creature as shadowy silhouettes.");
                bp.m_Icon = iconDaVAb;
                bp.AddComponent<SpellDescriptorComponent>(c => {
                    c.Descriptor = SpellDescriptor.SightBased;
                });
                bp.AddComponent<AddAreaEffect>(a => {
                    a.m_AreaEffect = DarkvisionAuraArea10ft.ToReference<BlueprintAbilityAreaEffectReference>();
                });
            });

            var Darkvision30ftActiveBuff = Helpers.CreateBuff("Darkvision30ftActiveBuff", bp => {

                bp.SetName("Darkvision in Use");
                bp.SetDescription("Your sight is adjusted to the current dimly-lit surroundings and you see everything through the gray shades of darkvision." +
                                   "/n This grants you a +5 circumstance bonus to perception and allows you to see any surrounding creature as shadowy silhouettes.");
                bp.m_Icon = iconDaVAb;
                bp.AddComponent<SpellDescriptorComponent>(c => {
                    c.Descriptor = SpellDescriptor.SightBased;
                });
                bp.AddComponent<AddAreaEffect>(a => {
                    a.m_AreaEffect = DarkvisionAuraArea30ft.ToReference<BlueprintAbilityAreaEffectReference>();
                });
            });

            var Darkvision60ftActiveBuff = Helpers.CreateBuff("Darkvision60ftActiveBuff", bp => {

                bp.SetName("Darkvision in Use");
                bp.SetDescription("Your sight is adjusted to the current dimly-lit surroundings and you see everything through the gray shades of darkvision." +
                                   "/n This grants you a +5 circumstance bonus to perception and allows you to see any surrounding creature as shadowy silhouettes.");
                bp.m_Icon = iconDaVAb;
                bp.AddComponent<SpellDescriptorComponent>(c => {
                    c.Descriptor = SpellDescriptor.SightBased;
                });
                bp.AddComponent<AddAreaEffect>(a => {
                    a.m_AreaEffect = DarkvisionAuraArea60ft.ToReference<BlueprintAbilityAreaEffectReference>();
                });
            });

            var Darkvision90ftActiveBuff = Helpers.CreateBuff("Darkvision90ftActiveBuff", bp => {

                bp.SetName("Darkvision in Use");
                bp.SetDescription("Your sight is adjusted to the current dimly-lit surroundings and you see everything through the gray shades of darkvision." +
                                   "/n This grants you a +5 circumstance bonus to perception and allows you to see any surrounding creature as shadowy silhouettes.");
                bp.m_Icon = iconDaVAb;
                bp.AddComponent<SpellDescriptorComponent>(c => {
                    c.Descriptor = SpellDescriptor.SightBased;
                });
                bp.AddComponent<AddAreaEffect>(a => {
                    a.m_AreaEffect = DarkvisionAuraArea90ft.ToReference<BlueprintAbilityAreaEffectReference>();
                });
            });

            var Darkvision120ftActiveBuff = Helpers.CreateBuff("Darkvision120ftActiveBuff", bp => {

                bp.SetName("Darkvision in Use");
                bp.SetDescription("Your sight is adjusted to the current dimly-lit surroundings and you see everything through the gray shades of darkvision." +
                                   "/n This grants you a +5 circumstance bonus to perception and allows you to see any surrounding creature as shadowy silhouettes.");
                bp.m_Icon = iconDaVAb;
                bp.AddComponent<SpellDescriptorComponent>(c => {
                    c.Descriptor = SpellDescriptor.SightBased;
                });
                bp.AddComponent<AddAreaEffect>(a => {
                    a.m_AreaEffect = DarkvisionAuraArea120ft.ToReference<BlueprintAbilityAreaEffectReference>();
                });
            });

            var Darkvision150ftActiveBuff = Helpers.CreateBuff("Darkvision150ftActiveBuff", bp => {

                bp.SetName("Darkvision in Use");
                bp.SetDescription("Your sight is adjusted to the current dimly-lit surroundings and you see everything through the gray shades of darkvision." +
                                   "/n This grants you a +5 circumstance bonus to perception and allows you to see any surrounding creature as shadowy silhouettes.");
                bp.m_Icon = iconDaVAb;
                bp.AddComponent<SpellDescriptorComponent>(c => {
                    c.Descriptor = SpellDescriptor.SightBased;
                });
                bp.AddComponent<AddAreaEffect>(a => {
                    a.m_AreaEffect = DarkvisionAuraArea150ft.ToReference<BlueprintAbilityAreaEffectReference>();
                });
            });

            var Darkvision180ftActiveBuff = Helpers.CreateBuff("Darkvision180ftActiveBuff", bp => {

                bp.SetName("Darkvision in Use");
                bp.SetDescription("Your sight is adjusted to the current dimly-lit surroundings and you see everything through the gray shades of darkvision." +
                                   "/n This grants you a +5 circumstance bonus to perception and allows you to see any surrounding creature as shadowy silhouettes.");
                bp.m_Icon = iconDaVAb;
                bp.AddComponent<SpellDescriptorComponent>(c => {
                    c.Descriptor = SpellDescriptor.SightBased;
                });
                bp.AddComponent<AddAreaEffect>(a => {
                    a.m_AreaEffect = DarkvisionAuraArea180ft.ToReference<BlueprintAbilityAreaEffectReference>();
                });
            });

            // These are the suppressors which is used both to disable the darkvision in daylight and when the activable ability get clicked.

            var iconDaVSb = AssetLoader.LoadInternal("Features", "Icon_DarkVisionSuppressedBuff.png");
            var DarkVisionSuppressedBuffActive = Helpers.CreateBuff("DarkVisionSuppressedBuffActive", bp => {      // This deactivates both the passive bonus to Perception and the graphic fx.
                bp.SetName("Darkvision not in Use");
                bp.SetDescription("Your sight purposefully disadapted your vision to darkness by looking directly at a light source" +
                   "/n  in anticipation for a brighter lit environment or a sudden flash.");
                bp.m_Icon = iconDaVSb;
                bp.AddComponent(Helpers.Create<SuppressBuffs>(c => {
                    c.m_Buffs = new BlueprintBuffReference[] { DarkvisionPassiveBuff.ToReference<BlueprintBuffReference>(), Darkvision10ftActiveBuff.ToReference<BlueprintBuffReference>(), Darkvision30ftActiveBuff.ToReference<BlueprintBuffReference>(), Darkvision60ftActiveBuff.ToReference<BlueprintBuffReference>(), Darkvision90ftActiveBuff.ToReference<BlueprintBuffReference>(), Darkvision120ftActiveBuff.ToReference<BlueprintBuffReference>(), Darkvision150ftActiveBuff.ToReference<BlueprintBuffReference>(), Darkvision180ftActiveBuff.ToReference<BlueprintBuffReference>(), };
                }));
            });

            var DarkVisionSuppressedBuffPassive = Helpers.CreateBuff("DarkVisionSuppressedBuffPassive", bp => {  // This deactivates both the passive bonus to Perception and the graphic fx AND the active deactivator buff (to avoid problems).
                bp.SetName("Darkvision not in Use");
                bp.SetDescription("Your sight is adjusted to the current brightly-lit surroundings.");
                bp.m_Icon = iconDaVSb;
                bp.AddComponent(Helpers.Create<SuppressBuffs>(c => {
                    c.m_Buffs = new BlueprintBuffReference[] { DarkvisionPassiveBuff.ToReference<BlueprintBuffReference>(), DarkVisionSuppressedBuffActive.ToReference<BlueprintBuffReference>(), Darkvision10ftActiveBuff.ToReference<BlueprintBuffReference>(), Darkvision30ftActiveBuff.ToReference<BlueprintBuffReference>(), Darkvision60ftActiveBuff.ToReference<BlueprintBuffReference>(), Darkvision90ftActiveBuff.ToReference<BlueprintBuffReference>(), Darkvision120ftActiveBuff.ToReference<BlueprintBuffReference>(), Darkvision150ftActiveBuff.ToReference<BlueprintBuffReference>(), Darkvision180ftActiveBuff.ToReference<BlueprintBuffReference>(), };
                }));
            });

            var DarkVisionSuppressedBuffIntrinsic = Helpers.CreateBuff("DarkVisionSuppressedBuffIntrinsic", bp => {  // This deactivates only the graphic fx and is meant for NPCs.
                bp.SetName("NPC Darkvision");
                bp.SetDescription("Your darkvision grants you some limited bonus in low-lit condition, but without any visible exterior impact.");
                bp.m_Icon = iconDaVSb;
                bp.AddComponent(Helpers.Create<SuppressBuffs>(c => {
                    c.m_Buffs = new BlueprintBuffReference[] { Darkvision10ftActiveBuff.ToReference<BlueprintBuffReference>(), Darkvision30ftActiveBuff.ToReference<BlueprintBuffReference>(), Darkvision60ftActiveBuff.ToReference<BlueprintBuffReference>(), Darkvision90ftActiveBuff.ToReference<BlueprintBuffReference>(), Darkvision120ftActiveBuff.ToReference<BlueprintBuffReference>(), Darkvision150ftActiveBuff.ToReference<BlueprintBuffReference>(), Darkvision180ftActiveBuff.ToReference<BlueprintBuffReference>(), };
                }));
            });

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

            });

            // These are the various versions of the Darkvision feature, adding Darkvision for different ranges.

            var Darkvision10ftFeature = Helpers.CreateBlueprint<BlueprintFeature>("Darkvision10ftFeature", bp => {
                    bp.SetName("Darkvision [10 feet]");
                    bp.SetDescription("You can see perfectly in the dark up to 10 feet. /n When in dimly-lit environment or in darkness, however, your sight is limited to shades of gray.");
                    bp.m_Icon = iconDaV;
                    bp.Groups = new FeatureGroup[] { FeatureGroup.Racial };
                    bp.IsClassFeature = true;
                    bp.Ranks = 1;
                    bp.AddComponent(Helpers.Create <AddBuffInDaylight>(c => {

                        c.EffectBuff = DarkVisionSuppressedBuffPassive;
                        c.EnhancedOnConcealment = false;

                    }));
                    bp.AddComponent(Helpers.Create<AddBuffIfIsPartyMember>(c => {

                        c.EffectBuff = DarkVisionSuppressedBuffIntrinsic;
                        c.Not = true;

                    }));
                    bp.AddComponent(Helpers.Create<AuraFeatureComponent>(c => {

                        c.m_Buff = DarkvisionAuraArea10ft.ToReference<BlueprintBuffReference>();

                    }));

            });

            var Darkvision30ftFeature = Helpers.CreateBlueprint<BlueprintFeature>("Darkvision10ftFeature", bp => {
                bp.SetName("Darkvision [30 feet]");
                bp.SetDescription("You can see perfectly in the dark up to 30 feet. /n When in dimly-lit environment or in darkness, however, your sight is limited to shades of gray.");
                bp.m_Icon = iconDaV;
                bp.Groups = new FeatureGroup[] { FeatureGroup.Racial };
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.AddComponent(Helpers.Create<AddBuffInDaylight>(c => {

                    c.EffectBuff = DarkVisionSuppressedBuffPassive;
                    c.EnhancedOnConcealment = false;

                }));
                bp.AddComponent(Helpers.Create<AddBuffIfIsPartyMember>(c => {

                    c.EffectBuff = DarkVisionSuppressedBuffIntrinsic;
                    c.Not = true;

                }));
                bp.AddComponent(Helpers.Create<AuraFeatureComponent>(c => {

                    c.m_Buff = DarkvisionAuraArea30ft.ToReference<BlueprintBuffReference>();

                }));

            });


            var Darkvision60ftFeature = Helpers.CreateBlueprint<BlueprintFeature>("Darkvision60ftFeature", bp => {
                bp.SetName("Darkvision [60 feet]");
                bp.SetDescription("You can see perfectly in the dark up to 60 feet. /n When in dimly-lit environment or in darkness, however, your sight is limited to shades of gray.");
                bp.m_Icon = iconDaV;
                bp.Groups = new FeatureGroup[] { FeatureGroup.Racial };
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.AddComponent(Helpers.Create<AddBuffInDaylight>(c => {

                    c.EffectBuff = DarkVisionSuppressedBuffPassive;
                    c.EnhancedOnConcealment = false;

                }));
                bp.AddComponent(Helpers.Create<AddBuffIfIsPartyMember>(c => {

                    c.EffectBuff = DarkVisionSuppressedBuffIntrinsic;
                    c.Not = true;

                }));
                bp.AddComponent(Helpers.Create<AuraFeatureComponent>(c => {

                    c.m_Buff = DarkvisionAuraArea60ft.ToReference<BlueprintBuffReference>();

                }));

            });

            var Darkvision90ftFeature = Helpers.CreateBlueprint<BlueprintFeature>("Darkvision90ftFeature", bp => {
                bp.SetName("Darkvision [90 feet]");
                bp.SetDescription("You can see perfectly in the dark up to 90 feet. /n When in dimly-lit environment or in darkness, however, your sight is limited to shades of gray.");
                bp.m_Icon = iconDaV;
                bp.Groups = new FeatureGroup[] { FeatureGroup.Racial };
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.AddComponent(Helpers.Create<AddBuffInDaylight>(c => {

                    c.EffectBuff = DarkVisionSuppressedBuffPassive;
                    c.EnhancedOnConcealment = false;

                }));
                bp.AddComponent(Helpers.Create<AddBuffIfIsPartyMember>(c => {

                    c.EffectBuff = DarkVisionSuppressedBuffIntrinsic;
                    c.Not = true;

                }));
                bp.AddComponent(Helpers.Create<AuraFeatureComponent>(c => {

                    c.m_Buff = DarkvisionAuraArea90ft.ToReference<BlueprintBuffReference>();

                }));

            });


            var Darkvision120ftFeature = Helpers.CreateBlueprint<BlueprintFeature>("Darkvision120ftFeature", bp => {
                bp.SetName("Darkvision [120 feet]");
                bp.SetDescription("You can see perfectly in the dark up to 120 feet. /n When in dimly-lit environment or in darkness, however, your sight is limited to shades of gray.");
                bp.m_Icon = iconDaV;
                bp.Groups = new FeatureGroup[] { FeatureGroup.Racial };
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.AddComponent(Helpers.Create<AddBuffInDaylight>(c => {

                    c.EffectBuff = DarkVisionSuppressedBuffPassive;
                    c.EnhancedOnConcealment = false;

                }));
                bp.AddComponent(Helpers.Create<AddBuffIfIsPartyMember>(c => {

                    c.EffectBuff = DarkVisionSuppressedBuffIntrinsic;
                    c.Not = true;

                }));
                bp.AddComponent(Helpers.Create<AuraFeatureComponent>(c => {

                    c.m_Buff = DarkvisionAuraArea120ft.ToReference<BlueprintBuffReference>();

                }));

            });

            var Darkvision150ftFeature = Helpers.CreateBlueprint<BlueprintFeature>("Darkvision150ftFeature", bp => {
                bp.SetName("Darkvision [150 feet]");
                bp.SetDescription("You can see perfectly in the dark up to 150 feet. /n When in dimly-lit environment or in darkness, however, your sight is limited to shades of gray.");
                bp.m_Icon = iconDaV;
                bp.Groups = new FeatureGroup[] { FeatureGroup.Racial };
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.AddComponent(Helpers.Create<AddBuffInDaylight>(c => {

                    c.EffectBuff = DarkVisionSuppressedBuffPassive;
                    c.EnhancedOnConcealment = false;

                }));
                bp.AddComponent(Helpers.Create<AddBuffIfIsPartyMember>(c => {

                    c.EffectBuff = DarkVisionSuppressedBuffIntrinsic;
                    c.Not = true;

                }));
                bp.AddComponent(Helpers.Create<AuraFeatureComponent>(c => {

                    c.m_Buff = DarkvisionAuraArea150ft.ToReference<BlueprintBuffReference>();

                }));

            });

            var Darkvision180ftFeature = Helpers.CreateBlueprint<BlueprintFeature>("Darkvision180ftFeature", bp => {
                bp.SetName("Darkvision [180 feet]");
                bp.SetDescription("You can see perfectly in the dark up to 180 feet. /n When in dimly-lit environment or in darkness, however, your sight is limited to shades of gray.");
                bp.m_Icon = iconDaV;
                bp.Groups = new FeatureGroup[] { FeatureGroup.Racial };
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.AddComponent(Helpers.Create<AddBuffInDaylight>(c => {

                    c.EffectBuff = DarkVisionSuppressedBuffPassive;
                    c.EnhancedOnConcealment = false;

                }));
                bp.AddComponent(Helpers.Create<AddBuffIfIsPartyMember>(c => {

                    c.EffectBuff = DarkVisionSuppressedBuffIntrinsic;
                    c.Not = true;

                }));
                bp.AddComponent(Helpers.Create<AuraFeatureComponent>(c => {

                    c.m_Buff = DarkvisionAuraArea180ft.ToReference<BlueprintBuffReference>();

                }));

            });

            if (ModSettings.AddedContent.NewSystems.IsDisabled("ShadowAndDarkness")) { return; }


        }
    }
}
