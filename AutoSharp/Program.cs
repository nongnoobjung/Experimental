using System;
using System.Linq;
using AutoSharp.Auto;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp
{
    class Program
    {
        public static Menu Config;
        public static MyOrbwalker.Orbwalker Orbwalker;

        public static void Init()
        {
            Config = new Menu("AutoSharp", "autosharp", true);
            Config.AddItem(new MenuItem("autosharp.mode", "Mode").SetValue(new StringList(new[] {"AUTO", "SBTW"}))).ValueChanged +=
                (sender, args) =>
                {
                    if (Config.Item("autosharp.mode").GetValue<StringList>().SelectedValue == "AUTO")
                    {
                        Autoplay.Load();
                    }
                    else
                    {
                        Autoplay.Unload();
                        Orbwalker.SetOrbwalkingPoint(Game.CursorPos);
                    }
                };
            var orbwalker = Config.AddSubMenu(new Menu("Orbwalker", "autosharp.orbwalker"));
            var randomizer = Config.AddSubMenu(new Menu("Randomizer", "autosharp.randomizer"));
            randomizer.AddItem(new MenuItem("autosharp.randomizer.minrand", "Min Rand By").SetValue(new Slider(0, 0, 250)));
            randomizer.AddItem(new MenuItem("autosharp.randomizer.maxrand", "Max Rand By").SetValue(new Slider(0, 0, 250)));
            randomizer.AddItem(new MenuItem("autosharp.randomizer.playdefensive", "Play Defensive?").SetValue(true));
            randomizer.AddItem(new MenuItem("autosharp.randomizer.auto", "Auto-Adjust? (ALPHA)").SetValue(true));

            CustomEvents.Game.OnGameLoad += args => { Cache.Load(); Game.OnUpdate += Positioning.OnUpdate; Autoplay.Load(); };


            Orbwalker = new MyOrbwalker.Orbwalker(orbwalker);
            Config.AddToMainMenu();

            Utility.DelayAction.Add(
                    new Random().Next(1000, 10000), () =>
                    {
                        new LeagueSharp.Common.AutoLevel(Utils.AutoLevel.GetSequence().Select(num => num - 1).ToArray());
                        LeagueSharp.Common.AutoLevel.Enable();
                        Console.WriteLine("AutoLevel Init Success!");
                    });
        }

        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += init => Init();
        }
    }
}
