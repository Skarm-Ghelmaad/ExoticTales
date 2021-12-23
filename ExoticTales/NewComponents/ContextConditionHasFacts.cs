using System;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.JsonSystem;
using UnityEngine;
using UnityEngine.Serialization;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Conditions;

namespace ExoticTales.NewComponents
{
    [TypeId("B0DCA7D1938B42ECBAC08F710A1DBD00")]
    public class ContextConditionHasFacts : ContextCondition
    {

        public ReferenceArrayProxy<BlueprintUnitFact, BlueprintUnitFactReference> Facts
        {
            get
            {
                return this.m_Facts;
            }
        }

        public override string GetConditionCaption()
        {
            return "";
        }

        public override bool CheckCondition()
        {
            bool hasfacts = false;
            int foundFacts = 0;

            for (int i = 0; i < this.Facts.Length; i++)
            {
                if (base.Target.Unit.Descriptor.HasFact(this.Facts[i]))
                {
                    foundFacts += 1;
                }

            }

            hasfacts = (foundFacts == this.Facts.Length);

            return hasfacts;
        }

        [SerializeField]
        public BlueprintUnitFactReference[] m_Facts;
    }
}
