using System;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Owlcat.Runtime.Visual.Effects.WeatherSystem;
using UnityEngine;
using UnityEngine.Serialization;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;
using Kingmaker.Utility;
using Kingmaker.AreaLogic;
using Kingmaker.Blueprints.Area;
using Kingmaker.Dungeon.Blueprints;
using ExoticTales.Utilities;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.Blueprints.JsonSystem;


namespace ExoticTales.NewComponents
{
    [TypeId("39F1A77E58654A2ABCB695D1B8FAE5E2")]
    class ContextConditionIsInBrightLight : ContextCondition
    {

        public ReferenceArrayProxy<BlueprintUnitFact, BlueprintUnitFactReference> TriggeringFacts
        {
            get
            {
                return this.m_TriggeringFacts;
            }
        }

        public ReferenceArrayProxy<BlueprintUnitFact, BlueprintUnitFactReference> SuppressingFacts
        {
            get
            {
                return this.m_SuppressingFacts;
            }
        }

        public ReferenceArrayProxy<BlueprintBuff, BlueprintBuffReference> TriggeringBuffs
        {
            get
            {
                return this.m_TriggeringBuffs;

            }
        }

        public ReferenceArrayProxy<BlueprintBuff, BlueprintBuffReference> SuppressingBuffs
        {
            get
            {
                return this.m_SuppressingBuffs;

            }
        }

        public override string GetConditionCaption()
        {
            return "Check if it is a bright light environment (or if the target or the caster is in bright light)";
        }

