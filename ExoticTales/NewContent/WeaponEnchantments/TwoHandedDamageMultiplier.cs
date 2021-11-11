using Kingmaker.Blueprints.Items.Ecnchantments;
using ExoticTales.Extensions;
using ExoticTales.NewComponents;
using ExoticTales.Utilities;

namespace ExoticTales.NewContent.WeaponEnchantments
{
    class TwoHandedDamageMultiplier
    {
        public static void AddTwoHandedDamageMultiplierEnchantment()
        {
            var TwoHandedDamageMultiplierEnchantment = Helpers.CreateBlueprint<BlueprintWeaponEnchantment>($"TwoHandedDamageMultiplierEnchantment", bp => {
                bp.SetName("Increased Damage Multiplier");
                bp.SetDescription("Attacks are made with a 1.5 damage multipler.");
                bp.SetPrefix("");
                bp.SetSuffix("");
                bp.m_EnchantmentCost = 1;
                bp.AddComponent<WeaponDamageMultiplierReplacement>(c => {
                    c.Multiplier = 1.5f;
                });
            });
        }
    }
}
