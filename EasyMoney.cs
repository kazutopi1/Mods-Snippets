using StardewValley;
using StardewValley.Mobile;
using StardewModdingAPI;
using HarmonyLib;

namespace blahblah
{
    public class idcifthisdoebntwork : Mod
    {
        private static bool wasBtappedlasttick = false;

        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(VirtualJoypad), nameof(VirtualJoypad.ButtonBPressed)),
                postfix: new HarmonyMethod(typeof(idcifthisdoebntwork), nameof(idcifthisdoebntwork.Postfix))
            );
        }
        private static void Postfix(ref bool __result)
        {
            if (__result && !wasBtappedlasttick)
            {
                Game1.player.Money += 1;
            }
            wasBtappedlasttick = __result;
        }
    }
}
