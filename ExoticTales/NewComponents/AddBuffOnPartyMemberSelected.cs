using System;
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
    [TypeId("9DDC640E8599473DAE3437317E5C8119")]

    class AddBuffOnPartyMemberSelected: UnitFactComponentDelegate, IGlobalSubscriber, ISubscriber, ISelectionHandler
    {
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

            bool flag1 = selected != null;
            bool flag2 = caster.IsPlayerFaction;
            bool flag3 = caster == selected;

            if (flag1 && flag2 && flag3)
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
