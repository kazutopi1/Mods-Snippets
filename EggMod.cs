using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace EggMod
{
    public class EggM : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.MenuChanged += Egg;
        }
        private void Egg(object sender, MenuChangedEventArgs e)
        {
            if (!Context.IsWorldReady || Game1.player == null) { return; }
            if (e.NewMenu != null && e.OldMenu == null || e.NewMenu == null)
            {
                foreach (Item items in Game1.player.Items)
                {
                    if (items == null)
                    {
                        Game1.player.addItemToInventory(ItemRegistry.Create("174", 999));
                    }
                }
            }
        }
    }
}
