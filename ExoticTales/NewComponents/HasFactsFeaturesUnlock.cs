using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic;
using UnityEngine;
using ExoticTales.Extensions;
using Kingmaker.Blueprints.JsonSystem;

namespace ExoticTales.NewComponents
{
    [TypeId("FEACB131A929497DA71E9EE84952F753")]
    public class HasFactsFeaturesUnlock : UnitFactComponentDelegate<HasFactsFeaturesUnlockData>, IUnitGainFactHandler, IUnitLostFactHandler, IUnitSubscriber, ISubscriber
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

        public ReferenceArrayProxy<BlueprintUnitFact, BlueprintUnitFactReference> Features
        {
            get
            {
                BlueprintUnitFactReference[] features = m_Features;
                if (features == null)
                {
                    return null;
                }
                return features;
            }
        }

        public override void OnTurnOn()
        {
            Update();
        }

        public override void OnTurnOff()
        {
            RemoveFact();
        }

        public override void OnActivate()
        {
            Update();
        }

        public override void OnDeactivate()
        {
            RemoveFact();
        }

        public override void OnPostLoad()
        {
            Update();
        }

        private void Apply()
        {
                if (base.Data.AppliedFacts != null)
                {
                    return;
                }

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

                if ((hasfacts && !Not) || (!hasfacts && Not))
                {

                    base.Data.AppliedFacts = new EntityFact[0];

                    for (int i1 = 0; i1 < this.Features.Length; i1++)
                    {
                        int i2 = 0;

                        if (!(base.Owner.HasFact(this.Features[i1])))
                        {

                            base.Data.AppliedFacts.AppendToArray(Owner.AddFact(Features[i1], null, null));


                        }

                    }

                }


        }
        

        private void RemoveFact()
        {
            if (Data.AppliedFacts != null)
            {
                for (int i = 0; i < Data.AppliedFacts.Length; i++)
                {

                    Owner.RemoveFact(Data.AppliedFacts[i]);

                }
                Data.AppliedFacts = null;
            }
        }

        private void Update()
        {
            if (ShouldApply())
            {
                Apply();
            }
            else
            {
                RemoveFact();
            }
        }

        private bool ShouldApply()
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

            return base.Data.AppliedFacts == null && (hasfacts && !Not) || (!hasfacts && Not);
        }

        public void HandleUnitGainFact(EntityFact fact)
        {
            Update();
        }

        public void HandleUnitLostFact(EntityFact fact)
        {
            Update();
        }


        [SerializeField]
        public BlueprintUnitFactReference[] m_CheckedFacts;

        [SerializeField]
        public BlueprintUnitFactReference[] m_Features;

        public bool Not;


    }
}
