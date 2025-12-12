using StardewValley;
using StardewValley.Tools;
using StardewValley.Mobile;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using HarmonyLib;
using StardewValley.Triggers;

namespace KT_Triggers
{
    public class Tiger : Mod
    {
        private static bool wasATapped = false;
        private static bool wasBTapped = false;
        private static int swingCount = 0;
        private const int swingThreshold = 3;

        public override void Entry(IModHelper helper)
        {
            TriggerActionManager.RegisterTrigger("KT_ButtonAPressed");
            TriggerActionManager.RegisterTrigger("KT_ButtonBPressed");
            TriggerActionManager.RegisterTrigger("KT_UpdateTicked");
            TriggerActionManager.RegisterTrigger("KT_DoSwing");
            TriggerActionManager.RegisterTrigger("KT_On3rdSwing");
            TriggerActionManager.RegisterTrigger("KT_UseTool");

            var harmony = new Harmony(this.ModManifest.UniqueID);

            helper.Events.GameLoop.UpdateTicked += KT_UpdateTicked;

            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(VirtualJoypad), nameof(VirtualJoypad.ButtonAPressed)),
                postfix: new HarmonyMethod(typeof(Tiger), nameof(Tiger.ButtonA))
            );
            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(VirtualJoypad), nameof(VirtualJoypad.ButtonBPressed)),
                postfix: new HarmonyMethod(typeof(Tiger), nameof(Tiger.ButtonB))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(MeleeWeapon), nameof(MeleeWeapon.doSwipe)),
                postfix: new HarmonyMethod(typeof(Tiger), nameof(Tiger.DoSwipe))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(MeleeWeapon), nameof(MeleeWeapon.doSwipe)),
                postfix: new HarmonyMethod(typeof(Tiger), nameof(Tiger.On3rdSwing))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.useTool)),
                postfix: new HarmonyMethod(typeof(Tiger), nameof(Tiger.UseTool))
            );
        }
        private static void ButtonA(ref bool __result)
        {
            if (__result && !wasATapped)
            {
                TriggerActionManager.Raise("KT_ButtonAPressed");
            }
            wasATapped = __result;
        }
        private static void ButtonB(ref bool __result)
        {
            if (__result && !wasBTapped)
            {
                TriggerActionManager.Raise("KT_ButtonBPressed");
            }
            wasBTapped = __result;
        }
        private void KT_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Context.IsPlayerFree || Context.CanPlayerMove)
            {
                TriggerActionManager.Raise("KT_UpdateTicked");
            }
        }
        private static void DoSwipe()
        {
            TriggerActionManager.Raise("KT_DoSwing");
        }
        private static void On3rdSwing()
        {
            swingCount += 1;
            if (swingCount >= swingThreshold)
            {
                TriggerActionManager.Raise("KT_On3rdSwing");
                swingCount = 0;
            }
        }
        private static void UseTool(Farmer who)
        {
            TriggerActionManager.Raise("KT_UseTool");
        }
    }
}
