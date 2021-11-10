using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using ExoticTales.Config;
using ExoticTales.Extensions;
using ExoticTales.NewComponents;
using ExoticTales.Utilities;

namespace ExoticTales.NewContent.Races
{
    static class Elf
    {

        private static readonly BlueprintRace ElfRace = Resources.GetBlueprint<BlueprintRace>("25a5878d125338244896ebd3238226c8");
        private static readonly BlueprintFeatureSelection ElvenHeritageSelection = Resources.GetBlueprint<BlueprintFeatureSelection>("5482f879dcfd40f9a3168fdb48bc938c");
        private static readonly BlueprintFeature ElvenWeaponFamiliarity = Resources.GetBlueprint<BlueprintFeature>("03fd1e043fc678a4baf73fe67c3780ce");
        private static readonly BlueprintFeature ElvenImmunities = Resources.GetBlueprint<BlueprintFeature>("2483a523984f44944a7cf157b21bf79c");
        private static readonly BlueprintFeature ElvenMagic = Resources.GetBlueprint<BlueprintFeature>("55edf82380a1c8540af6c6037d34f322");
        private static readonly BlueprintFeature KeenSenses = Resources.GetBlueprint<BlueprintFeature>("9c747d24f6321f744aa1bb4bd343880d");

        private static readonly BlueprintFeature BlightbornElf = Resources.GetBlueprint<BlueprintFeature>("2a300f4e0c13495bbde59160809fce7f");
        private static readonly BlueprintFeature LoremasterElf = Resources.GetBlueprint<BlueprintFeature>("fb69a451e7064015b5dbe512c9122ef8");

        private static readonly BlueprintFeature DestinyBeyondBirthMythicFeat = Resources.GetBlueprint<BlueprintFeature>("325f078c584318849bfe3da9ea245b9d");

        private static readonly BlueprintBuff StygianSlayerShadowyMistFormBuff = Resources.GetBlueprint<BlueprintBuff>("8de8e078992d7a4479f3d76e21aa1195");


