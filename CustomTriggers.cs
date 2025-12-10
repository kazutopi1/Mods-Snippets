qusing StardewValley;
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

        public override void Entry(IModHelper helper)
        {
            TriggerActionManager.RegisterTrigger("KT.ButtonAPressed");

            TriggerActionManager.RegisterTrigger("KT.ButtonBPressed");

            TriggerActionManager.RegisterTrigger("KT_UpdateTicked");

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
        }
        private static void ButtonA(ref bool __result)
        {
            if (__result && !wasATapped)
            {
                TriggerActionManager.Raise("KT.ButtonAPressed");
            }
            wasATapped = __result;
        }
        private static void ButtonB(ref bool __result)
        {
            if (__result && !wasBTapped)
            {
                TriggerActionManager.Raise("KT.ButtonBPressed");
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
    }
}
