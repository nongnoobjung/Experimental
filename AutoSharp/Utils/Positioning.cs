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
            if (Environment.TickCount - LastUpdate < 350) return;
            LastUpdate = Environment.TickCount;

            ValidPossibleMoves = new List<Vector3>();

            var farthestAlly =
                Heroes.AllyHeroes.OrderByDescending(h => h.Distance(HeadQuarters.AllyHQ)).FirstOrDefault();

            var teamPoly = (from hero in Heroes.AllyHeroes where hero.Distance(farthestAlly) < 1000 select new Geometry.Circle(hero.Position.To2D(), 350).ToPolygon()).ToList();

            teamPoly.ForEach(hp => hp.Points.ForEach(point => ValidPossibleMoves.Add(point.To3D())));

            foreach (var hp in teamPoly)
            {
                foreach (var point in hp.Points)
                {
                    var v3 = point.To3D();
                    if (!point.IsWall() && Heroes.EnemyHeroes.Count(e => e.Distance(v3) < 300) == 0)
                    {
                        ValidPossibleMoves.Add(v3);
                    }
                }
            }
            RandomlyChosenMove = ValidPossibleMoves.OrderBy(v => new Random(Environment.TickCount).Next(0, 42)).FirstOrDefault();
        }
    }
}