        public override bool CheckCondition()
        {
            var checkLight = false;

            BlueprintArea currentlyLoadedArea = Game.Instance.CurrentlyLoadedArea;
            BlueprintAreaPart currentlyLoadedAreaPart = Game.Instance.CurrentlyLoadedAreaPart;

            var flag0 = currentlyLoadedArea == null;
            var flag1 = currentlyLoadedAreaPart == null;

            TimeOfDay currentRealTimeOfDay = Game.Instance.TimeOfDay;
            TimeOfDay currentPerceivedAreaTimeOfDay = !flag0 ? currentlyLoadedArea.GetTimeOfDay() : currentRealTimeOfDay;
            TimeOfDay currentPerceivedTimeOfDay = !flag1 ? currentlyLoadedAreaPart.GetTimeOfDay() : currentPerceivedAreaTimeOfDay;
            InclemencyType currentWeather = Game.Instance.Player.Weather.ActualWeather;


            var flag2 = currentlyLoadedArea.IsSingleLightScene == true;
            var flag3 = (currentlyLoadedArea.IndoorType == IndoorType.None);
            var flag4 = (currentRealTimeOfDay == TimeOfDay.Morning) || (currentRealTimeOfDay == TimeOfDay.Day);

            var flag5 = (currentPerceivedTimeOfDay == TimeOfDay.Morning) || (currentPerceivedTimeOfDay == TimeOfDay.Day);

            var flag6 = (currentlyLoadedArea.Setting == AreaSetting.Abyss) && (currentlyLoadedArea.Setting == AreaSetting.Underground);
            var flag7 = !(currentlyLoadedArea.Setting == AreaSetting.None) && ((currentlyLoadedArea.ArtSetting == BlueprintArea.SettingType.Catacombs) || (currentlyLoadedArea.ArtSetting == BlueprintArea.SettingType.Caves)|| (currentlyLoadedArea.ArtSetting == BlueprintArea.SettingType.RaspingRifts));
            var flag8 = (currentlyLoadedArea is BlueprintDungeonArea) || currentlyLoadedArea.CampingSettings.IsDungeon;



            if (flag0)     // If the area is null, it's neither bright light nor dim light, so it is not daylight.
            {
                checkLight = false;
                goto end_of_environment_light;
            }
            if (flag2)    // If the scene is lit by a single light I assume it is bright enough to see in the whole area (generally seems to be used in areas that are brightly lit).
                          // note that by design, I have decided to supersede any further check because -for example- an environmental light
            {
                
                if (exactingCheck == false)     // If exactingCheck is disabled, single light environment counts as bright light.
                {

                    checkLight = true;
                    goto end_of_environment_light;

                }
                else
                {
                    if (flag5 && (!flag3))              // If exactingCheck is enabled, single light environment and perceived morning or day Time of Day indoors counts as bright light.
                    {
                        checkLight = true;
                        goto end_of_environment_light;
                    }
                    if (flag4 && (flag3))              // If exactingCheck is enabled, single light environment and real morning or day Time of Day outdoors counts as bright light.
                    {
                        checkLight = true;
                        goto end_of_environment_light;
                    }
                    else                                // If exactingCheck is enabled, single light environment and any other real or perceived Time of Day counts as NOT bright light.
                    {
                        checkLight = false;
                        goto end_of_environment_light;
                    }

                }
            }
            if (flag3)                                  // If the setting is outdoors.
            {
                if (flag6 || flag7 || flag8)            // If the setting is outdoors and we can reasonably assume by the AreaSetting (Abyss or Underground), by the ArtSetting (Caves, Catacombs or RaspingRifts), by the Blueprint type (BlueprintDungeonArea) or by the camping setting (IsDungeon) that is a generally dimly-lit area.
                {

                    if (exactingCheck == true)         // If the check is exacting, we can assume that the light of a generally-dimly-lit environment won't be as bright as the daylight of a generally brighter environment (i.e., Abyss and Underground are unlikely to be very lit even in the brightest day) IF it is not a single light scene.
                    {
                        checkLight = false;
                        goto end_of_environment_light;
                    }
                    else
                    {
                        if (flag4)                   // If the check is NOT exacting, a morning or day Time of Day outdoors will count as bright lit even for a usually dimly-lit environment  IF it is not a single light scene.
                        {
                            checkLight = true;
                            goto end_of_environment_light;
                        }
                        else
                        {
                            checkLight = false;
                            goto end_of_environment_light;
                        }

                    }
                }
                else                                // If the setting is outdoors and in a non-generally dimly-lit environment, the TimeOfDay will be the paramount factor.
                {
                    if (flag4)                   
                    {
                        checkLight = true;
                        goto end_of_environment_light;
                    }
                    else
                    {
                        checkLight = false;
                        goto end_of_environment_light;
                    }

                }
            }
            else if (!flag3)                        // If the setting is indoors...
            {
                if (exactingCheck == true)         // If the check is exacting, we can assume that the light of a generally-dimly-lit environment won't be as bright as the daylight of a generally brighter environment (i.e., Abyss and Underground are unlikely to be very lit even in the brightest day)
                {
                    if (flag6 || flag7 || flag8)            // This causes to check if the area is generally dim-lit to see if the Perceived TimeOfDay should be considered truly bright.
                    {

                        checkLight = false;
                        goto end_of_environment_light;


                    }
                    else
                    {

                        if (flag5)                          // This is a generally NON dim-lit in morning or day perceived TimeOfDay
                        {
                            checkLight = true;
                            goto end_of_environment_light;
                        }
                        else
                        {
                            checkLight = false;
                            goto end_of_environment_light;
                        }


                    }

                }
                else
                {

                    if (flag5)                          // If the check is not exacting, the TimeOfDay is the main factor.
                    {
                        checkLight = true;
                        goto end_of_environment_light;
                    }
                    else
                    {
                        checkLight = false;
                        goto end_of_environment_light;
                    }


                }


            }


        end_of_environment_light:

            if (weatherCheck)
            {
                if (currentWeather != InclemencyType.Clear)
                {
                    checkLight = false;
                }
            }


            if (checkCaster)                                                // If the caster has this buff, the environmental light is overridden and the target is in bright light.
            {
                UnitEntityData caster = base.Context.MaybeCaster;

                int trBf = 0;                               
                int spBf = 0;
                int trFc = 0;
                int spFc = 0;
                int blEf = 0;                           // Here the concept is simple: If the caster has a triggering buff or fact, blEf is increased by +1, if the caster has a suppressing buff or fact, blEf is decreased by -1
                                                        // A positive total balance means the environmental light is overridden by "bright light", a negative total balance means the environmental light is overridden by "NOT bright light"
                                                        // while a total balance of zero means the environmental light will be used.

                if (triggeringBuffs)
                {
                    foreach (BlueprintBuff blueprintBuff in this.TriggeringBuffs)
                    {
                        if (caster.Buffs.HasFact(blueprintBuff))
                        {
                            trBf++;
                        }
                    }

                    blEf += trBf;

                }
                if (suppressingBuffs)
                {
                    foreach (BlueprintBuff blueprintBuff in this.SuppressingBuffs)
                    {
                        if (caster.Buffs.HasFact(blueprintBuff))
                        {
                            spBf++;
                        }
                    }

                    blEf -= spBf;

                }
                if (triggeringFacts)                                         // If the target has this buff, the environmental light is overridden and the target is in bright light.
                {
                    foreach (BlueprintUnitFact blueprintUnitFact in this.TriggeringFacts)
                    {
                        if (caster.Descriptor.HasFact(blueprintUnitFact))
                        {
                            trFc++;
                        }
                    }

                    blEf += trFc;

                }
                if (suppressingFacts)                                         // If the target has this buff, the environmental light is overridden and the target is in bright light.
                {
                    foreach (BlueprintUnitFact blueprintUnitFact in this.SuppressingFacts)
                    {
                        if (caster.Descriptor.HasFact(blueprintUnitFact))
                        {
                            spFc++;
                        }
                    }

                    blEf -= spFc;
                }

                if (blEf > 0)
                {
                    checkLight = true;
                    goto end_of_caster_check;

                }
                else if (blEf < 0)
                {
                    checkLight = false;
                    goto end_of_caster_check;

                }
                else
                {
                    goto end_of_caster_check;

                }

            }


            end_of_caster_check:


            if (checkTarget)
            {
                UnitEntityData target = base.Target.Unit;

                int trBf = 0;
                int spBf = 0;
                int trFc = 0;
                int spFc = 0;
                int blEf = 0;                           // Here the concept is simple: If the caster has a triggering buff or fact, blEf is increased by +1, if the caster has a suppressing buff or fact, blEf is decreased by -1
                                                        // A positive total balance means the environmental light is overridden by "bright light", a negative total balance means the environmental light is overridden by "NOT bright light"
                                                        // while a total balance of zero means the environmental light will be used.

                if (triggeringBuffs)
                {
                    foreach (BlueprintBuff blueprintBuff in this.TriggeringBuffs)
                    {
                        if (target.Buffs.HasFact(blueprintBuff))
                        {
                            trBf++;
                        }
                    }

                    blEf += trBf;

                }
                if (suppressingBuffs)
                {
                    foreach (BlueprintBuff blueprintBuff in this.SuppressingBuffs)
                    {
                        if (target.Buffs.HasFact(blueprintBuff))
                        {
                            spBf++;
                        }
                    }

                    blEf -= spBf;

                }
                if (triggeringFacts)                                         // If the target has this buff, the environmental light is overridden and the target is in bright light.
                {
                    foreach (BlueprintUnitFact blueprintUnitFact in this.TriggeringFacts)
                    {
                        if (target.Descriptor.HasFact(blueprintUnitFact))
                        {
                            trFc++;
                        }
                    }

                    blEf += trFc;

                }
                if (suppressingFacts)                                         // If the target has this buff, the environmental light is overridden and the target is in bright light.
                {
                    foreach (BlueprintUnitFact blueprintUnitFact in this.SuppressingFacts)
                    {
                        if (target.Descriptor.HasFact(blueprintUnitFact))
                        {
                            spFc++;
                        }
                    }

                    blEf -= spFc;
                }

                if (blEf > 0)
                {
                    checkLight = true;
                    goto end_of_target_check;

                }
                else if (blEf < 0)
                {
                    checkLight = false;
                    goto end_of_target_check;

                }
                else
                {
                    goto end_of_target_check;

                }
            }

            end_of_target_check:

            return checkLight;
        }


