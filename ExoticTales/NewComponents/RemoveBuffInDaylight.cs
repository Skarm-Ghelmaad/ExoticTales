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

/* Note: This code was inspired by the Light Sensitivity and the ShadowBlending code in RacesUnleashed, which I have ported while taking 
additional inspiration from the AddBuffOnBadWeather code. However my goal was to have a single code that would remove or add a bluff when
the character is exposed to daylight with the following factors:
- Concealment allows for an enhanced buff and a specific descriptor for concealment could be added (this is to allow for 50% miss chance on Blur of ShadowBlending) and this setting is enabled 
  by setting "EnhancedOnConcealment = true", by inputing an "EnhancedEffectBuff" and, if necessary, adding "ConcealmentType" to the chosen descriptor, but
  the default value is set to "Blur". [These elements were added to allow for the ShadowBlending feature]
- I have have added an EnhancingFact check to allow for better expandibility as it wasn't in the original code, but seems a more versatile enhancement 
  condition than concealment. This is enabled by setting "EnhancingFact = true" and by inputing an "EnhancedEffectBuff". While it should work (in theory)
  alongside with the Concealment enhancement, I'd advise to not use them together as safe measure.
- A specific TriggeringBuff allows to activate the buff even if there is the condition "Daylight" ongoing. I have decided to not add a further deactivation
  buff (even if that would make it truly specular), because I felt that "extending" the buff was more valuable. However, the TriggeringBuff condition exists
  because Light Sensitivity is also activated by the Daylight spell. [These elements were added to allow for the Light Sensitivity feature]
- The AddBuffInDaylight is almost a perfect mirror of the above, apart from how the Triggering Buff works (which is the same way, so it enables the buff).
- The condition of "daylight" is defined in the same simplified way used by Race Unleashed, apart from one change:
  > You are outside a "Dungeon" environment.
  > You are not in an "Underground" or "Abyss" setting.
  > It is either "Morning" or "Day".
  > The Weather is "Clear".
  > Any "Evening" or "Night" time, any "Dungeon", any "Underground" or "Abyss" setting and any Weather that is not "Clear" will count as "Low-Light" for purpose of this simplified workaround.


*/

namespace ExoticTales.NewComponents
{

        [AllowedOn(typeof(BlueprintUnit), false)]
        [AllowedOn(typeof(BlueprintUnitFact), false)]
        [AllowedOn(typeof(BlueprintBuff), false)]
        [AllowedOn(typeof(BlueprintFeature), false)]
        [TypeId("CE2E75ED735A4DF4BB9A929B19F4A2B3")]
public class RemoveBuffInDaylight : UnitFactComponentDelegate, IWeatherChangeHandler, IGlobalSubscriber, ISubscriber, IAreaLoadingStagesHandler, IUnitBuffHandler 
        {

            public void OnWeatherChange()
            {
                this.Check();
            }

            public void OnAreaActivated()
            {
                this.Check();
            }

            public override void OnTurnOn()
            {
                this.Check();
            }

            public void OnAreaLoadingComplete()
            {
                this.Check();
            }

            public void OnAreaScenesLoaded()
            {
            }

            public override void OnTurnOff()
            {
                this.DeactivateBuff();
            }


            private void Check()
            {
                BlueprintArea currentlyLoadedArea = Game.Instance.CurrentlyLoadedArea;
                bool flag = (Game.Instance.Player.Weather.ActualWeather == InclemencyType.Clear);
                bool flag2 = !(currentlyLoadedArea.Setting == AreaSetting.Abyss) && !(currentlyLoadedArea.Setting == AreaSetting.Underground);
                if (currentlyLoadedArea != null && !(currentlyLoadedArea is BlueprintDungeonArea) && !currentlyLoadedArea.CampingSettings.IsDungeon && currentlyLoadedArea.CampingSettings.CampingAllowed && flag && flag2 && (Game.Instance.TimeOfDay == TimeOfDay.Morning || Game.Instance.TimeOfDay == TimeOfDay.Day))
                {
                    this.DeactivateBuff();
                    return;
                }
                this.ActivateBuff();
            }

