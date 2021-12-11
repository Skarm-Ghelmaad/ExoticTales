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





namespace ExoticTales.NewComponents
{


    // Note: This component allows both to calculate a resource based on the gestalt character level and to exclude certain classes from the character level calculation.
    [TypeId("1E82477739534D8C954D27372154D907")]
    public class IncreaseResourceAmountBasedOnTrueCharacterLevelOnly : UnitFactComponentDelegate, IResourceAmountBonusHandler, IUnitSubscriber, ISubscriber   
    {

        public int CalculateTrueCharacterLevel()
        {
            UnitDescriptor unit = base.Owner;

            List<BlueprintCharacterClass> unitClasses = unit.Progression.ClassesOrder;
            int vanillaCharacterLevel = unit.Progression.CharacterLevel;
            int gestaltCharacterLevel = 0;

            foreach ( BlueprintCharacterClass unitCharacterClass in unitClasses )
            {
                bool flag1 = true;
                bool flag2 = true;


                if (ApplyClassExclusion)
                {
                    int index = Array.IndexOf(ExcludedClass, unitCharacterClass);

                    if (index > -1)
                    {
                        flag1 = false;
                        vanillaCharacterLevel -= unit.Progression.GetClassLevel(unitCharacterClass);

                    }
                }

                if (ExcludeMythic)
                {
                    if (unitCharacterClass.IsMythic)
                    {
                        flag2 = false;

                    }
                }

                gestaltCharacterLevel += ((flag1 && flag2)? unit.Progression.GetClassLevel(unitCharacterClass) : 0);

            }

            if (gestaltCharacterLevel > vanillaCharacterLevel)
            {
                return gestaltCharacterLevel;

            }
            else
            {
                return vanillaCharacterLevel;

            }

        }

        public int CalculateBonusAmount( int truecharacterlevel )
        {
            int num = 0;
            
            
            if (IncreasedByLevel)
            {
                num = (int)((float)this.BaseValue + (truecharacterlevel * LevelIncrease) * TCLResourceMultiplier);

                goto end_result;

            }
            else if (IncreasedByLevelStartPlusDivStep)
            {
                if (this.StartingLevel <= truecharacterlevel)
                {
                    if (this.LevelStep == 0)
                    {
                        PFLog.Default.Error("LevelStep is 0. Can't divide by 0", Array.Empty<object>());
                        goto end_result;

                    }
                    else
                    {
                        num += Math.Max(this.StartingIncrease + this.PerStepIncrease * (truecharacterlevel - this.StartingLevel) / this.LevelStep, this.MinClassLevelIncrease);
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

                int trueCharacterLevel = CalculateTrueCharacterLevel();
                int resource_amount = CalculateBonusAmount(trueCharacterLevel);


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

        public ReferenceArrayProxy<BlueprintCharacterClass, BlueprintCharacterClassReference> ExcludedClass
        {
            get
            {
                return new ReferenceArrayProxy<BlueprintCharacterClass, BlueprintCharacterClassReference>(this.m_ExcludedClass);
            }
        }



        public BlueprintAbilityResourceReference m_Resource;

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

        public bool ApplyClassExclusion = false;

        [UsedImplicitly]
        [ShowIf("ApplyClassExclusion")]
        public BlueprintCharacterClassReference[] m_ExcludedClass;

        public bool ExcludeMythic = false;

        public float TCLResourceMultiplier = 1.0f;        // "True Character Level resource multiplier.

        public bool Subtract = false;




    }
}
