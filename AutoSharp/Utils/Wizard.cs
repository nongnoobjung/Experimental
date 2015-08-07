﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;
using GamePath = System.Collections.Generic.List<SharpDX.Vector2>;


namespace AutoSharp.Utils
{
    internal static class Extensions
    {
        public static Vector3 GetPRADAPos(this Obj_AI_Hero target)
        {
            var aRC = new Geometry.Circle(Heroes.Player.ServerPosition.To2D(), 300).ToPolygon().ToClipperPath();
            var tP = target.ServerPosition;
            var pList = new List<Vector3>();
            foreach (var p in aRC)
            {
                var v3 = new Vector2(p.X, p.Y).To3D();


                if (!v3.UnderTurret(true) && v3.Distance(tP) > 325 && v3.Distance(tP) < Heroes.Player.AttackRange &&
                    (v3.CountEnemiesInRange(425) <= v3.CountAlliesInRange(325))) pList.Add(v3);
            }
            return pList.Count > 1 ? pList.OrderByDescending(el => el.Distance(tP)).FirstOrDefault() : Vector3.Zero;
        }

        public static Obj_AI_Turret GetClosestEnemyTurret(this Vector3 point)
        {
            return Turrets.EnemyTurrets.OrderBy(t => t.Distance(point)).FirstOrDefault();
        }

        public static Obj_AI_Minion GetFarthestMinionOnLane(Vector3 lanepos)
        {
            return Minions.AllyMinions.OrderByDescending(m => m.Distance(lanepos)).FirstOrDefault();
        }

        public static int CountNearbyAllyMinions(this Obj_AI_Base x, int distance)
        {
            return Minions.AllyMinions.Count(minion => minion.Distance(x.Position) < distance);
        }

        public static int CountNearbyAllyMinions(this Vector3 x, int distance)
        {
            return Minions.AllyMinions.Count(minion => minion.Distance(x) < distance);
        }

        public static int CountNearbyAllies(this Obj_AI_Base x, int distance)
        {
            return Heroes.AllyHeroes.Count(hero => !hero.IsDead && !hero.IsMe && hero.Distance(x.Position) < distance);
        }

        public static int CountNearbyAllies(this Vector3 x, int distance)
        {
            return Heroes.AllyHeroes.Count(hero => !hero.IsDead && !hero.IsMe && hero.Distance(x) < distance);
        }

        public static int CountNearbyEnemies(this Vector3 x, int distance)
        {
            return Heroes.EnemyHeroes.Count(hero => !hero.IsDead && !hero.IsMe && hero.Distance(x) < distance);
        }

        public static bool IsHeroPet(this Obj_AI_Base minion)
        {
            var mn = minion.BaseSkinName;
            if (mn.Contains("teemo") || mn.Contains("shroom") || mn.Contains("turret") || mn.Contains("clone") || mn.Contains("blanc") || mn.Contains("trap") || mn.Contains("mine"))
            {
                return true;
            }
            return false;
        }

        public static bool IsAvoidable(this GameObject obj)
        {
            if (obj.IsAlly)
            {
                return false;
            }
            var on = obj.Name;
            if (on.Contains("teemo") || on.Contains("shroom") || on.Contains("turret") || on.Contains("clone") || on.Contains("blanc") || on.Contains("trap") || on.Contains("mine") || on.Contains("nida") || on.Contains("morg") || on.Contains("ziggs"))
            {
                return true;
            }
            return false;
        }

        public static bool IsLowHealth(this Obj_AI_Base x)
        {
            return x.HealthPercent < 30f;
        }

        public static int Randomize(this int i, int max = 200)
        {
            var r = new Random(Environment.TickCount);
            return i + r.Next(0, max);
        }

        public static Vector3 RandomizePosition(this GameObject o)
        {
            var r = new Random(Environment.TickCount);
            var minRandBy = Program.Config.Item("autosharp.randomizer.minrand").GetValue<Slider>().Value;
            var maxRandBy = Program.Config.Item("autosharp.randomizer.maxrand").GetValue<Slider>().Value;
            if (Program.Config.Item("autosharp.randomizer.playdefensive").GetValue<bool>() && Heroes.Player.CountEnemiesInRange(800) >= Heroes.Player.CountAlliesInRange(800))
            {
                minRandBy *= Wizard.GetDefensiveMultiplier();
                maxRandBy *= Wizard.GetDefensiveMultiplier();
            }
            else
            {
                minRandBy *= r.Next(-1, 1);
            }
            return new Vector2(o.Position.X + r.Next(minRandBy, maxRandBy), o.Position.Y + r.Next(minRandBy, maxRandBy)).To3D();
        }

