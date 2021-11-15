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
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using Kingmaker;
using Kingmaker.Blueprints.Root;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.Items;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.Loot;
using Kingmaker.Blueprints.Validation;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.Designers.EventConditionActionSystem.Conditions;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Designers.Mechanics.Recommendations;
using Kingmaker.Enums.Damage;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UI;
using Kingmaker.UI.Log;
using Kingmaker.UI.Common;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;
using static Kingmaker.UnitLogic.ActivatableAbilities.ActivatableAbilityResourceLogic;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.Blueprints.Items.Components;
using Kingmaker.Designers.Mechanics.Prerequisites;
using System.IO;
using ExoticTales.Extensions;
using ExoticTales.NewComponents;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Entities;


namespace ExoticTales.Utilities
{
    class ExpandedHelpers
    {


        // BORROWED CODE
        // Note: This was cannibalized from KingmakerToolkit.Shared in Races Unleashed Kingmaker mod and ported to WotR.

        public static T GetField<T>(object obj, string name)
        {
            return (T)((object)HarmonyLib.AccessTools.Field(obj.GetType(), name).GetValue(obj));
        }

        public static object GetField(Type type, object obj, string name)
        {
            return HarmonyLib.AccessTools.Field(type, name).GetValue(obj);
        }

        // -------------------------------------------------------------------------------------------------------------------------
        // Note: These were borrowed from Holic75's KingmakerRebalance/CotW Kingmaker mod. 
        // Copyright (c) 2019 Jennifer Messerly
        // Copyright (c) 2020 Denis Biryukov
        // This code is licensed under MIT license (see LICENSE for details)
        // -------------------------------------------------------------------------------------------------------------------------

        //-------------------------------------|CONDITION CHECKERS CREATORS|--------------------------------------------------------

        public static ConditionsChecker CreateConditionsCheckerAnd(params Condition[] conditions)
        {
            return new ConditionsChecker() { Conditions = conditions, Operation = Operation.And };
        }

        public static ConditionsChecker CreateConditionsCheckerOr(params Condition[] conditions)
        {
            return new ConditionsChecker() { Conditions = conditions, Operation = Operation.Or };
        }

        //---------------------------------------------|CONDITIONAL CREATORS|--------------------------------------------------------

        public static Conditional CreateConditional(Condition condition, GameAction ifTrue, GameAction ifFalse = null)
        {
            var c = Helpers.Create<Conditional>();
            c.ConditionsChecker = CreateConditionsCheckerAnd(condition);
            c.IfTrue = Helpers.CreateActionList(ifTrue);
            c.IfFalse = Helpers.CreateActionList(ifFalse);
            return c;
        }

        public static Conditional CreateConditional(Condition[] condition, GameAction ifTrue, GameAction ifFalse = null)
        {
            var c = Helpers.Create<Conditional>();
            c.ConditionsChecker = CreateConditionsCheckerAnd(condition);
            c.IfTrue = Helpers.CreateActionList(ifTrue);
            c.IfFalse = Helpers.CreateActionList(ifFalse);
            return c;
        }


        public static Conditional CreateConditionalOr(Condition[] condition, GameAction ifTrue, GameAction ifFalse = null)
        {
            var c = Helpers.Create<Conditional>();
            c.ConditionsChecker = CreateConditionsCheckerOr(condition);
            c.IfTrue = Helpers.CreateActionList(ifTrue);
            c.IfFalse = Helpers.CreateActionList(ifFalse);
            return c;
        }

        public static Conditional CreateConditional(Condition[] condition, GameAction[] ifTrue, GameAction[] ifFalse = null)
        {
            var c = Helpers.Create<Conditional>();
            c.ConditionsChecker = CreateConditionsCheckerAnd(condition);
            c.IfTrue = Helpers.CreateActionList(ifTrue);
            c.IfFalse = Helpers.CreateActionList(ifFalse);
            return c;
        }

        public static Conditional CreateConditional(ConditionsChecker conditions, GameAction ifTrue, GameAction ifFalse = null)
        {
            var c = Helpers.Create<Conditional>();
            c.ConditionsChecker = conditions;
            c.IfTrue = Helpers.CreateActionList(ifTrue);
            c.IfFalse = Helpers.CreateActionList(ifFalse);
            return c;
        }

        public static Conditional CreateConditional(Condition condition, GameAction[] ifTrue, GameAction[] ifFalse = null)
        {
            var c = Helpers.Create<Conditional>();
            c.ConditionsChecker = new ConditionsChecker() { Conditions = new Condition[] { condition } };
            c.IfTrue = Helpers.CreateActionList(ifTrue);
            c.IfFalse = Helpers.CreateActionList(ifFalse ?? Array.Empty<GameAction>());
            return c;
        }

