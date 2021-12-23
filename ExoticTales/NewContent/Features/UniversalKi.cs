using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Facts;
using Kingmaker.UnitLogic.Mechanics.Components;
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
using ExoticTales.Config;
using ExoticTales.Utilities;
using ExoticTales.Extensions;
using HlP = ExoticTales.Utilities.Helpers;
using ExH = ExoticTales.Utilities.ExpandedHelpers;
using Kingmaker.Enums;


namespace ExoticTales.NewContent.Features
{
    class UniversalKi
    {

        public static void AddUniversalKi()
        {

            if (ModSettings.AddedContent.NewSystems.IsDisabled("UniversalKi")) { return; }

            var iconLifeForce = AssetLoader.LoadInternal("Features", "Icon_LifeEnergy.png");

            // Define a list of extra classes and extra archetypes that can produce Ki.

            BlueprintCharacterClass monk = Resources.GetBlueprint<BlueprintCharacterClass>("e8f21e5b58e0569468e420ebea456124");

            var exclusionKiClasses = new BlueprintCharacterClassReference[0];
            exclusionKiClasses[0] = monk.ToReference<BlueprintCharacterClassReference>();

            // Change KiPowerResource to render it universal [increased the maximum and removed the stat modifier].

            BlueprintAbilityResource kiResource = Resources.GetBlueprint<BlueprintAbilityResource>("9d9c90a9a1f52d04799294bf91c80a82"); // KiPowerResource of the monk class.

            kiResource.m_Max = 5000;
            kiResource.m_MaxAmount.IncreasedByStat = false;

            Main.Log("Monk Ki resource altered.");



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
                bp.AddComponent(Helpers.Create<IncreaseResourceAmountBasedOnTrueCharacterLevelOnly>(c => {
                    c.m_Resource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                    c.ApplyClassExclusion = true;
               
                    c.m_ExcludedClass = exclusionKiClasses;

                }));


            });

            Main.Log("Potential Ki resource created.");

            // Create Ki Pool Bonuses features and adds a Ki Potential Attribute Bonus based on the highest attribute.


