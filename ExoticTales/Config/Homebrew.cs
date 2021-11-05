using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoticTales.Config
{
    class Homebrew : IUpdatableSettings
    {
        public bool NewSettingsOffByDefault = false;
        public OverpoweredContentGroup OverpoweredContent = new OverpoweredContentGroup();

        public void Init()
        {
            OverpoweredContent.Init();
        }

        public void OverrideSettings(IUpdatableSettings userSettings)
        {
            var loadedSettings = userSettings as Homebrew;
            OverpoweredContent.LoadOverpoweredContentGroup(loadedSettings.OverpoweredContent, NewSettingsOffByDefault);
        }
    }
    public class OverpoweredContentGroup : IDisableableGroup, ICollapseableGroup
    {
        public bool IsExpanded = true;
        ref bool ICollapseableGroup.IsExpanded() => ref IsExpanded;
        public void SetExpanded(bool value) => IsExpanded = value;
        public bool DisableAll = false;
        public bool GroupIsDisabled() => DisableAll;
        public void SetGroupDisabled(bool value) => DisableAll = value;
        public NestedSettingGroup Test1;

        public OverpoweredContentGroup()
        {
            Test1 = new NestedSettingGroup(this);
        }

        public void Init()
        {
            Test1.Parent = this;

        }

        public void LoadOverpoweredContentGroup(OverpoweredContentGroup group, bool frozen)
        {
            DisableAll = group.DisableAll;
            Test1.LoadSettingGroup(group.Test1, frozen);

        }
    }
}
