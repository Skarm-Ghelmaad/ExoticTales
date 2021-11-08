﻿using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Items;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Parts;
using ExoticTales.NewUnitParts;

namespace ExoticTales.NewComponents.AbilitySpecific
{
    [AllowedOn(typeof(BlueprintUnitFact))]
    [TypeId("a945c1d2b2d44247bd37d651665d4f54")]
    class FocusedWeaponDamageComponent : UnitFactComponentDelegate,
        IInitiatorRulebookHandler<RuleCalculateWeaponStats>,
        IRulebookHandler<RuleCalculateWeaponStats>,
        ISubscriber,
        IInitiatorRulebookSubscriber
    {

        public void OnEventAboutToTrigger(RuleCalculateWeaponStats evt)
        {
            if (IsValidWeapon(evt.Weapon))
            {
                var classLevel = this.Owner.Progression.GetClassLevel(CheckedClass);
                DiceFormula? formula = classLevel switch
                {
                    >= 1 and < 6 => new DiceFormula(1, DiceType.D6),
                    >= 6 and < 10 => new DiceFormula(1, DiceType.D8),
                    >= 10 and < 15 => new DiceFormula(1, DiceType.D10),
                    >= 15 and < 20 => new DiceFormula(2, DiceType.D6),
                    20 => new DiceFormula(2, DiceType.D8),
                    _ => null
                };
                evt.WeaponDamageDiceOverride = formula;
            }
        }

        public bool HasWeaponTraining(ItemEntityWeapon weapon)
        {
            var weaponTaining = this.Owner.Get<UnitPartWeaponTraining>();
            return (weaponTaining?.GetWeaponRank(weapon) > 0);
        }

        public bool IsValidWeapon(ItemEntityWeapon weapon)
        {
            var focusedWeaponPart = base.Owner.Ensure<UnitPartFocusedWeapon>();
            return HasWeaponTraining(weapon) && focusedWeaponPart.HasEntry(weapon.Blueprint.Category);
        }

        public void OnEventDidTrigger(RuleCalculateWeaponStats evt)
        {
        }

        public BlueprintCharacterClassReference CheckedClass;
    }
}
