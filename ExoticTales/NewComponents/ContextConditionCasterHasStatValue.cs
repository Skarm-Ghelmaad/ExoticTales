using System;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using UnityEngine;
using UnityEngine.Serialization;

// New ContextCondition inspired by ContextConditionStatValue and ContextConditionCasterHasFact: Checks whether (or not) the caster has a Stat >= N.

namespace ExoticTales.NewComponents
{

    [TypeId("X")]
    class ContextConditionCasterHasStatValue : ContextCondition
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
            return stat != null && stat.ModifiedValue >= this.N && (modifiableValueAttributeStat == null || !modifiableValueAttributeStat.Disabled);

        }

        public int N;

        public StatType Stat;


    }
}
