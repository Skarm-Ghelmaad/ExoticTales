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
    [TypeId("7E959215CA1946858DD01AD9E5B8D3A6")]
    public class AOEConditionalRevealZone : AbilityAreaEffectLogic   //This is a variant of AOERevealZone generated to minimize the creation of revealers by checking if the caster is in Fog of War.
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

                bool flag1 = areaEffectEntityData.Context.MaybeCaster == areaEffect.Context.MaybeCaster;
                bool flag2;

                if (flag1)
                {
                    flag2 = areaEffect.Context.MaybeCaster.IsInFogOfWar;
                }
                else
                {
                    flag2 = false;
                }

                if (areaEffectEntityData != areaEffect && areaEffectEntityData.Blueprint == this.AreaBlueprint && flag2)
                {
                    areaEffectEntityData.FadeOutViewAndDestroy();
                }
            }
        }


        public BlueprintAbilityAreaEffectReference m_AreaBlueprint;

    }


}
