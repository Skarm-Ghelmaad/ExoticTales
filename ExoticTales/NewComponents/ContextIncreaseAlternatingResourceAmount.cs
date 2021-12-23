using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Utility;
using UnityEngine;
using Kingmaker.Blueprints.Facts;


namespace ExoticTales.NewComponents
{
    [TypeId("8D7C69A829924323A6ECFE88CFBDC283")]
    class ContextIncreaseAlternatingResourceAmount : UnitFactComponentDelegate, IResourceAmountBonusHandler, IUnitSubscriber, ISubscriber
    {

        public ReferenceArrayProxy<BlueprintUnitFact, BlueprintUnitFactReference> CheckedFacts
        {
            get
            {
                BlueprintUnitFactReference[] checkedFacts = m_CheckedFacts;
                if (checkedFacts == null)
                {
                    return null;
                }
                return checkedFacts;
            }
        }

        public bool HasCheckedFacts()
        {
            bool hasfacts = false;
            int foundFacts = 0;

            for (int i = 0; i < this.CheckedFacts.Length; i++)
            {
                if (base.Owner.HasFact(this.CheckedFacts[i]))
                {
                    foundFacts += 1;
                }

            }

            hasfacts = (foundFacts == this.CheckedFacts.Length);

            return (hasfacts && !Not) || (!hasfacts && Not);

        }

        public BlueprintAbilityResource BaseResource
        {
            get
            {
                BlueprintAbilityResourceReference resource = this.m_BaseResource;
                if (resource == null)
                {
                    return null;
                }
                return resource.Get();
            }
        }

        public BlueprintAbilityResource AlternateResource
        {
            get
            {
                BlueprintAbilityResourceReference resource = this.m_AlternateResource;
                if (resource == null)
                {
                    return null;
                }
                return resource.Get();
            }
        }

        public void CalculateMaxResourceAmount(BlueprintAbilityResource resource, ref int bonus)
        {
            if (base.Fact.Active && resource == m_BaseResource.Get() && (!HasCheckedFacts()))
            {
                if (BaseSubtract)
                {
                    bonus -= BaseValue.Calculate(base.Context);
                }
                else
                {
                    bonus += BaseValue.Calculate(base.Context);
                }
            }
            else if (base.Fact.Active && resource == m_AlternateResource.Get() && (HasCheckedFacts()))
            {
                if (AlternateSubtract)
                {
                    bonus -= AlternateValue.Calculate(base.Context);
                }
                else
                {
                    bonus += AlternateValue.Calculate(base.Context);
                }
            }

        }

        public BlueprintAbilityResourceReference m_BaseResource;
        public BlueprintAbilityResourceReference m_AlternateResource;
        public ContextValue BaseValue;
        public ContextValue AlternateValue;
        public bool BaseSubtract;
        public bool AlternateSubtract;

        [SerializeField]
        public BlueprintUnitFactReference[] m_CheckedFacts;

        public bool Not;
    }
}