        public static void AddElfHeritage()
        {

            var ElfAbilityModifiers = Helpers.CreateBlueprint<BlueprintFeature>("ElfAbilityModifiers", bp => {
                bp.IsClassFeature = true;
                bp.HideInUI = true;
                bp.Ranks = 1;
                bp.HideInCharacterSheetAndLevelUp = true;
                bp.Groups = new FeatureGroup[] { FeatureGroup.Racial };
                bp.SetName("Elf Ability Modifiers");
                bp.SetDescription("Elves are nimble, both in body and mind, but their form is frail. They gain +2 Dexterity, +2 Intelligence, and –2 Constitution.");
                bp.AddComponent(Helpers.Create<AddStatBonus>(c => {
                    c.Descriptor = ModifierDescriptor.Racial;
                    c.Stat = StatType.Intelligence;
                    c.Value = 2;
                }));
                bp.AddComponent(Helpers.Create<AddStatBonus>(c => {
                    c.Descriptor = ModifierDescriptor.Racial;
                    c.Stat = StatType.Dexterity;
                    c.Value = 2;
                }));
                bp.AddComponent(Helpers.Create<AddStatBonusIfHasFact>(c => {
                    c.Descriptor = ModifierDescriptor.Racial;
                    c.Stat = StatType.Constitution;
                    c.Value = -2;
                    c.InvertCondition = true;
                    c.m_CheckedFacts = new BlueprintUnitFactReference[] { DestinyBeyondBirthMythicFeat.ToReference<BlueprintUnitFactReference>() };
                }));
                bp.AddComponent(Helpers.Create<RecalculateOnFactsChange>(c => {
                    c.m_CheckedFacts = new BlueprintUnitFactReference[] { DestinyBeyondBirthMythicFeat.ToReference<BlueprintUnitFactReference>() };
                }));
            });
            var ElfNoAlternateTrait = Helpers.CreateBlueprint<BlueprintFeature>("ElfNoAlternateTrait", bp => {
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.Groups = new FeatureGroup[] { FeatureGroup.Racial };
                bp.HideInUI = true;
                bp.HideInCharacterSheetAndLevelUp = true;
                bp.SetName("None");
                bp.SetDescription("No Alternate Trait");
            });




            var ElfDrowFeature = Helpers.CreateBlueprint<BlueprintFeature>("ElfDrowFeature", bp => {
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.Groups = new FeatureGroup[] { FeatureGroup.Racial };
                bp.SetName("Drow Elf");
                bp.SetDescription("Cruel and cunning, drow are a dark reflection of the elven race. Also called dark elves, they dwell deep underground "
                    + "\nin elaborate cities shaped from the rock of cyclopean caverns. Drow seldom make themselves known to surface folk,"
                    + "\npreferring to remain legends while advancing their sinister agendas through proxies and agents."
                    + "\n Drow have no love for anyone but themselves, and are adept at manipulating other creatures."
                    + "\n While they are not born evil, malignancy is deep-rooted in their culture and society, and nonconformists"
                    + "\nrarely survive for long."
                    + "\n   Some stories tell that given the right circumstances, a particularly hateful elf might turn into a drow,"
                    + "\nthough such a transformation would require a truly heinous individual."
                    + "\n Elves with this racial trait gain +2 Dexterity, +2 Charisma, and -2 Constitution and "
                    + "\n gain the Spell Resistance, the Drow Magic, the Drow Weapon Familiarity, the Ancestral Grudge and"
                    + "\n the Poison Use racial traits, but also gain the Light Sensitivity and the Stained Reputation weaknesses"
                    + "\n and lose the Elven Magic trait.");
                bp.AddComponent(Helpers.Create<AddStatBonus>(c => {
                    c.Descriptor = ModifierDescriptor.Racial;
                    c.Stat = StatType.Charisma;
                    c.Value = 2;
                }));
                bp.AddComponent(Helpers.Create<AddStatBonus>(c => {
                    c.Descriptor = ModifierDescriptor.Racial;
                    c.Stat = StatType.Dexterity;
                    c.Value = 2;
                }));
                bp.AddComponent(Helpers.Create<AddStatBonusIfHasFact>(c => {
                    c.Descriptor = ModifierDescriptor.Racial;
                    c.Stat = StatType.Constitution;
                    c.Value = -2;
                    c.InvertCondition = true;
                    c.m_CheckedFacts = new BlueprintUnitFactReference[] { DestinyBeyondBirthMythicFeat.ToReference<BlueprintUnitFactReference>() };
                }));
                bp.AddComponent(Helpers.Create<RecalculateOnFactsChange>(c => {
                    c.m_CheckedFacts = new BlueprintUnitFactReference[] { DestinyBeyondBirthMythicFeat.ToReference<BlueprintUnitFactReference>() };
                }));
                bp.AddTraitReplacment(ElfAbilityModifiers);
                bp.AddTraitReplacment(ElvenMagic);
                bp.AddTraitReplacment(ElvenWeaponFamiliarity);
                bp.AddSelectionCallback(ElvenHeritageSelection);
            });


            BlightbornElf.RemoveComponents<RemoveFeatureOnApply>();
            BlightbornElf.AddTraitReplacment(ElvenImmunities);
            BlightbornElf.AddTraitReplacment(ElvenMagic);
            BlightbornElf.AddSelectionCallback(ElvenHeritageSelection);

            LoremasterElf.RemoveComponents<RemoveFeatureOnApply>();
            LoremasterElf.AddTraitReplacment(KeenSenses);
            LoremasterElf.AddTraitReplacment(ElvenMagic);
            LoremasterElf.AddSelectionCallback(ElvenHeritageSelection);

            if (ModSettings.AddedContent.Races.IsDisabled("ElfAlternateTraits")) { return; }
            ElfRace.SetComponents(Helpers.Create<AddFeatureOnApply>(c => {
                c.m_Feature = ElfAbilityModifiers.ToReference<BlueprintFeatureReference>();
            }));
            ElvenHeritageSelection.SetName("Alternate Traits");
            ElvenHeritageSelection.SetDescription("The following alternate traits are available.");
            ElvenHeritageSelection.Group = FeatureGroup.KitsuneHeritage;
            ElvenHeritageSelection.SetFeatures(
                ElfNoAlternateTrait,
                BlightbornElf,
                LoremasterElf
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
