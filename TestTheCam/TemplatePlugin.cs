using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;
using UnityEngine;

// IMPCIAEIBNB = SystemConsole

namespace TestTheCam
{
    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    public class TemplatePlugin : BasePlugin
    {
        public const string Id = "de.mintendo-programmer.TestTheCam";

        public Harmony Harmony { get; } = new Harmony(Id);

        public ConfigEntry<string> Name { get; private set; }

        public override void Load()
        {
            Name = Config.Bind("Fake", "Name", ":>");

            Harmony.PatchAll();
        }

        public static void OpenAdmin() {
            DestroyableSingleton<HudManager>.Instance.ShowMap((System.Action<MapBehaviour>)delegate(MapBehaviour m)
            {
                m.ShowCountOverlay();
            });
        }

        public static void OpenConsoleWindow(string identifier)
        {
            var obj = Object.FindObjectsOfType<SystemConsole>(); //.FirstOrDefault(x => x.name.Contains("Surv") );
            var survCam = obj[0];
            foreach(var test in obj) {
                if(test.name.Contains(identifier)) {
                    survCam = test;
                }
            }
            if (survCam == null ) {
                Debug.Log("ERROR!!");
                return;
            }
            var minigame = Object.Instantiate(survCam.MinigamePrefab, Camera.main.transform, false );
            minigame.transform.localPosition = new Vector3(0f, 0f, -50f);
            minigame.Begin(null);
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public static class ExamplePatch
        {
            public static void Postfix(PlayerControl __instance)
            {

                __instance.nameText.Text = PluginSingleton<TemplatePlugin>.Instance.Name.Value;
            }
        }

        /*[HarmonyPatch]
        public static class KillButtonPatch
        {
            [HarmonyPatch(typeof(KillButtonManager), "PerformKill")]
            static void Postfix(Object __originalMethod)
            {
                //Debug.Log(__originalMethod);
                PluginSingleton<TemplatePlugin>.Instance.Name.Value = "I just killed him, muhahahah!";
                OpenConsoleWindow("Vital");
            }
        }*/

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
        public static class SecurityButton
        {
            private static CooldownButton btn;

            public static void Postfix(HudManager __instance)
            {
                btn = new CooldownButton(
                    () =>
                    {
                        // Restores the cooldown
                        btn.Timer = btn.MaxTimer;
                        PluginSingleton<TemplatePlugin>.Instance.Name.Value = "Button?";
                        OpenConsoleWindow("Surv");
                    },
                    0f,
                    "",
                    1f,
                    new Vector2(0f, 0.125f),
                    CooldownButton.Category.Everyone,
                    __instance
                );
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
        public static class AdminButton
        {
            private static CooldownButton btn;

            public static void Postfix(HudManager __instance)
            {
                btn = new CooldownButton(
                    () =>
                    {
                        // Restores the cooldown
                        btn.Timer = btn.MaxTimer;
                        PluginSingleton<TemplatePlugin>.Instance.Name.Value = "Button?";
                        OpenAdmin();
                    },
                    0f,
                    "",
                    1f,
                    new Vector2(1.25f, 0.125f),
                    CooldownButton.Category.Everyone,
                    __instance
                );
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
        public static class VitalsButton
        {
            private static CooldownButton btn;

            public static void Postfix(HudManager __instance)
            {
                btn = new CooldownButton(
                    () =>
                    {
                        // Restores the cooldown
                        btn.Timer = btn.MaxTimer;
                        PluginSingleton<TemplatePlugin>.Instance.Name.Value = "Button?";
                        OpenConsoleWindow("Vitals");
                    },
                    0f,
                    "",
                    1f,
                    new Vector2(2.5f, 0.125f),
                    CooldownButton.Category.Everyone,
                    __instance
                );
            }
        }

        

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class CooldownButtonUpdatePatch
        {
            public static void Postfix(HudManager __instance)
            {
                CooldownButton.HudUpdate();
            }
        }
    }
}
