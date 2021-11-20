using System;
using Kingmaker.UnitLogic.Buffs;
using Newtonsoft.Json;
using UnityEngine.Serialization;
using Kingmaker.Blueprints.JsonSystem;

namespace ExoticTales.NewComponents
{
    [TypeId("BDD0BC97DC4B465F86C023F832B60BEE")]
    class NocturnalAuraFeatureComponentData
    {
        [JsonProperty]
        public Buff AppliedBuff;
    }
}
