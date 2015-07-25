using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Utils
{

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
            get { return _heroes.FindAll(h => h.IsAlly); }
        }

        public static List<Obj_AI_Hero> EnemyHeroes
        {
            get { return _heroes.FindAll(h => h.IsEnemy); }
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
        private static int LastUpdate;

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
            if (Environment.TickCount - LastUpdate < 500) return;
            LastUpdate = Environment.TickCount;

            _minions = ObjectManager.Get<Obj_AI_Minion>().FindAll(m => !m.IsDead && m.IsValid && m.IsVisible);
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
