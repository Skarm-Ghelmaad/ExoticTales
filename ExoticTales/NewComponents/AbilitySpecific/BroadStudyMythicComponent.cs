using Kingmaker.Blueprints;
using Kingmaker.UnitLogic;
using ExoticTales.NewUnitParts;

namespace ExoticTales.NewComponents.AbilitySpecific
{
    class BroadStudyMythicComponent : UnitFactComponentDelegate
    {
        public override void OnTurnOn()
        {
            base.Owner.Ensure<UnitPartBroadStudy>().AddMythicSource(base.Fact);
            foreach (var book in Spellbooks)
            {
                base.Owner.Ensure<UnitPartBroadStudy>().AddMythicSpellbook(book, base.Fact);
            }
        }

        public override void OnTurnOff()
        {
            base.Owner.Ensure<UnitPartBroadStudy>().RemoveEntry(base.Fact);
        }

        public BlueprintSpellbookReference[] Spellbooks;
    }
}
