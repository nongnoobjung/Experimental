using System.Linq;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace AutoSharp.Auto.HowlingAbyss
{
    internal static class Decisions
    {
        internal static bool HealUp()
        {
            if (Heroes.Player.HealthPercent > 75) return false;

            var closestEnemyBuff =
                HealingBuffs.EnemyBuffs.FirstOrDefault(eb => eb.Position.Distance(Heroes.Player.Position) < 800);
            var closestAllyBuff = HealingBuffs.AllyBuffs.FirstOrDefault();

            if (HealingBuffs.AllyBuffs.FirstOrDefault() == null && closestEnemyBuff == null) return false;

            var buffPos = closestEnemyBuff != null
                ? closestEnemyBuff.Position.RandomizePosition()
                : closestAllyBuff.Position.RandomizePosition();

            if (!buffPos.IsValid()) return false;

            Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.None;
            Heroes.Player.IssueOrder(GameObjectOrder.MoveTo, buffPos);

            if (Heroes.Player.Distance(buffPos) < 75) { HealingBuffs.RemoveBuff(buffPos); }

            return true;
        }

        internal static bool Farm()
        {
            var minion = Wizard.GetFarthestMinion();
            var minionPos = minion != null ? minion.Position.Extend(HeadQuarters.AllyHQ.Position, 250).RandomizePosition() : (Heroes.Player.Team == GameObjectTeam.Order ? new Vector2(5483, 5001).RandomizePosition() : new Vector2(7783, 7137).RandomizePosition());
            if ((minionPos.CountEnemiesInRange(1000) != 0 || Heroes.Player.CountEnemiesInRange(1000) != 0) && minionPos.CountAlliesInRange(1000) != 0) return false;
            Program.Orbwalker.SetOrbwalkingPoint(minionPos.RandomizePosition());
            Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.LaneClear;
            return true;
        }

        internal static void Fight()
        {
            Program.Orbwalker.SetOrbwalkingPoint(Positioning.RandomlyChosenMove);
            Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.Combo;
        }
    }
}