        public bool exactingCheck = false;          /* This is to allow for some nuancing in what constitutes "bright light" for the specific check, because, for example, in general, an area that is lit by a single light
                                              in the game is unlikely to have many shadows, but, for examples, Kenabres Square is outdoors and in daylght while Defender's Heart is
                                              indoors and at night (IndoorLikeNight), but both are a single light -which generally means that the whole area is lit- ...
                                              ...however, enabling Darkvision in the Defender's Heart would be silly, because it is obviously an area brightly lit in which Darkvision would not "kick in"!
                                              on other hand, even if it is brightly lit, the Defender's Heart is probably lit by torches or lanterns and its light can't be as strong as broad daylight, so enforcing Light Vulnerability there would be unfair!
                                              The first case would have exactingCheck set to "false", because merely being in a single light environment would be seen as "bright light".
                                              On other hand, the second case would have exactingCheck set to "false", because you'd want to check what the TimeOfDay (or its equivalent) would be.
                                            */

        public bool weatherCheck = false;          /* This setting is, again, to allow for some nuancing:
                                             * For some conditions, having a weather that would hinder sunlight would suffice to say it's not "bright light", but you'd weather to affect all of such checks:
                                             * As an example, outdoor and in daylight would be fair to check weather for Light Vulnerability, since having the sun covered by clouds (or worse by a storm) would probably mitigate its blinding shine.
                                             * On other hand, you'd not want Darkvision to "kick in" in bad weather, so you' remove the check for weather.
                                             * To clarify...this would translate into weatherCheck set to "true" for the first case and weatherCheck set to "false" for the second.
                                             */

        public bool checkCaster = false;           // Check if caster is in bright light. Only to consider if you have included enhancing buffs or facts.
        public bool checkTarget = false;           // Check if target is in bright light. Only to consider if you have included enhancing buffs or facts.
        public bool triggeringBuffs = false;        // Set as "true" if you want to add a list of buffs that, if applied to the caster or the target, will override the current environmental light and cause the bright light.
        public bool triggeringFacts = false;        // Set as "true" if you want to add a list of facts that, if applied to the caster or the target, will override the current environmental light and cause the bright light.
        public bool suppressingBuffs = false;        // Set as "true" if you want to add a list of buffs that, if applied to the caster or the target, will override the current environmental light and negating the bright light.
        public bool suppressingFacts = false;        // Set as "true" if you want to add a list of facts that, if applied to the caster or the target, will override the current environmental light and negating the bright light.



        [SerializeField]
        [FormerlySerializedAs("Buffs")]
        public BlueprintBuffReference[] m_TriggeringBuffs;

        [SerializeField]
        [FormerlySerializedAs("Buffs")]
        public BlueprintBuffReference[] m_SuppressingBuffs;

        [SerializeField]
        [FormerlySerializedAs("Facts")]
        public BlueprintUnitFactReference[] m_TriggeringFacts;

        [SerializeField]
        [FormerlySerializedAs("Facts")]
        public BlueprintUnitFactReference[] m_SuppressingFacts;


    }
}
