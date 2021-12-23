using System;
using Kingmaker.Blueprints;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.Utility;
using UnityEngine;
using Kingmaker.Blueprints.Classes;
using UnityEngine.Serialization;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules.Abilities;
using System.Linq;
using Kingmaker.UnitLogic;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.JsonSystem;





namespace ExoticTales.NewComponents
{
    [TypeId("1EDF171F73FB4B35AA402376CCCDF2A6")]
    public class ReplaceAbilityResource : UnitFactComponentDelegate, IAbilityResourceLogic, IInitiatorRulebookHandler<RuleCalculateAbilityParams>, IRulebookHandler<RuleCalculateAbilityParams>, ISubscriber, IInitiatorRulebookSubscriber
    {


        public BlueprintAbility ChangedAbility
        {
            get
            {
                return this.m_Ability;
            }
        }


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

        public BlueprintAbilityResource RequiredResource
        {
            get
            {
                if (this.ChangedAbility == m_CurrentAbility)
                {

                    if (m_EnableResourceSwitch)
                    {
                        return this.m_AlternateAbilityResource;
                    }
                    return this.m_OriginalAbilityResource;
                }
                else
                {

                    IAbilityResourceLogic specificResourceLogic = m_CurrentAbilityData.ResourceLogic;
                    BlueprintAbilityResourceReference requiredResource = specificResourceLogic.RequiredResource.ToReference<BlueprintAbilityResourceReference>();
                    if (requiredResource == null)
                    {
                        return null;
                    }
                    return requiredResource.Get();

                }

            }
        }

        public bool IsSpendResource
        {
            get
            {
                if (this.ChangedAbility == m_CurrentAbility)
                {
                    return true;
                }
                else
                {
                    IAbilityResourceLogic specificResourceLogic = m_CurrentAbilityData.ResourceLogic;
                    return specificResourceLogic.IsSpendResource;
                }
            }
        }

        public void OnEventAboutToTrigger(RuleCalculateAbilityParams evt)
        {

            m_CurrentAbility = evt.Spell;
            m_CurrentAbilityData = evt.AbilityData;

            if (this.ChangedAbility == m_CurrentAbility)
            {
                m_EventInitializator = evt.Initiator.Descriptor;

                if (HasCheckedFacts())
                {
                    m_CurrentResourceLogic = evt.AbilityData.ResourceLogic;
                    m_EnableResourceSwitch = true;

                }


            }


        }


        public void OnEventDidTrigger(RuleCalculateAbilityParams evt)
        {
        }


        public void Spend(AbilityData ability)
        {

        }

        public int CalculateCost(AbilityData ability)
        {
            if (this.ChangedAbility == m_CurrentAbility)
            {

                if (m_EnableResourceSwitch)
                {
                    int originalAmount = 0;
                    int convertedAmount = 0;

                    if (m_OriginalAbilityResource != null)
                    {
                        originalAmount = m_CurrentResourceLogic.CalculateCost(m_CurrentAbilityData);

                    }
                    if ((originalAmount != 0) && (m_AlternateAbilityResource != null))
                    {
                        convertedAmount = (int)(originalAmount * m_ConvertionRatio);

                    }

                    return convertedAmount;
                }
                else
                {
                    int originalAmount = 0;
                    originalAmount = m_CurrentResourceLogic.CalculateCost(m_CurrentAbilityData);
                    return originalAmount;

                }

            }
            else
            {
                IAbilityResourceLogic specificResourceLogic = m_CurrentAbilityData.ResourceLogic;
                return specificResourceLogic.CalculateCost(m_CurrentAbilityData);

            }


        }


        [SerializeField]
        public BlueprintAbilityResourceReference m_OriginalAbilityResource;

        [SerializeField]
        public BlueprintAbilityResourceReference m_AlternateAbilityResource;

        [SerializeField]
        public float m_ConvertionRatio = 1.0f;

        [SerializeField]
        [FormerlySerializedAs("Abilites")]
        public BlueprintAbilityReference m_Ability;

        [SerializeField]
        public BlueprintUnitFactReference[] m_CheckedFacts;

        private BlueprintAbility m_CurrentAbility = null;

        private AbilityData m_CurrentAbilityData = null;

        private UnitDescriptor m_EventInitializator = null;

        private bool m_EnableResourceSwitch = false;

        private IAbilityResourceLogic m_CurrentResourceLogic = null;

        public bool Not;

    }
}
