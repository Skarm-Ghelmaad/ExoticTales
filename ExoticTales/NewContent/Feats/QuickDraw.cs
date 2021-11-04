using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExoticTales.Utilities;
using ExoticTales.Extensions;
using Kingmaker.Blueprints.Facts;


namespace ExoticTales.NewContent.Feats
{
    class QuickDraw
    {
        public static void CreateQuickDraw()
        {
            var icon = AssetLoader.LoadInternal("Feats", "Icon_QuickDraw.png");
            var QuickDraw = Helpers.CreateBlueprint<BlueprintUnitFact>("QuickDraw", bp =>
            {

                bp.SetName("Quick Draw");
                bp.SetDescription("Your arms temporarily grow in length, increasing your reach with those limbs by 5 feet.");
                bp.m_Icon = icon;


            });

        }

    }
}
