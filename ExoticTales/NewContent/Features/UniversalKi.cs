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
using HlP = ExoticTales.Utilities.Helpers;
using ExH = ExoticTales.Utilities.ExpandedHelpers;

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
                bp.AddComponent(Helpers.Create<IncreaseResourceAmountBasedOnTrueCharacterLevelOnly>(c => {
                    c.m_Resource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();

                }));


            });


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


            // Alter the ScaledFist abilities converted to use standard ki power.


            scaledfistAbudantStep.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 2;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistBarkskin.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 1;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistColdicestrike.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistBlackbreathweaponability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistBluebreathweaponability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistBrassbreathweaponability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistBronzebreathweaponability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistCopperbreathweaponability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistGoldbreathweaponability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistGreenbreathweaponability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistRedbreathweaponability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistSilverbreathweaponability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistWhitebreathweaponability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistDraconicfuryacidability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 1;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistDraconicfurycoldability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 1;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistDraconicfuryelectricityability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 1;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistDraconicfuryfireability.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 1;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistPoisoncast.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 2;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistQuiveringpalm.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 4;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistRestoration.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 2;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistScorchingray.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 2;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistShout.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 3;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistSpitvenom.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 2;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistTruestrike.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 1;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistWholenessofbody.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 2;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistExtraattack.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
                c.m_IsSpendResource = true;
                c.Amount = 1;
                c.ResourceCostIncreasingFacts = null;
                c.ResourceCostDecreasingFacts = null;
            }));

            scaledfistSuddenspeed.ReplaceComponents<AbilityResourceLogic>(Helpers.Create<AbilityResourceLogic>(c => {
                c.m_RequiredResource = KiPotentialResource.ToReference<BlueprintAbilityResourceReference>();
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


            // Create a Ki Attribute selection group.


            BlueprintFeatureReference[] kiAttributes = {
                KiAttributeStrength.ToReference<BlueprintFeatureReference>(),
                KiAttributeDexterity.ToReference<BlueprintFeatureReference>(),
                KiAttributeConstitution.ToReference<BlueprintFeatureReference>(),
                KiAttributeIntelligence.ToReference<BlueprintFeatureReference>(),
                KiAttributeWisdom.ToReference<BlueprintFeatureReference>(),
                KiAttributeCharisma.ToReference<BlueprintFeatureReference>()
            };

            var KiAttributeSelection = Helpers.CreateBlueprint<BlueprintFeatureSelection>("KiAttributeSelection", bp =>
            {
                bp.SetName("Ki Attribute");
                bp.SetDescription("Any character with a ki pool can select which attribute will be used for its abilities and to calculate his ki pool.");
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.Mode = SelectionMode.OnlyNew;
                bp.m_Features = kiAttributes;
            });

            // Create custom Ki Attribute AC bonuses and add reference to the original ones.


            BlueprintFeature monkWisACbonusunlock = Resources.GetBlueprint<BlueprintFeature>("2615c5f87b3d72b42ac0e73b56d895e0"); // Unlock Wis bonus to AC when unarmored.
            BlueprintFeature monkWisACbonus = Resources.GetBlueprint<BlueprintFeature>("e241bdfd6333b9843a7bfd674d607ac4"); // Unlock Wis bonus to AC (apply buff).
            BlueprintBuff monkWisACbonusbuff = Resources.GetBlueprint<BlueprintBuff>("f132c4c4279e4646a05de26635941bfe"); // Wis bonus to AC buff.

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










        }


    }
}
