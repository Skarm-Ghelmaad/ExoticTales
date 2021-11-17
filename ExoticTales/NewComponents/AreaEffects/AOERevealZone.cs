using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker;
using Kingmaker.Controllers;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.ElementsSystem;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using UnityEngine;
using UnityEngine.Serialization;

namespace ExoticTales.NewComponents.AreaEffects
{
    [TypeId("98BE3472C43B4F4B966AC6F5889D16AB")]
    public class AOERevealZone : AbilityAreaEffectLogic
    {

        public BlueprintAbilityAreaEffect AreaBlueprint
        {
            get
            {
                BlueprintAbilityAreaEffectReference areablueprint = this.m_AreaBlueprint;
                if (areablueprint == null)
                {
                    return null;
                }
                return areablueprint.Get();
            }
        }


        public override void OnRound(MechanicsContext context, AreaEffectEntityData areaEffect)
        {
            FogOfWarController.AddRevealer(areaEffect.View.transform);
            foreach (AreaEffectEntityData areaEffectEntityData in Game.Instance.State.AreaEffects)
            {
                if (areaEffectEntityData != areaEffect && areaEffectEntityData.Blueprint == this.AreaBlueprint && areaEffectEntityData.Context.MaybeCaster == areaEffect.Context.MaybeCaster)
                {
                    areaEffectEntityData.FadeOutViewAndDestroy();
                }
            }
        }


        public BlueprintAbilityAreaEffectReference m_AreaBlueprint;

    }


}
