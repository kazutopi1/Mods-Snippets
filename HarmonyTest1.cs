using StardewValley;
using StardewModdingAPI;
using HarmonyLib;

namespace Bruhh
{
    public class Brotha : Mod
    {
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.useTool)),
                postfix: new HarmonyMethod(typeof(Brotha), nameof(Brotha.Postfix))
            );
        }
        private static void Postfix()
        {
            Game1.player.health += 5;
        }
    }
}
