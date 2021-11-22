using System;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.EntitySystem.Entities;
using Kingmaker;
using ExoticTales.Utilities;


namespace ExoticTales.NewComponents
{
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
