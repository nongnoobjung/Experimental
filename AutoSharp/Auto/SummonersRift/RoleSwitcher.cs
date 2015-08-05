using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Auto.SummonersRift
{
    public static class RoleSwitcher
    {
        private static bool _paused = false;

        public static void Load()
        {
            Utility.DelayAction.Add(new Random().Next(35000, 45000), () => Game.OnUpdate += ChooseBest);
        }

        public static void Unload()
        {
            Game.OnUpdate -= ChooseBest;
        }

        public static void ChooseBest(EventArgs args)
        {
            if (_paused) return;

            if (MyTeam.MyRole == MyTeam.Roles.Unknown)
            {
                if (MyTeam.Support == null) MyTeam.MyRole = MyTeam.Roles.Support;
                if (MyTeam.ADC == null) MyTeam.MyRole = MyTeam.Roles.ADC;
                if (MyTeam.Midlaner == null) MyTeam.MyRole = MyTeam.Roles.Midlaner;
                if (MyTeam.Toplaner == null) MyTeam.MyRole = MyTeam.Roles.Toplaner;
                if (MyTeam.Jungler == null) MyTeam.MyRole = MyTeam.Roles.Toplaner;
            }

            #region BOTLANE

            if (MyTeam.MyRole == MyTeam.Roles.Support || MyTeam.MyRole == MyTeam.Roles.ADC)
            {
                var enemyOuterTurret = Heroes.Player.Team == GameObjectTeam.Order
                    ? Turrets.EnemyTurrets.FirstOrDefault(
                        t => t.IsValidTarget() && t.Distance(Map.BottomLane.Red_Outer_Turret) < 800)
                    : Turrets.EnemyTurrets.FirstOrDefault(
                        t => t.IsValidTarget() && t.Distance(Map.BottomLane.Blue_Outer_Turret) < 800);
                var enemyInnerTurret = Heroes.Player.Team == GameObjectTeam.Order
                    ? Turrets.EnemyTurrets.FirstOrDefault(
                        t => t.IsValidTarget() && t.Distance(Map.BottomLane.Red_Inner_Turret) < 800)
                    : Turrets.EnemyTurrets.FirstOrDefault(
                        t => t.IsValidTarget() && t.Distance(Map.BottomLane.Blue_Inner_Turret) < 800);
                var myOuterTurret = Heroes.Player.Team == GameObjectTeam.Order
                    ? Turrets.EnemyTurrets.FirstOrDefault(t => t.Distance(Map.BottomLane.Blue_Outer_Turret) < 800)
                    : Turrets.EnemyTurrets.FirstOrDefault(t => t.Distance(Map.BottomLane.Red_Outer_Turret) < 800);

                if (enemyInnerTurret != null)
                {
                    var followMinion =
                        Minions.AllyMinions.OrderBy(
                            m =>
                                m.Distance(enemyOuterTurret ?? enemyInnerTurret))
                            .FirstOrDefault();

                    if (followMinion != null)
                    {
                        Program.Orbwalker.SetOrbwalkingPoint(followMinion.RandomizePosition());
                        Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.LaneClear;
                    }
                    else
                    {
                        Program.Orbwalker.SetOrbwalkingPoint(myOuterTurret.RandomizePosition());
                        Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.LaneClear;
                    }
                }
                else MyTeam.MyRole = MyTeam.Roles.Midlaner;
            }

            #endregion BOTLANE

            #region TOPLANE

            if (MyTeam.MyRole == MyTeam.Roles.Toplaner)
            {
                var enemyOuterTurret = Heroes.Player.Team == GameObjectTeam.Order
                    ? Turrets.EnemyTurrets.FirstOrDefault(
                        t => t.IsValidTarget() && t.Distance(Map.TopLane.Red_Outer_Turret) < 800)
                    : Turrets.EnemyTurrets.FirstOrDefault(
                        t => t.IsValidTarget() && t.Distance(Map.TopLane.Blue_Outer_Turret) < 800);
                var enemyInnerTurret = Heroes.Player.Team == GameObjectTeam.Order
                    ? Turrets.EnemyTurrets.FirstOrDefault(
                        t => t.IsValidTarget() && t.Distance(Map.TopLane.Red_Inner_Turret) < 800)
                    : Turrets.EnemyTurrets.FirstOrDefault(
                        t => t.IsValidTarget() && t.Distance(Map.TopLane.Blue_Inner_Turret) < 800);
                var myOuterTurret = Heroes.Player.Team == GameObjectTeam.Order
                    ? Turrets.EnemyTurrets.FirstOrDefault(t => t.Distance(Map.TopLane.Blue_Outer_Turret) < 800)
                    : Turrets.EnemyTurrets.FirstOrDefault(t => t.Distance(Map.TopLane.Red_Outer_Turret) < 800);

                if (enemyInnerTurret != null)
                {
                    var followMinion =
                        Minions.AllyMinions.OrderBy(
                            m =>
                                m.Distance(enemyOuterTurret ?? enemyInnerTurret))
                            .FirstOrDefault();

                    if (followMinion != null)
                    {
                        Program.Orbwalker.SetOrbwalkingPoint(followMinion.RandomizePosition());
                        Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.LaneClear;
                    }
                    else
                    {
                        Program.Orbwalker.SetOrbwalkingPoint(myOuterTurret.RandomizePosition());
                        Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.LaneClear;
                    }
                }
                else MyTeam.MyRole = MyTeam.Roles.Midlaner;
            }

            #endregion TOPLANE

            #region MIDLANE

            if (MyTeam.MyRole == MyTeam.Roles.Midlaner)
            {

                var enemyOuterTurret = Heroes.Player.Team == GameObjectTeam.Order
                    ? Turrets.EnemyTurrets.FirstOrDefault(
                        t => t.IsValidTarget() && t.Distance(Map.MidLane.Red_Outer_Turret) < 800)
                    : Turrets.EnemyTurrets.FirstOrDefault(
                        t => t.IsValidTarget() && t.Distance(Map.MidLane.Blue_Outer_Turret) < 800);
                var enemyInnerTurret = Heroes.Player.Team == GameObjectTeam.Order
                    ? Turrets.EnemyTurrets.FirstOrDefault(
                        t => t.IsValidTarget() && t.Distance(Map.MidLane.Red_Inner_Turret) < 800)
                    : Turrets.EnemyTurrets.FirstOrDefault(
                        t => t.IsValidTarget() && t.Distance(Map.MidLane.Blue_Inner_Turret) < 800);
                var myOuterTurret = Heroes.Player.Team == GameObjectTeam.Order
                    ? Turrets.EnemyTurrets.FirstOrDefault(t => t.Distance(Map.MidLane.Blue_Outer_Turret) < 800)
                    : Turrets.EnemyTurrets.FirstOrDefault(t => t.Distance(Map.MidLane.Red_Outer_Turret) < 800);

                if (enemyInnerTurret != null)
                {
                    var followMinion =
                        Minions.AllyMinions.OrderBy(
                            m =>
                                m.Distance(enemyOuterTurret ?? enemyInnerTurret))
                            .FirstOrDefault();

                    if (followMinion != null)
                    {
                        Program.Orbwalker.SetOrbwalkingPoint(followMinion.RandomizePosition());
                        Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.LaneClear;
                    }
                    else
                    {
                        Program.Orbwalker.SetOrbwalkingPoint(myOuterTurret.RandomizePosition());
                        Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.LaneClear;
                    }
                }
                else
                {
                    var followMinion =
                        Minions.AllyMinions.OrderBy(
                            m =>
                                m.Distance(HeadQuarters.EnemyHQ))
                            .FirstOrDefault();

                    Program.Orbwalker.SetOrbwalkingPoint(followMinion.RandomizePosition());
                    Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.LaneClear;
                }
            }

            #endregion MIDLANE
        }

        public static void Pause()
        {
            _paused = true;
        }

        public static void Unpause()
        {
            _paused = false;
        }
    }
}
