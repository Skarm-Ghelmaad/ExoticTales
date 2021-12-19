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
using UnityEngine;
using Kingmaker.Blueprints.Facts;


namespace ExoticTales.NewComponents
{
    class ContextIncreaseAlternateResourceAmountBasedOnTrueCharacterLevelOnly : UnitFactComponentDelegate, IResourceAmountBonusHandler, IUnitSubscriber, ISubscriber
    {

        public ReferenceArrayProxy<BlueprintUnitFact, BlueprintUnitFactReference> CheckedFacts
        {
            get
            {
                BlueprintUnitFactReference[] checkedFacts = m_CheckedFacts;
                if (checkedFacts == null)
                {
                    return null;
                }
                return checkedFacts;
            }
        }

        public bool HasCheckedFacts()
        {
            bool hasfacts = false;
            int foundFacts = 0;

            for (int i = 0; i < this.CheckedFacts.Length; i++)
            {
                if (base.Owner.HasFact(this.CheckedFacts[i]))
                {
                    foundFacts += 1;
                }

            }

            hasfacts = (foundFacts == this.CheckedFacts.Length);

            return (hasfacts && !Not) || (!hasfacts && Not);

        }

        public int CalculateTrueCharacterLevel()
        {
            UnitDescriptor unit = base.Owner;

            List<BlueprintCharacterClass> unitClasses = unit.Progression.ClassesOrder;
            int vanillaCharacterLevel = unit.Progression.CharacterLevel;
            int gestaltCharacterLevel = 0;

            foreach (BlueprintCharacterClass unitCharacterClass in unitClasses)
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

                gestaltCharacterLevel += ((flag1 && flag2) ? unit.Progression.GetClassLevel(unitCharacterClass) : 0);

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

        public int CalculateBonusAmount(int truecharacterlevel)
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

            if (base.Fact.Active && resource == m_BaseResource.Get() && (!HasCheckedFacts()))
            {

                int trueCharacterLevel = CalculateTrueCharacterLevel();
                int resource_amount = (int)((float)CalculateBonusAmount(trueCharacterLevel) * BaseBonusOverrideMultiplier);


                if (BaseSubtract)
                {
                    bonus -= resource_amount;
                }
                else
                {
                    bonus += resource_amount;
                }
            }
            else if (base.Fact.Active && resource == m_AlternateResource.Get() && (HasCheckedFacts()))
            {

                int trueCharacterLevel = CalculateTrueCharacterLevel();
                int resource_amount = (int)((float)CalculateBonusAmount(trueCharacterLevel) * AlternateBonusOverrideMultiplier);


                if (AlternateSubtract)
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



        public BlueprintAbilityResourceReference m_BaseResource;
        public BlueprintAbilityResourceReference m_AlternateResource;

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

        public bool BaseSubtract;
        public bool AlternateSubtract;

        [SerializeField]
        public BlueprintUnitFactReference[] m_CheckedFacts;

        public bool Not;

        public float BaseBonusOverrideMultiplier = 1.0f;        // Multiplier to alter the bonus amount for the bonus resource.
        public float AlternateBonusOverrideMultiplier = 1.0f;        // Multiplier to alter the bonus amount for the alternate resource.






    }
}

