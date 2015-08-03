using System;
using System.Collections.Generic;
using System.Linq;
using AutoSharp.Auto;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

// ReSharper disable ObjectCreationAsStatement

namespace AutoSharp
{
    class Program
    {
        public static Utility.Map.MapType Map;
        public static Menu Config;
        public static MyOrbwalker.Orbwalker Orbwalker;
        public static List<Render.Line> lstLines = new List<Render.Line>();

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
            var options = Config.AddSubMenu(new Menu("Options: ", "autosharp.options"));
            options.AddItem(new MenuItem("autosharp.options.healup", "Take Heals?").SetValue(true));
            var orbwalker = Config.AddSubMenu(new Menu("Orbwalker", "autosharp.orbwalker"));
            var randomizer = Config.AddSubMenu(new Menu("Randomizer", "autosharp.randomizer"));
            randomizer.AddItem(new MenuItem("autosharp.randomizer.minrand", "Min Rand By").SetValue(new Slider(0, 0, 250)));
            randomizer.AddItem(new MenuItem("autosharp.randomizer.maxrand", "Max Rand By").SetValue(new Slider(300, 0, 250)));
            randomizer.AddItem(new MenuItem("autosharp.randomizer.playdefensive", "Play Defensive?").SetValue(true));
            randomizer.AddItem(new MenuItem("autosharp.randomizer.auto", "Auto-Adjust? (ALPHA)").SetValue(true));

            new PluginLoader();
            CustomEvents.Game.OnGameLoad += args =>
            {
                Map = Utility.Map.GetMap().Type; 
                Cache.Load(); 
                Game.OnUpdate += Positioning.OnUpdate; 
                Autoplay.Load();
                Obj_AI_Base.OnIssueOrder += AntiShrooms;
            };


            Orbwalker = new MyOrbwalker.Orbwalker(orbwalker);

            Utility.DelayAction.Add(
                    new Random().Next(1000, 10000), () =>
                    {
                        new LeagueSharp.Common.AutoLevel(Utils.AutoLevel.GetSequence().Select(num => num - 1).ToArray());
                        LeagueSharp.Common.AutoLevel.Enable();
                        Console.WriteLine("AutoLevel Init Success!");
                    });
        }

        private static void AntiShrooms(Obj_AI_Base sender, GameObjectIssueOrderEventArgs args)
        {
            if (sender.IsMe && args.Order == GameObjectOrder.MoveTo)
            {
                if (Traps.EnemyTraps.Any(t => t.Position.Distance(args.TargetPosition) < 125)) { args.Process = false; }
            }
        }

        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += init => Init();
        }
    }
}
