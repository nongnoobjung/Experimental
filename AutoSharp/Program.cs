using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using AutoSharp.Auto;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;
// ReSharper disable ObjectCreationAsStatement

namespace AutoSharp
{
    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    class Program
    {
        public static Menu Config;
        public static MyOrbwalker.Orbwalker Orbwalker;

        public static void Init()
        {
            Config = new Menu("AutoSharp: " + ObjectManager.Player.ChampionName, "autosharp." + ObjectManager.Player.ChampionName, true);
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
            randomizer.AddItem(new MenuItem("autosharp.randomizer.maxrand", "Max Rand By").SetValue(new Slider(300, 0, 250)));
            randomizer.AddItem(new MenuItem("autosharp.randomizer.playdefensive", "Play Defensive?").SetValue(true));
            randomizer.AddItem(new MenuItem("autosharp.randomizer.auto", "Auto-Adjust? (ALPHA)").SetValue(true));

            new PluginLoader();
            CustomEvents.Game.OnGameLoad += args => { Cache.Load(); Game.OnUpdate += Positioning.OnUpdate; Autoplay.Load(); };


            Orbwalker = new MyOrbwalker.Orbwalker(orbwalker);

            Utility.DelayAction.Add(
                    new Random().Next(1000, 10000), () =>
                    {
                        new LeagueSharp.Common.AutoLevel(Utils.AutoLevel.GetSequence().Select(num => num - 1).ToArray());
                        LeagueSharp.Common.AutoLevel.Enable();
                        Console.WriteLine("AutoLevel Init Success!");
                    });
        }


        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += initArgs => Init();
            CustomEvents.Game.OnGameEnd += exitArgs => Exit();
            Game.OnEnd += exit2Args => Exit();

            if (HeadQuarters.AllyHQ == null || HeadQuarters.EnemyHQ == null || HeadQuarters.AllyHQ.Health < 200 || HeadQuarters.EnemyHQ.Health < 200)
            {
                Exit();
            }
        }


        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static void Exit()
        {
            Process.Start("taskkill /f /im \"LeagueSharp of Legends.exe\"");
        }
    }
}
