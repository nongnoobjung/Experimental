using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp.Common;
using SharpDX;

namespace AutoSharp.Utils
{
    internal static class Positioning
    {
        internal static List<Vector3> ValidPossibleMoves;
        internal static Vector3 RandomlyChosenMove;
        internal static int LastUpdate;

        internal static void OnUpdate(EventArgs args)
        {
            if (Environment.TickCount - LastUpdate < 250) return;
            LastUpdate = Environment.TickCount;

            ValidPossibleMoves = new List<Vector3>();

            var farthestAlly = Heroes.AllyHeroes.OrderByDescending(h => h.Distance(HeadQuarters.AllyHQ)).FirstOrDefault();

            var team = Heroes.AllyHeroes.Where(h => !h.IsDead && h.Distance(farthestAlly) < Heroes.Player.AttackRange).ToList();

            var teamPoly = team.Select(hero => new Geometry.Circle(hero.Position.To2D(), 250).ToPolygon()).ToList();

            foreach (var hp in teamPoly)
            {
                foreach (var point in hp.Points)
                {
                    var v3 = point.To3D();
                    if (!point.IsWall() && Heroes.EnemyHeroes.Count(e => e.Distance(v3) < 300) < Heroes.AllyHeroes.Count(e=>e.Distance(v3) <= 300))
                    {
                        ValidPossibleMoves.Add(v3);
                    }
                }
            }
            RandomlyChosenMove = ValidPossibleMoves.OrderBy(v => Heroes.Player.IsMelee ? new Random(Environment.TickCount).Next(0, 42) : v.Distance(HeadQuarters.AllyHQ.Position)).FirstOrDefault();
        }
    }
}
