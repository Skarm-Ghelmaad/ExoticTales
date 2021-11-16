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
using Kingmaker.UI.UnitSettings.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.ResourceLinks;
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
            var iconDaVAb = AssetLoader.LoadInternal("Features", "Icon_DarkVisionActiveBuff.png");

            // The (purely graphic fx) buff to highlight creatures around you.

            var DarkvisionAuraEffectBuff = Helpers.CreateBlueprint<BlueprintBuff>("DarkvisionAuraEffectBuff", bp => {
                bp.SetName("Dark Silhouette");
                bp.SetDescription("You are merely a vague shadowy silhouette in shades of gray when seen by a creature with darkvision.");
                bp.m_Icon = iconSiD;
                bp.IsClassFeature = true;
                bp.FxOnStart = ExH.createPrefabLink("ea8ddc3e798aa25458e2c8a15e484c68"); //Arcanist Exploit Shadow Veil Starting Fx
                bp.FxOnRemove = ExH.createPrefabLink(""); //Create an empty prefab link.
            });



            // The aura area of effect (applying the graphic buff to anyone in range).

            var DarkvisionAuraArea60ft = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>("DarkvisionAuraArea60ft", bp => {
                bp.AggroEnemies = false;
                bp.AffectDead = true;
                bp.Shape = AreaEffectShape.Cylinder;
                bp.Size = new Feet() { m_Value = 60 };
                bp.AddComponent(ExH.CreateAreaEffectRunAction(ExH.CreateApplyBuff(buff: DarkvisionAuraEffectBuff, duration: null, fromSpell: false, dispellable: false, toCaster: false, asChild: false, permanent: true), ExH.createContextActionRemoveBuff(DarkvisionAuraEffectBuff)));

        });

            //bp.AddComponent(ExH.CreateAreaEffectRunAction(ExH.CreateApplyBuff(buff: DarkvisionAuraEffectBuff, duration: null, fromSpell: false, dispellable: false, toCaster: false, asChild: false, permanent: true), ExH.createContextActionRemoveBuff(DarkvisionAuraEffectBuff)));               

            //ExH.CreateAreaEffectRunAction(ExH.CreateConditional(ExH.CreateConditionsCheckerAnd(ExH.createContextConditionIsCaster(not: true)), ExH.CreateApplyBuff(buff: DarkvisionAuraEffectBuff, duration: null, fromSpell: false, dispellable: false, toCaster: false, asChild: false, permanent: true), null)


            /* bp.AddComponent<AbilityAreaEffectBuff>(a =>
            {
                a.m_Buff = DarkvisionAuraEffectBuff.ToReference<BlueprintBuffReference>();
                a.Condition = new ConditionsChecker()
                {
                    Conditions = new Condition[] {
                            ExH.createContextConditionIsCaster(true)
                        }
                };

            }); */


            // new ContextConditionIsCaster() {Not = true},

            // This is the active (aura-attaching) buff that applies the area of effect to the character.

            var Darkvision60ftActiveBuff = Helpers.CreateBlueprint<BlueprintBuff>("Darkvision60ftActiveBuff", bp => {

                bp.SetName("Darkvision in Use");
                bp.SetDescription("Your sight is adjusted to the current dimly-lit surroundings and you see everything through the gray shades of darkvision." +
                                   "\n This grants you a +5 circumstance bonus to perception and allows you to see any surrounding creature as shadowy silhouettes.");
                bp.IsClassFeature = true;
                bp.m_Icon = iconDaVAb;
                bp.AddComponent<AddAreaEffect>(a => {
                    a.m_AreaEffect = DarkvisionAuraArea60ft.ToReference<BlueprintAbilityAreaEffectReference>();

                });
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
                bp.AddComponent(Helpers.Create<AuraFeatureComponent>(c => {

                    c.m_Buff = Darkvision60ftActiveBuff.ToReference<BlueprintBuffReference>();

                }));

            });


            // Old code

            // a.Condition = ExH.CreateConditionsCheckerOr(ExH.createContextConditionIsCaster(not: true));

            /* var DarkvisionAuraArea60ft = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>("DarkvisionAuraArea60ft", bp => {
                bp.AggroEnemies = false;
                bp.Shape = AreaEffectShape.Cylinder;
                bp.Size = 60.Feet();
                bp.AddComponent<AbilityAreaEffectBuff>(a =>
                {
                    a.m_Buff = ShadeInTheDarkEffect.ToReference<BlueprintBuffReference>();
                    a.Condition = ExH.CreateConditionsCheckerOr(ExH.createContextConditionIsCaster(not : true));
                });
            }); */



            if (ModSettings.AddedContent.NewSystems.IsDisabled("ShadowAndDarkness")) { return; }


        }
    }
}