            var KiStrengthBonus = Helpers.CreateBlueprint<BlueprintFeature>("KiStrengthBonus", bp =>
            {
                bp.SetName("Ki Pool Bonus - {g|Encyclopedia:Strength}Strength{/g}");
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
                bp.SetName("Ki Pool Bonus - {g|Encyclopedia:Dexterity}Dexterity{/g}");
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
                bp.SetName("Ki Pool Bonus - {g|Encyclopedia:Constitution}Constitution{/g}");
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
                bp.SetName("Ki Pool Bonus - {g|Encyclopedia:Intelligence}Intelligence{/g}");
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
                bp.SetName("Ki Pool Bonus - {g|Encyclopedia:Wisdom}Wisdom{/g}");
                bp.SetDescription("This character adds his {g|Encyclopedia:Wisdom}Wisdom{/g} modifier to his ki pool, to the DC of his ki abilities and"
                    + "\n to any other ability parameter that would be affected by an attribute.");
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
                bp.SetName("Ki Pool Bonus - {g|Encyclopedia:Charisma}Charisma{/g}");
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

            Main.Log("Attribute Ki pool extra points created.");

            BlueprintUnitFactReference[] kiStatBonusAllFeatures = { KiStrengthBonus.ToReference<BlueprintUnitFactReference>(), KiDexterityBonus.ToReference<BlueprintUnitFactReference>(), KiConstitutionBonus.ToReference<BlueprintUnitFactReference>(), KiIntelligenceBonus.ToReference<BlueprintUnitFactReference>(), KiWisdomBonus.ToReference<BlueprintUnitFactReference>(), KiCharismaBonus.ToReference<BlueprintUnitFactReference>() };


            var KiPotentialHighestStatBonus = Helpers.CreateBlueprint<BlueprintFeature>("KiPotentialHighestStatBonus", bp =>
            {
                bp.SetName("Ki Potential Pool Highest Attribute Bonus");
                bp.SetDescription("This character adds his highest attribute modifier to his ki potential pool."
                    + "\n If the character gains a ki pool, this modifier will not have any effect.");
                bp.m_Icon = iconLifeForce;
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.AddComponent(Helpers.Create<IncreaseResourceAmountBasedOnStatOnly>(c => {
                    c.m_Resource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                    c.TCLResourceMultiplier = 1.0f;
                    c.Subtract = false;
                    c.NotUseHighestStat = false;

                }));
            });

            BlueprintUnitFactReference[] kiPotentialBonusFeature = { KiPotentialHighestStatBonus.ToReference<BlueprintUnitFactReference>() };

            var KiPotentialHighestStatBonusChecker = Helpers.CreateBlueprint<BlueprintFeature>("KiPotentialHighestStatBonusChecker", bp =>
            {
                bp.SetName("Ki Potential Pool Highest Attribute Bonus");
                bp.SetDescription("This character adds his highest attribute modifier to his ki potential pool."
                    + "\n If the character gains a ki pool, this modifier will not have any effect.");
                bp.m_Icon = iconLifeForce;
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.AddComponent(Helpers.Create<HasFactsFeaturesUnlock>(c => {
                    c.m_CheckedFacts = kiStatBonusAllFeatures;
                    c.m_Features = kiPotentialBonusFeature;
                    c.Not = true;

                }));

            });

            Main.Log("Ki Potential attribute bonus created.");


            // Create a list of Ki Powers with Abilities.

            BlueprintAbility monkAbudantStep = Resources.GetBlueprint<BlueprintAbility>("336a841704b7e2341b51f89fc9491f54");
            BlueprintAbility monkBarkskin = Resources.GetBlueprint<BlueprintAbility>("6dc679bdc40092e4c86933c337481a0f");
            BlueprintAbility monkColdicestrike = Resources.GetBlueprint<BlueprintAbility>("b1a1b7d59e4b6c44cb4e622baa171eb6");
            BlueprintAbility monkPoisoncast = Resources.GetBlueprint<BlueprintAbility>("d4b5f47fbe1074d4e9127dd08f21abda");
            BlueprintAbility monkQuiveringpalm = Resources.GetBlueprint<BlueprintAbility>("4de518e69f9b8094fb996b1599d00314");
            BlueprintAbility monkRestoration = Resources.GetBlueprint<BlueprintAbility>("81d2a0b908e7bb74790a6ecdf5795a69");
            BlueprintAbility monkScorchingray = Resources.GetBlueprint<BlueprintAbility>("450a8d492a3342742917c3a3b357f25e");
            BlueprintAbility monkShout = Resources.GetBlueprint<BlueprintAbility>("6e423d7de48eef74b999ebc8a62df8b8");
            BlueprintAbility monkSpitvenom = Resources.GetBlueprint<BlueprintAbility>("a9e6e105350562f45b41a8ebea1d8d87");
            BlueprintAbility monkSuddenspeed = Resources.GetBlueprint<BlueprintAbility>("8c98b8f3ac90fa245afe14116e48c7da");
            BlueprintAbility monkTruestrike = Resources.GetBlueprint<BlueprintAbility>("61da79969661b1349b042aa99623439d");
            BlueprintAbility monkWholenessofbody = Resources.GetBlueprint<BlueprintAbility>("f5f25b1319eef254f9197e71a969c03b");

            BlueprintAbility scaledfistAbudantStep = Resources.GetBlueprint<BlueprintAbility>("838b3be2249b33e4d96a0392132d1604");
            BlueprintAbility scaledfistBarkskin = Resources.GetBlueprint<BlueprintAbility>("0a1c08c4bfa268d45a3553008878f20d");
            BlueprintAbility scaledfistColdicestrike = Resources.GetBlueprint<BlueprintAbility>("578ad0a7bfa144c4cbfcbc641e97cf9d");
            BlueprintAbility scaledfistPoisoncast = Resources.GetBlueprint<BlueprintAbility>("3a9d9ca38885fb144a766f0ea5962e98");
            BlueprintAbility scaledfistQuiveringpalm = Resources.GetBlueprint<BlueprintAbility>("749e77f7014cb4e4487400e508e70a59");
            BlueprintAbility scaledfistRestoration = Resources.GetBlueprint<BlueprintAbility>("02c38239b97f44b4bb9d83e4352d76f9");
            BlueprintAbility scaledfistScorchingray = Resources.GetBlueprint<BlueprintAbility>("1b95baefa8931574aa15a579e4423063");
            BlueprintAbility scaledfistShout = Resources.GetBlueprint<BlueprintAbility>("e6b2906f33e7abe4394c053465563353");
            BlueprintAbility scaledfistSpitvenom = Resources.GetBlueprint<BlueprintAbility>("3fdcebb333e8f394099ddc105993e85a");
            BlueprintAbility scaledfistSuddenspeed = Resources.GetBlueprint<BlueprintAbility>("f91f00c3f59d63348a908195ee6d9e56");
            BlueprintAbility scaledfistTruestrike = Resources.GetBlueprint<BlueprintAbility>("2719c3185b6c3e946bfcdb788ae9adc6");
            BlueprintAbility scaledfistWholenessofbody = Resources.GetBlueprint<BlueprintAbility>("f3054941690b5b84986e6a65c7037e50");

            BlueprintAbility senseiBarkskinsingle = Resources.GetBlueprint<BlueprintAbility>("fd268041665a99f469b979046a463e2d");
            BlueprintAbility senseiRestorationsingle = Resources.GetBlueprint<BlueprintAbility>("8ddfed5bc95560e4d83756c8e5e2c33d");
            BlueprintAbility senseiSuddenspeedsingle = Resources.GetBlueprint<BlueprintAbility>("62071f9b0fada4f459a96ee0457745a3");
            BlueprintAbility senseiTruestrikedsingle = Resources.GetBlueprint<BlueprintAbility>("9615ccc21a817e8418062a49eae51b8b");
            BlueprintAbility senseiWholenessofbodysingle = Resources.GetBlueprint<BlueprintAbility>("c6484ec9a9d805a44a33281fd4652998");

            BlueprintAbility senseiBarkskinmass = Resources.GetBlueprint<BlueprintAbility>("7e82395f05961e14cbc14a75d3a94f0f");
            BlueprintAbility senseiRestorationmass = Resources.GetBlueprint<BlueprintAbility>("0b129e96521e54e44b061df1ddb3b486");
            BlueprintAbility senseiSuddenspeedmass = Resources.GetBlueprint<BlueprintAbility>("af6aa2b6398b8a749898fd71175d73f2");
            BlueprintAbility senseiTruestrikedmass = Resources.GetBlueprint<BlueprintAbility>("6f4bc76e64e557745a1fb04d958a65fe");
            BlueprintAbility senseiWholenessofbodymass = Resources.GetBlueprint<BlueprintAbility>("dff4b9cb0b24c9842a971c4176088f11");

            BlueprintAbility scaledfistBlackbreathweaponability = Resources.GetBlueprint<BlueprintAbility>("45fe18c8dcd11e54e8499646b9389029");
            BlueprintAbility scaledfistBluebreathweaponability = Resources.GetBlueprint<BlueprintAbility>("9bfed63e0b9bfa1478c521f0527fb772");
            BlueprintAbility scaledfistBrassbreathweaponability = Resources.GetBlueprint<BlueprintAbility>("f0bd350c96848364d8c8f7d3167499e9");
            BlueprintAbility scaledfistBronzebreathweaponability = Resources.GetBlueprint<BlueprintAbility>("38b32df1eb0aa45409b114ed345a1631");
            BlueprintAbility scaledfistCopperbreathweaponability = Resources.GetBlueprint<BlueprintAbility>("0019e0810a828a049b4c37a7effa2385");
            BlueprintAbility scaledfistGoldbreathweaponability = Resources.GetBlueprint<BlueprintAbility>("625958680ce6de844a996bde77c99e5b");
            BlueprintAbility scaledfistGreenbreathweaponability = Resources.GetBlueprint<BlueprintAbility>("cf128ce20d9ebdc46804e471b24ce71c");
            BlueprintAbility scaledfistRedbreathweaponability = Resources.GetBlueprint<BlueprintAbility>("aec48af6d05577248a91b69ed843e4cf");
            BlueprintAbility scaledfistSilverbreathweaponability = Resources.GetBlueprint<BlueprintAbility>("b08e7d0bdd3830f4f8292668b1ffac2e");
            BlueprintAbility scaledfistWhitebreathweaponability = Resources.GetBlueprint<BlueprintAbility>("5a2accb17ffde8b4dafb8c8f7e00b711");
            BlueprintAbility scaledfistDraconicfuryacidability = Resources.GetBlueprint<BlueprintAbility>("5a2accb17ffde8b4dafb8c8f7e00b711");
            BlueprintAbility scaledfistDraconicfurycoldability = Resources.GetBlueprint<BlueprintAbility>("9ae37b34f8d9dbc42a9651fc16465c99");
            BlueprintAbility scaledfistDraconicfuryelectricityability = Resources.GetBlueprint<BlueprintAbility>("e5d62d1a0deea44489c659c63d5b2682");
            BlueprintAbility scaledfistDraconicfuryfireability = Resources.GetBlueprint<BlueprintAbility>("a405b9c06947ac24f84ec044e869919c");
            BlueprintAbility scaledfistExtraattack = Resources.GetBlueprint<BlueprintAbility>("ca948bb4ce1a2014fbf4d8d44b553074");


            BlueprintAbilityReference[] kiPowerAbilities = {

                monkAbudantStep.ToReference<BlueprintAbilityReference>(),
                monkBarkskin.ToReference<BlueprintAbilityReference>(),
                monkColdicestrike.ToReference<BlueprintAbilityReference>(),
                monkPoisoncast.ToReference<BlueprintAbilityReference>(),
                monkQuiveringpalm.ToReference<BlueprintAbilityReference>(),
                monkRestoration.ToReference<BlueprintAbilityReference>(),
                monkScorchingray.ToReference<BlueprintAbilityReference>(),
                monkShout.ToReference<BlueprintAbilityReference>(),
                monkSpitvenom.ToReference<BlueprintAbilityReference>(),
                monkWholenessofbody.ToReference<BlueprintAbilityReference>(),
                monkSuddenspeed.ToReference<BlueprintAbilityReference>(),
                scaledfistAbudantStep.ToReference<BlueprintAbilityReference>(),
                scaledfistBarkskin.ToReference<BlueprintAbilityReference>(),
                scaledfistColdicestrike.ToReference<BlueprintAbilityReference>(),
                scaledfistPoisoncast.ToReference<BlueprintAbilityReference>(),
                scaledfistQuiveringpalm.ToReference<BlueprintAbilityReference>(),
                scaledfistRestoration.ToReference<BlueprintAbilityReference>(),
                scaledfistScorchingray.ToReference<BlueprintAbilityReference>(),
                scaledfistShout.ToReference<BlueprintAbilityReference>(),
                scaledfistSpitvenom.ToReference<BlueprintAbilityReference>(),
                scaledfistSuddenspeed.ToReference<BlueprintAbilityReference>(),
                scaledfistTruestrike.ToReference<BlueprintAbilityReference>(),
                scaledfistWholenessofbody.ToReference<BlueprintAbilityReference>(),
                senseiBarkskinsingle.ToReference<BlueprintAbilityReference>(),
                senseiRestorationsingle.ToReference<BlueprintAbilityReference>(),
                senseiSuddenspeedsingle.ToReference<BlueprintAbilityReference>(),
                senseiTruestrikedsingle.ToReference<BlueprintAbilityReference>(),
                senseiWholenessofbodysingle.ToReference<BlueprintAbilityReference>(),
                senseiBarkskinmass.ToReference<BlueprintAbilityReference>(),
                senseiRestorationmass.ToReference<BlueprintAbilityReference>(),
                senseiSuddenspeedmass.ToReference<BlueprintAbilityReference>(),
                senseiTruestrikedmass.ToReference<BlueprintAbilityReference>(),
                senseiWholenessofbodymass.ToReference<BlueprintAbilityReference>(),
                scaledfistBlackbreathweaponability.ToReference<BlueprintAbilityReference>(),
                scaledfistBluebreathweaponability.ToReference<BlueprintAbilityReference>(),
                scaledfistBrassbreathweaponability.ToReference<BlueprintAbilityReference>(),
                scaledfistBronzebreathweaponability.ToReference<BlueprintAbilityReference>(),
                scaledfistCopperbreathweaponability.ToReference<BlueprintAbilityReference>(),
                scaledfistGoldbreathweaponability.ToReference<BlueprintAbilityReference>(),
                scaledfistGreenbreathweaponability.ToReference<BlueprintAbilityReference>(),
                scaledfistRedbreathweaponability.ToReference<BlueprintAbilityReference>(),
                scaledfistSilverbreathweaponability.ToReference<BlueprintAbilityReference>(),
                scaledfistWhitebreathweaponability.ToReference<BlueprintAbilityReference>(),
                scaledfistDraconicfuryacidability.ToReference<BlueprintAbilityReference>(),
                scaledfistDraconicfurycoldability.ToReference<BlueprintAbilityReference>(),
                scaledfistDraconicfuryelectricityability.ToReference<BlueprintAbilityReference>(),
                scaledfistDraconicfuryfireability.ToReference<BlueprintAbilityReference>(),
        };

            Main.Log("Ki Abilities list created.");


            // Alter the KiPower feature to make the Ki Extra Attack unlockable with a feature.


            var KiExtraAttackUnlock = Helpers.CreateBlueprint<BlueprintFeature>("KiExtraAttackUnlock", bp =>
            {
                bp.SetName("Ki Extra Attack - Unlock");
                bp.SetDescription("The ki pool of this class grants the extra attacks ki power.");
                bp.m_Icon = iconLifeForce;
                bp.IsClassFeature = true;
                bp.Ranks = 1;
            });

            BlueprintAbility kiExtraAttack = Resources.GetBlueprint<BlueprintAbility>("7f6ea312f5dad364fa4a896d7db39fdd");

            BlueprintFeature monkKiPowerFeature = Resources.GetBlueprint<BlueprintFeature>("e9590244effb4be4f830b1e3fffced13");


            monkKiPowerFeature.ReplaceComponents<AddFacts>(Helpers.Create<HasFactFeatureUnlock>(c=>{
                c.m_CheckedFact = KiExtraAttackUnlock.ToReference<BlueprintUnitFactReference>();
                c.m_Feature = kiExtraAttack.ToReference<BlueprintUnitFactReference>();
            }));

            Main.Log("Ki extra attack unlocker added.");


            // Alter the ScaledFist abilities converted to use standard ki power.


            scaledfistAbudantStep.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 2;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistBarkskin.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 1;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistColdicestrike.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistBlackbreathweaponability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistBluebreathweaponability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistBrassbreathweaponability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistBronzebreathweaponability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistCopperbreathweaponability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistGoldbreathweaponability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistGreenbreathweaponability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistRedbreathweaponability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistSilverbreathweaponability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistWhitebreathweaponability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistDraconicfuryacidability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 1;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistDraconicfurycoldability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 1;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistDraconicfuryelectricityability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 1;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistDraconicfuryfireability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 1;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistPoisoncast.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 2;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistQuiveringpalm.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 4;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistRestoration.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 2;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistScorchingray.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 2;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistShout.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistSpitvenom.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 2;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistTruestrike.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 1;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistWholenessofbody.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 2;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistExtraattack.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 1;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistSuddenspeed.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 1;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            // Alter the KiPower feature to make the Ki Extra Attack unlockable with a feature.

            BlueprintFeature scaledfistPowerFeature = Resources.GetBlueprint<BlueprintFeature>("ae98ab7bda409ef4bb39149a212d6732");


            scaledfistPowerFeature.ReplaceComponents<AddFacts>(Helpers.Create<HasFactFeatureUnlock>(c => {
                c.m_CheckedFact = KiExtraAttackUnlock.ToReference<BlueprintUnitFactReference>();
                c.m_Feature = scaledfistExtraattack.ToReference<BlueprintUnitFactReference>();
            }));

            scaledfistPowerFeature.ReplaceComponents<AddAbilityResources>(Helpers.Create<AddAbilityResources>(c => {
                c.m_Resource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.RestoreAmount = true;
            }));

            Main.Log("Change scaled fist abilities to use standard ki resources.");

            // Create Ki Attribute (unlocking) features.

            var KiAttributeStrength = Helpers.CreateBlueprint<BlueprintFeature>("KiAttributeStrength", bp =>
            {
                bp.SetName("Ki Attribute - {g|Encyclopedia:Strength}Strength{/g}");
                bp.SetDescription("This character adds his {g|Encyclopedia:Strength}Strength{/g} modifier to his ki pool, to the DC of his ki abilities and"
                    + "\n to any other ability parameter that would be affected by an attribute.");
                bp.m_Icon = iconLifeForce;
                bp.IsClassFeature = true;
                bp.Ranks = 1;
            });

            var KiAttributeDexterity = Helpers.CreateBlueprint<BlueprintFeature>("KiAttributeDexterity", bp =>
            {
                bp.SetName("Ki Attribute - {g|Encyclopedia:Dexterity}Dexterity{/g}");
                bp.SetDescription("This character adds his {g|Encyclopedia:Dexterity}Dexterity{/g} modifier to his ki pool, to the DC of his ki abilities and"
                    + "\n to any other ability parameter that would be affected by an attribute.");
                bp.m_Icon = iconLifeForce;
                bp.IsClassFeature = true;
                bp.Ranks = 1;
            });

            var KiAttributeConstitution = Helpers.CreateBlueprint<BlueprintFeature>("KiAttributeConstitution", bp =>
            {
                bp.SetName("Ki Attribute - {g|Encyclopedia:Constitution}Constitution{/g}");
                bp.SetDescription("This character adds his {g|Encyclopedia:Constitution}Constitution{/g} modifier to his ki pool, to the DC of his ki abilities and"
                    + "\n to any other ability parameter that would be affected by an attribute.");
                bp.m_Icon = iconLifeForce;
                bp.IsClassFeature = true;
                bp.Ranks = 1;
            });

            var KiAttributeIntelligence = Helpers.CreateBlueprint<BlueprintFeature>("KiAttributeIntelligence", bp =>
            {
                bp.SetName("Ki Attribute - {g|Encyclopedia:Intelligence}Intelligence{/g}");
                bp.SetDescription("This character adds his {g|Encyclopedia:Intelligence}Intelligence{/g} modifier to his ki pool, to the DC of his ki abilities and"
                    + "\n to any other ability parameter that would be affected by an attribute.");
                bp.m_Icon = iconLifeForce;
                bp.IsClassFeature = true;
                bp.Ranks = 1;
            });


            var KiAttributeWisdom = Helpers.CreateBlueprint<BlueprintFeature>("KiAttributeWisdom", bp =>
            {
                bp.SetName("Ki Attribute - {g|Encyclopedia:Wisdom}Wisdom{/g}");
                bp.SetDescription("This character adds his {g|Encyclopedia:Wisdom}Wisdom{/g} modifier to his ki pool, to the DC of his ki abilities and"
                    + "\n to any other ability parameter that would be affected by an attribute.");
                bp.m_Icon = iconLifeForce;
                bp.IsClassFeature = true;
                bp.Ranks = 1;
            });

            var KiAttributeCharisma = Helpers.CreateBlueprint<BlueprintFeature>("KiAttributeCharisma", bp =>
            {
                bp.SetName("Ki Attribute - {g|Encyclopedia:Charisma}Charisma{/g}");
                bp.SetDescription("This character adds his {g|Encyclopedia:Charisma}Charisma{/g} modifier to his ki pool, to the DC of his ki abilities and"
                    + "\n to any other ability parameter that would be affected by an attribute.");
                bp.m_Icon = iconLifeForce;
                bp.IsClassFeature = true;
                bp.Ranks = 1;
            });


            Main.Log("Create Ki attribute unlocking feature.");

            // Create a Ki Attribute selection group.


            BlueprintFeatureReference[] kiAttributes = {
                KiAttributeStrength.ToReference<BlueprintFeatureReference>(),
                KiAttributeDexterity.ToReference<BlueprintFeatureReference>(),
                KiAttributeConstitution.ToReference<BlueprintFeatureReference>(),
                KiAttributeIntelligence.ToReference<BlueprintFeatureReference>(),
                KiAttributeWisdom.ToReference<BlueprintFeatureReference>(),
                KiAttributeCharisma.ToReference<BlueprintFeatureReference>()
            };

            BlueprintUnitFactReference[] kiAttributesFacts = {
                KiAttributeStrength.ToReference<BlueprintUnitFactReference>(),
                KiAttributeDexterity.ToReference<BlueprintUnitFactReference>(),
                KiAttributeConstitution.ToReference<BlueprintUnitFactReference>(),
                KiAttributeIntelligence.ToReference<BlueprintUnitFactReference>(),
                KiAttributeWisdom.ToReference<BlueprintUnitFactReference>(),
                KiAttributeCharisma.ToReference<BlueprintUnitFactReference>()
            };

            var statFeatureLists = new Dictionary<BlueprintFeatureReference, StatType>();
            statFeatureLists.Add(KiAttributeStrength.ToReference<BlueprintFeatureReference>(), StatType.Strength);
            statFeatureLists.Add(KiAttributeDexterity.ToReference<BlueprintFeatureReference>(), StatType.Dexterity);
            statFeatureLists.Add(KiAttributeConstitution.ToReference<BlueprintFeatureReference>(), StatType.Constitution);
            statFeatureLists.Add(KiAttributeIntelligence.ToReference<BlueprintFeatureReference>(), StatType.Intelligence);
            statFeatureLists.Add(KiAttributeWisdom.ToReference<BlueprintFeatureReference>(), StatType.Wisdom);
            statFeatureLists.Add(KiAttributeCharisma.ToReference<BlueprintFeatureReference>(), StatType.Charisma);

            var KiAttributeSelection = Helpers.CreateBlueprint<BlueprintFeatureSelection>("KiAttributeSelection", bp =>
            {
                bp.SetName("Ki Attribute");
                bp.SetDescription("Any character with a ki pool can select which attribute will be used for its abilities and to calculate his ki pool.");
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.Mode = SelectionMode.OnlyNew;
                bp.m_Features = kiAttributes;
            });

            Main.Log("Create Ki attribute selection.");

            // Create custom Ki Attribute AC bonuses and add reference to the original ones.


            BlueprintFeature monkWisACbonusunlock = Resources.GetBlueprint<BlueprintFeature>("2615c5f87b3d72b42ac0e73b56d895e0"); // Unlock Wis bonus to AC when unarmored.
            BlueprintFeature monkWisACbonus = Resources.GetBlueprint<BlueprintFeature>("e241bdfd6333b9843a7bfd674d607ac4"); // Unlock Wis bonus to AC (apply buff).
            BlueprintBuff monkWisACbonusbuff = Resources.GetBlueprint<BlueprintBuff>("f132c4c4279e4646a05de26635941bfe"); // Wis bonus to AC buff.
            BlueprintBuff monkWisstunningfatiguefeature = Resources.GetBlueprint<BlueprintBuff>("819645da2e446f84d9b168ed1676ec29"); // Stunning fist fatigued based on Wisdom.
            BlueprintBuff monkWisstunningfistsickenedfeature = Resources.GetBlueprint<BlueprintBuff>("d256ab3837538cc489d4b571e3a813eb"); // Stunning fist sickened based on Wisdom.


            BlueprintFeature monkChaACbonusunlock = Resources.GetBlueprint<BlueprintFeature>("2a8922e28b3eba54fa7a244f7b05bd9e"); // Unlock Cha bonus to AC when unarmored.
            BlueprintFeature monkChaACbonus = Resources.GetBlueprint<BlueprintFeature>("3929bfd1beeeed243970c9fc0cf333f8"); // Unlock Cha bonus to AC (apply buff).
            BlueprintBuff monkChastunningfatiguefeature = Resources.GetBlueprint<BlueprintBuff>("abcf396b95e3dbc4686c8547783a719c"); // Stunning fist fatigued based on Charisma.
            BlueprintBuff monkChastunningfistsickenedfeature = Resources.GetBlueprint<BlueprintBuff>("e754ea837ee7a6e438ff7ebf6da40b79"); // Stunning fist sickened based on Charisma.


            string monkChaACbonusDescription = monkChaACbonus.Description.Replace("scaled fist", "monk");       // Made feature for scaled fist description generic.
            monkChaACbonus.SetDescription(monkChaACbonusDescription);

            // STRENGTH

            var monkStrACbonus = Helpers.CreateCopy(monkChaACbonus, bp => {

                var old_contextrankconfig = bp.GetComponents<ContextRankConfig>().Where(crc => crc.Type == Kingmaker.Enums.AbilityRankType.DamageDice).First();
                old_contextrankconfig.m_Stat = StatType.Strength;
                bp.ReplaceComponents<RecalculateOnStatChange>(Helpers.Create<RecalculateOnStatChange>(c => {
                                    c.Stat = StatType.Strength;
                }));

            });

            var monkStrstunningfatiguefeature = Helpers.CreateCopy(monkChastunningfatiguefeature, bp => {
                bp.ReplaceComponents<ReplaceAbilityDC>(Helpers.Create<ReplaceAbilityDC>(c => {
                    c.Stat = StatType.Strength;
                }));
            });

            var monkStrstunningfistsickenedfeature = Helpers.CreateCopy(monkChastunningfistsickenedfeature, bp => {
                bp.ReplaceComponents<ReplaceAbilityDC>(Helpers.Create<ReplaceAbilityDC>(c => {
                    c.Stat = StatType.Strength;
                }));
            });

            var monkStrACbonusunlock = Helpers.CreateCopy(monkChaACbonusunlock, bp => {
                bp.ReplaceComponents<MonkNoArmorFeatureUnlock>(Helpers.Create<MonkNoArmorFeatureUnlock>(c => {
                    c.m_NewFact = monkStrACbonus.ToReference<BlueprintUnitFactReference>();
                }));
                bp.ReplaceComponents<SavesFixerReplaceInProgression>(Helpers.Create<SavesFixerReplaceInProgression>(c => {
                    c.m_NewFeature = monkStrstunningfistsickenedfeature.ToReference<BlueprintFeatureReference>();
                }));

            });


            // DEXTERITY


            var monkDexACbonus = Helpers.CreateCopy(monkChaACbonus, bp => {

                var old_contextrankconfig = bp.GetComponents<ContextRankConfig>().Where(crc => crc.Type == Kingmaker.Enums.AbilityRankType.DamageDice).First();
                old_contextrankconfig.m_Stat = StatType.Dexterity;
                bp.ReplaceComponents<RecalculateOnStatChange>(Helpers.Create<RecalculateOnStatChange>(c => {
                    c.Stat = StatType.Dexterity;
                }));

            });

            var monkDexstunningfatiguefeature = Helpers.CreateCopy(monkChastunningfatiguefeature, bp => {
                bp.ReplaceComponents<ReplaceAbilityDC>(Helpers.Create<ReplaceAbilityDC>(c => {
                    c.Stat = StatType.Dexterity;
                }));
            });

            var monkDexstunningfistsickenedfeature = Helpers.CreateCopy(monkChastunningfistsickenedfeature, bp => {
                bp.ReplaceComponents<ReplaceAbilityDC>(Helpers.Create<ReplaceAbilityDC>(c => {
                    c.Stat = StatType.Dexterity;
                }));
            });

            var monkDexACbonusunlock = Helpers.CreateCopy(monkChaACbonusunlock, bp => {
                bp.ReplaceComponents<MonkNoArmorFeatureUnlock>(Helpers.Create<MonkNoArmorFeatureUnlock>(c => {
                    c.m_NewFact = monkDexACbonus.ToReference<BlueprintUnitFactReference>();
                }));
                bp.ReplaceComponents<SavesFixerReplaceInProgression>(Helpers.Create<SavesFixerReplaceInProgression>(c => {
                    c.m_NewFeature = monkDexstunningfistsickenedfeature.ToReference<BlueprintFeatureReference>();
                }));

            });


            // CONSTITUTION

            var monkConACbonus = Helpers.CreateCopy(monkChaACbonus, bp => {

                var old_contextrankconfig = bp.GetComponents<ContextRankConfig>().Where(crc => crc.Type == Kingmaker.Enums.AbilityRankType.DamageDice).First();
                old_contextrankconfig.m_Stat = StatType.Constitution;
                bp.ReplaceComponents<RecalculateOnStatChange>(Helpers.Create<RecalculateOnStatChange>(c => {
                    c.Stat = StatType.Constitution;
                }));

            });

            var monkConstunningfatiguefeature = Helpers.CreateCopy(monkChastunningfatiguefeature, bp => {
                bp.ReplaceComponents<ReplaceAbilityDC>(Helpers.Create<ReplaceAbilityDC>(c => {
                    c.Stat = StatType.Constitution;
                }));
            });

            var monkConstunningfistsickenedfeature = Helpers.CreateCopy(monkChastunningfistsickenedfeature, bp => {
                bp.ReplaceComponents<ReplaceAbilityDC>(Helpers.Create<ReplaceAbilityDC>(c => {
                    c.Stat = StatType.Constitution;
                }));
            });

            var monkConACbonusunlock = Helpers.CreateCopy(monkChaACbonusunlock, bp => {
                bp.ReplaceComponents<MonkNoArmorFeatureUnlock>(Helpers.Create<MonkNoArmorFeatureUnlock>(c => {
                    c.m_NewFact = monkConACbonus.ToReference<BlueprintUnitFactReference>();
                }));
                bp.ReplaceComponents<SavesFixerReplaceInProgression>(Helpers.Create<SavesFixerReplaceInProgression>(c => {
                    c.m_NewFeature = monkConstunningfistsickenedfeature.ToReference<BlueprintFeatureReference>();
                }));

            });

            // INTELLIGENCE

            var monkIntACbonus = Helpers.CreateCopy(monkChaACbonus, bp => {

                var old_contextrankconfig = bp.GetComponents<ContextRankConfig>().Where(crc => crc.Type == Kingmaker.Enums.AbilityRankType.DamageDice).First();
                old_contextrankconfig.m_Stat = StatType.Intelligence;
                bp.ReplaceComponents<RecalculateOnStatChange>(Helpers.Create<RecalculateOnStatChange>(c => {
                    c.Stat = StatType.Intelligence;
                }));

            });

            var monkIntstunningfatiguefeature = Helpers.CreateCopy(monkChastunningfatiguefeature, bp => {
                bp.ReplaceComponents<ReplaceAbilityDC>(Helpers.Create<ReplaceAbilityDC>(c => {
                    c.Stat = StatType.Intelligence;
                }));
            });

            var monkIntstunningfistsickenedfeature = Helpers.CreateCopy(monkChastunningfistsickenedfeature, bp => {
                bp.ReplaceComponents<ReplaceAbilityDC>(Helpers.Create<ReplaceAbilityDC>(c => {
                    c.Stat = StatType.Intelligence;
                }));
            });

            var monkIntACbonusunlock = Helpers.CreateCopy(monkChaACbonusunlock, bp => {
                bp.ReplaceComponents<MonkNoArmorFeatureUnlock>(Helpers.Create<MonkNoArmorFeatureUnlock>(c => {
                    c.m_NewFact = monkIntACbonus.ToReference<BlueprintUnitFactReference>();
                }));
                bp.ReplaceComponents<SavesFixerReplaceInProgression>(Helpers.Create<SavesFixerReplaceInProgression>(c => {
                    c.m_NewFeature = monkIntstunningfistsickenedfeature.ToReference<BlueprintFeatureReference>();
                }));

            });

            Main.Log("Create Ki attribute AC unlocker.");

            // Create Ki Pool Strength feature.

            var monkStrKiPowerFeature = Helpers.CreateCopy(monkKiPowerFeature, bp => {
                string monkStrKiPowerFeatureName = monkKiPowerFeature.Name + " {g|Encyclopedia:Strength}Strength{/g}";
                string monkStrKiPowerFeatureDescription = monkKiPowerFeature.Description.Replace("Wisdom", "Strength");
                bp.SetName(monkStrKiPowerFeatureName);
                bp.SetDescription(monkStrKiPowerFeatureDescription);
                bp.AddComponent<AddFacts>(c => {
                    c.m_Facts = new BlueprintUnitFactReference[] { KiStrengthBonus.ToReference<BlueprintUnitFactReference>() };
                    });
                });

            // Create Ki Pool Dexterity feature.

            var monkDexKiPowerFeature = Helpers.CreateCopy(monkKiPowerFeature, bp => {
                string monkDexKiPowerFeatureName = monkKiPowerFeature.Name + " {g|Encyclopedia:Dexterity}Dexterity{/g}";
                string monkDexKiPowerFeatureDescription = monkKiPowerFeature.Description.Replace("Wisdom", "Dexterity");
                bp.SetName(monkDexKiPowerFeatureName);
                bp.SetDescription(monkDexKiPowerFeatureDescription);
                bp.AddComponent<AddFacts>(c => {
                    c.m_Facts = new BlueprintUnitFactReference[] { KiDexterityBonus.ToReference<BlueprintUnitFactReference>() };
                });
            });

            // Create Ki Pool Constitution feature.

            var monkConKiPowerFeature = Helpers.CreateCopy(monkKiPowerFeature, bp => {
                string monkConKiPowerFeatureName = monkKiPowerFeature.Name + " {g|Encyclopedia:Constitution}Constitution{/g}";
                string monkConKiPowerFeatureDescription = monkKiPowerFeature.Description.Replace("Wisdom", "Constitution");
                bp.SetName(monkConKiPowerFeatureName);
                bp.SetDescription(monkConKiPowerFeatureDescription);
                bp.AddComponent<AddFacts>(c => {
                    c.m_Facts = new BlueprintUnitFactReference[] { KiConstitutionBonus.ToReference<BlueprintUnitFactReference>() };
                });
            });

            // Create Ki Pool Intelligence feature.

            var monkIntKiPowerFeature = Helpers.CreateCopy(monkKiPowerFeature, bp => {
                string monkIntKiPowerFeatureName = monkKiPowerFeature.Name + " {g|Encyclopedia:Intelligence}Intelligence{/g}";
                string monkIntKiPowerFeatureDescription = monkKiPowerFeature.Description.Replace("Wisdom", "Intelligence");
                bp.SetName(monkIntKiPowerFeatureName);
                bp.SetDescription(monkIntKiPowerFeatureDescription);
                bp.AddComponent<AddFacts>(c => {
                    c.m_Facts = new BlueprintUnitFactReference[] { KiIntelligenceBonus.ToReference<BlueprintUnitFactReference>() };
                });
            });

            // Change original Ki Pool (Wisdom) feature to match the other ki pool features.


            string monkKiPowerFeatureName = monkKiPowerFeature.Name + " {g|Encyclopedia:Wisdom}Wisdom{/g}";
            monkKiPowerFeature.SetName(monkKiPowerFeatureName);
            monkKiPowerFeature.AddComponent<AddFacts>(c => {
                c.m_Facts = new BlueprintUnitFactReference[] { KiWisdomBonus.ToReference<BlueprintUnitFactReference>() };
            });

            // Change original scaled fist Ki Pool (Charisma) feature to match the other ki pool features.

            string scaledfistPowerFeatureName = scaledfistPowerFeature.Name + " {g|Encyclopedia:Charisma}Charisma{/g}";
            string scaledfistPowerFeatureDescription = scaledfistPowerFeature.Description.Replace("Wisdom", "Charisma");
            scaledfistPowerFeature.SetName(scaledfistPowerFeatureName);
            scaledfistPowerFeature.SetDescription(scaledfistPowerFeatureDescription);
            scaledfistPowerFeature.AddComponent<AddFacts>(c => {
                c.m_Facts = new BlueprintUnitFactReference[] { KiCharismaBonus.ToReference<BlueprintUnitFactReference>() };
            });

            Main.Log("Ki Pool feature by attribute created.");

            // Alter stunning fist to replace ki stat based on Ability DC and add ki points when the character has a ki pool.

            BlueprintFeature baseStunningFist = Resources.GetBlueprint<BlueprintFeature>("a29a582c3daa4c24bb0e991c596ccb28"); // Standard stunning fist
            BlueprintAbility baseStunningFistAbility = Resources.GetBlueprint<BlueprintAbility>("732ae7773baf15447a6737ae6547fc1e"); // Standard stunning fist ability
            BlueprintAbilityResource baseStunningFistAbilityResource = Resources.GetBlueprint<BlueprintAbilityResource>("d2bae584db4bf4f4f86dd9d15ae56558"); // Standard stunning fist resource

            baseStunningFistAbilityResource.m_Max = 40;

            string baseStunningFistDescription = baseStunningFist.Description.Replace("cannot be stunned.", "cannot be stunned. If you have a ki pool, this feat uses ki points, you gain 1 bonus ki point for each monk level and for every 4 non-monk levels and your Ki Attribute is used to determine the DC of this ability.");
            string baseStunningFistAbilityDescription = baseStunningFistAbility.Description.Replace("cannot be stunned.", "cannot be stunned. If you have a ki pool, this feat uses ki points, you gain 1 bonus ki point for each monk level and for every 4 non-monk levels and your Ki Attribute is used to determine the DC of this ability.");

            baseStunningFist.SetDescription(baseStunningFistDescription);
            baseStunningFistAbility.SetDescription(baseStunningFistAbilityDescription);

            baseStunningFist.ReplaceComponents<MonkReplaceAbilityDC>(Helpers.Create<KiStatReplaceAbilityDC>(c => {
                c.m_Ability = baseStunningFistAbility.ToReference<BlueprintAbilityReference>();
                c.DefaultBonusStat = StatType.Wisdom;
                c.m_KiStatFeature = statFeatureLists;

            }));

            baseStunningFist.ReplaceComponents<AddAbilityResources>(Helpers.Create<ConditionalAddAbilityResourcesOrContextAmount>(c => {
                c.m_CheckedFacts = kiAttributesFacts;
                c.m_BaseResource = baseStunningFistAbilityResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_ReplacementResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.UseThisAsResource = false;
                c.RestoreAmount = true;
                c.RestoreOnLevelUp = false;
                c.Not = false;
                c.Subtract = false;
                c.Value = new ContextValue()
                {
                    ValueType = ContextValueType.Rank,
                    ValueRank = AbilityRankType.StatBonus
                };


            }));

            baseStunningFist.AddComponent(Helpers.Create<ContextIncreaseAlternateResourceAmountBasedOnTrueCharacterLevelOnly>(c => {
                c.m_CheckedFacts = kiAttributesFacts;
                c.m_BaseResource = baseStunningFistAbilityResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_AlternateResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.Not = false;
                c.BaseSubtract = false;
                c.AlternateSubtract = false;
                c.ExcludeMythic = true;
                c.TCLResourceMultiplier = 1.0f;
                c.BaseBonusOverrideMultiplier = 0f;
                c.AlternateBonusOverrideMultiplier = 0.25f;
                c.ApplyClassExclusion = true;
                c.m_ExcludedClass = exclusionKiClasses;

            }));

            baseStunningFist.AddContextRankConfig(c => {
                c.m_Type = AbilityRankType.StatBonus;
                c.m_BaseValueType = ContextRankBaseValueType.ClassLevel;
                c.m_Progression = ContextRankProgression.StartPlusDivStep;
                c.m_Min = 1;
                c.m_UseMin = true;
                c.m_StartLevel = 1;
                c.m_StepLevel = 1;
                c.m_Class = exclusionKiClasses;
                c.m_ExceptClasses = false;
            });

            Main.Log("Stunning Fist changes implemented.");

            // Alter Dragon Roar to replace ki stat based on Ability DC and add ki points when the character has a ki pool.

            BlueprintFeature baseDragonRoar = Resources.GetBlueprint<BlueprintFeature>("3fca938ad6a5b8348a8523794127c5bc"); // Standard dragon roar 
            BlueprintAbility baseDragonRoarAbility = Resources.GetBlueprint<BlueprintAbility>("7e87f6e176b28d54a98b3490f8cba9db"); // Standard dragon roar ability


            string baseDragonRoarDescription = baseDragonRoar.Description.Replace("prevents a target from being shaken.", "prevents a target from being shaken. If you have a ki pool, you gain 1 bonus ki point, spend 2 ki points to use this feat and your Ki Attribute is used to determine the DC of this ability.");
            string baseDragonRoarAbilityDescription = baseDragonRoarAbility.Description.Replace("prevents a target from being shaken.", "prevents a target from being shaken. If you have a ki pool, you gain 1 bonus ki point, spend 2 ki points to use this feat and your Ki Attribute is used to determine the DC of this ability.");


            baseDragonRoar.SetDescription(baseDragonRoarDescription);
            baseDragonRoarAbility.SetDescription(baseDragonRoarAbilityDescription);

            baseDragonRoar.ReplaceComponents<MonkReplaceAbilityDC>(Helpers.Create<KiStatReplaceAbilityDC>(c => {
                c.m_Ability = baseStunningFistAbility.ToReference<BlueprintAbilityReference>();
                c.DefaultBonusStat = StatType.Wisdom;
                c.m_KiStatFeature = statFeatureLists;

            }));

            baseDragonRoar.ReplaceComponents<AddAbilityResources>(Helpers.Create<ContextIncreaseAlternatingResourceAmount>(c => {
                c.m_CheckedFacts = kiAttributesFacts;
                c.m_BaseResource = baseStunningFistAbilityResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_AlternateResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.Not = false;
                c.BaseSubtract = false;
                c.AlternateSubtract = false;
                c.BaseValue = 1;
                c.AlternateValue = 1;
            }));

            Main.Log("Dragon Roar changes implemented.");

            // Alter Perfect Strike to add ki points when the character has a ki pool.

            BlueprintFeature basePerfectStrike = Resources.GetBlueprint<BlueprintFeature>("7477e2e0b72f4ce4fb674f4b21d5e81d"); // Standard perfect strike
            BlueprintAbility basePerfectStrikeAbility = Resources.GetBlueprint<BlueprintAbility>("bc656f51e407aad40bc8d974f3d5b04a"); // Standard perfect strike ability
            BlueprintAbilityResource basePerfectStrikeAbilityResource = Resources.GetBlueprint<BlueprintAbilityResource>("b6c1efe47c946ab48bea52df06146f97"); // Standard perfect strike resource

            string basePerfectStrikeDescription = basePerfectStrike.Description.Replace("A monk may attempt an perfect strike attack a number of times per day equal to his monk level, plus one more time per day for every four levels he has in classes other than monk.", "If you have a ki pool, each perfect strike attack will cost 1 ki point and you gain 1 bonus ki point for each monk level and for every 4 non-monk levels.");
            string basePerfectStrikeAbilityDescription = basePerfectStrikeAbility.Description.Replace("A monk may attempt an perfect strike attack a number of times per day equal to his monk level, plus one more time per day for every four levels he has in classes other than monk.", "If you have a ki pool, each perfect strike attack will cost 1 ki point and you gain 1 bonus ki point for each monk level and for every 4 non-monk levels.");


            basePerfectStrikeAbilityResource.m_Max = 40;

            basePerfectStrike.ReplaceComponents<AddAbilityResources>(Helpers.Create<ConditionalAddAbilityResourcesOrContextAmount>(c => {
                c.m_CheckedFacts = kiAttributesFacts;
                c.m_BaseResource = basePerfectStrikeAbilityResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_ReplacementResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                c.UseThisAsResource = false;
                c.RestoreAmount = true;
                c.RestoreOnLevelUp = false;
                c.Not = false;
                c.Subtract = false;
                c.Value = new ContextValue()
                {
                    ValueType = ContextValueType.Rank,
                    ValueRank = AbilityRankType.StatBonus
                };

            }));

            basePerfectStrike.AddComponent(Helpers.Create<ContextIncreaseAlternateResourceAmountBasedOnTrueCharacterLevelOnly>(c => {
                 c.m_CheckedFacts = kiAttributesFacts;
                 c.m_BaseResource = basePerfectStrikeAbilityResource.ToReference<BlueprintAbilityResourceReference>();
                 c.m_AlternateResource = kiResource.ToReference<BlueprintAbilityResourceReference>();
                 c.Not = false;
                 c.BaseSubtract = false;
                 c.AlternateSubtract = false;
                 c.ExcludeMythic = true;
                 c.TCLResourceMultiplier = 1.0f;
                 c.BaseBonusOverrideMultiplier = 0f;
                 c.AlternateBonusOverrideMultiplier = 0.25f;
                 c.ApplyClassExclusion = true;
                 c.m_ExcludedClass = exclusionKiClasses;

             }));

            basePerfectStrike.AddContextRankConfig(c => {
                c.m_Type = AbilityRankType.StatBonus;
                c.m_BaseValueType = ContextRankBaseValueType.ClassLevel;
                c.m_Progression = ContextRankProgression.StartPlusDivStep;
                c.m_Min = 1;
                c.m_UseMin = true;
                c.m_StartLevel = 1;
                c.m_StepLevel = 1;
                c.m_Class = exclusionKiClasses;
                c.m_ExceptClasses = false;
            });

            Main.Log("Perfect Strike changes implemented.");


            // Create alternate Insightful Strikes.

            BlueprintFeature senseiWisInsightfulStrike = Resources.GetBlueprint<BlueprintFeature>("f4a3f9ede5a57c142b30a9dfbb8efa90"); // Replace Wis for Str and Dex with unarmed and monk weapons attacks.


            // STRENGTH

            var senseiStrInsightfulStrike = Helpers.CreateCopy(senseiWisInsightfulStrike, bp => {
                string senseiStrInsightfulStrikeName = senseiWisInsightfulStrike.Name + " {g|Encyclopedia:Strength}Strength{/g}";
                string senseiStrInsightfulStrikeDescription = senseiWisInsightfulStrike.Description.Replace("Strength or ", "");
                senseiStrInsightfulStrikeDescription = senseiStrInsightfulStrikeDescription.Replace("Wisdom", "Strength");
                bp.SetName(senseiStrInsightfulStrikeName);
                bp.SetDescription(senseiStrInsightfulStrikeDescription);
                var changedAttackStatReplacement = bp.GetComponent<AttackStatReplacement>();
                var changedReplaceCombatManeuverStat = bp.GetComponent<ReplaceCombatManeuverStat>();
                changedAttackStatReplacement.ReplacementStat = StatType.Strength;
                changedReplaceCombatManeuverStat.StatType = StatType.Strength;

            });

            // DEXTERITY

            var senseiDexInsightfulStrike = Helpers.CreateCopy(senseiWisInsightfulStrike, bp => {
                string senseiDexInsightfulStrikeName = senseiWisInsightfulStrike.Name + " {g|Encyclopedia:Dexterity}Dexterity{/g}";
                string senseiDexInsightfulStrikeDescription = senseiWisInsightfulStrike.Description.Replace(" or Dexterity", "");
                senseiDexInsightfulStrikeDescription = senseiDexInsightfulStrikeDescription.Replace("Wisdom", "Dexterity");
                bp.SetName(senseiDexInsightfulStrikeName);
                bp.SetDescription(senseiDexInsightfulStrikeDescription);
                var changedAttackStatReplacement = bp.GetComponent<AttackStatReplacement>();
                var changedReplaceCombatManeuverStat = bp.GetComponent<ReplaceCombatManeuverStat>();
                changedAttackStatReplacement.ReplacementStat = StatType.Dexterity;
                changedReplaceCombatManeuverStat.StatType = StatType.Dexterity;

            });

            // CONSTITUTION

            var senseiConInsightfulStrike = Helpers.CreateCopy(senseiWisInsightfulStrike, bp => {
                string senseiConInsightfulStrikeName = senseiWisInsightfulStrike.Name + " {g|Encyclopedia:Constitution}Constitution{/g}";
                string senseiConInsightfulStrikeDescription = senseiWisInsightfulStrike.Description.Replace("Wisdom", "Constitution");
                bp.SetName(senseiConInsightfulStrikeName);
                bp.SetDescription(senseiConInsightfulStrikeDescription);
                var changedAttackStatReplacement = bp.GetComponent<AttackStatReplacement>();
                var changedReplaceCombatManeuverStat = bp.GetComponent<ReplaceCombatManeuverStat>();
                changedAttackStatReplacement.ReplacementStat = StatType.Constitution;
                changedReplaceCombatManeuverStat.StatType = StatType.Constitution;

            });

            // INTELLIGENCE

            var senseiIntInsightfulStrike = Helpers.CreateCopy(senseiWisInsightfulStrike, bp => {
                string senseiIntInsightfulStrikeName = senseiWisInsightfulStrike.Name + " {g|Encyclopedia:Intelligence}Intelligence{/g}";
                string senseiIntInsightfulStrikeDescription = senseiWisInsightfulStrike.Description.Replace("Wisdom", "Intelligence");
                bp.SetName(senseiIntInsightfulStrikeName);
                bp.SetDescription(senseiIntInsightfulStrikeDescription);
                var changedAttackStatReplacement = bp.GetComponent<AttackStatReplacement>();
                var changedReplaceCombatManeuverStat = bp.GetComponent<ReplaceCombatManeuverStat>();
                changedAttackStatReplacement.ReplacementStat = StatType.Intelligence;
                changedReplaceCombatManeuverStat.StatType = StatType.Intelligence;

            });


            // CHARISMA

            var senseiChaInsightfulStrike = Helpers.CreateCopy(senseiWisInsightfulStrike, bp => {
                string senseiChaInsightfulStrikeName = senseiWisInsightfulStrike.Name + " {g|Encyclopedia:Charisma}Charisma{/g}";
                string senseiChaInsightfulStrikeDescription = senseiWisInsightfulStrike.Description.Replace("Wisdom", "Charisma");
                bp.SetName(senseiChaInsightfulStrikeName);
                bp.SetDescription(senseiChaInsightfulStrikeDescription);
                var changedAttackStatReplacement = bp.GetComponent<AttackStatReplacement>();
                var changedReplaceCombatManeuverStat = bp.GetComponent<ReplaceCombatManeuverStat>();
                changedAttackStatReplacement.ReplacementStat = StatType.Charisma;
                changedReplaceCombatManeuverStat.StatType = StatType.Charisma;

            });

            // WISDOM

            string senseiWisInsightfulStrikeName = senseiWisInsightfulStrike.Name + " {g|Encyclopedia:Wisdom}Wisdom{/g}";

            senseiWisInsightfulStrike.SetName(senseiWisInsightfulStrikeName);

            Main.Log("Ki Attribute Insightful Strike variants created.");


            // Create a feature which both change parameters used for ki abilities and ability resource change for ki-based feats.

            var kiStatFeatures = new Dictionary<BlueprintFeatureReference, StatType>();


            var KiParametersAndResourcesUnlocker = Helpers.CreateBlueprint<BlueprintFeature>("KiParametersAndResourcesUnlocker", bp =>
            {
                bp.SetName("Ki Parameters and Resource Configurator");
                bp.SetDescription("You use your Ki Ability to determine parameters for all abiliies based on ki and use ki points to fuel any ki-related feat.");
                bp.m_Icon = iconLifeForce;
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.AddComponent(ExH.CreateReplaceAbilityResource(baseStunningFistAbilityResource, kiResource, baseStunningFistAbility.ToReference<BlueprintAbilityReference>(), kiAttributesFacts, 1.0f));
                bp.AddComponent(ExH.CreateReplaceAbilityResource(baseStunningFistAbilityResource, kiResource, baseDragonRoarAbility.ToReference<BlueprintAbilityReference>(), kiAttributesFacts, 1.0f));
                bp.AddComponent(ExH.CreateReplaceAbilityResource(basePerfectStrikeAbilityResource, kiResource, basePerfectStrikeAbility.ToReference<BlueprintAbilityReference>(), kiAttributesFacts, 1.0f));
                bp.AddComponent(ExH.CreateBindAbilitiesToStackableKiClassAndKiStat(kiPowerAbilities, monk.ToReference<BlueprintCharacterClassReference>(), null, null, statFeatureLists, StatType.Wisdom, false, true, 1, 1, true));
            });

            Main.Log("Ki parameters and resources unlocker feature created.");

            // Create a feature which unlocks AC bonus by Ki attribute


            var monkBaseACbonusunlock = Helpers.CreateBlueprint<BlueprintFeature>("monkBaseACbonusunlock", bp =>
            {
                string monkBaseACbonusunlockName = monkWisACbonusunlock.Name + " - Ki Attribute";
                string monkBaseACbonusunlockDescription = monkWisACbonusunlock.Description.Replace("Wisdom", "Ki Attribute");


                bp.SetName(monkBaseACbonusunlockName);
                bp.SetDescription(monkBaseACbonusunlockDescription);
                bp.m_Icon = iconLifeForce;
                bp.IsClassFeature = true;
                bp.Ranks = 1;

                var statUnlockedFeature = new Dictionary<BlueprintFeatureReference, BlueprintFeatureReference>();
                statUnlockedFeature.Add(KiAttributeStrength.ToReference<BlueprintFeatureReference>(), monkStrACbonusunlock.ToReference<BlueprintFeatureReference>());
                statUnlockedFeature.Add(KiAttributeDexterity.ToReference<BlueprintFeatureReference>(), monkDexACbonusunlock.ToReference<BlueprintFeatureReference>());
                statUnlockedFeature.Add(KiAttributeConstitution.ToReference<BlueprintFeatureReference>(), monkConACbonusunlock.ToReference<BlueprintFeatureReference>());
                statUnlockedFeature.Add(KiAttributeIntelligence.ToReference<BlueprintFeatureReference>(), monkIntACbonusunlock.ToReference<BlueprintFeatureReference>());
                statUnlockedFeature.Add(KiAttributeWisdom.ToReference<BlueprintFeatureReference>(), monkWisACbonusunlock.ToReference<BlueprintFeatureReference>());
                statUnlockedFeature.Add(KiAttributeCharisma.ToReference<BlueprintFeatureReference>(), monkChaACbonusunlock.ToReference<BlueprintFeatureReference>());

                bp.AddComponent(ExH.CreateConditionalFactsFeaturesUnlock(statUnlockedFeature,false));


            });

            Main.Log("General attribute AC bonus unlocker feature created.");


            // Create a feature which unlocks ki pool feature.

            var monkBaseKiPowerFeature = Helpers.CreateBlueprint<BlueprintFeature>("monkBaseKiPowerFeature", bp =>
            {
                string monkBaseKiPowerFeatureName = monkKiPowerFeature.Name + " - Ki Attribute";
                string monkBaseKiPowerFeatureDescription = monkKiPowerFeature.Description.Replace("Wisdom", "Ki Attribute");


                bp.SetName(monkBaseKiPowerFeatureName);
                bp.SetDescription(monkBaseKiPowerFeatureDescription);
                bp.m_Icon = iconLifeForce;
                bp.IsClassFeature = true;
                bp.Ranks = 1;

                var statUnlockedFeature = new Dictionary<BlueprintFeatureReference, BlueprintFeatureReference>();
                statUnlockedFeature.Add(KiAttributeStrength.ToReference<BlueprintFeatureReference>(), monkStrKiPowerFeature.ToReference<BlueprintFeatureReference>());
                statUnlockedFeature.Add(KiAttributeDexterity.ToReference<BlueprintFeatureReference>(), monkDexKiPowerFeature.ToReference<BlueprintFeatureReference>());
                statUnlockedFeature.Add(KiAttributeConstitution.ToReference<BlueprintFeatureReference>(), monkConKiPowerFeature.ToReference<BlueprintFeatureReference>());
                statUnlockedFeature.Add(KiAttributeIntelligence.ToReference<BlueprintFeatureReference>(), monkIntKiPowerFeature.ToReference<BlueprintFeatureReference>());
                statUnlockedFeature.Add(KiAttributeWisdom.ToReference<BlueprintFeatureReference>(), monkKiPowerFeature.ToReference<BlueprintFeatureReference>());
                statUnlockedFeature.Add(KiAttributeCharisma.ToReference<BlueprintFeatureReference>(), scaledfistPowerFeature.ToReference<BlueprintFeatureReference>());

                bp.AddComponent(ExH.CreateConditionalFactsFeaturesUnlock(statUnlockedFeature, false));


            });

            Main.Log("General attribute ki pool unlocker feature created.");

            // Create a feature which unlocks insightful strike.

            var senseiBaseInsightfulStrike = Helpers.CreateBlueprint<BlueprintFeature>("senseiBaseInsightfulStrike", bp =>
            {
                string monkBaseKiPowerFeatureName = monkKiPowerFeature.Name + " - Ki Attribute";
                string monkBaseKiPowerFeatureDescription = monkKiPowerFeature.Description.Replace("Wisdom", "Ki Attribute");


                bp.SetName(monkBaseKiPowerFeatureName);
                bp.SetDescription(monkBaseKiPowerFeatureDescription);
                bp.m_Icon = iconLifeForce;
                bp.IsClassFeature = true;
                bp.Ranks = 1;

                var statUnlockedFeature = new Dictionary<BlueprintFeatureReference, BlueprintFeatureReference>();
                statUnlockedFeature.Add(KiAttributeStrength.ToReference<BlueprintFeatureReference>(), senseiStrInsightfulStrike.ToReference<BlueprintFeatureReference>());
                statUnlockedFeature.Add(KiAttributeDexterity.ToReference<BlueprintFeatureReference>(), senseiDexInsightfulStrike.ToReference<BlueprintFeatureReference>());
                statUnlockedFeature.Add(KiAttributeConstitution.ToReference<BlueprintFeatureReference>(), senseiConInsightfulStrike.ToReference<BlueprintFeatureReference>());
                statUnlockedFeature.Add(KiAttributeIntelligence.ToReference<BlueprintFeatureReference>(), senseiIntInsightfulStrike.ToReference<BlueprintFeatureReference>());
                statUnlockedFeature.Add(KiAttributeWisdom.ToReference<BlueprintFeatureReference>(), senseiWisInsightfulStrike.ToReference<BlueprintFeatureReference>());
                statUnlockedFeature.Add(KiAttributeCharisma.ToReference<BlueprintFeatureReference>(), senseiChaInsightfulStrike.ToReference<BlueprintFeatureReference>());

                bp.AddComponent(ExH.CreateConditionalFactsFeaturesUnlock(statUnlockedFeature, false));


            });

            Main.Log("General attribute insightful strike unlocker feature created.");

            // Change stunning fist fatigue and sickened names and create unlockers.

            var monkBasestunningfatiguefeature = Helpers.CreateBlueprint<BlueprintFeature>("monkBasestunningfatiguefeature", bp =>
            {
                string monkBasestunningfatiguefeatureName = monkWisstunningfatiguefeature.Name + " - Ki Attribute";
                string monkBasestunningfatiguefeatureDescription = monkWisstunningfatiguefeature.Description;

                bp.SetName(monkBasestunningfatiguefeatureName);
                bp.SetDescription(monkBasestunningfatiguefeatureDescription);
                bp.m_Icon = iconLifeForce;
                bp.IsClassFeature = true;
                bp.Ranks = 1;

            });

            var monkBasestunningfistsickenedfeature = Helpers.CreateBlueprint<BlueprintFeature>("monkWisstunningfistsickenedfeature", bp =>
            {
                string monkBasestunningfistsickenedfeatureName = monkWisstunningfistsickenedfeature.Name + " - Ki Attribute";
                string monkBasestunningfistsickenedfeatureDescription = monkWisstunningfistsickenedfeature.Description;

                bp.SetName(monkBasestunningfistsickenedfeatureName);
                bp.SetDescription(monkBasestunningfistsickenedfeatureDescription);
                bp.m_Icon = iconLifeForce;
                bp.IsClassFeature = true;
                bp.Ranks = 1;

            });

            string monkStrstunningfatiguefeatureName = monkStrstunningfatiguefeature.Name + " {g|Encyclopedia:Strength}Strength{/g}";
            monkStrstunningfatiguefeature.SetName(monkStrstunningfatiguefeatureName);
            string monkStrstunningfistsickenedfeatureName = monkStrstunningfistsickenedfeature.Name + " {g|Encyclopedia:Strength}Strength{/g}";
            monkStrstunningfistsickenedfeature.SetName(monkStrstunningfistsickenedfeatureName);
            string monkDexstunningfatiguefeatureName = monkDexstunningfatiguefeature.Name + " {g|Encyclopedia:Dexterity}Dexterity{/g}";
            monkDexstunningfatiguefeature.SetName(monkDexstunningfatiguefeatureName);
            string monkDexstunningfistsickenedfeatureName = monkDexstunningfistsickenedfeature.Name + " {g|Encyclopedia:Dexterity}Dexterity{/g}";
            monkDexstunningfistsickenedfeature.SetName(monkDexstunningfistsickenedfeatureName);
            string monkConstunningfatiguefeatureName = monkConstunningfatiguefeature.Name + " {g|Encyclopedia:Constitution}Constitution{/g}";
            monkConstunningfatiguefeature.SetName(monkConstunningfatiguefeatureName);
            string monkConstunningfistsickenedfeatureName = monkConstunningfistsickenedfeature.Name + " {g|Encyclopedia:Constitution}Constitution{/g}";
            monkConstunningfistsickenedfeature.SetName(monkConstunningfistsickenedfeatureName);
            string monkIntstunningfatiguefeatureName = monkIntstunningfatiguefeature.Name + " {g|Encyclopedia:Intelligence}Intelligence{/g}";
            monkIntstunningfatiguefeature.SetName(monkIntstunningfatiguefeatureName);
            string monkIntstunningfistsickenedfeatureName = monkIntstunningfistsickenedfeature.Name + " {g|Encyclopedia:Intelligence}Intelligence{/g}";
            monkIntstunningfistsickenedfeature.SetName(monkIntstunningfistsickenedfeatureName);
            string monkWisstunningfatiguefeatureName = monkWisstunningfatiguefeature.Name + " {g|Encyclopedia:Wisdom}Wisdom{/g}";
            monkWisstunningfatiguefeature.SetName(monkWisstunningfatiguefeatureName);
            string monkWisstunningfistsickenedfeatureName = monkWisstunningfistsickenedfeature.Name + " {g|Encyclopedia:Wisdom}Wisdom{/g}";
            monkWisstunningfistsickenedfeature.SetName(monkWisstunningfistsickenedfeatureName);
            string monkChastunningfatiguefeatureName = monkChastunningfatiguefeature.Name + " {g|Encyclopedia:Charisma}Charisma{/g}";
            monkChastunningfatiguefeature.SetName(monkChastunningfatiguefeatureName);
            string monkChastunningfistsickenedfeatureName = monkChastunningfistsickenedfeature.Name + " {g|Encyclopedia:Charisma}Charisma{/g}";
            monkChastunningfistsickenedfeature.SetName(monkChastunningfistsickenedfeatureName);

            var statUnlockedstunningfatiguefeature = new Dictionary<BlueprintFeatureReference, BlueprintFeatureReference>();
            statUnlockedstunningfatiguefeature.Add(KiAttributeStrength.ToReference<BlueprintFeatureReference>(), monkStrstunningfatiguefeature.ToReference<BlueprintFeatureReference>());
            statUnlockedstunningfatiguefeature.Add(KiAttributeDexterity.ToReference<BlueprintFeatureReference>(), monkDexstunningfatiguefeature.ToReference<BlueprintFeatureReference>());
            statUnlockedstunningfatiguefeature.Add(KiAttributeConstitution.ToReference<BlueprintFeatureReference>(), monkConstunningfatiguefeature.ToReference<BlueprintFeatureReference>());
            statUnlockedstunningfatiguefeature.Add(KiAttributeIntelligence.ToReference<BlueprintFeatureReference>(), monkIntstunningfatiguefeature.ToReference<BlueprintFeatureReference>());
            statUnlockedstunningfatiguefeature.Add(KiAttributeWisdom.ToReference<BlueprintFeatureReference>(), monkWisstunningfatiguefeature.ToReference<BlueprintFeatureReference>());
            statUnlockedstunningfatiguefeature.Add(KiAttributeCharisma.ToReference<BlueprintFeatureReference>(), monkChastunningfatiguefeature.ToReference<BlueprintFeatureReference>());

            var statUnlockedstunningsickenedfeature = new Dictionary<BlueprintFeatureReference, BlueprintFeatureReference>();
            statUnlockedstunningsickenedfeature.Add(KiAttributeStrength.ToReference<BlueprintFeatureReference>(), monkStrstunningfistsickenedfeature.ToReference<BlueprintFeatureReference>());
            statUnlockedstunningsickenedfeature.Add(KiAttributeDexterity.ToReference<BlueprintFeatureReference>(), monkDexstunningfistsickenedfeature.ToReference<BlueprintFeatureReference>());
            statUnlockedstunningsickenedfeature.Add(KiAttributeConstitution.ToReference<BlueprintFeatureReference>(), monkConstunningfistsickenedfeature.ToReference<BlueprintFeatureReference>());
            statUnlockedstunningsickenedfeature.Add(KiAttributeIntelligence.ToReference<BlueprintFeatureReference>(), monkIntstunningfistsickenedfeature.ToReference<BlueprintFeatureReference>());
            statUnlockedstunningsickenedfeature.Add(KiAttributeWisdom.ToReference<BlueprintFeatureReference>(), monkWisstunningfistsickenedfeature.ToReference<BlueprintFeatureReference>());
            statUnlockedstunningsickenedfeature.Add(KiAttributeCharisma.ToReference<BlueprintFeatureReference>(), monkChastunningfistsickenedfeature.ToReference<BlueprintFeatureReference>());


            monkBasestunningfatiguefeature.AddComponent(ExH.CreateConditionalFactsFeaturesUnlock(statUnlockedstunningfatiguefeature, false));
            monkBasestunningfistsickenedfeature.AddComponent(ExH.CreateConditionalFactsFeaturesUnlock(statUnlockedstunningsickenedfeature, false));

            Main.Log("General attribute stunning fist fatigue and sickness unlocker feature created.");


            // Add unlockers to Monk and its archetypes.

            BlueprintProgression monkProgression = Resources.GetBlueprint<BlueprintProgression>("8a91753b978e3b34b9425419179aafd6");
            BlueprintArchetype scaledfistArchetype = Resources.GetBlueprint<BlueprintArchetype>("5868fc82eb11a4244926363983897279");
            BlueprintArchetype senseiArchetype = Resources.GetBlueprint<BlueprintArchetype>("f8767821ec805bf479706392fcc3394c");
            BlueprintArchetype zenarcherArchetype = Resources.GetBlueprint<BlueprintArchetype>("2b1a58a7917084f49b097e86271df21c");
            BlueprintArchetype soheiArchetype = Resources.GetBlueprint<BlueprintArchetype>("fad7c56737ed12e42aacc330acc86428");
            BlueprintArchetype quarterstaffmasterArchetype = Resources.GetBlueprint<BlueprintArchetype>("dde7724382ae4f63aa9786cb9b3b64b2");


            monkProgression.LevelEntries[0].m_Features[2] = monkBaseACbonusunlock.ToReference<BlueprintFeatureBaseReference>();
            monkProgression.LevelEntries[0].Features.Add(KiAttributeSelection.ToReference<BlueprintFeatureBaseReference>());
            monkProgression.LevelEntries[0].Features.Add(KiParametersAndResourcesUnlocker.ToReference<BlueprintFeatureBaseReference>());
            monkProgression.LevelEntries[2].m_Features[0] = monkBaseKiPowerFeature.ToReference<BlueprintFeatureBaseReference>();
            monkProgression.LevelEntries[2].Features.Add(KiExtraAttackUnlock.ToReference<BlueprintFeatureBaseReference>());
            monkProgression.LevelEntries[3].m_Features[3] = monkBasestunningfatiguefeature.ToReference<BlueprintFeatureBaseReference>();
            monkProgression.LevelEntries[7].m_Features[2] = monkBasestunningfistsickenedfeature.ToReference<BlueprintFeatureBaseReference>();

            senseiArchetype.AddFeatures[1].m_Features[0] = senseiBaseInsightfulStrike.ToReference<BlueprintFeatureBaseReference>();

            scaledfistArchetype.AddFeatures[0].Features.Remove(monkChaACbonusunlock.ToReference<BlueprintFeatureBaseReference>());
            scaledfistArchetype.AddFeatures[7].Features.Remove(monkChastunningfatiguefeature.ToReference<BlueprintFeatureBaseReference>());
            scaledfistArchetype.AddFeatures[9].Features.Remove(monkChastunningfistsickenedfeature.ToReference<BlueprintFeatureBaseReference>());
            scaledfistArchetype.RemoveFeatures[0].Features.Remove(monkWisACbonusunlock.ToReference<BlueprintFeatureBaseReference>());
            scaledfistArchetype.RemoveFeatures[7].Features.Remove(monkWisstunningfatiguefeature.ToReference<BlueprintFeatureBaseReference>());
            scaledfistArchetype.RemoveFeatures[9].Features.Remove(monkWisstunningfistsickenedfeature.ToReference<BlueprintFeatureBaseReference>());

            zenarcherArchetype.RemoveFeatures[2].m_Features[1] = monkBasestunningfatiguefeature.ToReference<BlueprintFeatureBaseReference>();
            zenarcherArchetype.RemoveFeatures[12].m_Features[0] = monkBasestunningfistsickenedfeature.ToReference<BlueprintFeatureBaseReference>();

            soheiArchetype.RemoveFeatures[3].m_Features[0] = monkBasestunningfatiguefeature.ToReference<BlueprintFeatureBaseReference>();
            soheiArchetype.RemoveFeatures[6].m_Features[1] = monkBasestunningfistsickenedfeature.ToReference<BlueprintFeatureBaseReference>();

            quarterstaffmasterArchetype.RemoveFeatures[1].m_Features[1] = monkBasestunningfatiguefeature.ToReference<BlueprintFeatureBaseReference>();
            quarterstaffmasterArchetype.RemoveFeatures[2].m_Features[1] = monkBasestunningfistsickenedfeature.ToReference<BlueprintFeatureBaseReference>();

            Main.Log("Classes and archetypes altered to add new features.");

        }


    }
}
