using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp.Common;
using SharpDX;
// ReSharper disable InconsistentNaming

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

            if (farthestAlly == null)
            {
                var minion = Minions.AllyMinions.OrderByDescending(t => t.Distance(HeadQuarters.AllyHQ)).FirstOrDefault();
                if (minion != null)
                {
                    RandomlyChosenMove = minion.Position.RandomizePosition();
                }
                else
                {
                    // ReSharper disable once PossibleNullReferenceException
                    //if its null, its over anyways lol since only nexus is left and I'm lazy #TODO
                    RandomlyChosenMove =
                        Turrets.AllyTurrets.OrderByDescending(t => t.Distance(HeadQuarters.AllyHQ))
                            .FirstOrDefault()
                            .Position.RandomizePosition();
                }
                return;
            }

            if (Heroes.Player.IsMelee) RandomlyChosenMove = farthestAlly.ServerPosition.Randomize(0, 130);

            ValidPossibleMoves.Add(farthestAlly.Position.RandomizePosition()); //initialize the vectorlist with a position known to exist,
                                                                               //so it doesn't follow the mouse anymore

            var team = Heroes.AllyHeroes.Where(h => !h.IsDead && h.Distance(farthestAlly) < 300).ToList();

            var teamPoly = team.Select(hero => new Geometry.Circle(hero.Position.To2D(), 200).ToPolygon()).ToList();

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
            if (Heroes.Player.CountEnemiesInRange(700) != 0)
            {
                var hero = Heroes.EnemyHeroes.OrderBy(h => h.Distance(Heroes.Player)).FirstOrDefault();
                if (hero != null)
                {
                    var PRADAPos = hero.GetPRADAPos();
                    RandomlyChosenMove = PRADAPos != Vector3.Zero ? PRADAPos : ValidPossibleMoves.OrderBy(v => v.Distance(HeadQuarters.AllyHQ.Position)).FirstOrDefault();
                    return;
                }
            }
            RandomlyChosenMove = ValidPossibleMoves.OrderBy(v => v.Distance(HeadQuarters.AllyHQ.Position)).FirstOrDefault();
        }
    }
}
