using System;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using Kingmaker.Utility;


namespace ExoticTales.NewComponents
{
    class KiStatReplaceAbilityDC : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCalculateAbilityParams>, IRulebookHandler<RuleCalculateAbilityParams>, ISubscriber, IInitiatorRulebookSubscriber
    {
        public BlueprintAbility Ability
        {
            get
            {
                BlueprintAbilityReference ability = this.m_Ability;
                if (ability == null)
                {
                    return null;
                }
                return ability.Get();
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
            if (evt.Spell == this.Ability)
            {
                UnitDescriptor unit = evt.Initiator.Descriptor;
                evt.ReplaceStat = new StatType?(FindKiAttributeStat(unit));
            }
        }

        public void OnEventDidTrigger(RuleCalculateAbilityParams evt)
        {
        }

        [SerializeField]
        [FormerlySerializedAs("Ability")]
        public BlueprintAbilityReference m_Ability;

        public StatType DefaultBonusStat;

        public IDictionary<BlueprintFeatureReference, StatType> m_KiStatFeature = new Dictionary<BlueprintFeatureReference, StatType>();


    }
}
