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




namespace ExoticTales.NewContent.Features
{
    class DarkVision
    {



        public static void AddDarkVision()
        {

            // Darkvision is extremely simplified and works in the following way:
            // - For all creatures it grants them a +5 circumstance bonus on Perception checks when not in daylight, but for anyone who is not a party member,
            //   apart from that, is merely a tag.
            // - For party members it generates an aura with a radius equal to its vision range which covers all creatures with a blur fx (so it highlights them,
            //   but in a "shadowy" way AND generates a fog of war revealer.

            BlueprintBuff ArcanistExploitShadowVeilBuff = Resources.GetBlueprint<BlueprintBuff>("5ceedff361efd4c4eb8e8369c13b03ea"); // Shadow Veil Arcanist Exploit (closest in appearance to my desired effects)
            BlueprintAbilityAreaEffect AuraOfFaithArea = Resources.GetBlueprint<BlueprintAbilityAreaEffect>("7fc8dbff8ba688d4b864b1c1be45fe97"); // Aura Of Faith seems close enough to not require any special adjustment.
            BlueprintBuff AuraOfFaithBuff = Resources.GetBlueprint<BlueprintBuff>("d4aa6633c583f304cbd4b02d0572af28"); // Aura Of Faith seems close enough to not require any special adjustment.







            var iconSiD = AssetLoader.LoadInternal("Features", "Icon_ShadeInTheDarkEffect.png");
            var iconDaV = AssetLoader.LoadInternal("Features", "Icon_DarkVision.png");

            BlueprintBuff ShadeInTheDarkEffect = Helpers.CreateCopy<BlueprintBuff>(ArcanistExploitShadowVeilBuff, bp => {
                var setAttackerMissChance = bp.GetComponent<SetAttackerMissChance>();
                var addStatBonus = bp.GetComponent<AddStatBonus>();
                bp.SetName("Dark Silhouette");
                bp.SetDescription("You are merely a vague shadowy silhouette in shades of gray when seen by a creature with darkvision.");
                bp.RemoveComponent(setAttackerMissChance);
                bp.RemoveComponent(addStatBonus);

            });



            var DarkvisionArea = Helpers.CreateBlueprint<BlueprintAbilityAreaEffect>("DarkvisionArea", bp => {


                bp.AggroEnemies = false;




                bp.Size = 10.Feet();
                // abilityAreaEffectBuff.m_Buff = ShadeInTheDarkEffect.ToReference<BlueprintBuffReference>();


                /* abilityAreaEffectBuff.Condition.Conditions.ConditionsChecker = new ConditionsChecker()
                {
                    Conditions = new Condition[] {
                                    Helpers.Create<ContextConditionTargetIsYourself>( c => {
                                        c.Not = true;
                                    })
                              }
                }; */





                });




            /*     var ShadeInTheDarkBuff = Helpers.CreateBuff("ShadeInTheDarkBuff", bp => {
                     bp.SetName("Shadowy Silhouette");
                     bp.SetDescription("This creature seemed a shadowy silhouette in shades of gray when seen through darkvision...");
                     bp.m_Icon = iconSiD;
                     bp.m_Flags = BlueprintBuff.Flags.StayOnDeath;  // BlueprintBuff.Flags.HiddenInUi | 

                 }); */



        }
    }
}
