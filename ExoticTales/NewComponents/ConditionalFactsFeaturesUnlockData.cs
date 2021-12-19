using System;
using Kingmaker.EntitySystem;
using Newtonsoft.Json;
using Kingmaker.Designers.Mechanics.Facts;

namespace ExoticTales.NewComponents
{
    public class ConditionalFactsFeaturesUnlockData
    {
        [JsonProperty]
        public EntityFact AppliedFact;
    }
}
