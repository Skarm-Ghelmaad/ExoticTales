﻿using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Blueprints.Validation;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic.Mechanics.Properties;

namespace ExoticTales.NewComponents.Properties
{
    [TypeId("5f193022788a43d28c0bdaa913a21117")]
    class StatValueGetter : PropertyValueGetter
    {
        public override int GetBaseValue(UnitEntityData unit)
        {
            return unit.Stats.GetStat(this.Stat).ModifiedValue;
        }

        public override void ApplyValidation(ValidationContext context)
        {
            base.ApplyValidation(context);
        }

        public StatType Stat;
    }
}
