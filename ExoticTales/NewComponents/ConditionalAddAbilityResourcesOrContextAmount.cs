using System;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic;
using Kingmaker.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.UnitLogic.Mechanics;

namespace ExoticTales.NewComponents
{
    [TypeId("600C338FDD284C38B97423BC2E11F4FF")]
    public class ConditionalAddAbilityResourcesOrContextAmount : UnitFactComponentDelegate, ISubscriber, IUnitSubscriber, IUnitReapplyFeaturesOnLevelUpHandler, IResourceAmountBonusHandler
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

        public BlueprintAbilityResource ReplacementResource
        {
            get
            {
                BlueprintAbilityResourceReference resource = this.m_ReplacementResource;
                if (resource == null)
                {
                    return null;
                }
                return resource.Get();
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

        public override void OnActivate()
        {
            if (!this.HasCheckedFacts())
            {
                BlueprintScriptableObject blueprint = this.UseThisAsResource ? base.Fact.Blueprint : this.BaseResource;
                if (!base.IsReapplying)
                {
                    base.Owner.Resources.Add(blueprint, this.RestoreAmount);
                }
            }
        }

        public override void OnDeactivate()
        {
            if (!this.HasCheckedFacts())
            {
                BlueprintScriptableObject blueprint = this.UseThisAsResource ? base.Fact.Blueprint : this.BaseResource;
                if (!base.IsReapplying)
                {
                    base.Owner.Resources.Remove(blueprint);
                }
            }
        }

        public void HandleUnitReapplyFeaturesOnLevelUp()
        {

                if (!this.HasCheckedFacts())
                {
                    BlueprintScriptableObject blueprint = this.UseThisAsResource ? base.Fact.Blueprint : this.BaseResource;
                    if (!base.Owner.Resources.ContainsResource(blueprint))
                    {
                        base.Owner.Resources.Add(blueprint, this.RestoreAmount);
                    }
                    else
                    {
                        if (this.RestoreOnLevelUp)
                        {
                            base.Owner.Resources.Restore(blueprint);
                        }
                    }
                }
                else
                {
                    BlueprintScriptableObject blueprint = this.UseThisAsResource ? base.Fact.Blueprint : this.BaseResource;
                    if (base.Owner.Resources.ContainsResource(blueprint))
                    {
                        base.Owner.Resources.Remove(blueprint);
                    }

                }
            
        }

        public void CalculateMaxResourceAmount(BlueprintAbilityResource resource, ref int bonus)
        {
            if (base.Fact.Active && resource == this.ReplacementResource && this.HasCheckedFacts())
            {
                if (Subtract)
                {
                    bonus -= Value.Calculate(base.Context);
                }
                else
                {
                    bonus += Value.Calculate(base.Context);
                }
            }
        }

        public bool UseThisAsResource;

        // Token: 0x0400AD62 RID: 44386
        [HideIf("UseThisAsResource")]
        [SerializeField]
        [FormerlySerializedAs("Resource")]
        public BlueprintAbilityResourceReference m_BaseResource;

        // Token: 0x0400AD63 RID: 44387
        [ShowIf("UseThisAsResource")]
        public int Amount;

        // Token: 0x0400AD64 RID: 44388
        public bool RestoreAmount;

        // Token: 0x0400AD65 RID: 44389
        public bool RestoreOnLevelUp;

        [SerializeField]
        public BlueprintUnitFactReference[] m_CheckedFacts;

        public bool Not;

        public BlueprintAbilityResourceReference m_ReplacementResource;

        public ContextValue Value;
        public bool Subtract;


















    }
        }
