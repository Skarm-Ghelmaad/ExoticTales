﻿using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using ExoticTales.Config;
using ExoticTales.Extensions;
using ExoticTales.NewComponents;
using ExoticTales.Utilities;

namespace ExoticTales.NewContent.Races
{
    static class Gnome
    {

        private static readonly BlueprintRace GnomeRace = Resources.GetBlueprint<BlueprintRace>("ef35a22c9a27da345a4528f0d5889157");
        private static readonly BlueprintFeatureSelection GnomeHeritageSelection = Resources.GetBlueprint<BlueprintFeatureSelection>("584d8b50817b49b2bb7aab3d6add8d3a");
        private static readonly BlueprintFeature KeenSenses = Resources.GetBlueprint<BlueprintFeature>("9c747d24f6321f744aa1bb4bd343880d");
        private static readonly BlueprintFeature HatredReptilian = Resources.GetBlueprint<BlueprintFeature>("020bd2ae61de90341b7f874b788c160a");
        private static readonly BlueprintFeature DwarfDefensiveTrainingGiants = Resources.GetBlueprint<BlueprintFeature>("f268a00e42618144e86c9db76af7f3e9");
        private static readonly BlueprintFeature IllusionResistance = Resources.GetBlueprint<BlueprintFeature>("d030df93ea56d2b468650c4acf608f00");
        private static readonly BlueprintFeature GnomeMagic = Resources.GetBlueprint<BlueprintFeature>("deaf46650a9d2dd4f85736ca552f9fb1");
        private static readonly BlueprintFeature Obsessive = Resources.GetBlueprint<BlueprintFeature>("428899c30699b9c44a6c5ee4f74b5f57");
        private static readonly BlueprintFeature SlowSpeedGnome = Resources.GetBlueprint<BlueprintFeature>("09bc9ccb8ee0ffe4b8827066b1ed7e11");
        private static readonly BlueprintFeature PyromaniacGnome = Resources.GetBlueprint<BlueprintFeature>("fc74a68e18a8479a9e8af34e761a70b3");
        private static readonly BlueprintFeature TravellerGnome = Resources.GetBlueprint<BlueprintFeature>("67bf059158b94f8383f21c148489dfb6");

        private static readonly BlueprintFeature DestinyBeyondBirthMythicFeat = Resources.GetBlueprint<BlueprintFeature>("325f078c584318849bfe3da9ea245b9d");

        public static void AddGnomeHeritage()
        {

            var GnomeAbilityModifiers = Helpers.CreateBlueprint<BlueprintFeature>("GnomeAbilityModifiers", bp => {
                bp.IsClassFeature = true;
                bp.HideInUI = true;
                bp.Ranks = 1;
                bp.HideInCharacterSheetAndLevelUp = true;
                bp.Groups = new FeatureGroup[] { FeatureGroup.Racial };
                bp.SetName("Gnome Ability Modifiers");
                bp.SetDescription("Gnomes are physically weak but surprisingly hardy, and their attitude "
                    + "makes them naturally agreeable. They gain +2 Constitution, +2 Charisma, and –2 Strength.");
                bp.AddComponent(Helpers.Create<AddStatBonus>(c => {
                    c.Descriptor = ModifierDescriptor.Racial;
                    c.Stat = StatType.Charisma;
                    c.Value = 2;
                }));
                bp.AddComponent(Helpers.Create<AddStatBonus>(c => {
                    c.Descriptor = ModifierDescriptor.Racial;
                    c.Stat = StatType.Constitution;
                    c.Value = 2;
                }));
                bp.AddComponent(Helpers.Create<AddStatBonusIfHasFact>(c => {
                    c.Descriptor = ModifierDescriptor.Racial;
                    c.Stat = StatType.Strength;
                    c.Value = -2;
                    c.InvertCondition = true;
                    c.m_CheckedFacts = new BlueprintUnitFactReference[] { DestinyBeyondBirthMythicFeat.ToReference<BlueprintUnitFactReference>() };
                }));
                bp.AddComponent(Helpers.Create<RecalculateOnFactsChange>(c => {
                    c.m_CheckedFacts = new BlueprintUnitFactReference[] { DestinyBeyondBirthMythicFeat.ToReference<BlueprintUnitFactReference>() };
                }));
            });
            var GnomeNoAlternateTrait = Helpers.CreateBlueprint<BlueprintFeature>("GnomeNoAlternateTrait", bp => {
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.Groups = new FeatureGroup[] { FeatureGroup.Racial };
                bp.HideInUI = true;
                bp.HideInCharacterSheetAndLevelUp = true;
                bp.SetName("None");
                bp.SetDescription("No Alternate Trait");
            });

            PyromaniacGnome.RemoveComponents<RemoveFeatureOnApply>();
            PyromaniacGnome.AddTraitReplacment(GnomeMagic);
            PyromaniacGnome.AddTraitReplacment(IllusionResistance);
            PyromaniacGnome.AddSelectionCallback(GnomeHeritageSelection);

            TravellerGnome.RemoveComponents<RemoveFeatureOnApply>();
            TravellerGnome.AddTraitReplacment(DwarfDefensiveTrainingGiants);
            TravellerGnome.AddTraitReplacment(HatredReptilian);
            TravellerGnome.AddTraitReplacment(SlowSpeedGnome);
            TravellerGnome.AddSelectionCallback(GnomeHeritageSelection);

            if (ModSettings.AddedContent.Races.IsDisabled("GnomeAlternateTraits")) { return; }
            GnomeRace.SetComponents(Helpers.Create<AddFeatureOnApply>(c => {
                c.m_Feature = GnomeAbilityModifiers.ToReference<BlueprintFeatureReference>();
            }));
            GnomeHeritageSelection.SetName("Alternate Traits");
            GnomeHeritageSelection.SetDescription("The following alternate traits are available.");
            GnomeHeritageSelection.Group = FeatureGroup.KitsuneHeritage;
            GnomeHeritageSelection.SetFeatures(
                GnomeNoAlternateTrait,
                PyromaniacGnome,
                TravellerGnome
            );
        }

        private static void AddTraitReplacment(this BlueprintFeature feature, BlueprintFeature replacement)
        {
            feature.AddComponent(Helpers.Create<RemoveFeatureOnApply>(c => {
                c.m_Feature = replacement.ToReference<BlueprintUnitFactReference>();
            }));
            feature.AddPrerequisiteFeature(replacement);
        }

        private static void AddSelectionCallback(this BlueprintFeature feature, BlueprintFeatureSelection selection)
        {
            feature.AddComponent(Helpers.Create<AddAdditionalRacialFeatures>(c => {
                c.Features = new BlueprintFeatureBaseReference[] { selection.ToReference<BlueprintFeatureBaseReference>() };
            }));
        }
    }
}
