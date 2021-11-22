using System;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Buffs;
using Newtonsoft.Json;
using ExoticTales.Utilities;


namespace ExoticTales.NewComponents
{

    [AllowedOn(typeof(BlueprintUnit), false)]
    [AllowedOn(typeof(BlueprintUnitFact), false)]
    [AllowedOn(typeof(BlueprintBuff), false)]
    [AllowedOn(typeof(BlueprintFeature), false)]
    [TypeId("06D1B9B0C7B145ADB9F0C50B9332637A")]

    class AddBuffOnNonPlayerCharacter : UnitFactComponentDelegate, IGlobalSubscriber, ISubscriber, IPartyHandler
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
            this.Check();
        }


        public override void OnTurnOn()
        {
            this.Check();
        }


        public override void OnTurnOff()
        {
            this.DeactivateBuff();
        }


        public void HandleAddCompanion(UnitEntityData unit)
        {
            this.Check();
        }

        public void HandleCompanionActivated(UnitEntityData unit)
        {
            this.Check();
        }

        public void HandleCompanionRemoved(UnitEntityData unit, bool stayInGame)
        {
            this.Check();
        }

        public void HandleCapitalModeChanged()
        {
            this.Check();
        }

        private void Check()
        {
            UnitEntityData caster = base.Context.MaybeCaster;


            if (!(caster.IsPlayerFaction))
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
