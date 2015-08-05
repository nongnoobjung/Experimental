using System;
using System.Collections.Generic;
using System.Linq;
using AutoSharp.Auto;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

// ReSharper disable ObjectCreationAsStatement

namespace AutoSharp
{
    class Program
    {
        public static Utility.Map.MapType Map;
        public static Menu Config;
        public static MyOrbwalker.Orbwalker Orbwalker;
        private static int _lastMovementTick = 0;

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
            Config.AddItem(new MenuItem("autosharp.playmode", "Play Mode").SetValue(new StringList(new[] {"AUTOSHARP", "AIM"})));
            Config.AddItem(new MenuItem("autosharp.humanizer", "Humanize Movement by ").SetValue(new Slider(175, 125, 350)));
            Config.AddItem(new MenuItem("autosharp.quit", "Quit after Game End").SetValue(true));
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
                Game.OnEnd += OnEnd;
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

        private static void OnEnd(GameEndEventArgs args)
        {
            if (Config.Item("autosharp.quit").GetValue<bool>())
            {
                Game.Quit();
            }
        }

        private static void AntiShrooms(Obj_AI_Base sender, GameObjectIssueOrderEventArgs args)
        {
            if (sender.IsMe && args.Order == GameObjectOrder.MoveTo)
            {
                //Humanizer
                if (Environment.TickCount - _lastMovementTick <
                    Config.Item("autosharp.humanizer").GetValue<Slider>().Value)
                {
                    args.Process = false;
                }
                //AntiShrooms
                if (Traps.EnemyTraps.Any(t => t.Position.Distance(args.TargetPosition) < 125)) { args.Process = false; }
                //The movement will occur
                _lastMovementTick = Environment.TickCount;
            }
        }

        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += init => Init();
        }
    }
}
