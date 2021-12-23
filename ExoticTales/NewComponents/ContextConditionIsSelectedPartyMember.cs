using System;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.EntitySystem.Entities;
using Kingmaker;
using ExoticTales.Utilities;
using Kingmaker.Blueprints.JsonSystem;


namespace ExoticTales.NewComponents
{
    [TypeId("AE46D7C482C8443998415D2FD95ECD78")]
    public class ContextConditionIsSelectedPartyMember : ContextCondition
    {
        public override string GetConditionCaption()
        {
            return "Check if caster is the last selected character within your party";
        }

        public override bool CheckCondition()
        {

            UnitEntityData checkedcharacter = base.Context.MaybeCaster;
            UnitEntityData lastselectedcharacter = Game.Instance.SelectionCharacter.CurrentSelectedCharacter;

            if ((lastselectedcharacter == checkedcharacter) && (checkedcharacter.IsPlayerFaction))
            {

                return true;

            }
            else 
            {

                return false;

            }

        }


    }
}
