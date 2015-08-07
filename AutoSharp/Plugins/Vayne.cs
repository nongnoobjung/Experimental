﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;
using GamePath = System.Collections.Generic.List<SharpDX.Vector2>;

namespace AutoSharp.Plugins
{
    public class Vayne : PluginBase
    {
        public Vayne()
        {
            Q = new Spell(SpellSlot.Q, 300f);
            E = new Spell(SpellSlot.E, 545f);
            R = new Spell(SpellSlot.R);
        }

        public override void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Q.IsReady() && gapcloser.End.Distance(Player.ServerPosition) < 300)
            {
                Q.Cast(GetTumblePos(gapcloser.Sender));
            }
            //We really don't want to get turret aggro for nothin'.
            if (Player.UnderTurret(true)) return;
            //we wanna check if the mothafucka can actually do shit to us.
            if (Player.Distance(gapcloser.End) > 350) return;
            //ok we're no pussies, we don't want to condemn the unsuspecting akali when we can jihad her.
            if (Player.Level > gapcloser.Sender.Level + 1) return;
            //k so that's not the case, we're going to check if we should condemn the gapcloser away.
            if (Player.HealthPercent < 25)
            E.Cast(gapcloser.Sender);
        }

        public override void OnPossibleToInterrupt(Obj_AI_Hero unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (spell.DangerLevel == Interrupter2.DangerLevel.High && E.IsReady() && E.IsInRange(unit))
            {
                E.Cast(unit);
            }
        }

        public override void OnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (!args.Unit.IsMe || !Q.IsReady()) return;
            if (HasUltiBuff() && HasTumbleBuff() &&
                Heroes.EnemyHeroes.Any(h => h.IsMelee && h.Distance(Player) < h.AttackRange + 75))
            {
                args.Process = false;
            }
            if (args.Target.IsValid<Obj_AI_Hero>())
            {
                var target = (Obj_AI_Hero) args.Target;
                if (R.IsReady())
                {
                    if (target.UnderTurret(true))
                    {
                        if (Heroes.Player.UnderTurret(true))
                        {
                            R.Cast();
                        }
                    }
                    else
                    {
                        R.Cast();
                    }
                }

                var t = (Obj_AI_Hero) args.Target;

                if (t.IsMelee && t.IsFacing(Player))
                {
                    if (t.Distance(Player.ServerPosition) < 325)
                    {
                        args.Process = false;
                        Q.Cast(GetTumblePos(t));
                    }
                }
            }
        }

        public override void OnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (target.IsValid<Obj_AI_Hero>() && Q.IsReady())
            {
                Q.Cast(GetTumblePos((Obj_AI_Hero)target));
            }
        }

        public override void OnUpdate(EventArgs args)
        {
            if (Player.UnderTurret(true)) return;

            if (E.IsReady())
            {
                foreach (var hero in Heroes.EnemyHeroes)
                {
                    if (IsCondemnable(hero)) E.Cast(hero);
                }
            }
        }

        bool IsCondemnable(Obj_AI_Hero hero)
        {
            if (!hero.IsValidTarget(550f) || hero.HasBuffOfType(BuffType.SpellShield) ||
                hero.HasBuffOfType(BuffType.SpellImmunity) || hero.IsDashing()) return false;
            var pP = Heroes.Player.ServerPosition;
            var pD = 425;

            var prediction = E.GetPrediction(hero);
            for (var i = 15; i < pD; i += 100)
            {
                var posCF = NavMesh.GetCollisionFlags(
                    prediction.UnitPosition.To2D()
                        .Extend(
                            pP.To2D(),
                            -i)
                        .To3D());
                if (posCF.HasFlag(CollisionFlags.Wall) || posCF.HasFlag(CollisionFlags.Building))
                {
                    return true;
                }
            }
            return false;
        }

        Vector3 GetTumblePos(Obj_AI_Hero target)
        {
            //if the target is not a melee and he's alone he's not really a danger to us, proceed to 1v1 him :^ )
            if (!target.IsMelee && Heroes.Player.CountEnemiesInRange(800) == 1) return Game.CursorPos;

            var aRC = new Utils.Geometry.Circle(Heroes.Player.ServerPosition.To2D(), 300).ToPolygon().ToClipperPath();
            var cP = Game.CursorPos;
            var tP = target.ServerPosition;
            var pList = new List<Vector3>();
            var additionalDistance = (0.106 + Game.Ping / 2000f) * target.MoveSpeed;

            if ((!cP.IsWall() && !cP.UnderTurret(true) && cP.Distance(tP) > 325 && cP.Distance(tP) < 550 &&
                 (cP.CountEnemiesInRange(425) <= cP.CountAlliesInRange(325)))) return cP;

            foreach (var p in aRC)
            {
                var v3 = new Vector2(p.X, p.Y).To3D();

                if (target.IsFacing(Heroes.Player))
                {
                    if (!v3.IsWall() && !v3.UnderTurret(true) && v3.Distance(tP) > 325 && v3.Distance(tP) < 550 &&
                        (v3.CountEnemiesInRange(425) <= v3.CountAlliesInRange(325))) pList.Add(v3);
                }
                else
                {
                    if (!v3.IsWall() && !v3.UnderTurret(true) && v3.Distance(tP) > 325 &&
                        v3.Distance(tP) < (550 - additionalDistance) &&
                        (v3.CountEnemiesInRange(425) <= v3.CountAlliesInRange(325))) pList.Add(v3);
                }
            }
            if (Heroes.Player.UnderTurret() || Heroes.Player.CountEnemiesInRange(800) == 1)
            {
                return pList.Count > 1 ? pList.OrderBy(el => el.Distance(cP)).FirstOrDefault() : Vector3.Zero;
            }
            return pList.Count > 1 ? pList.OrderByDescending(el => el.Distance(tP)).FirstOrDefault() : Vector3.Zero;
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboR", "Use R", true);
        }

        bool HasUltiBuff()
        {
            return Player.Buffs.Any(b => b.Name.ToLower().Contains("vayneinquisition"));
        }

        bool HasTumbleBuff()
        {
            return Player.Buffs.Any(b => b.Name.ToLower().Contains("vaynetumblebonus"));
        }
    }
}