using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

// ReSharper disable InconsistentNaming

namespace AutoSharp.Utils
{

    public static class HealRunes
    {
        public static List<GameObject> Runes { get; private set; }

        public static void Load()
        {
            Runes = ObjectManager.Get<GameObject>().Where(h => h.Name.Contains("heal_rune")).ToList();
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (!sender.Name.Contains("heal_rune")) return;
            if (!Runes.Contains(sender))
            {
                Runes.Add(sender);
            }
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            if (!sender.Name.Contains("heal_rune")) return;
            Runes.RemoveAll(hb => hb.NetworkId == sender.NetworkId);
        }

        public static void RemoveBuff(Vector3 buffPos)
        {
            Runes.RemoveAll(hb => hb.Position == buffPos);
        }
    }

    public static class HealingBuffs
    {
        private static List<GameObject> _healingBuffs;

        public static List<GameObject> AllyBuffs
        {
            get { return _healingBuffs.FindAll(hb => LeagueSharp.Common.Geometry.Distance(hb.Position, HeadQuarters.AllyHQ.Position) < 5400).OrderBy(buff => buff.Position.Distance(Heroes.Player.Position)).ToList(); }
        }

        public static List<GameObject> EnemyBuffs
        {
            get { return _healingBuffs.FindAll(hb => LeagueSharp.Common.Geometry.Distance(hb.Position, HeadQuarters.AllyHQ.Position) > 5400); }
        }

        public static void Load()
        {
            _healingBuffs = ObjectManager.Get<GameObject>().Where(h=>h.Name.Contains("healingBuff")).ToList();
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (!sender.Name.Contains("healingBuff")) return;
            if (!_healingBuffs.Contains(sender))
            {
                _healingBuffs.Add(sender);
            }
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            if (!sender.Name.Contains("healingBuff")) return;
            _healingBuffs.RemoveAll(hb => hb.NetworkId == sender.NetworkId);
        }

        public static void RemoveBuff(Vector3 buffPos)
        {
            _healingBuffs.RemoveAll(hb => hb.Position == buffPos);
        }
    }

    public static class Turrets
    {
        private static List<Obj_AI_Turret> _turrets;

        public static List<Obj_AI_Turret> AllyTurrets
        {
            get { return _turrets.FindAll(t => t.IsAlly); }
        }
        public static List<Obj_AI_Turret> EnemyTurrets
        {
            get { return _turrets.FindAll(t => t.IsEnemy); }
        }

        public static void Load()
        {
            _turrets = ObjectManager.Get<Obj_AI_Turret>().ToList();
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Type != GameObjectType.obj_AI_Turret) return;
            if (!_turrets.Contains(sender))
            {
                _turrets.Add(sender as Obj_AI_Turret);
            }
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Type != GameObjectType.obj_AI_Turret) return;
            _turrets.RemoveAll(t => t.NetworkId == sender.NetworkId);
        }
    }

    public static class HeadQuarters
    {
        private static List<Obj_HQ> _headQuarters;

        public static Obj_HQ AllyHQ
        {
            get { return _headQuarters.FirstOrDefault(t => t.IsAlly); }
        }
        public static Obj_HQ EnemyHQ
        {
            get { return _headQuarters.FirstOrDefault(t => t.IsEnemy); }
        }

        public static void Load()
        {
            _headQuarters = ObjectManager.Get<Obj_HQ>().ToList();
        }
    }

    public static class Heroes
    {
        private static List<Obj_AI_Hero> _heroes;

        public static Obj_AI_Hero Player = ObjectManager.Player;

        public static List<Obj_AI_Hero> AllyHeroes
        {
            get { return _heroes.FindAll(h => h.IsAlly && !h.IsDead); }
        }

        public static List<Obj_AI_Hero> EnemyHeroes
        {
            get { return _heroes.FindAll(h => h.IsEnemy && !h.IsDead ); }
        }

        public static void Load()
        {
            Player = ObjectManager.Player;
            _heroes = ObjectManager.Get<Obj_AI_Hero>().ToList();
        }
    }

    public static class Minions
    {
        
        private static List<Obj_AI_Minion> _minions;
        private static int _lastUpdate;

        public static List<Obj_AI_Minion> AllyMinions
        {
            get { return _minions.FindAll(t => t.IsAlly); }
        }
        public static List<Obj_AI_Minion> EnemyMinions
        {
            get { return _minions.FindAll(t => t.IsValidTarget()); }
        }

        public static void Load()
        {
            _minions = ObjectManager.Get<Obj_AI_Minion>().ToList();
            Game.OnUpdate += OnUpdate;
        }

        public static void OnUpdate(EventArgs args)
        {
            if (Environment.TickCount - _lastUpdate < 500) return;
            _lastUpdate = Environment.TickCount;

            _minions = ObjectManager.Get<Obj_AI_Minion>().Where(m => !m.IsDead && m.IsValid && m.IsVisible).ToList();
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
