using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Owlcat.Runtime.Visual.Effects.WeatherSystem;
using UnityEngine;
using UnityEngine.Serialization;
using Kingmaker.EntitySystem.Entities;
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
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.Controllers;
using JetBrains.Annotations;
using Kingmaker.Blueprints.Validation;
using Kingmaker.Settings;
using Owlcat.Runtime.Core.Utils;
using Kingmaker.EntitySystem;

namespace ExoticTales.NewComponents
{
    class AddBuffOnNotPartyMemberSelected : UnitFactComponentDelegate, IGlobalSubscriber, ISubscriber, ISelectionHandler   
    {
        // Note: This is a special component that applies the buff if the character is a NPC or a non-selected party member.

        public Buff FindAppliedBuff(UnitEntityData unit, BlueprintBuff checkedbpBuff)
        {
            foreach (Buff buff in unit.Buffs.RawFacts)
            {
                if (buff.Blueprint == checkedbpBuff)
                {
                    return buff;
                }
            }
            return null;
        }


        public BlueprintBuff EffectBuff
        {
            get
            {
                BlueprintBuffReference buff = this.m_EffectBuff;
                if (buff == null)
                {
                    return null;
                }
                return buff.Get();
            }
        }

        public void OnAreaActivated()
        {
            UnitEntityData selected = this.m_LastSelectedUnit;
            this.Check(selected);
        }

        public override void OnTurnOn()
        {
            UnitEntityData selected = this.m_LastSelectedUnit;
            this.Check(selected);
        }


        public override void OnTurnOff()
        {
            this.DeactivateBuff();
        }


        public void OnUnitSelectionAdd(UnitEntityData selected)
        {
            this.m_LastSelectedUnit = selected;
            this.Check(selected);
        }

        public void OnUnitSelectionRemove(UnitEntityData selected)
        {
            this.Check(null);
        }


        private void Check(UnitEntityData selected)
        {
            UnitEntityData caster = base.Context.MaybeCaster;

            bool flag1 = selected == null;
            bool flag2 = caster.IsPlayerFaction;
            bool flag3 = caster != selected;

            if (flag1 || (flag2 && !flag3) || !flag2)
            {
                this.ActivateBuff();
                return;
            }
            this.DeactivateBuff();
        }



        public void ActivateBuff()
        {

            BlueprintBuff blueprintBuff = this.EffectBuff;
            UnitEntityData unit = base.Owner;

            Buff AppliedBuff = this.FindAppliedBuff(unit, blueprintBuff);


            if (AppliedBuff != null)
            {
                return;
            }
            AppliedBuff = base.Owner.AddBuff(blueprintBuff, this.Context, null);
            if (AppliedBuff == null)
            {
                return;
            }
            AppliedBuff.IsNotDispelable = true;
            AppliedBuff.IsFromSpell = false;
            this.m_AppliedBuff = AppliedBuff;
        }

        public void DeactivateBuff()
        {
            Buff AppliedBuff = this.m_AppliedBuff;

            if (AppliedBuff != null)
            {
                AppliedBuff.Remove();
                this.m_AppliedBuff = null;
            }
        }

        public BlueprintBuffReference m_EffectBuff;

        [JsonProperty]
        private Buff m_AppliedBuff;

        private UnitEntityData m_LastSelectedUnit;

    }

}