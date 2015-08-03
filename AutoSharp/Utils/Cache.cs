using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

// ReSharper disable InconsistentNaming

namespace AutoSharp.Utils
{

    public static class HealingBuffs
    {
        private static List<GameObject> _healingBuffs;
        private static int LastUpdate = 0;

        public static List<GameObject> AllyBuffs
        {
            get { return _healingBuffs.FindAll(hb => hb.IsValid && LeagueSharp.Common.Geometry.Distance(hb.Position, HeadQuarters.AllyHQ.Position) < 5400).OrderBy(buff => buff.Position.Distance(Heroes.Player.Position)).ToList(); }
        }

        public static List<GameObject> EnemyBuffs
        {
            get { return _healingBuffs.FindAll(hb => hb.IsValid && LeagueSharp.Common.Geometry.Distance(hb.Position, HeadQuarters.AllyHQ.Position) > 5400); }
        }

        public static void Load()
        {
            _healingBuffs = ObjectManager.Get<GameObject>().Where(h=>h.Name.Contains("healingBuff")).ToList();
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
            Game.OnUpdate += UpdateBuffs;
        }

        private static void UpdateBuffs(EventArgs args)
        {
            if (Environment.TickCount > LastUpdate + 1000)
            {
                foreach (var buff in _healingBuffs)
                {
                    if (Heroes.Player.ServerPosition.Distance(buff.Position) < 80) _healingBuffs.Remove(buff);
                }
                LastUpdate = Environment.TickCount;
            }
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name.Contains("healingBuff"))
            {
                _healingBuffs.Add(sender);
            }
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            foreach (var buff in _healingBuffs)
            {
                if (buff.NetworkId == sender.NetworkId) _healingBuffs.Remove(buff);
            }
        }
    }

    public static class Turrets
    {
        private static List<Obj_AI_Turret> _turrets;

        public static List<Obj_AI_Turret> AllyTurrets
        {
            get { return _turrets.FindAll(t => t.IsValid<Obj_AI_Turret>() && !t.IsDead && t.IsAlly); }
        }
        public static List<Obj_AI_Turret> EnemyTurrets
        {
            get { return _turrets.FindAll(t => t.IsValid<Obj_AI_Turret>() && !t.IsDead && t.IsEnemy); }
        }

        public static void Load()
        {
            Utility.DelayAction.Add(6000, () => _turrets = ObjectManager.Get<Obj_AI_Turret>().ToList());
            Obj_AI_Turret.OnCreate += OnCreate;
            Obj_AI_Turret.OnDelete += OnDelete;
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsValid<Obj_AI_Turret>()) _turrets.Add((Obj_AI_Turret)sender);
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            foreach (var turret in _turrets)
            {
                if (turret.NetworkId == sender.NetworkId) _turrets.Remove(turret);
            }
        }
    }

    public static class HeadQuarters
    {
        private static List<Obj_HQ> _headQuarters;

        public static Obj_HQ AllyHQ
        {
            get { return _headQuarters.FirstOrDefault(t => t.IsValid<Obj_HQ>() && t.IsAlly); }
        }
        public static Obj_HQ EnemyHQ
        {
            get { return _headQuarters.FirstOrDefault(t => t.IsValid<Obj_HQ>() && t.IsEnemy); }
        }

        public static void Load()
        {
            Utility.DelayAction.Add(6000, () => _headQuarters = ObjectManager.Get<Obj_HQ>().ToList());
        }
    }

    public static class Heroes
    {
        private static List<Obj_AI_Hero> _heroes;

        public static Obj_AI_Hero Player = ObjectManager.Player;

        public static List<Obj_AI_Hero> AllyHeroes
        {
            get { return _heroes.FindAll(h => h.IsValid<Obj_AI_Hero>() && h.IsAlly); }
        }

        public static List<Obj_AI_Hero> EnemyHeroes
        {
            get { return _heroes.FindAll(h => h.IsValid<Obj_AI_Hero>() && h.IsEnemy); }
        }

        public static void Load()
        {
            Player = ObjectManager.Player;
            Utility.DelayAction.Add(6000, () => _heroes = ObjectManager.Get<Obj_AI_Hero>().ToList());
        }
    }

    public static class Minions
    {
        
        private static List<Obj_AI_Minion> _minions;

        public static List<Obj_AI_Minion> AllyMinions
        {
            get { return _minions.FindAll(t => t.IsValid<Obj_AI_Minion>() && !t.IsDead && t.IsAlly); }
        }
        public static List<Obj_AI_Minion> EnemyMinions
        {
            get { return _minions.FindAll(t => t.IsValid<Obj_AI_Minion>() && !t.IsDead && t.IsValidTarget()); }
        }

        public static void Load()
        {
            _minions = new List<Obj_AI_Minion>();
            Obj_AI_Minion.OnCreate += OnCreate;
            Obj_AI_Minion.OnDelete += OnDelete;
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            foreach (var minion in _minions)
            {
                if (minion.NetworkId == sender.NetworkId) _minions.Remove(minion);
            }
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsValid<Obj_AI_Minion>()) _minions.Add((Obj_AI_Minion)sender);
        }

    }

    public static class Cache
    {
        public static void Load()
        {
            Turrets.Load();
            HeadQuarters.Load();
            Heroes.Load();
            Minions.Load();
            HealingBuffs.Load();
        }
    }
}
