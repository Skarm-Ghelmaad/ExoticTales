using System;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic.Mechanics.Conditions;

// New ContextCondition inspired by ContextConditionStatValue: Checks whether the target has a Stat within a range between N_UB and N_UL, which can be included or not.

namespace ExoticTales.NewComponents
{

    [TypeId("334C565E8F11487DA38F03B730B9BB2A")]
    public class ContextConditionStatValueWithinRange : ContextCondition
    {
        public override string GetConditionCaption()
        {
            return "";
        }

        // Token: 0x0600DA5E RID: 55902 RVA: 0x003289AC File Offset: 0x00326BAC
        public override bool CheckCondition()
        {
            UnitEntityData unit = base.Target.Unit;
            if (unit == null)
            {
                return false;
            }
            ModifiableValue stat = unit.Stats.GetStat(this.Stat);
            ModifiableValueAttributeStat modifiableValueAttributeStat = stat as ModifiableValueAttributeStat;

            bool flag1 = stat != null && (modifiableValueAttributeStat == null || !modifiableValueAttributeStat.Disabled);

            if (flag1 == false)
            {
                return false;
            }

            bool flag2 = true;
            bool flag3 = true;

            if (UB_Incl)
            {
                flag2 = stat.ModifiedValue >= this.N_UB;
            }
            else
            {
                flag2 = stat.ModifiedValue > this.N_UB;
            }

            if (LB_Incl)
            {
                flag3 = stat.ModifiedValue <= this.N_LB;
            }
            else
            {
                flag3 = stat.ModifiedValue < this.N_LB;
            }


            return flag2 && flag3;
        }

        public int N_UB;  //Upper bound of the range.

        public bool UB_Incl = true; //Upper bound is INCLuded in the range [UB_Incl = true : Stat >= N_UB, UB_Incl = false : Stat > N_UB)

        public int N_LB;  //Lower bound of the range.

        public bool LB_Incl = true; //Lower bound is INCLuded in the range [UB_Incl = true : Stat <= N_LB, LB_Incl = false : Stat < N_UB)


        public StatType Stat;
    }

}
