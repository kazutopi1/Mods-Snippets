using StardewValley;
using StardewModdingAPI;
using HarmonyLib;
using StardewValley.Tools;
using StardewValley.Buffs;
using Microsoft.Xna.Framework.Graphics;

namespace PressTheAttack
{
    public class PTA : Mod
    {
        private static int useToolCount = 0;

        private const int Threshold = 3;

        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(MeleeWeapon), nameof(MeleeWeapon.doSwipe)),
                prefix: new HarmonyMethod(typeof(PTA), nameof(PTA.Prefix))
            );
        }
        private static Buff Pierce()
        {
            Buff Pierce = new Buff(
                id: "df.pierce",
                displayName: "Pierce",
                iconTexture: Game1.content.Load<Texture2D>("TileSheets/BuffsIcons"),
                iconSheetIndex: 11,
                duration: 500,
                effects: new BuffEffects()
                {
                    CriticalPowerMultiplier = { 2 },
                    CriticalChanceMultiplier = { 1000 },
                    WeaponSpeedMultiplier = { 3 },
                }
            );
            return Pierce;
        }
        private static void Prefix()
        {
            var player = Game1.player;

            if (player.professions.Contains(Farmer.desperado))
            {
                useToolCount += 1;

                if (useToolCount == Threshold
                && !player.hasBuff("df.pierce"))
                {
                    player.applyBuff(Pierce());
                    useToolCount = 0;
                }
            }
        }
    }
}
