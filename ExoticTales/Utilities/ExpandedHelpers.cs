using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Localization;
using Kingmaker.ResourceLinks;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics.Properties;
using Kingmaker.Utility;

namespace ExoticTales.Utilities
{
    class ExpandedHelpers
    {

        // Note: This was cannibalized from KingmakerToolkit.Shared in Races Unleashed and ported to WotR.

        public static T GetField<T>(object obj, string name)
        {
            return (T)((object)HarmonyLib.AccessTools.Field(obj.GetType(), name).GetValue(obj));
        }

        public static object GetField(Type type, object obj, string name)
        {
            return HarmonyLib.AccessTools.Field(type, name).GetValue(obj);
        }

    }
}
