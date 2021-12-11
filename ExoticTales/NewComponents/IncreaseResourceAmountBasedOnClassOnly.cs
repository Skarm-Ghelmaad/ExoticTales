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

    [TypeId("AE9FD43CB1F64389B49859D1C5C0E24D")]
    public class IncreaseResourceAmountBasedOnClassOnly : UnitFactComponentDelegate, IResourceAmountBonusHandler, IUnitSubscriber, ISubscriber
    {
        [CanBeNull]
        public BlueprintCharacterClass CharacterClass
        {
            get
            {
                BlueprintCharacterClassReference characterClass = this.m_CharacterClass;
                if (characterClass == null)
                {
                    return null;
                }
                return characterClass.Get();
            }
        }

        public int CalculateBonusAmount(int classLevel)
        {
            int num = 0;


            if (IncreasedByLevel)
            {
                num = (int)((float)this.BaseValue + (classLevel * LevelIncrease) * TCLResourceMultiplier);

                goto end_result;

            }
            else if (IncreasedByLevelStartPlusDivStep)
            {
                if (this.StartingLevel <= classLevel)
                {
                    if (this.LevelStep == 0)
                    {
                        PFLog.Default.Error("LevelStep is 0. Can't divide by 0", Array.Empty<object>());
                        goto end_result;

                    }
                    else
                    {
                        num += Math.Max(this.StartingIncrease + this.PerStepIncrease * (classLevel - this.StartingLevel) / this.LevelStep, this.MinClassLevelIncrease);
                        goto end_result;
                    }

                }

            }

        end_result:

            return num;


        }

        public void CalculateMaxResourceAmount(BlueprintAbilityResource resource, ref int bonus)
        {

            if (base.Fact.Active && resource == m_Resource.Get())
            {
                UnitDescriptor unit = base.Owner;
                int classlevel = unit.Progression.GetClassLevel(CharacterClass);
                int resource_amount = CalculateBonusAmount(classlevel);


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

        public BlueprintCharacterClassReference m_CharacterClass;

        [UsedImplicitly]
        public int BaseValue = 0;

        [UsedImplicitly]
        public bool IncreasedByLevel;

        [UsedImplicitly]
        [ShowIf("IncreasedByLevel")]
        public int LevelIncrease;

        [UsedImplicitly]
        public bool IncreasedByLevelStartPlusDivStep;

        [UsedImplicitly]
        [ShowIf("IncreasedByLevelStartPlusDivStep")]
        public int StartingLevel;

        [UsedImplicitly]
        [ShowIf("IncreasedByLevelStartPlusDivStep")]
        public int StartingIncrease;

        [UsedImplicitly]
        [ShowIf("IncreasedByLevelStartPlusDivStep")]
        public int LevelStep;

        [UsedImplicitly]
        [ShowIf("IncreasedByLevelStartPlusDivStep")]
        public int PerStepIncrease;


        [UsedImplicitly]
        [ShowIf("IncreasedByLevelStartPlusDivStep")]
        public int MinClassLevelIncrease;

        public float TCLResourceMultiplier = 1.0f;        // "True Character Level resource multiplier.

        public bool Subtract = false;




    }
}
