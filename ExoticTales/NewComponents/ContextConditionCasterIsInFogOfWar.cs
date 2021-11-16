using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using UnityEngine;
using UnityEngine.Serialization;
using Kingmaker.Controllers;

namespace ExoticTales.NewComponents
{

    [TypeId("B996A0F6821547329F62D9DF9245876C")]
    public class ContextConditionCasterIsInFogOfWar : ContextCondition
    {

        public override string GetConditionCaption()
        {
            return "Check if caster is in fog of war";
        }


        public override bool CheckCondition()
        {
            if (base.Context.MaybeCaster == null)
            {
                return false;
            }

            return base.Context.MaybeCaster.IsInFogOfWar;
        }

    }
}
