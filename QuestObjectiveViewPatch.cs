using System;
using System.Linq;
using System.Reflection;
using SPT.Reflection.Patching;
using EFT;
using EFT.Quests;
using EFT.UI;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace SkipQuest
{
    public class QuestObjectiveViewPatch : ModulePatch
    {
        private static string UnderlyingQuestControllerClassName;
        internal static GameObject LastSeenObjectivesBlock;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(QuestObjectiveView), nameof(QuestObjectiveView.Show));
        }

        [PatchPostfix]
        private static void PatchPostfix([CanBeNull] DefaultUIButton ____handoverButton, AbstractQuestControllerClass questController, Condition condition, QuestClass quest, QuestObjectiveView __instance)
        {
            if (!Plugin.ModEnabled.Value)
            {
                return;
            }

            // The handover button is usually only missing in the non-trader task view screens, where we don't want to allow skipping either way
            if (____handoverButton == null)
            {
                return;
            }

            if (UnderlyingQuestControllerClassName == null)
            {
                var type = AccessTools.GetTypesFromAssembly(typeof(AbstractGame).Assembly)
                    .SingleOrDefault(t => t.GetEvent("OnConditionQuestTimeExpired", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance) != null);

                if (type == null)
                {
                    Plugin.Logger.LogError("Failed to locate a specific quest controller type");
                    return;
                }

                UnderlyingQuestControllerClassName = type.Name.Split('`')[0];
                Plugin.Logger.LogDebug($"Resolved {nameof(UnderlyingQuestControllerClassName)} to be {UnderlyingQuestControllerClassName}");
            }

            LastSeenObjectivesBlock = __instance.transform.parent.gameObject;

            
                var skipButton = UnityEngine.Object.Instantiate(____handoverButton, ____handoverButton.transform.parent.transform);

                skipButton.SetRawText("SKIP", 22);
                skipButton.gameObject.name = Plugin.SkipButtonName;
                skipButton.gameObject.GetComponent<UnityEngine.UI.LayoutElement>().minWidth = 100f;
                skipButton.gameObject.SetActive(Plugin.AlwaysDisplay.Value && !quest.IsConditionDone(condition));

                skipButton.OnClick.RemoveAllListeners();

            if (Plugin.ShowConfirmation.Value)
            {
                skipButton.OnClick.AddListener(() => ItemUiContext.Instance.ShowMessageWindow(
                    description: "Are you sure you want to autocomplete this quest objective?",
                    acceptAction: () =>
                    {
                        if (quest.IsConditionDone(condition))
                        {
                            skipButton.gameObject.SetActive(false);
                            return;
                        }

                        Skip(questController, condition, quest);

                        skipButton.gameObject.SetActive(false);
                    },
                    cancelAction: () => { },
                    caption: "Confirmation"));
            } else
            {
                skipButton.OnClick.AddListener(() => Skip(questController, condition, quest));
            }
                

            
        }

        private static void Skip(AbstractQuestControllerClass questController, Condition condition, QuestClass quest)
        {
            Plugin.Logger.LogDebug($"Setting condition {condition.id} value to {condition.value}");

            quest.ProgressCheckers[condition].SetCurrentValueGetter(_ => condition.value);

            var fields = questController.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            object target = null;
            MethodInfo method = null;

            foreach (var field in fields)
            {
                var value = field.GetValue(questController);
                if (value == null) continue;

                var candidate = value.GetType().GetMethod(
                    "SetConditionCurrentValue",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
                );

                if (candidate != null)
                {
                    target = value;
                    method = candidate;
                    Plugin.Logger.LogDebug($"Found condition controller: {value.GetType().Name}");
                    break;
                }
            }

            if (target == null || method == null)
            {
                Plugin.Logger.LogError("Could not find any field with SetConditionCurrentValue.");
                return;
            }

            try
            {
                method.Invoke(target, new object[]
                {
            quest, EQuestStatus.AvailableForFinish, condition, condition.value, true
                });

                Plugin.Logger.LogDebug($"Successfully called SetConditionCurrentValue on {target.GetType().Name}");
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Error invoking SetConditionCurrentValue: {ex}");
            }
        }
    }
}