    private bool HasEnhancingFact()
    {
        if (EnhancingFact = null)
        {
            return false;
        }

        return base.Owner.HasFact(EnhancingFact);       

    }
  
    private bool HasConcealment()
            {
                if (!EnhancedOnConcealment)
                {
                    return false;
                }

                UnitPartConcealment unitPartConcealment = base.Owner.Get<UnitPartConcealment>();
                if (unitPartConcealment == null)
                {
                    return false;
                }
                List<UnitPartConcealment.ConcealmentEntry> field = ExpandedHelpers.GetField<List<UnitPartConcealment.ConcealmentEntry>>(unitPartConcealment, "m_Concealments");
                if (field == null)
                {
                    return false;
                }

                switch (ConcealmentType)
                {
                    case "InitiatorIsBlind": 
                         return field.Any((UnitPartConcealment.ConcealmentEntry e) => e.Descriptor == ConcealmentDescriptor.InitiatorIsBlind);

                    case "TargetIsInvisible":
                        return field.Any((UnitPartConcealment.ConcealmentEntry e) => e.Descriptor == ConcealmentDescriptor.TargetIsInvisible);

                    case "Blur":
                        return field.Any((UnitPartConcealment.ConcealmentEntry e) => e.Descriptor == ConcealmentDescriptor.Blur);

                    case "Fog":
                        return field.Any((UnitPartConcealment.ConcealmentEntry e) => e.Descriptor == ConcealmentDescriptor.Fog);

                    case "Displacement":
                        return field.Any((UnitPartConcealment.ConcealmentEntry e) => e.Descriptor == ConcealmentDescriptor.Displacement);

                    case "WindsOfVengenance":
                        return field.Any((UnitPartConcealment.ConcealmentEntry e) => e.Descriptor == ConcealmentDescriptor.WindsOfVengenance);

                    default:
                        return field.Any((UnitPartConcealment.ConcealmentEntry e) => e.Descriptor == ConcealmentDescriptor.Blur);

                }
            }

            public void ActivateBuff()                      // This activates the desired EffectBuff (i.e., dazzled for Light Sensitivity).
            {

                BlueprintBuff blueprintBuff = ((this.HasConcealment() && EnhancedOnConcealment) || this.HasEnhancingFact()) ? this.EnhancedEffectBuff : this.EffectBuff;        // Since the if statement wasn't taken, I had to resort to force HasConcealment to retun "false" if EnhancedConcealment is false.

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

            public void HandleBuffDidAdded(Buff buff)    // This re-activates the desired EffectBuff ALSO when a specific TriggeringBuff is applied to the character.
            {
                if (buff.Blueprint == this.TriggeringBuff && TriggeringBuff != null && buff.Owner == base.Owner && this.m_AppliedBuff == null)
                {
                    this.ActivateBuff();
                }
            }

            public void HandleBuffDidRemoved(Buff buff) // This re-activates the desired EffectBuff when the TriggeringBuff is removed.
            {
                if (buff.Blueprint == this.TriggeringBuff && TriggeringBuff != null && buff.Owner == base.Owner && !UnitHelper.HasFact(base.Owner, this.TriggeringBuff))
                {
                    this.DeactivateBuff();
                    this.Check();
                }
            }

            public BlueprintBuff EffectBuff;

            public BlueprintBuff EnhancedEffectBuff = null;

            public BlueprintBuff TriggeringBuff = null;

            public BlueprintUnitFact EnhancingFact = null;

            public bool EnhancedOnConcealment = false;

            public string ConcealmentType = "Blur";

            public InclemencyType Weather;

            [JsonProperty]
            private Buff m_AppliedBuff;


        }

    
}
