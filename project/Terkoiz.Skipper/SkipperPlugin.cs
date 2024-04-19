using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using JetBrains.Annotations;

namespace Terkoiz.Skipper
{
    [BepInPlugin("com.terkoiz.skipper", "Terkoiz.Skipper", "1.0.0")]
    public class SkipperPlugin : BaseUnityPlugin
    {
        internal new static ManualLogSource Logger { get; private set; }

        private const string MainSectionName = "Main";
        internal static ConfigEntry<bool> ModEnabled;

        [UsedImplicitly]
        internal void Start()
        {
            Logger = base.Logger;
            InitConfiguration();

            new QuestObjectiveViewPatch().Enable();
        }

        private void InitConfiguration()
        {
            ModEnabled = Config.Bind(
                MainSectionName,
                "Enabled",
                true,
                "Global mod toggle. Will need to re-open the quest window for the setting change to take effect.");
        }
    }
}
