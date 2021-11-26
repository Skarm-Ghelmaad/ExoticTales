using System;
using System.Collections.Generic;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using System.Linq;
using JetBrains.Annotations;
using Kingmaker.Armies.TacticalCombat;
using Kingmaker.Armies.TacticalCombat.Parts;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Blueprints.Root;
using Kingmaker.Localization;
using Kingmaker.PubSubSystem;
using Kingmaker.UI;
using Kingmaker.UnitLogic;
using Kingmaker.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using ExoticTales.NewComponents;
using ExoticTales.Utilities;
using ExoticTales.Extensions;

namespace ExoticTales.NewContent.Features
{
    class UniversalKi
    {

        public static void AddUniversalKi()
        {

            var iconLifeForce = AssetLoader.LoadInternal("Features", "Icon_LifeEnergy.png");

            // Define a list of extra classes and extra archetypes that can produce Ki.

            BlueprintCharacterClass monk = Resources.GetBlueprint<BlueprintCharacterClass>("e8f21e5b58e0569468e420ebea456124");

            // Change KiPowerResource to render it universal [increased the maximum and removed the stat modifier].

            BlueprintAbilityResource kiResource = Resources.GetBlueprint<BlueprintAbilityResource>("9d9c90a9a1f52d04799294bf91c80a82"); // KiPowerResource of the monk class.

            kiResource.m_Max = 5000;
            kiResource.m_MaxAmount.IncreasedByStat = false;


            // Create a Ki Potential resource and feature.

            var KiPotentialResource = Helpers.CreateBlueprint<BlueprintAbilityResource>("KiPotentialResource", bp =>
            {
                bp.m_Max = 5000;
                bp.m_MaxAmount.m_Class = null;
                bp.m_MaxAmount.m_Archetypes = null;
                bp.m_MaxAmount.m_ClassDiv = null;
                bp.m_MaxAmount.m_ArchetypesDiv = null;
                bp.m_MaxAmount.LevelIncrease = 1;
                bp.m_MaxAmount.IncreasedByLevelStartPlusDivStep = false;
                bp.m_MaxAmount.LevelStep = 2;
                bp.m_MaxAmount.PerStepIncrease = 1;
                bp.m_MaxAmount.IncreasedByStat = false;
                bp.m_MaxAmount.ResourceBonusStat = StatType.Wisdom;

            });

            var KiPotentialPoolFeature = Helpers.CreateBlueprint<BlueprintFeature>("KiPotentialPoolFeature", bp =>
            {
                bp.SetName("Ki Potential");
                bp.SetDescription("Any creature has a potental ki pool, which is equal to 1/2 of the sum of the class levels in classes not providing ki"
                     + "\n + its highest Ability modifier (if they have no ki pool) or without modifier (if they had ki pool).");
                bp.m_Icon = iconLifeForce;
                bp.Groups = new FeatureGroup[] { FeatureGroup.Racial };
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.AddComponent(Helpers.Create<AddAbilityResources>(c => {
                    c.m_Resource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                    c.RestoreAmount = true;
                    c.RestoreOnLevelUp = true;
                }));



            });


            // Create Ki Pool Attribute Bonuses features.

            var KiStrengthBonus = Helpers.CreateBlueprint<BlueprintFeature>("KiStrengthBonus", bp =>
            {
                bp.SetName("Ki Pool Strength Bonus");
                bp.SetDescription("This character adds his {g|Encyclopedia:Strength}Strength{/g} modifier to his ki pool.");
                bp.m_Icon = iconLifeForce;
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.AddComponent(Helpers.Create<IncreaseResourceAmountBasedOnStatOnly>(c => {
                    c.m_Resource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                    c.TCLResourceMultiplier = 1.0f;
                    c.Subtract = false;
                    c.NotUseHighestStat = true;
                    c.ResourceBonusStat = StatType.Strength;

                }));
            });

            var KiDexterityBonus = Helpers.CreateBlueprint<BlueprintFeature>("KiDexterityBonus", bp =>
            {
                bp.SetName("Ki Pool Dexterity Bonus");
                bp.SetDescription("This character adds his {g|Encyclopedia:Dexterity}Dexterity{/g} modifier to his ki pool.");
                bp.m_Icon = iconLifeForce;
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.AddComponent(Helpers.Create<IncreaseResourceAmountBasedOnStatOnly>(c => {
                    c.m_Resource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                    c.TCLResourceMultiplier = 1.0f;
                    c.Subtract = false;
                    c.NotUseHighestStat = true;
                    c.ResourceBonusStat = StatType.Dexterity;

                }));
            });

            var KiConstitutionBonus = Helpers.CreateBlueprint<BlueprintFeature>("KiConstitutionBonus", bp =>
            {
                bp.SetName("Ki Pool Constitution Bonus");
                bp.SetDescription("This character adds his {g|Encyclopedia:Constitution}Constitution{/g} modifier to his ki pool.");
                bp.m_Icon = iconLifeForce;
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.AddComponent(Helpers.Create<IncreaseResourceAmountBasedOnStatOnly>(c => {
                    c.m_Resource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                    c.TCLResourceMultiplier = 1.0f;
                    c.Subtract = false;
                    c.NotUseHighestStat = true;
                    c.ResourceBonusStat = StatType.Constitution;

                }));
            });

            var KiIntelligenceBonus = Helpers.CreateBlueprint<BlueprintFeature>("KiIntelligenceBonus", bp =>
            {
                bp.SetName("Ki Pool Intelligence Bonus");
                bp.SetDescription("This character adds his {g|Encyclopedia:Intelligence}Intelligence{/g} modifier to his ki pool.");
                bp.m_Icon = iconLifeForce;
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.AddComponent(Helpers.Create<IncreaseResourceAmountBasedOnStatOnly>(c => {
                    c.m_Resource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                    c.TCLResourceMultiplier = 1.0f;
                    c.Subtract = false;
                    c.NotUseHighestStat = true;
                    c.ResourceBonusStat = StatType.Intelligence;

                }));
            });

            var KiWisdomBonus = Helpers.CreateBlueprint<BlueprintFeature>("KiWisdomBonus", bp =>
            {
                bp.SetName("Ki Pool Wisdom Bonus");
                bp.SetDescription("This character adds his {g|Encyclopedia:Wisdom}Wisdom{/g} modifier to his ki pool.");
                bp.m_Icon = iconLifeForce;
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.AddComponent(Helpers.Create<IncreaseResourceAmountBasedOnStatOnly>(c => {
                    c.m_Resource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                    c.TCLResourceMultiplier = 1.0f;
                    c.Subtract = false;
                    c.NotUseHighestStat = true;
                    c.ResourceBonusStat = StatType.Wisdom;

                }));
            });

            var KiCharismaBonus = Helpers.CreateBlueprint<BlueprintFeature>("KiCharismaBonus", bp =>
            {
                bp.SetName("Ki Pool Charisma Bonus");
                bp.SetDescription("This character adds his {g|Encyclopedia:Charisma}Charisma{/g} modifier to his ki pool.");
                bp.m_Icon = iconLifeForce;
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.AddComponent(Helpers.Create<IncreaseResourceAmountBasedOnStatOnly>(c => {
                    c.m_Resource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                    c.TCLResourceMultiplier = 1.0f;
                    c.Subtract = false;
                    c.NotUseHighestStat = true;
                    c.ResourceBonusStat = StatType.Charisma;

                }));
            });

            // Create Ki Pool Attribute Bonuses features.























        }


    }
}
