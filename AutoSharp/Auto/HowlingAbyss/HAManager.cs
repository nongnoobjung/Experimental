using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Auto.HowlingAbyss
{
    public static class HAManager
    {
        public static void Load()
        {
            Game.OnUpdate += DecisionMaker.OnUpdate;
            AttackableUnit.OnDamage += OnDamage;
            Obj_AI_Base.OnIssueOrder += OnIssueOrder;
            ARAMShopAI.Main.Init();
        }

        public static void Unload()
        {
            Game.OnUpdate -= DecisionMaker.OnUpdate;
            AttackableUnit.OnDamage -= OnDamage;
        }

        public static void FastHalt()
        {
            Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.None;
        }

        public static void OnIssueOrder(Obj_AI_Base sender, GameObjectIssueOrderEventArgs args)
        {
            if (sender == Heroes.Player && args.Order == GameObjectOrder.MoveTo)
            {
                var nearbyEnemyTurret = args.TargetPosition.GetClosestEnemyTurret();
                if (nearbyEnemyTurret != null && nearbyEnemyTurret.Position.CountNearbyAllyMinions(700) <= 2)
                {
                    args.Process = false;
                }
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
