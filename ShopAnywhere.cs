using StardewValley;
using StardewValley.Menus;
using StardewValley.Mobile;
using StardewModdingAPI;
using HarmonyLib;
using Microsoft.Xna.Framework;
using System;

namespace ShopAnywhere
{
    public class Shop : Mod
    {
        private static Response[] categories;
        private static StardewValley.GameLocation.afterQuestionBehavior categoriesOptionsLogic;
        private static bool wasBTapped = false;
        private static string lastLocationName;
        private static Vector2 lastTilePos;

        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(VirtualJoypad), nameof(VirtualJoypad.ButtonBPressed)),
                postfix: new HarmonyMethod(typeof(Shop), nameof(Shop.Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.returnToCarpentryMenu)),
                postfix: new HarmonyMethod(typeof(Shop), nameof(Shop.Postfix2))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.returnToCarpentryMenuAfterSuccessfulBuild)),
                postfix: new HarmonyMethod(typeof(Shop), nameof(Shop.Postfix3))
            );
        }
        private static void Postfix(ref bool __result)
        {
            if (Context.IsPlayerFree && !wasBTapped && __result)
            {
                if (Game1.player.CurrentItem is StardewValley.Item item)
                {
                    if (item.QualifiedItemId.Equals("(O)kt.shop"))
                    {
                        categories = new Response[]
                        {
                            new Response("category1", "General Goods"),
                            new Response("category2", "Combat and Mining"),
                            new Response("category3", "Building"),
                            new Response("others", "Others"),
                            new Response("doNothing", "Close")
                        };
                        categoriesOptionsLogic = (Farmer who, string whichAnswer) =>
                        {
                            switch (whichAnswer)
                            {
                                case "category1":
                                    DelayedAction.functionAfterDelay(category1, 34);
                                    break;
                                case "category2":
                                    DelayedAction.functionAfterDelay(category2, 34);
                                    break;
                                case "category3":
                                    DelayedAction.functionAfterDelay(category3, 34);
                                    break;
                                case "others":
                                    DelayedAction.functionAfterDelay(others, 34);
                                    break;
                            }
                        };
                        Game1.currentLocation.createQuestionDialogue(
                            question: "Categories",
                            answerChoices: categories,
                            afterDialogueBehavior: categoriesOptionsLogic,
                            speaker: null
                        );
                    }
                }
            }
            wasBTapped = __result;
        }
        private static void Postfix2()
        {
            WarpPlayer();
        }
        private static void Postfix3()
        {
            WarpPlayer();
        }
        private static void category1()
        {
            Response[] cat1 = new Response[]
            {
                new Response("seedShop", "Pierre's General Store"),
                new Response("fishShop", "Willy's Shop"),
                new Response("saloon", "Saloon"),
                new Response("return", "Return")
            };
            StardewValley.GameLocation.afterQuestionBehavior cat1Logic = (Farmer who, string cat1answers) =>
            {
                switch (cat1answers)
                {
                    case "seedShop":
                        Utility.TryOpenShopMenu(Game1.shop_generalStore, null, false);
                        break;
                    case "fishShop":
                        Utility.TryOpenShopMenu(Game1.shop_fish, null, false);
                        break;
                    case "saloon":
                        Utility.TryOpenShopMenu(Game1.shop_saloon, null, false);
                        break;
                    case "return":
                        MainCategory();
                        break;
                }
            };
            Game1.currentLocation.createQuestionDialogue(
                question: "General Goods",
                answerChoices: cat1,
                afterDialogueBehavior: cat1Logic,
                speaker: null
            );
        }
        private static void category2()
        {
            Response[] cat2 = new Response[]
            {
                new Response("adventureShop", "Adventurer's Guild Shop"),
                new Response("blacksmith", "Clint's Shop"),
                new Response("toolUpgrades", "Tool Upgrades"),
                new Response("desertTrader", "Desert Trader"),
                new Response("return2", "Return")
            };
            StardewValley.GameLocation.afterQuestionBehavior cat2Logic = (Farmer who, string cat2answers) =>
            {
                switch (cat2answers)
                {
                    case "adventureShop":
                        Utility.TryOpenShopMenu(Game1.shop_adventurersGuild, null, false);
                        break;
                    case "blacksmith":
                        Utility.TryOpenShopMenu(Game1.shop_blacksmith, null, false);
                        break;
                    case "toolUpgrades":
                        Utility.TryOpenShopMenu(Game1.shop_blacksmithUpgrades, null, false);
                        break;
                    case "desertTrader":
                        Utility.TryOpenShopMenu(Game1.shop_desertTrader, null, false);
                        break;
                    case "return2":
                        MainCategory();
                        break;
                }
            };
            Game1.currentLocation.createQuestionDialogue(
                question: "Combat and Mining",
                answerChoices: cat2,
                afterDialogueBehavior: cat2Logic,
                speaker: null
            );
        }
        private static void category3()
        {
            Response[] cat3 = new Response[]
            {
                new Response("carpenter", "Robin's Shop"),
                new Response("buildBuildings", "Build Buildings"),
                new Response("return3", "Return")
            };
            StardewValley.GameLocation.afterQuestionBehavior cat3Logic = (Farmer who, string cat3answers) =>
            {
                switch (cat3answers)
                {
                    case "carpenter":
                        Utility.TryOpenShopMenu(Game1.shop_carpenter, null, false);
                        break;
                    case "buildBuildings":
                        BuildingMenu();
                        break;
                    case "return3":
                        MainCategory();
                        break;
                }
            };
            Game1.currentLocation.createQuestionDialogue(
                question: "Building",
                answerChoices: cat3,
                afterDialogueBehavior: cat3Logic,
                speaker: null
            );
        }
        private static void others()
        {
            Response[] oth = new Response[]
            {
                new Response("wanderingTrader", "Traveling Cart"),
                new Response("dwarf", "Dwarf's Shop"),
                new Response("krobus", "Krobus's Shop"),
                new Response("othReturn", "Return")
            };
            StardewValley.GameLocation.afterQuestionBehavior othLogic = (Farmer who, string othAnswers) =>
            {
                switch (othAnswers)
                {
                    case "wanderingTrader":
                        Utility.TryOpenShopMenu(Game1.shop_travelingCart, null, false);
                        break;
                    case "dwarf":
                        Utility.TryOpenShopMenu(Game1.shop_dwarf, null, false);
                        break;
                    case "krobus":
                        Utility.TryOpenShopMenu(Game1.shop_krobus, null, false);
                        break;
                    case "othReturn":
                        MainCategory();
                        break;
                }
            };
            Game1.currentLocation.createQuestionDialogue(
                question: "Other Shops",
                answerChoices: oth,
                afterDialogueBehavior: othLogic,
                speaker: null
            );
        }
        private static void MainCategory()
        {
            DelayedAction.functionAfterDelay(() =>
            {
                Game1.currentLocation.createQuestionDialogue(
                    question: "Categories",
                    answerChoices: categories,
                    afterDialogueBehavior: categoriesOptionsLogic,
                    speaker: null
                );
            }, 34);
        }
        private static void WarpPlayer()
        {
            DelayedAction.functionAfterDelay(() =>
            {
                Game1.warpFarmer(
                    lastLocationName,
                    (int)lastTilePos.X,
                    (int)lastTilePos.Y,
                    Game1.player.FacingDirection,
                    doFade: false
                );
                Game1.player.viewingLocation.Value = null;
                Game1.displayHUD = true;
                Game1.currentLocation.resetForPlayerEntry();
                Game1.player.forceCanMove();
                Game1.exitActiveMenu();
            }, 50);
        }
        private static void BuildingMenu()
        {
            DelayedAction.functionAfterDelay(() =>
            {
                lastLocationName = Game1.currentLocation.Name;
                lastTilePos = Game1.player.Tile;
                Game1.activeClickableMenu = new StardewValley.Menus.CarpenterMenu("Robin");
            }, 34);
        }
    }
}
