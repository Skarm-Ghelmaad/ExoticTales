using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Blueprints.Classes;
using System;
using System.Linq;
using JetBrains.Annotations;
using Kingmaker.Utility;
using System.Collections.Generic;
using Kingmaker.EntitySystem.Stats;



namespace ExoticTales.NewComponents
{
    [TypeId("02E80EAE216B49F3B586BC7D3A8CDF8C")]
    public class IncreaseResourceAmountBasedOnStatOnly : UnitFactComponentDelegate, IResourceAmountBonusHandler, IUnitSubscriber, ISubscriber
    {


        public ModifiableValueAttributeStat FindHighestAttributeStat(UnitDescriptor unit)
        {
           

            ModifiableValueAttributeStat strength = unit.Stats.GetStat(StatType.Strength) as ModifiableValueAttributeStat;
            ModifiableValueAttributeStat dexterity = unit.Stats.GetStat(StatType.Dexterity) as ModifiableValueAttributeStat;
            ModifiableValueAttributeStat constitution = unit.Stats.GetStat(StatType.Constitution) as ModifiableValueAttributeStat;
            ModifiableValueAttributeStat intelligence = unit.Stats.GetStat(StatType.Intelligence) as ModifiableValueAttributeStat;
            ModifiableValueAttributeStat wisdom = unit.Stats.GetStat(StatType.Wisdom) as ModifiableValueAttributeStat;
            ModifiableValueAttributeStat charisma = unit.Stats.GetStat(StatType.Charisma) as ModifiableValueAttributeStat;


            ModifiableValueAttributeStat[] unitAttributeStats = { strength, dexterity, constitution, intelligence, wisdom, charisma };

            ModifiableValueAttributeStat maxStat = unitAttributeStats.Max();

            return maxStat;

        }

    public int CalculateStatBonusAmount()
        {
            UnitDescriptor unit = base.Owner;

            int num = 0;

            ModifiableValueAttributeStat modifiableValueAttributeStat = null;

            if (NotUseHighestStat)
            {

                modifiableValueAttributeStat = unit.Stats.GetStat(this.ResourceBonusStat) as ModifiableValueAttributeStat;

            }
            else
            {

                modifiableValueAttributeStat = FindHighestAttributeStat(unit);

            }


            if (modifiableValueAttributeStat != null)
            {
                num += (int)((float)modifiableValueAttributeStat.Bonus* TCLResourceMultiplier);
            }
            else
            {
                PFLog.Default.Error("Can't use stat {0} in ability resource's count formula", new object[]
                {
                        this.ResourceBonusStat
                });
            }

            return num;

        }

        public void CalculateMaxResourceAmount(BlueprintAbilityResource resource, ref int bonus)
        {

            if (base.Fact.Active && resource == m_Resource.Get())
            {

                int resource_amount = CalculateStatBonusAmount();


                if (Subtract)
                {
                    bonus -= resource_amount;
                }
                else
                {
                    bonus += resource_amount;
                }


            }

        }



        public BlueprintAbilityResourceReference m_Resource;

        public float TCLResourceMultiplier = 1.0f;        // "True Character Level resource multiplier.

        public bool Subtract = false;

        public bool NotUseHighestStat = true;

        [UsedImplicitly]
        [ShowIf("NotUseHighestStat")]
        public StatType ResourceBonusStat;

    }
}