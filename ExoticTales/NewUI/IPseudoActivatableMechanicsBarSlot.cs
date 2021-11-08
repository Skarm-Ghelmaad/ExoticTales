using Kingmaker.Blueprints;
using Kingmaker.UnitLogic.Abilities;

namespace ExoticTales.NewUI
{
    public interface IPseudoActivatableMechanicsBarSlot
    {
        public abstract BlueprintBuffReference BuffToWatch { get; set; }
        public abstract bool ShouldBeActive { get; set; }
        public abstract AbilityData PseudoActivatableAbility { get; }
    }
}

