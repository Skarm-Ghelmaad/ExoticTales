using System;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using UnityEngine;
using UnityEngine.Serialization;

// New ContextCondition inspired by ContextConditionStatValue, ContextConditionCasterHasFact and ContextConditionStatUpToValue: Checks whether (or not) the caster has a Stat equal or below the number N.
// In essence it allows to check both if a Stat is equal or below N or (as false) if is greater than N.

namespace ExoticTales.NewComponents
{

    [TypeId("BA8A2E513142488D8AB1C32801030977")]
    class ContextConditionCasterHasStatUpToValue : ContextCondition
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
            return stat != null && stat.ModifiedValue <= this.N && (modifiableValueAttributeStat == null || !modifiableValueAttributeStat.Disabled);

        }

        public int N;

        public StatType Stat;


    }
}
