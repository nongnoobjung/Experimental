using System;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Auto.HowlingAbyss
{
    internal static class DecisionMaker
    {
        public static void OnUpdate(EventArgs args)
        {
            var player = Heroes.Player;

            if (Decisions.ImSoLonely())
            {
                return;
            }

            if (Decisions.HealUp())
            {
                return;
            }

            if (player.UnderTurret(true) && Wizard.GetClosestEnemyTurret().CountNearbyAllyMinions(700) <= 2 && Wizard.GetClosestEnemyTurret().CountAlliesInRange(700) == 0)
            {
                Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.Mixed;
                Program.Orbwalker.SetOrbwalkingPoint(player.Position.Extend(HeadQuarters.AllyHQ.Position.RandomizePosition(), 800));
                return;
            }

            if (Heroes.Player.InFountain())
            {
                Shopping.Shop();
                Wizard.AntiAfk();
            }

            if (Decisions.Farm())
            {
                return;
            }
            Decisions.Fight();

            if (Program.Orbwalker.GetOrbwalkingPoint().IsZero ||
                Program.Orbwalker.GetOrbwalkingPoint() == Game.CursorPos)
            {
                Decisions.ImSoLonely();
            }
        }
    }
}
