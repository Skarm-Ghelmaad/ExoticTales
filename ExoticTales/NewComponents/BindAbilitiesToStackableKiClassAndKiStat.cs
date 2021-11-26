using System;
using System.Collections.Generic;
using System.Linq;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.Utility;
using ExoticTales.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace ExoticTales.NewComponents
{

    [AllowMultipleComponents]
    [ComponentName("Stack classes and Attribute bonuses based on Features for Ability DC")]
    [AllowedOn(typeof(BlueprintUnitFact), false)]
    class BindAbilitiesToStackableKiClassAndKiStat : UnitFactComponentDelegate, IRulebookHandler<RuleDispelMagic>, IRulebookHandler<RuleSpellResistanceCheck>, IInitiatorRulebookHandler<RuleCalculateAbilityParams>, IRulebookHandler<RuleCalculateAbilityParams>, IInitiatorRulebookHandler<RuleDispelMagic>, IInitiatorRulebookHandler<RuleSpellResistanceCheck>, ISubscriber, IInitiatorRulebookSubscriber
    {

        public ReferenceArrayProxy<BlueprintAbility, BlueprintAbilityReference> Abilites
        {
            get
            {
                return this.m_Abilites;
            }
        }

        public BlueprintCharacterClass CharacterClass
        {
            get
            {
                BlueprintCharacterClassReference characterClass = this.m_CharacterClass;
                if (characterClass == null)
                {
                    return null;
                }
                return characterClass.Get();
            }
        }

        public ReferenceArrayProxy<BlueprintCharacterClass, BlueprintCharacterClassReference> StackableClasses
        {
            get
            {

                UnitDescriptor unit = this.m_EventInitializator;

                var checkedfeature = new BlueprintFeatureReference();

                BlueprintCharacterClassReference[] stackableClasses = new BlueprintCharacterClassReference[0];

                foreach ( var stckcls in m_StackableClasses)
                {
                    checkedfeature = stckcls.Key;

                    if ((stckcls.Key!= null) && (unit.HasFact(stckcls.Key)))
                    {
                        if ((stackableClasses.Length) == 1)
                        {
                            stackableClasses[0] = stckcls.Value;

                        }
                        else
                        {
                            stackableClasses.AppendToArray(stckcls.Value);

                        }
                    }

                }

                if (stackableClasses[0] != null)
                {
                    return stackableClasses;
                }
                else
                {
                    return null;
                }
            }
         }


        public ReferenceArrayProxy<BlueprintArchetype, BlueprintArchetypeReference> StackableArchetypes
        {
            get
            {

                UnitDescriptor unit = this.m_EventInitializator;

                var checkedfeature = new BlueprintFeatureReference();

                BlueprintArchetypeReference[] stackableArchetypes = new BlueprintArchetypeReference[0];

                foreach (var stckarc in m_StackableArchetypes)
                {
                    checkedfeature = stckarc.Key;

                    if ((stckarc.Key != null) && (unit.HasFact(stckarc.Key)))
                    {
                        if ((stackableArchetypes.Length) == 1)
                        {
                            stackableArchetypes[0] = stckarc.Value;

                        }
                        else
                        {
                            stackableArchetypes.AppendToArray(stckarc.Value);

                        }
                    }

                }

                if (stackableArchetypes[0] != null)
                {
                    return stackableArchetypes;
                }
                else
                {
                    return null;
                }
            }
        }

        public StatType FindKiAttributeStat(UnitDescriptor unit)
        {

            var checkedfeature = new BlueprintFeatureReference();

            var resultingstat = this.DefaultBonusStat;

            foreach (var kistfea in m_KiStatFeature)
            {
                checkedfeature = kistfea.Key;

                if ((kistfea.Key != null) && (unit.HasFact(kistfea.Key)))
                {
                    return resultingstat = kistfea.Value;
                }
            }

            return resultingstat;

        }


        public void OnEventAboutToTrigger(RuleCalculateAbilityParams evt)
        {

            if (this.Abilites.Contains(evt.Spell))
            {
                int level = this.GetLevel(evt.Initiator.Descriptor);
                evt.ReplaceStat = new StatType?(FindKiAttributeStat(evt.Initiator.Descriptor));
                evt.ReplaceCasterLevel = new int?(this.GetLevelBase(level));
                evt.ReplaceSpellLevel = new int?(this.Cantrip ? 0 : (level / 2));
            }


        }


        public void OnEventDidTrigger(RuleCalculateAbilityParams evt)
        {
        }

        public void OnEventAboutToTrigger(RuleDispelMagic evt)
        {
            IEnumerable<BlueprintAbility> source = this.Abilites;
            AbilityData ability = evt.Reason.Ability;
            if (source.Contains((ability != null) ? ability.Blueprint : null) && this.FullCasterChecks)
            {
                evt.Bonus += this.GetLevelDiff(evt.Initiator.Descriptor);
            }
        }

        public void OnEventDidTrigger(RuleDispelMagic evt)
        {
        }

        public void OnEventAboutToTrigger(RuleSpellResistanceCheck evt)
        {
            if (this.Abilites.Contains(evt.Ability) && this.FullCasterChecks)
            {
                evt.AddSpellPenetration(this.GetLevelDiff(evt.Initiator.Descriptor), ModifierDescriptor.UntypedStackable);
            }
        }

        public void OnEventDidTrigger(RuleSpellResistanceCheck evt)
        {
        }

        public int GetLevel(UnitDescriptor unit)
        {
            this.m_EventInitializator = unit;

            int CL = 0;

            CL = ReplaceCasterLevelOfAbility.CalculateClassLevel(this.CharacterClass, this.StackableClasses.ToArray<BlueprintCharacterClass>(), unit, this.StackableArchetypes.ToArray<BlueprintArchetype>());

            if ((this.SetMinCasterLevel) && (CL < this.m_MinCasterLevel))
            {
                return m_MinCasterLevel;
            }
            else
            {
                return CL;

            }

        }

        public int GetLevelBase(int level)
        {

            return (level - (this.Odd ? 1 : 0)) / (this.Cantrip ? 1 : this.LevelStep);

        }

        public int GetLevelDiff(UnitDescriptor unit)
        {
            this.m_EventInitializator = unit;
            int level = this.GetLevel(unit);
            int levelBase = this.GetLevelBase(level);
            return Math.Max(level - levelBase, 0);


        }


        [SerializeField]
        [FormerlySerializedAs("Abilites")]
        public BlueprintAbilityReference[] m_Abilites;

        [SerializeField]
        [FormerlySerializedAs("CharacterClass")]
        public BlueprintCharacterClassReference m_CharacterClass;

        public IDictionary<BlueprintFeatureReference, BlueprintCharacterClassReference> m_StackableClasses = new Dictionary<BlueprintFeatureReference, BlueprintCharacterClassReference>();

        public IDictionary<BlueprintFeatureReference, BlueprintArchetypeReference> m_StackableArchetypes = new Dictionary<BlueprintFeatureReference, BlueprintArchetypeReference>();

        private UnitDescriptor m_EventInitializator;

        bool SetMinCasterLevel = false;

        int m_MinCasterLevel = 1;

        public bool Cantrip;

        [HideIf("Cantrip")]
        public int LevelStep = 1;

        [HideIf("Cantrip")]
        public bool Odd;

        public StatType DefaultBonusStat;

        public IDictionary<BlueprintFeatureReference, StatType> m_KiStatFeature = new Dictionary<BlueprintFeatureReference, StatType>();

        public bool FullCasterChecks = true;

    }
}