        public static Vector3 RandomizePosition(this Vector3 v)
        {
            var r = new Random(Environment.TickCount);
            var minRandBy = Program.Config.Item("autosharp.randomizer.minrand").GetValue<Slider>().Value;
            var maxRandBy = Program.Config.Item("autosharp.randomizer.maxrand").GetValue<Slider>().Value;
            if (Program.Config.Item("autosharp.randomizer.playdefensive").GetValue<bool>() && Heroes.Player.CountEnemiesInRange(800) >= Heroes.Player.CountAlliesInRange(800))
            {
                minRandBy *= Wizard.GetDefensiveMultiplier();
                maxRandBy *= Wizard.GetDefensiveMultiplier();
            }
            else
            {
                minRandBy *= r.Next(-1, 1);
            }
            return new Vector2(v.X + r.Next(minRandBy, maxRandBy), v.Y + r.Next(minRandBy, maxRandBy)).To3D();
        }

        public static Vector3 RandomizePosition(this Vector2 v)
        {
            var r = new Random(Environment.TickCount);
            var minRandBy = Program.Config.Item("autosharp.randomizer.minrand").GetValue<Slider>().Value;
            var maxRandBy = Program.Config.Item("autosharp.randomizer.maxrand").GetValue<Slider>().Value;
            if (Program.Config.Item("autosharp.randomizer.playdefensive").GetValue<bool>() && Heroes.Player.CountEnemiesInRange(800) >= Heroes.Player.CountAlliesInRange(800))
            {
                minRandBy *= Wizard.GetDefensiveMultiplier();
                maxRandBy *= Wizard.GetDefensiveMultiplier();
            }
            else
            {
                minRandBy *= r.Next(-1, 1);
            }
            return new Vector2(v.X + r.Next(minRandBy, maxRandBy), v.Y + r.Next(minRandBy, maxRandBy)).To3D();
        }

        public static bool HasItem(this Obj_AI_Hero hero, ItemId item)
        {
            return hero.InventoryItems.Any(slot => slot.Id == item);
        }

        public static bool HasSupportItems(this Obj_AI_Hero hero)
        {
            return hero.HasItem(ItemId.Ancient_Coin) || hero.HasItem(ItemId.Relic_Shield) ||
                   hero.HasItem(ItemId.Spellthiefs_Edge);
        }

        public static bool IsValidAlly(this Obj_AI_Base unit, float range = float.MaxValue)
        {
            return unit.Distance(ObjectManager.Player) < range && unit.IsValid<Obj_AI_Hero>() && unit.IsAlly &&
                   !unit.IsDead && unit.IsTargetable;
        }

        public static double HealthBuffer(this Obj_AI_Base hero, int buffer)
        {
            return hero.Health - (hero.MaxHealth * buffer / 100);
        }

        public static bool CastCheck(this Items.Item item, Obj_AI_Base target)
        {
            return item.IsReady() && target.IsValidTarget(item.Range);
        }

        public static bool CastCheck(this Spell spell,
            Obj_AI_Base target,
            string menu,
            bool range = true,
            bool team = true)
        {
            return spell.IsReady() && target.IsValidTarget() && !ObjectManager.Player.UnderTurret(true);
        }

        public static bool CastCheck(this Spell spell, Obj_AI_Base target, bool range = true, bool team = true)
        {
            return spell.IsReady() && target.IsValidTarget() && !ObjectManager.Player.UnderTurret(true);
        }

        public static bool IsInRange(this Spell spell, Obj_AI_Base target)
        {
            return ObjectManager.Player.Distance(target) < spell.Range;
        }

        public static bool IsInRange(this Items.Item item, Obj_AI_Base target)
        {
            return ObjectManager.Player.Distance(target) < item.Range;
        }

        public static bool WillKill(this Obj_AI_Base caster, Obj_AI_Base target, string spell, int buffer = 10)
        {
            return caster.GetSpellDamage(target, spell) >= target.HealthBuffer(buffer);
        }

        public static bool WillKill(this Obj_AI_Base caster, Obj_AI_Base target, SpellData spell, int buffer = 10)
        {
            return caster.GetSpellDamage(target, spell.Name) >= target.HealthBuffer(buffer);
        }

