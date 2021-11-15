using System;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using UnityEngine;
using UnityEngine.Serialization;

// New ContextCondition inspired by ContextConditionStatValue, ContextConditionCasterHasFact and ContextConditionStatUpToValue: Checks whether (or not) the caster  a Stat within a range between N_UB and N_UL, which can be included or not.

namespace ExoticTales.NewComponents
{

    [TypeId("8BB2D9B4105A49A4AA0D5F820A531F96")]
    class ContextConditionCasterHasStatWithinRange : ContextCondition
    {

        public override string GetConditionCaption()
        {
            return "";
        }

        // Token: 0x0600D9D1 RID: 55761 RVA: 0x00327ED8 File Offset: 0x003260D8
        public override bool CheckCondition()
        {


            UnitEntityData unit = base.Context.MaybeCaster;
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
