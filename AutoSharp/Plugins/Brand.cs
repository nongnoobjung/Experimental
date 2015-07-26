//Plugins Brand Take Combo Part From Hellsing Brand Credit to Hellsing

using System;
using System.Collections.Generic;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Brand : PluginBase
    {
        // ReSharper disable once InconsistentNaming
        public readonly List<Spell> SpellList = new List<Spell>();

        public Brand()
        {
            Q = new Spell(SpellSlot.Q, 1100);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 625);
            R = new Spell(SpellSlot.R, 750);

            SpellList.AddRange(new[] { Q, W, E, R });
            Q.SetSkillshot(0.25f, 60, 1600, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(1, 240, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetTargetted(0.25f, float.MaxValue);
            R.SetTargetted(0.25f, 1000);
        }

        public override void OnUpdate(EventArgs args)
        {
            OnCombo(Target);
        }

        private void OnCombo(Obj_AI_Hero target)
        {
            // Target validation
            if (target == null)
            {
                return;
            }

            //0KTW
            if (Q.IsReady())
            {
                Q.CastIfHitchanceEquals(target, HitChance.VeryHigh);
            }

            if (W.IsReady())
            {
                W.CastIfHitchanceEquals(target, HitChance.VeryHigh);
            }

            if (E.IsReady())
            {
                E.CastIfHitchanceEquals(target, HitChance.VeryHigh);
            }

            if (R.IsReady())
            {
                R.CastIfHitchanceEquals(target, HitChance.VeryHigh);
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