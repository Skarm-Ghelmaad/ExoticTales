Credits:  
-   This is the full list of the files that have been copied (albeit in accordance with their license) from other modders.
-   Please note that I did so because I feel their code, helpers and solutions are much better than mine and, in truth, if was possible, I'd rather add content to their mods than doing my own!
    (however I feel my content has a potential for being simply too broken or unbalanced, too openly violating Pathfinder's canon or too controversial to be accepted as a collaboration by 
     the much more talented modders from which I am essentially "copying parts of code").
-   As a general rule that I have given myself I tried to avoid "copying new content", but, within the limits of the license chosen by the other modders, I will copy the parts of code that I feel I
    can use for my own code and the only exception to this "honor code tenet" are new mechanics/components/parts, because I might want to add them and integrate them in the most seamless way possible.
    For example, Vek17 has added a racial heritage mechanic for races which didn't have it...and I would like to addd new heritage or alternate racial traits, but I also want people (and especially MYSELF) to
    be able to play both with mine and Vek's content...so in that case a part of new content added by Vek17 will be added to my mod as well -though for Vek's heritages and trait you'll have to look at his amazing mod!-.
-   As a further rule, unless I made some customization, I will do my best to ensure that, if the original mod is installed, my "cloned" elements will be replaced by the original, but I do recognize that
    within Pathfinder Wrath of the Righteous modding community there is no "appetite" to develop a common community modding library or to have one mod become the "building block" for others (as happened with 
    Call of the Wild mod for Pathfinder Kingmaker), so I will try to copy to ensure that my "bits" will work as smoothly as possible with those of other modders -even without a dependency- ...
-   If the original modders will allow me I will offer them any tweak or improvement done in my code as contribution to their mod, because I really appreciate their work!!

**||List of "Borrowed" Code by Author-Mod-File||**

**Vek17/Sean Petrie** [https://github.com/Vek17]
*Tabletop Tweaks* [https://github.com/Vek17/WrathMods-TabletopTweaks]
Note: I really like Vek17's mod, which is why so much code gets borrowed from him...if he allowed my broken/controversial stuff, Exotic Tales would probably just be a part of 
his amazing mod...

-   ExoticTales/Main.cs
-   ExoticTales/Resources.cs
-   ExoticTales/SaveGamePatch.cs
-   ExoticTales/UMMSettingsUI.cs
-   ExoticTales/Config/AddedContent.cs
-   ExoticTales/Config/AddedContent.json
-   ExoticTales/Config/Blueprints.cs
-   ExoticTales/Config/Blueprints.json
-   ExoticTales/Config/Fixes.cs
-   ExoticTales/Config/Fixes.json
-   ExoticTales/Config/Homebrew.cs
-   ExoticTales/Config/Homebrew.json
-   ExoticTales/Config/ICollapseableGroup.cs
-   ExoticTales/Config/IDisableableGroup.cs
-   ExoticTales/Config/IUpdatableSettings.cs
-   ExoticTales/Config/ModSettings.cs
-   ExoticTales/Config/NestedSettingGroup.cs
-   ExoticTales/Config/SettingGroup.cs
-   ExoticTales/Extensions/ExtentionMethods.cs
-   ExoticTales/NewComponents/AddCustomMechanicsFeature.cs
-   ExoticTales/NewUnitParts/UnitPartCustomMechanicsFeatures.cs
-   ExoticTales/NewEvents/ISpontaneousConversionHandler.cs


Note II: A few considerations...

**Main**: I know...this is *surreal*, if one is not aware that Main is essentially to "hook up" the mod to Unity Mod Manager!!
**Config**: The Config folder has been essentially lifted straight-away from Vek17's mod and adjusted since I am clueless about UMM interface!!
**Extensions/Utilities**: These are the main reason that tempted me into copying Vek17's mod, because they are absolutely brilliant and ultra-useful!
**NewComponents/NewUnitParts/NewEvents/NewRules/NewActions/NewUI**: Initially I felt bad about copying these, but they are as much instrumental and useful as the utilities and I also want to ensure the maximum compatibility with Vek's features.
**MechanicsChanges**: This part will be taken from Vek17's mod because I want to be sure to have maximum compatibility with his mod.
**Bugfixes**: These will be generally left untouched as the only fixes I want to add to my mod are mine.
**NewContent**: I have "borrowed" Quick Draw because I needed for my mod and when was introduced I was still struggling over it, but Vek's implementation is simply much better, moreover
I will borrow the Templates and the setup of the Racial Heritages introduced by Vek and the NonStackingTempEnchantments and TwoHandedDamageMultiplier WeaponEnchantments, but in general I want
to leave Vek's stuff un touched and add my new content in this mod...ideally to allow to play with both enabled (as I will do!)!!

