using System;
using Kingmaker.UnitLogic.Buffs;
using Newtonsoft.Json;
using UnityEngine.Serialization;
using Kingmaker.Blueprints.JsonSystem;

namespace ExoticTales.NewComponents
{
    [TypeId("7063267CE4784360AE5F60E3FBFCC838")]
    class DiurnalAuraFeatureComponentData
    {
        [JsonProperty]
        public Buff AppliedBuff;
    }
}
