﻿using System;
using System.Linq;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
	public class Poppy : PluginBase
	{
		public static string[] Supports =
		{

            "Alistar", "Blitzcrank", "Braum", "Janna", "Karma", "Leona", "Lulu",
            "Morgana", "Nunu", "Nami", "Soraka", "Sona", "Taric", "Thresh", "Zyra"
		};

		public Poppy()
		{
			//spelldata from Mechanics-StackOverflow Galio //wat? XD
            Q = new Spell(SpellSlot.Q);
			W = new Spell(SpellSlot.W);
			E = new Spell(SpellSlot.E, 525);
			R = new Spell(SpellSlot.R, 900);
		}

		public override void OnAfterAttack(AttackableUnit unit, AttackableUnit target)
		{
			var t = target as Obj_AI_Hero;
			if (t != null && unit.IsMe)
			{
				if (unit.IsMe && Q.IsReady() && t.IsValidTarget(Q.Range))
				{
					Q.Cast();
					Orbwalking.ResetAutoAttackTimer();
				}
			}
		}

		public override void OnUpdate(EventArgs args)
		{
			if (Q.IsReady()&&Player.CountEnemiesInRange(900) >= 1)
			{
				Q.Cast();
			}	var tarpop = TargetSelector.GetTarget(900, TargetSelector.DamageType.Physical);

                DoCombo(tarpop);
           
        }

        private Obj_AI_Hero FindTank()
        {
            Obj_AI_Hero getTank = null;
            var tempmaxhp = 0.0f;
            foreach (
                var target in
                    ObjectManager.Get<Obj_AI_Hero>().Where(x => Player.Distance(x) <= R.Range && x.IsEnemy && !x.IsDead)
                )
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (target != null)
                {
                    if (target.MaxHealth > tempmaxhp)
                    {
                        tempmaxhp = target.MaxHealth;
                        getTank = target;
                    }
                }
            }

            return getTank;
        }

        private bool IsSupport(Obj_AI_Hero hero)
        {
            return Supports.Any(support => hero.CharData.BaseSkinName.ToLower() == support.ToLower());
        }

        private void DoCombo(Obj_AI_Hero target)
        {
            if (target == null)
            {
                return;
            }


            if (Player.CountEnemiesInRange(500) >= 2)
            {
                foreach (
                    var hero in
                        from hero in
                            ObjectManager.Get<Obj_AI_Hero>()
                                .Where(
                                    hero =>
                                        hero.IsValidTarget(R.Range) && hero.IsEnemy && !hero.IsDead && IsSupport(hero))
                        select hero)
                {
                    if (hero != null)
                    {
                        R.Cast(hero);
                    }
                }

                R.Cast(FindTank());
            }

            if (W.IsReady() && W.Range >= Player.Distance(target))
            {
                W.Cast();
            }

            if (E.IsReady())
            {
                //from vayne markmans
                foreach (
                    var hero in from hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(525f))
                        let prediction = E.GetPrediction(hero)
                        where
                            NavMesh.GetCollisionFlags(
                                prediction.UnitPosition.To2D()
                                    .Extend(ObjectManager.Player.ServerPosition.To2D(), -300)
                                    .To3D()).HasFlag(CollisionFlags.Wall) ||
                            NavMesh.GetCollisionFlags(
                                prediction.UnitPosition.To2D()
                                    .Extend(ObjectManager.Player.ServerPosition.To2D(), -(300 / 2))
                                    .To3D()).HasFlag(CollisionFlags.Wall)
                        select hero)
                {
                    E.Cast(hero);
                }
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboR", "Use R", true);
        }
    }
}