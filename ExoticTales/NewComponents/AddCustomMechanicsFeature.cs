// Taken from Sean Petrie/Vek17's Tabletop Tweaks

using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic;
using ExoticTales.NewUnitParts;

namespace ExoticTales.NewComponents
{
    [TypeId("38965e585f1c4da78db9276a47571209")]
    class AddCustomMechanicsFeature : UnitFactComponentDelegate
    {
        public override void OnTurnOn()
        {
            Owner.CustomMechanicsFeature(Feature).Retain();
        }

        public override void OnTurnOff()
        {
            Owner.CustomMechanicsFeature(Feature).Release();
        }

        public UnitPartCustomMechanicsFeatures.CustomMechanicsFeature Feature;
    }
}