        public static Conditional CreateConditional(ConditionsChecker condition, GameAction[] ifTrue, GameAction[] ifFalse = null)
        {
            var c = Helpers.Create<Conditional>();
            c.ConditionsChecker = condition;
            c.IfTrue = Helpers.CreateActionList(ifTrue);
            c.IfFalse = Helpers.CreateActionList(ifFalse ?? Array.Empty<GameAction>());
            return c;
        }

        //---------------------------------------------|CONTEXT CONDITION CREATORS|--------------------------------------------------------

        public static ContextConditionAlignment CreateContextConditionAlignment(AlignmentComponent alignment, bool check_caster = false, bool not = false)
        {
            var c = Helpers.Create<ContextConditionAlignment>();
            c.Alignment = alignment;
            c.Not = not;
            c.CheckCaster = check_caster;
            return c;
        }

        static public ContextConditionCasterHasFact createContextConditionCasterHasFact(BlueprintUnitFact fact, bool has = true)
        {
            var c = Helpers.Create<ContextConditionCasterHasFact>();
            c.m_Fact = fact.ToReference<BlueprintUnitFactReference>();
            c.Not = !has;
            return c;
        }

        public static ContextConditionHasBuffFromCaster createContextConditionHasBuffFromCaster(BlueprintBuff buff, bool not = false)
        {
            var c = Helpers.Create<ContextConditionHasBuffFromCaster>();
            c.m_Buff = buff.ToReference<BlueprintBuffReference>();
            c.Not = not;
            return c;
        }

        static public ContextConditionHasFact createContextConditionHasFact(BlueprintUnitFact fact, bool has = true)
        {
            var c = Helpers.Create<ContextConditionHasFact>();
            c.m_Fact = fact.ToReference<BlueprintUnitFactReference>();
            c.Not = !has;
            return c;
        }

        static public ContextConditionIsCaster createContextConditionIsCaster(bool not = false)
        {
            var c = Helpers.Create<ContextConditionIsCaster>();
            c.Not = not;
            return c;
        }





        // ORIGINAL or ADAPTED CODE

        //---------------------------------------------|CONTEXT CONDITION CREATORS|--------------------------------------------------------


        static public ContextConditionIsPartyMember createContextConditionIsPartyMember(bool not = false)
        {
            var c = Helpers.Create<ContextConditionIsPartyMember>();
            c.Not = not;
            return c;
        }

        static public ContextConditionCasterIsInFogOfWar createContextConditionCasterIsInFogOfWar(bool not = false) 
        {
            var c = Helpers.Create<ContextConditionCasterIsInFogOfWar>();
            c.Not = not;
            return c;
        }

        static public ContextConditionCasterHasStatValue createContextConditionCasterHasStatValue (StatType stat, int num, bool not = false)
        {
            var c = Helpers.Create<ContextConditionCasterHasStatValue>();
            c.Stat = stat;
            c.N = num;
            c.Not = not;
            return c;
        }

        static public ContextConditionCasterHasStatWithinRange createContextConditionCasterHasStatWithinRange(StatType stat, int nub, bool ubin, int nlb, bool lbin, bool not = false)
        {
            var c = Helpers.Create<ContextConditionCasterHasStatWithinRange>();
            c.Stat = stat;
            c.N_UB = nub;
            c.UB_Incl = ubin;
            c.N_LB = nlb;
            c.LB_Incl = lbin;
            c.Not = not;
            return c;
        }

        //---------------------------------------------|CONDITIONAL CREATORS|--------------------------------------------------------


        public static Conditional CreateStackedConditional(ConditionsChecker conditions1, ConditionsChecker conditions2, GameAction if1True, GameAction if2True, GameAction if2False = null)
        {
            var c1 = Helpers.Create<Conditional>();
            c1.ConditionsChecker = conditions1;
            c1.IfTrue = Helpers.CreateActionList(if1True);
            c1.IfFalse = Helpers.CreateActionList(ExpandedHelpers.CreateConditional(conditions2, if2True, if2False));
            return c1;
        }


        public static Conditional CreateStackedConditional(ConditionsChecker condition1, ConditionsChecker condition2, GameAction[] if1True, GameAction[] if2True, GameAction[] if2False = null)
    {
        var c1 = Helpers.Create<Conditional>();
        c1.ConditionsChecker = condition1;
        c1.IfTrue = Helpers.CreateActionList(if1True);
        c1.IfFalse = Helpers.CreateActionList(ExpandedHelpers.CreateConditional(condition2, if2True, if2False));

       
        return c1;
    }






















    }

}
