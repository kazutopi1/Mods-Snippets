using StardewValley;
using StardewValley.Mobile;
using StardewModdingAPI;
using HarmonyLib;
using System;

namespace BlahBlah
{
    public class Shop : Mod
    {
        private static bool wasBTapped = false;

        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(VirtualJoypad), nameof(VirtualJoypad.ButtonBPressed)),
                postfix: new HarmonyMethod(typeof(Shop), nameof(Shop.Postfix))
            );
        }
        private static void Postfix(ref bool __result)
        {
            if (Context.IsPlayerFree && !wasBTapped && __result)
            {
                if (Game1.player.CurrentItem is StardewValley.Item item)
                {
                    if (item.QualifiedItemId.Equals("(O)388"))
                    {
                        Response[] shops = new Response[]
                        {
                            new Response("openCarpenter", "Robin's Shop"),
                            new Response("openSeedShop", "Pierre's General Store"),
                            new Response("doNothing", "Close")
                        };

                        StardewValley.GameLocation.afterQuestionBehavior optionLogic = (Farmer who, string whichAnswer) =>
                        {
                            if (whichAnswer == "openCarpenter")
                            {
                                Utility.TryOpenShopMenu(
                                    shopId: "Carpenter",
                                    ownerName: null
                                );
                            }
                            else if (whichAnswer == "openSeedShop")
                            {
                                Utility.TryOpenShopMenu(
                                    shopId: "SeedShop",
                                    ownerName: null
                                );
                            }
                        };

                        Game1.currentLocation.createQuestionDialogue(
                            question: "Open which shop?",
                            answerChoices: shops,
                            afterDialogueBehavior: optionLogic,
                            speaker: null
                        );
                    }
                }
            }
            wasBTapped = __result;
        }
    }
}
