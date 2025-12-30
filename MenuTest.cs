using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using HarmonyLib;
using System;

namespace HybridMenuTest
{
    public class MTPrefix : Mod
    {
        private Harmony harmony;
        private Response[] menuOptions;
        private StardewValley.GameLocation.afterQuestionBehavior menuOptionsActions;
        private static IMonitor M;
        private static bool canSkip = false;

        public override void Entry(IModHelper helper)
        {
            M = this.Monitor;
            helper.Events.Input.ButtonPressed += OpenMenuList;
            helper.Events.GameLoop.GameLaunched += Cache;
            helper.Events.Display.MenuChanged += FlagReset;

            harmony = new Harmony(ModManifest.UniqueID);
            var prefix = new HarmonyMethod(typeof(MTPrefix), nameof(MTPrefix.Skip));

            try
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.returnToCarpentryMenu)),
                    prefix: prefix
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.returnToCarpentryMenuAfterSuccessfulBuild)),
                    prefix: prefix
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.setUpForReturnAfterPurchasingAnimal)),
                    prefix: prefix
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.setUpForReturnToShopMenu)),
                    prefix: prefix
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Patch failed: {ex}");
            }
        }
        private void Cache(object sender, GameLaunchedEventArgs e)
        {
            menuOptions = new Response[]
            {
                new Response("carpenter", "Robin"),
                new Response("wizard", "Wizard"),
                new Response("animal", "Marnie"),
                new Response("close", "Close")
            };
            menuOptionsActions = (Farmer who, string menuOptionsAnswers) =>
            {
                switch (menuOptionsAnswers)
                {
                    case "carpenter":
                        canSkip = true;
                        Game1.activeClickableMenu = new StardewValley.Menus.CarpenterMenu("Robin");
                        break;
                    case "wizard":
                        canSkip = true;
                        Game1.activeClickableMenu = new StardewValley.Menus.CarpenterMenu("Wizard");
                        break;
                    case "animal":
                        canSkip = true;
                        var location = Game1.getFarm();
                        List<StardewValley.Object> stock = Utility.getPurchaseAnimalStock(location);
                        Game1.activeClickableMenu = new PurchaseAnimalsMenu(stock);
                        break;
                }
            };
        }
        private void OpenMenuList(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.Q && Context.IsWorldReady)
            {
                Game1.currentLocation.createQuestionDialogue(
                    question: "Menus",
                    answerChoices: menuOptions,
                    afterDialogueBehavior: menuOptionsActions,
                    speaker: null
                );
            }
        }
        private void FlagReset(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu == null && canSkip)
            {
                canSkip = false;
            }
        }
        private static bool Skip()
        {
            if (!canSkip) { return true; }

            try
            {
                canSkip = false;
                Game1.activeClickableMenu = null;
                Game1.dialogueUp = false;
                Game1.viewportFreeze = false;
                Game1.player.viewingLocation.Value = null;
                Game1.displayFarmer = true;
                Game1.displayHUD = true;
                Game1.currentLocation.resetForPlayerEntry();
                Game1.player.forceCanMove();
                return false;
            }
            catch (Exception ex)
            {
                M.Log($"Failed to skip method: {ex}", LogLevel.Error);
                return true;
            }
        }
    }
}
