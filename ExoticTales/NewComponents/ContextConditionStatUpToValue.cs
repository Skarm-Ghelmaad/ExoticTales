using System;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic.Mechanics.Conditions;

// New ContextCondition inspired by ContextConditionStatValue: Checks whether the target has a Stat equal or below the number N.
// In essence it allows to check both if a Stat is equal or below N or (as false) if is greater than N.

namespace ExoticTales.NewComponents
{

    [TypeId("ED8C658F142F4DE090F24B25B7F31E81")]
    public class ContextConditionStatUpToValue : ContextCondition
    {
        public override string GetConditionCaption()
        {
            return "";
        }


        public override bool CheckCondition()
        {
            UnitEntityData unit = base.Target.Unit;
            if (unit == null)
            {
                return false;
            }
            ModifiableValue stat = unit.Stats.GetStat(this.Stat);
            ModifiableValueAttributeStat modifiableValueAttributeStat = stat as ModifiableValueAttributeStat;
            return stat != null && stat.ModifiedValue <= this.N && (modifiableValueAttributeStat == null || !modifiableValueAttributeStat.Disabled);
        }

        public int N;  //Upper bound of the range.

        public StatType Stat;
    }

}

