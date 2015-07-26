using System;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Auto.SummonersRift
{
    public static class SRManager
    {
        public static void Load()
        {
            RoleSwitcher.Load();
            AttackableUnit.OnDamage += OnDamage;
        }

        public static void Unload()
        {
            AttackableUnit.OnDamage -= OnDamage;
            RoleSwitcher.UnloadAll();
        }

        public static void FastHalt()
        {
            Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.None;
            RoleSwitcher.PauseAll();
        }

        public static void OnUpdate(EventArgs args)
        {
            if (Heroes.Player.InFountain() && !Heroes.Player.IsDead)
            {
                Shopping.Shop();
                Wizard.AntiAfk();
                RoleSwitcher.ChooseBest();
                Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.Mixed;
            }
            if (Heroes.Player.HealthPercent < 30f || Heroes.Player.Gold > 2000.Randomize(1000))
            {
                FastHalt();
                Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.None;
                Heroes.Player.IssueOrder(GameObjectOrder.MoveTo, HeadQuarters.AllyHQ);
                if (Heroes.Player.CountEnemiesInRange(1000) == 0) Heroes.Player.Spellbook.CastSpell(SpellSlot.Recall);
            }
        }

        public static void OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (sender is Obj_AI_Minion && args.TargetNetworkId == Heroes.Player.NetworkId)
            {
                if (Heroes.Player.Position.CountNearbyAllies(1000) <
                    Heroes.Player.Position.CountNearbyEnemies(1000) ||
                    Heroes.Player.Position.CountNearbyAllies(1000) <= 1)
                {
                    Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.None;
                    Wizard.MoveBehindClosestAllyMinion();
                    Utility.DelayAction.Add(2500, () => Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.Mixed);
                }
            }
        }
    }
}
