﻿
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.UnitLogic.Parts;

namespace ExoticTales.NewComponents
{
    [TypeId("df38cda426724a6b9d5065585340d9d0")]
    class AbilityRequirementHasWeaponTrainingWithWeapon : BlueprintComponent, IAbilityRestriction
    {
        public string GetAbilityRestrictionUIText()
        {
            return $"You must have weapon training with your current weapon";
        }

        public bool IsAbilityRestrictionPassed(AbilityData ability)
        {
            var weaponTraining = ability.Caster.Get<UnitPartWeaponTraining>();
            var weapon = ability.Caster.Body.PrimaryHand.MaybeWeapon;
            if (weaponTraining == null || weapon == null) { return false; }
            return weaponTraining.GetWeaponRank(weapon) > 0;
        }
    }
}
