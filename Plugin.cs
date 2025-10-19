using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using EFT.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace SkipQuest
{
    [BepInPlugin("com.zas.questskipper", "QuestSkipper", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        internal const string SkipButtonName = "SkipButton";

        internal new static ManualLogSource Logger { get; private set; }

        private const string MainSectionName = "Main";
        internal static ConfigEntry<bool> ModEnabled;
        internal static ConfigEntry<bool> AlwaysDisplay;
        internal static ConfigEntry<KeyboardShortcut> DisplayHotkey;
        internal static ConfigEntry<bool> ShowConfirmation;

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
                "1. Enabled",
                true,
                "Global mod toggle. Will need to re-open the quest window for the setting change to take effect.");

            AlwaysDisplay = Config.Bind(
                MainSectionName,
                "2. Always display Skip button",
                false,
                "If enabled, the Skip button will always be visible.");

            DisplayHotkey = Config.Bind(
                MainSectionName,
                "3. Display hotkey",
                new KeyboardShortcut(KeyCode.LeftControl),
                "Holding down this key will make the Skip buttons appear.");

            ShowConfirmation = Config.Bind(
                MainSectionName,
                "4. Show confirmation button",
                true,
                "If enabled, the box of confirmation will pop up in each skip.");
        }

        [UsedImplicitly]
        internal void Update()
        {
            if (!ModEnabled.Value || AlwaysDisplay.Value)
            {
                return;
            }

            if (QuestObjectiveViewPatch.LastSeenObjectivesBlock == null || !QuestObjectiveViewPatch.LastSeenObjectivesBlock.activeSelf)
            {
                return;
            }

            if (DisplayHotkey.Value.IsDown())
            {
                ChangeButtonVisibility(true);
            }

            if (DisplayHotkey.Value.IsUp())
            {
                ChangeButtonVisibility(false);
            }
        }

        private static void ChangeButtonVisibility(bool setVisibilityTo)
        {
            foreach (var button in QuestObjectiveViewPatch.LastSeenObjectivesBlock.GetComponentsInChildren<DefaultUIButton>(includeInactive: true))
            {
                if (button.name != SkipButtonName) continue;

                button.gameObject.SetActive(setVisibilityTo);
            }
        }
    }
}
