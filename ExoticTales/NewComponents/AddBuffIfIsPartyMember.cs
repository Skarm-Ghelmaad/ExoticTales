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

/* Note: This code was added because I was unable to disable Darkvision aura for non-Party members, so I added a component that checks if the unit is a party member
 * and adds a suppressive buff if it's not.

*/


namespace ExoticTales.NewComponents
{

    [AllowedOn(typeof(BlueprintUnit), false)]
    [AllowedOn(typeof(BlueprintUnitFact), false)]
    [AllowedOn(typeof(BlueprintBuff), false)]
    [AllowedOn(typeof(BlueprintFeature), false)]
    [TypeId("2CACC410DE2143D2857FBF515D66EAFE")] 

public class AddBuffIfIsPartyMember : UnitFactComponentDelegate, IGlobalSubscriber, ISubscriber
    {



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

        private void Check()                          // This checks if the unit is a party member.
        {
            if (this.Not == false)
            {
                if (this.OwnerIsPartyMember() == true)
                {

                    this.ActivateBuff();
                    return;
                }
                else
                {
                    this.DeactivateBuff();
                    return;
                }
            }
            if (this.Not == true)
            {
                if (this.OwnerIsPartyMember() == false)
                {

                    this.ActivateBuff();
                    return;
                }
                else
                {
                    this.DeactivateBuff();
                    return;
                }

            }
        }

        private bool OwnerIsPartyMember()
        {
            return base.Owner.IsPlayerFaction;

        }

        public void ActivateBuff()                      // This activates the desired EffectBuff.
        {

            BlueprintBuff blueprintBuff = this.EffectBuff;   

            if (this.m_AppliedBuff != null && this.m_AppliedBuff.Blueprint == blueprintBuff)
            {
                return;
            }
            if (this.m_AppliedBuff != null && this.m_AppliedBuff.Blueprint != blueprintBuff)
            {
                this.m_AppliedBuff.Remove();
                this.m_AppliedBuff = null;
            }
            this.m_AppliedBuff = base.Owner.AddBuff(blueprintBuff, this.Context, null);
            if (this.m_AppliedBuff == null)
            {
                return;
            }
            this.m_AppliedBuff.IsNotDispelable = true;
            this.m_AppliedBuff.IsFromSpell = false;
        }

        public void DeactivateBuff()
        {
            if (this.m_AppliedBuff != null)
            {
                this.m_AppliedBuff.Remove();
                this.m_AppliedBuff = null;
            }
        }


        public BlueprintBuff EffectBuff;

        public bool Not = false;

        [JsonProperty]
        private Buff m_AppliedBuff;

    }
}
