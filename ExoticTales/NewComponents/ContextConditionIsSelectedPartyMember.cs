using System;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker;
using Kingmaker.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Facts;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Owlcat.Runtime.Visual.Effects.WeatherSystem;
using UnityEngine;
using UnityEngine.Serialization;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Parts;
using Kingmaker.Utility;
using Kingmaker.AreaLogic;
using Kingmaker.Blueprints.Area;
using Kingmaker.Dungeon.Blueprints;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Visual.WeatherSystem;
using Newtonsoft.Json;
using ExoticTales.Utilities;
using Kingmaker.Enums;
using JetBrains.Annotations;
using Kingmaker.Blueprints.Validation;
using Kingmaker.Settings;
using Owlcat.Runtime.Core.Utils;

namespace ExoticTales.NewComponents
{
    public class ContextConditionIsSelectedPartyMember : ContextCondition
    {
        public override string GetConditionCaption()
        {
            return "Check if target is the last selected character within your party";
        }

        public override bool CheckCondition()
        {

            UnitEntityData checkedcharacter = base.Target.Unit;
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