        public static void AddList(this Menu menu, string name, string displayName, string[] list)
        {
            menu.AddItem(
                new MenuItem("autosharp." + ObjectManager.Player.ChampionName + "." + name, displayName).SetValue(new StringList(list)));
        }

        public static void AddBool(this Menu menu, string name, string displayName, bool value)
        {
            menu.AddItem(new MenuItem("autosharp." + ObjectManager.Player.ChampionName + "." + name, displayName).SetValue(value));
        }

        public static void AddSlider(this Menu menu, string name, string displayName, int value, int min, int max)
        {
            menu.AddItem(
                new MenuItem("autosharp." + ObjectManager.Player.ChampionName + "." + name, displayName).SetValue(
                    new Slider(value, min, max)));
        }

        public static void AddObject(this Menu menu, string name, string displayName, object value)
        {
            menu.AddItem(new MenuItem("autosharp." + ObjectManager.Player.ChampionName + "." + name, displayName).SetValue(value));
        }
    }

    internal static class Wizard
    {
        private static int TimeSinceLastTakenDecision = 0;

        public static void AntiAfk()
        {
            if (Environment.TickCount - TimeSinceLastTakenDecision > 120000)
            {
                Heroes.Player.IssueOrder(GameObjectOrder.MoveTo,
                    Heroes.Player.Position.RandomizePosition());
                TimeSinceLastTakenDecision = Environment.TickCount;
            }
        }

        public static Vector3 GetBestRecallPosition()
        {
            var t = GetClosestAllyTurret().Position;
            var i = 250.Randomize(50)*GetDefensiveMultiplier(); 
            return new Vector2(t.X + i, t.Y + i).To3D();
        }

        public static Obj_AI_Turret GetFarthestAllyTurret()
        {
            return Turrets.AllyTurrets.OrderByDescending(t => t.Distance(HeadQuarters.AllyHQ.Position)).FirstOrDefault();
        }

        public static Obj_AI_Turret GetClosestAllyTurret()
        {
            return Turrets.AllyTurrets.OrderBy(t => t.Distance(Heroes.Player)).FirstOrDefault();
        }

        public static Obj_AI_Turret GetClosestEnemyTurret()
        {
            return Turrets.EnemyTurrets.OrderBy(t => t.Distance(Heroes.Player)).FirstOrDefault();
        }

        public static Obj_AI_Minion GetFarthestMinion()
        {
            return Minions.AllyMinions.OrderByDescending(m => m.Distance(HeadQuarters.AllyHQ.Position)).FirstOrDefault();
        }

        public static Obj_AI_Minion GetClosestAllyMinion()
        {
            return Minions.AllyMinions.OrderBy(m => m.Distance(Heroes.Player.ServerPosition)).FirstOrDefault();
        }

        public static Obj_AI_Minion GetClosestEnemyMinion()
        {
            return Minions.EnemyMinions.OrderBy(m => m.Distance(Heroes.Player.ServerPosition)).FirstOrDefault();
        }

        public static int GetAggressiveMultiplier()
        {
            if (Heroes.Player.Team == GameObjectTeam.Order)
            {
                return 1;
            }
            return -1;
        }

        public static int GetDefensiveMultiplier()
        {
            if (Heroes.Player.Team == GameObjectTeam.Order)
            {
                return -1;
            }
            return 1;
        }

        public static void MoveToClosestAllyMinion()
        {
            Heroes.Player.IssueOrder(GameObjectOrder.MoveTo, GetFarthestMinion().RandomizePosition());
        }

        public static void MoveBehindClosestAllyMinion()
        {
            var m = GetClosestAllyMinion().Position;
            var i = 100.Randomize(50)*GetDefensiveMultiplier();
            Heroes.Player.IssueOrder(GameObjectOrder.MoveTo, new Vector2(m.X + i, m.Y + i).To3D());
        }

        public static void MoveToRandomAllyMinion()
        {
            var randomAllyMinion = Minions.AllyMinions.OrderBy(m => new Random().Next()).FirstOrDefault();
            if (randomAllyMinion == null || !randomAllyMinion.IsValid) { return; }
            Heroes.Player.IssueOrder(GameObjectOrder.MoveTo, randomAllyMinion.Position.RandomizePosition());
        }
    }
}