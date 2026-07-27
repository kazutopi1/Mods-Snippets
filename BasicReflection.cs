using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Reflection;
using System;
using StardewValley.Menus;

namespace Test
{
    public class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; }

        public override void Entry(IModHelper helper)
        {
            Instance = this;

            new EventHandler(helper, Monitor);
        }
    }
    public class EventHandler
    {
        IMonitor Monitor;

        MethodInfo savePosition;

        FieldInfo canSkip;

        object Shops_Instance;


        public EventHandler(IModHelper helper, IMonitor monitor)
        {
            Monitor = monitor;

            helper.Events.GameLoop.GameLaunched += Initialize;
            helper.Events.Input.ButtonPressed += Key;
            helper.Events.Display.MenuChanged += FlagReset;
        }
        void Initialize(object s, GameLaunchedEventArgs e)
        {
            Type shopsType = Utils.TypeByName("ShopAnywhereAndroid.Shops");

            if (shopsType != null)
            {
                PropertyInfo shopsInstanceGetter = shopsType.GetProperty(
                    "Instance",
                    BindingFlags.Static | BindingFlags.Public
                );
                Utils.Log(shopsInstanceGetter);

                savePosition = shopsType.GetMethod(
                    "SavePosition",
                    BindingFlags.Public | BindingFlags.Instance
                );
                Utils.Log(savePosition);

                canSkip = shopsType.GetField(
                    "canSkip",
                    BindingFlags.Public | BindingFlags.Instance
                );
                Utils.Log(canSkip);

                if (shopsInstanceGetter == null)
                {
                    Monitor.Log($"Property {shopsInstanceGetter} not found", LogLevel.Error);
                    return;
                }

                Shops_Instance = shopsInstanceGetter.GetValue(null);
            }
            else
            {
                Monitor.Log("Type not found", LogLevel.Error);
            }
        }
        void Key(object s, ButtonPressedEventArgs e)
        {
            if (Context.IsWorldReady)
            {
                if (e.Button is SButton.K)
                {
                    if (Shops_Instance == null && savePosition == null)
                    {
                        return;
                    }
                    Game1.activeClickableMenu = new CarpenterMenu("Robin");
                    savePosition.Invoke(Shops_Instance, null);
                }
            }
        }
        void FlagReset(object s, MenuChangedEventArgs e)
        {
            if (Context.IsWorldReady)
            {
                if (canSkip != null && e.NewMenu == null)
                {
                    canSkip.SetValue(Shops_Instance, false);
                }
            }
        }
    }
    public static class Utils
    {
        public static void Log(object obj)
        {
            if (obj != null)
            {
                ModEntry.Instance.Monitor.Log($"{obj.ToString()} found", LogLevel.Info);
            }
            else
            {
                ModEntry.Instance.Monitor.Log($"Object not found", LogLevel.Error);
            }
        }

        public static Type TypeByName(string name)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(name, false, true);

                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }
    }
}
